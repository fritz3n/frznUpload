using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using frznUpload.Shared;
using System.IO;

namespace frznUpload.Client
{
    static class Program
    { 
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            FileInfo f = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location);

            Directory.SetCurrentDirectory(f.Directory.FullName);

            ExplorerIntegrationHandler.Init();

            if (PipeHandler.IsServer)
            {

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new IconHandler(args));
            }
            else
            {
                PipeHandler.SendMessage(args);
            }
        }
    }
}
