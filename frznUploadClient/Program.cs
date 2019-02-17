using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using frznUpload.Shared;

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
