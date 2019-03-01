using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace frznUpload.Client
{
    static class ExplorerIntegrationHandler
    {
        private static RegistryKey rkApp = Registry.CurrentUser.CreateSubKey("SOFTWARE\\" + AppName);
        private const string AppName = "frznUpload";
        private const string PathKey = "path";
        private const string EnabledKey = "explorerEnabled";

        public static void Init()
        {
            if (!IsPathNull() && !PathIsSame())
            {
                SetToCurrentPath();
            }
        }

        public static bool Enable()
        {
            if (IsEnabled())
                return true;
            
            string dll = ExtractResource("frznUpload.Client.ExplorerServer.dll");
            string dependency = ExtractResource("SharpShell.dll");
            string srm = ExtractResource("ServerRegistrationManager.exe");

            //string regasm = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory() + "regasm.exe";

            Process p = new Process();
            p.StartInfo.FileName = srm;
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.Verb = "runas";
            p.StartInfo.Arguments = $"install \"{dll}\" -codebase";
            try
            {
                p.Start();
                
                p.WaitForExit();

                Console.WriteLine(p.ExitCode);
                
                File.Delete(srm);
                SetEnabledValue(true);
                return true;
            }
            catch
            {
                MessageBox.Show("Sorry for the inconvenience, but Administrator rights are needed for this");

                try
                {
                    File.Delete(srm);
                    File.Delete(dll);
                    File.Delete(dependency);
                }catch{ }
                return false;
            }
        }

        private static string ExtractResource(string name)
        {
            if (File.Exists(name))
                return Directory.GetCurrentDirectory() + "/" + name;

            Stream stream = typeof(MainForm).Assembly.GetManifestResourceStream("frznUpload.Client.ExplorerResources." + name);
            byte[] bytes = new byte[(int)stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            File.WriteAllBytes(name, bytes);
            stream.Dispose();

            return Directory.GetCurrentDirectory() + "/" + name;
        }

        public static void Disable()
        {
            if (!IsEnabled())
                return;

            string dll = ExtractResource("frznUpload.Client.ExplorerServer.dll");
            string dependency = ExtractResource("SharpShell.dll");
            string srm = ExtractResource("ServerRegistrationManager.exe");

            Process p = new Process();
            p.StartInfo.FileName = srm;
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.Verb = "runas";
            p.StartInfo.Arguments = $"uninstall \"{dll}\"";
            p.Start();
            
            p.WaitForExit();

            Console.WriteLine(p.ExitCode);
            SetEnabledValue(false);
        }

        public static bool IsEnabled()
        {
            if (IsEnabledNull())
                return false;

            try
            {
                return (string)rkApp.GetValue(EnabledKey) == "true" ? true : false;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsPathNull()
        {
            return rkApp.GetValue(PathKey) == null;
        }

        public static bool IsEnabledNull()
        {
            return rkApp.GetValue(EnabledKey) == null;
        }

        public static bool PathIsSame()
        {
            if ((string)rkApp.GetValue(PathKey) == GetExecutingPath())
            {
                return true;
            }
            return false;
        }

        public static string GetExecutingPath()
        {
            return System.Windows.Forms.Application.ExecutablePath;
        }

        public static void SetPathValue(string val)
        {
            rkApp.SetValue(PathKey, val);
        }

        public static void SetEnabledValue(bool val)
        {
            rkApp.SetValue(EnabledKey, val ? "true" : "false");
        }

        public static void SetToCurrentPath()
        {
            SetPathValue(GetExecutingPath());
        }
        
    }
}
