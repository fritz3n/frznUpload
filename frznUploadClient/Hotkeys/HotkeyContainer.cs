using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using frznUpload.Client.Hotkey;

namespace frznUpload.Client
{
    public class HotkeyContainer : IEnumerable<HotkeyConfig>
    {
        Dictionary<(ModifierKeys, Keys), HotkeyHandler> HotKeys = new Dictionary<(ModifierKeys, Keys), HotkeyHandler>();

        public HotkeyContainer(ClientManager client, MainForm form)
        {

            string serialized = Properties.Settings.Default.Hotkeys;
            
            string[] hotkeys = serialized.Split('|');

            foreach(string hotkey in hotkeys)
            {
                if (hotkey != "")
                {
                    var hc = HotkeyHandler.Deserialize(hotkey.Replace("%bar%", "|"));
                    if (hc == null)
                        continue;
                    hc.Enabled = true;
                    HotKeys.Add((hc.Config.Modifier, hc.Config.Key), hc);
                }
            }

        }

        private void Save()
        {
            string[] serializedStrings = new string[HotKeys.Count];

            var list = HotKeys.Values.ToList();

            for(int i = 0; i < HotKeys.Count; i++)
            {
                serializedStrings[i] = list[i].Serialize().Replace("|", "%bar%");
            }

            string serialized = string.Join("|", serializedStrings);

            Properties.Settings.Default.Hotkeys = serialized;

            Properties.Settings.Default.Save();
        }

        public IEnumerator<HotkeyConfig> GetEnumerator()
        {
            return HotKeys.Values.Select((e) => e.Config).GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(HotkeyConfig hotkey)
        {
            Remove(hotkey);
            (ModifierKeys, Keys) Key = (hotkey.Modifier, hotkey.Key);
            var hotkeyHandler = new HotkeyHandler(hotkey);
            hotkeyHandler.Enabled = true;
            HotKeys.Add(Key, hotkeyHandler);
            Save();
        }

        public void Remove(HotkeyConfig hotkey)
        {
            (ModifierKeys, Keys) Key = (hotkey.Modifier, hotkey.Key);
            if (HotKeys.ContainsKey(Key))
            {
                HotKeys[Key].Enabled = false;
                HotKeys.Remove(Key);
                Save();
            }
        }
    }
}