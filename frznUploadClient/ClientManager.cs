using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frznUpload.Client
{
    class ClientManager
    {
        Client ActiveClient;
        DateTime ActiveTime;
        TimeSpan cachedTime = new TimeSpan(0, 10, 0);
        public bool LoggedIn { get; private set; } = false;

        public ClientManager()
        {
            Retry(() => { return true; });
        }

        public async Task Login(string username, string password)
        {
            await Retry(async () => await ActiveClient.AuthWithPass(username, password, Properties.Settings.Default.KeyFile));
        }

        public async Task Logout()
        {
            File.Delete(Properties.Settings.Default.KeyFile);
            await Retry(() => ActivateClient());
        }

        public async Task<FileUploader> UploadFile(string path)
        {
            return await Retry(async () => await ActiveClient.UploadFile(path));
        }

        public async Task<string> ShareFile(string fileIdentifier, bool firstView = false, bool isPublic = true, bool publicRegistered = true, bool whitelisted = false, string whitelist = "")
        {
            return await Retry(() => ActiveClient.ShareFile(fileIdentifier, firstView, isPublic, publicRegistered, whitelisted, whitelist));
        }

        public async Task<List<RemoteFile>> GetFiles()
        {
            return await Retry(() => ActiveClient.GetFiles());
        }
        
        private async Task ActivateClient()
        {
            ActiveClient?.Dispose();
            LoggedIn = false;

            ActiveClient = new Client(Properties.Settings.Default.Url, Properties.Settings.Default.Port);
            try
            {
                await ActiveClient.AuthWithKey(Properties.Settings.Default.KeyFile);
                LoggedIn = true;
            }
            catch { }
        }

        private T Retry<T>(Func<T> func, int maxRetry = 5)
        {
            ActiveTime = DateTime.Now;
            List<Exception> exceptions = new List<Exception>();

            for (int i = 0; i < maxRetry; i++)
            {
                try
                {
                    EnsureActivated();
                    T returned = func();
                    return returned;
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
                ActivateClient().Wait();

            if (EnsureLoggedIn & !ActiveClient.IsAuthenticated)
                return true;

            return false;
        }
        
    }

    public class RetryException : Exception
    {
        public List<Exception> Exceptions { get; private set; }
        public Exception LastException { get => Exceptions.LastOrDefault(); }
        public int RetryCount { get; private set; }

        public RetryException(List<Exception> exceptions, int retryCount)
        {
            Exceptions = exceptions;
            RetryCount = retryCount;
        }
    }
}
