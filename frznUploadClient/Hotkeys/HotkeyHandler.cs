using frznUpload.Client.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace frznUpload.Client.Hotkey
{
    class HotkeyHandler
    {
        public HotkeyConfig Config { get; set; }
        bool _Enabled;
        public bool Enabled { get => _Enabled; set { if (value) Register(); else Unregister(); _Enabled = value; } }

        private HotKey Hotkey;
        private HotkeyContainer Parent;

        public HotkeyHandler(HotkeyContainer parent, HotkeyConfig config)
        {
            Config = config;

            Parent = parent;

            Hotkey = new HotKey(Config.Modifier, Config.Key);
            Hotkey.Tag = this;
            Hotkey.Pressed += parent.Pressed;
        }

        public List<UploadContract> Execute()
        {
            IFileProvider provider = null;
            switch (Config.Provider)
            {
                case FileProvider.Clipboard:
                    provider = new ClipboardProvider();
                    break;
                case FileProvider.ScreenClip:
                    provider = new ScreenClip();
                    break;
                case FileProvider.Screenshot:
                    provider = new Screenshot();
                    break;
                default:
                    throw new InvalidOperationException();
            }

            List<UploadFile> uploads = provider.GetFile(Config.Format);

            var list = new List<UploadContract>();

            foreach (UploadFile f in uploads) {
                var c = new UploadContract
                {
                    Uploader = null,
                    Path = f.Path,
                    Filename = f.Filename,

                    Share = Config.Share != ShareType.DontShare,
                    FirstView = (Config.Share & ShareType.FirstView) == ShareType.FirstView,
                    Public = (Config.Share & ShareType.Public) == ShareType.Public,
                    PublicRegistered = (Config.Share & ShareType.PublicRegistered) == ShareType.PublicRegistered,
                    Whitelisted = (Config.Share & ShareType.Whitelisted) == ShareType.Whitelisted,
                    Whitelist = Config.Whitelist
                };

                list.Add(c);
            }

            return list;
        }

        public bool Register() => Hotkey.Register();
        public void Unregister() => Hotkey.Unregister();

        public string Serialize()
        {
            string str = $"{(int)Config.Modifier},{(int)Config.Key},{(int)Config.Share},{(int)Config.Provider},'{Config.Whitelist.Replace("'", "\\'")}','{Config.Format.Replace("'", "\\'")}'";

            return str;
        }

        public static HotkeyHandler Unserialize(HotkeyContainer parent, string str)
        {
            Regex r = new Regex(@"^(\d+),(\d+),(\d+),(\d+),'((?:\\'|[^\\',])*)','((?:\\'|[^\\',])*)'$");

            Match m = r.Match(str);

            if (!m.Success)
                return null;

            int ModifierInt = int.Parse(m.Groups[1].Value);
            var ModifierKeys = (ModifierKeys)ModifierInt;

            int KeyInt = int.Parse(m.Groups[2].Value);
            var Keys = (Keys)KeyInt;

            int ShareInt = int.Parse(m.Groups[3].Value);
            var Share = (ShareType)ShareInt;

            int ProviderInt = int.Parse(m.Groups[4].Value);
            var Provider = (FileProvider)ProviderInt;

            string Format = m.Groups[5].Value;
            Format = Format.Replace("\\'", "'");

            string Whitelist = m.Groups[6].Value;
            Whitelist = Format.Replace("\\'", "'");

            HotkeyConfig config = new HotkeyConfig
            {
                Modifier = ModifierKeys,
                Key = Keys,
                Share = Share,
                Provider = Provider,
                Format = Format,
                Whitelist = Whitelist,
            };

            return new HotkeyHandler(parent, config);
        }
    }

    [Flags]
    public enum ShareType
    {
        DontShare = 0,
        Share = 1,
        FirstView = 2,
        Public = 4,
        PublicRegistered = 8,
        Whitelisted = 16,
    }

    [Flags]
    public enum FileProvider
    {
        None = 0, // Not a valid Value
        Clipboard = 1,
        ScreenClip = 2,
        Screenshot = 3,
    }
}