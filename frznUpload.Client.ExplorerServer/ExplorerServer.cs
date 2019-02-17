using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System;
using System.Collections.Generic;
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
        static Image icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location).ToBitmap();
        

        protected override bool CanShowMenu()
        {
            //  We always show the menu.
            return CheckIfRunning();
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();

            //  Create a 'count lines' item.
            var itemUpload = new ToolStripMenuItem
            {
                Text = "Upload file to fritzen.tk",
                Image = icon,
            };

            //  When we click, we'll count the lines.
            itemUpload.Click += ItemUpload_Click;

            //  Add the item to the context menu.
            menu.Items.Add(itemUpload);

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
                MessageBox.Show("Please start the uploader before trying to upload!");
            }
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