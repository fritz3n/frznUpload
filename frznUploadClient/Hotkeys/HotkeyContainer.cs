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
        ClientManager Client;
        Dictionary<(ModifierKeys, Keys), HotkeyHandler> HotKeys = new Dictionary<(ModifierKeys, Keys), HotkeyHandler>();
        MainForm Form;
        Queue<UploadContract> UploadQueue = new Queue<UploadContract>();

        public HotkeyContainer(ClientManager client, MainForm form)
        {
            Client = client;
            Form = form;

            Form.UploadFinished += Form_UploadFinished;

            string serialized = Properties.Settings.Default.Hotkeys;

            string[] hotkeys = serialized.Split('|');

            foreach(string hotkey in hotkeys)
            {
                var hc = HotkeyHandler.Unserialize(this, hotkey.Replace("%bar%", "|"));
                HotKeys.Add((hc.Config.Modifier, hc.Config.Key), hc);
            }

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
            var hotkeyHandler = new HotkeyHandler(this, hotkey);
            hotkeyHandler.Enabled = true;
            HotKeys.Add(Key, hotkeyHandler);
        }

        public void Remove(HotkeyConfig hotkey)
        {
            (ModifierKeys, Keys) Key = (hotkey.Modifier, hotkey.Key);
            if (HotKeys.ContainsKey(Key))
            {
                HotKeys[Key].Enabled = false;
            }
        }

        private void Form_UploadFinished(object sender, EventArgs e)
        {
            StartUpload();
        }
        
        private void Pressed(object sender, EventArgs e)
        {
            var contracts = ((sender as HotKey).Tag as HotkeyHandler).Execute();
            foreach(var c in contracts)
            {
                UploadQueue.Enqueue(c);
            }
            StartUpload();
        }

        private async void StartUpload()
        {
            if (Form.Uploading)
                return;
            if (UploadQueue.Count == 0)
                return;

            var contract = UploadQueue.Dequeue();
            contract.Uploader = await Client.UploadFile(contract.Path, contract.Filename);
            Form.StartUpload(contract);
        }
    }
}