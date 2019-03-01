using System;
using System.IO;

namespace frznUpload.Client
{
    class ArgumentsHandler
    {
        private MainForm Form;

        public ArgumentsHandler(MainForm form)
        {
            Form = form;
        }

        public void HandleArguments(string[] args)
        {
            if (args.Length == 0)
                Form.Invoke(new Action(Form.Show));
            else
            {
                foreach(string s in args)
                {
                    if (File.Exists(s))
                    {
                        FileUploadHandler.UploadFile(s);
                    }else if (Directory.Exists(s))
                    {
                        FileUploadHandler.UploadDirectory(s);
                    }
                }
            }
        }
    }
}