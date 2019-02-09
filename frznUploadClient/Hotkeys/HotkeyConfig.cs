using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace frznUpload.Client.Hotkey
{
    public struct HotkeyConfig
    {
        public ModifierKeys Modifier;
        public Keys Key;
        public ShareType Share;
        public FileProvider Provider;
        public string Whitelist;
        public string Format;
    }
}
