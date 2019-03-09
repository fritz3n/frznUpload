using Microsoft.Win32;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace frznUpload.Client.ExplorerServer
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.AllFilesAndFolders)]
    public class ExplorerHandler : SharpContextMenu
    {
        Image icon = null;
        private RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\" + AppName, false);
        private const string AppName = "frznUpload";
        private const string PathKey = "path";

        public ExplorerHandler()
        {
            Stream stream = GetType().Assembly.GetManifestResourceStream("frznUpload.Client.ExplorerServer.upload_Wr5_icon.ico");
            icon = new Bitmap(new Icon(stream).ToBitmap(), new Size(16, 16));
            stream.Dispose();
        }

        protected override bool CanShowMenu()
        {
            //  We always show the menu.
            return true;
        }

        protected override ContextMenuStrip CreateMenu()
        {

            var menu = new ContextMenuStrip();
            menu.AutoSize = true;
            menu.ImageScalingSize = new Size(16, 16);

            //  Create a 'count lines' item.
            var itemUpload = new ToolStripMenuItem
            (
                "Upload file to fritzen.tk"
            );
            itemUpload.AutoSize = true;
            itemUpload.ImageScaling = ToolStripItemImageScaling.SizeToFit;

            itemUpload.Image = icon;

            //  When we click, we'll count the lines.
            itemUpload.Click += ItemUpload_Click;

            //  Add the item to the context menu.
            menu.Items.Add(itemUpload);

            menu.PerformLayout();

            //  Return the menu.
            return menu;
        }

        private void ItemUpload_Click(object sender, EventArgs e)
        {
            string[] args = SelectedItemPaths.ToArray();

            if (CheckIfRunning())
            {
                SendMessage(args);
            }
            else
            {
                StartServer(args);
            }
        }
        
        private void StartServer(string[] args)
        {
            string path = GetServerPath();

            if(path == null)
            {
                MessageBox.Show("Couldn´t find the client");
                return;
            }
            
            Process p = new Process();
            p.StartInfo.FileName = path;

            string arguments = string.Join(" ", args.Select((s) => '"' + s + '"'));
            p.StartInfo.Arguments = arguments;

            p.Start();
        }

        private string GetServerPath()
        {
            if (rkApp == null)
                return null;

            string path = (string)(rkApp.GetValue(PathKey) ?? "");

            if (!File.Exists(path))
                return null;

            return path;
        }

        /// <summary>
        /// Check if the Uploader is already running
        /// </summary>
        /// <returns></returns>
        private bool CheckIfRunning()
        {
            bool running;

            Mutex mutex = new Mutex(false, "frznUploadRunning");

            try
            {
                running = !mutex.WaitOne(TimeSpan.Zero, true);
            }
            catch (AbandonedMutexException e)
            {
                running = true;
                Console.WriteLine("Mutex abandoned");
            }
            finally
            {
                mutex.Dispose();
            }

            return running;
        }

        /// <summary>
        /// Sends arguments to the running server
        /// </summary>
        /// <param name="arguments">the arguments to be sent</param>
        private void SendMessage(string[] arguments)
        {
            NamedPipeClientStream pipeClient =
                    new NamedPipeClientStream(".", "frznUploadPipe",
                        PipeDirection.Out, PipeOptions.Asynchronous);

            Console.WriteLine("Connecting...");

            pipeClient.Connect();

            var serializer = new XmlSerializer(typeof(string[]));

            var writer = new StreamWriter(pipeClient);

            using (StringWriter stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, arguments);

                writer.Write(stringWriter.ToString());
            }

            Console.WriteLine("Sent!");

            writer.Dispose();
            pipeClient.Dispose();
        }
    }
}