using frznUpload.Client.Config;
using frznUpload.Client.Hotkey;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using ModifierKeys = frznUpload.Client.Hotkey.ModifierKeys;

namespace frznUpload.Client
{
	public class HotkeyContainer : IEnumerable<HotkeyConfig>
	{
		Dictionary<(ModifierKeys, Keys), HotkeyHandler> HotKeys = new Dictionary<(ModifierKeys, Keys), HotkeyHandler>();

		public HotkeyContainer(ClientManager client, MainForm form)
		{


			string[] hotkeys = ConfigHandler.Config.Hotkeys;

			foreach (string hotkey in hotkeys)
			{
				if (hotkey != "")
				{
					var hc = HotkeyHandler.Deserialize(hotkey);
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

			for (int i = 0; i < HotKeys.Count; i++)
			{
				serializedStrings[i] = list[i].Serialize();
			}

			ConfigHandler.Config.Hotkeys = serializedStrings;

			ConfigHandler.Save();
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