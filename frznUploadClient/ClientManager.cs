﻿using frznUpload.Client.Config;
using frznUpload.Client.Handlers;
using frznUpload.Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace frznUpload.Client
{
	public class ClientManager : IDisposable
	{
		const bool verbose = true;


		Client ActiveClient;
		DateTime ActiveTime;
		TimeSpan cachedTime = new TimeSpan(0, 10, 0);
		Timer cacheTimer;
		public bool LoggedIn { get; private set; } = false;
		public string Username
		{
			get
			{
				EnsureActivated();
				return ActiveClient.Name;
			}
		}

		public ClientManager()
		{
			cacheTimer = new Timer(cachedTime.TotalMilliseconds / 2);
			cacheTimer.Elapsed += CacheTimer_Elapsed;
			cacheTimer.AutoReset = true;
			cacheTimer.Enabled = true;
		}

		private void CacheTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (DateTime.Now - ActiveTime >= cachedTime)
				Disconnect();
		}

		public async Task Connect()
		{
			await ActivateClient();
		}

		public async Task<bool> Login(string username, string password)
		{
			LoggedIn = await RetryAsync(async () => await ActiveClient.AuthWithPass(username, password), false);
			await ActivateClient();
			return ActiveClient.IsAuthenticated;
		}

		public async Task Logout()
		{
			await RetryAsync(async () => { await ActiveClient.DeauthKey(); await ActivateClient(); });
		}

		/// <summary>
		/// Starts the upload of a file and returns the fileuploader doing all the hard work for you lazy bumscratcher
		/// </summary>
		/// <param name="path"> The path of the file</param>
		/// <returns></returns>
		public async Task<FileUploader> UploadFile(string path, string Filename = null)
		{
			return await RetryAsync(async () =>
			{
				Client c = await GetNewClient();
				return c.UploadFile(path, true, Filename);
			}
			);
		}

		public async Task<string> ShareFile(string fileIdentifier, bool firstView = false, bool isPublic = true, bool publicRegistered = true, bool whitelisted = false, string whitelist = "")
		{
			return await RetryAsync(() => ActiveClient.ShareFile(fileIdentifier, firstView, isPublic, publicRegistered, whitelisted, whitelist));
		}

		public async Task<List<RemoteFile>> GetFiles()
		{
			return await RetryAsync(() => ActiveClient.GetFiles());
		}

		public async Task DeleteFile(string fileIdentifier)
		{
			await RetryAsync(() => ActiveClient.DeleteFileAsync(fileIdentifier));
		}

		public async Task RemoveTwoFa()
		{
			await RetryAsync(() => ActiveClient.RemoveTwoFaAsync());
		}

		public async Task<bool> GetHasTwoFaEnabled()
		{
			return await RetryAsync(() => ActiveClient.GetHasTwoFaEnabled());
		}

		public async Task<string> GetTwoFaSecret()
		{
			return await RetryAsync(() => ActiveClient.GetTwoFaSecret());
		}



		public void Disconnect()
		{
			ActiveClient?.Disconnect();
			ActiveClient?.Dispose();
			ActiveClient = null;
		}

		private async Task<Client> GetNewClient()
		{
			var c = new Client();
			await c.ConnectAsync(ConfigHandler.Config.Url, ConfigHandler.Config.Port, verbose);
			return c;
		}

		private async Task ActivateClient()
		{
			ActiveClient?.Dispose();
			LoggedIn = false;

			ActiveClient = new Client();
			await ActiveClient.ConnectAsync(ConfigHandler.Config.Url, ConfigHandler.Config.Port, verbose);

			LoggedIn = ActiveClient.IsAuthenticated;

			if (LoggedIn && CertificateHandler.ShouldRenew)
				await ActiveClient.RenewCert();
		}

		private T Retry<T>(Func<T> func, bool ensureLogin = false, int maxRetry = 5)
		{
			ActiveTime = DateTime.Now;
			var exceptions = new List<Exception>();

			for (int i = 0; i < maxRetry; i++)
			{
				try
				{
					if (EnsureActivated(ensureLogin))
						throw new UnauthorizedAccessException();
					T returned = func();
					return returned;
				}
				catch (UnauthorizedAccessException)
				{
					throw;
				}
				catch (Exception e)
				{
					exceptions.Add(e);
				}
			}

			throw new RetryException(exceptions, maxRetry);
		}

		private async Task<T> RetryAsync<T>(Func<Task<T>> func, bool ensureLogin = false, int maxRetry = 5)
		{
			ActiveTime = DateTime.Now;
			var exceptions = new List<Exception>();

			for (int i = 0; i < maxRetry; i++)
			{
				try
				{
					if (await EnsureActivatedAsync(ensureLogin))
						throw new UnauthorizedAccessException();
					Task<T> returned = func();
					return await returned;
				}
				catch (UnauthorizedAccessException)
				{
					throw;
				}
				catch (Exception e)
				{
					exceptions.Add(e);
				}
			}

			throw new RetryException(exceptions, maxRetry);
		}

		private async Task RetryAsync(Func<Task> func, bool ensureLogin = false, int maxRetry = 5)
		{
			ActiveTime = DateTime.Now;
			var exceptions = new List<Exception>();

			for (int i = 0; i < maxRetry; i++)
			{
				try
				{
					if (await EnsureActivatedAsync(ensureLogin))
						throw new UnauthorizedAccessException();
					Task returned = func();
					await returned;
					return;
				}
				catch (UnauthorizedAccessException)
				{
					throw;
				}
				catch (Exception e)
				{
					exceptions.Add(e);
				}
			}

			throw new RetryException(exceptions, maxRetry);
		}

		private bool EnsureActivated(bool EnsureLoggedIn = true)
		{
			if (ActiveClient == null || !ActiveClient.Connected)
			{
				if (!ActivateClient().Wait(10000))
					throw new TimeoutException();
			}

			if (EnsureLoggedIn & !ActiveClient.IsAuthenticated)
				return true;

			return false;
		}

		private async Task<bool> EnsureActivatedAsync(bool EnsureLoggedIn = true)
		{
			if (ActiveClient == null || !ActiveClient.Connected)
				await ActivateClient();

			if (EnsureLoggedIn & !ActiveClient.IsAuthenticated)
				return true;

			return false;
		}

		public void Dispose()
		{
			ActiveClient?.Dispose();
			cacheTimer?.Dispose();
			GC.SuppressFinalize(this);
		}

		~ClientManager()
		{
			Dispose();
		}
	}

	public class RetryException : Exception
	{
		public List<Exception> Exceptions { get; private set; }
		public Exception LastException { get => Exceptions.LastOrDefault(); }
		public int RetryCount { get; private set; }
		public new string Message => LastException.Message;

		public RetryException(List<Exception> exceptions, int retryCount)
		{
			Exceptions = exceptions;
			RetryCount = retryCount;
		}

		public override string ToString() => LastException.ToString();
	}
}
