using frznUpload.Client.Files;
using frznUpload.Client.Hotkey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frznUpload.Client.Hotkey
{
	class FileProviderBuilder
	{

		public static IFileProvider BuildProvider(FileProvider fileProvider)
		{
			switch (fileProvider)
			{
				case FileProvider.None:
					throw new InvalidOperationException();
				case FileProvider.Clipboard:
					return new ClipboardProvider();
				case FileProvider.ScreenClip:
					return new ScreenClip();
				case FileProvider.Screenshot:
					return new Screenshot();
				default:
					List<IFileProvider> fileProviders = new();

					if ((fileProvider & FileProvider.Clipboard) == FileProvider.Clipboard)
						fileProviders.Add(new ClipboardProvider(false));

					if ((fileProvider & FileProvider.ScreenClip) == FileProvider.ScreenClip)
						fileProviders.Add(new ScreenClip());

					if ((fileProvider & FileProvider.Screenshot) == FileProvider.Screenshot)
						fileProviders.Add(new Screenshot());

					return new ChainedFileProvider(fileProviders);
			}
		}

	}
}
