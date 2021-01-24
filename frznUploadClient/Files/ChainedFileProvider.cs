using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frznUpload.Client.Files
{
	class ChainedFileProvider : IFileProvider
	{
		private readonly IEnumerable<IFileProvider> fileProviders;
		IFileProvider chosenProvider;

		public ChainedFileProvider(IEnumerable<IFileProvider> fileProviders)
		{
			this.fileProviders = fileProviders;
		}

		public void FreeFile()
		{
			chosenProvider?.FreeFile();
		}

		public List<UploadFile> GetFile(string format)
		{
			foreach (IFileProvider fp in fileProviders)
			{
				if (!fp.IsAvailable())
					continue;
				List<UploadFile> file = null;
				try
				{
					file = fp.GetFile(format);
				}
				catch (Exception) { }

				if (file != null)
				{
					chosenProvider = fp;
					return file;
				}
			}
			throw new InvalidOperationException();
		}

		public bool IsAvailable()
		{
			return fileProviders.Any(f => f.IsAvailable());
		}
	}
}
