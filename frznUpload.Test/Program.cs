﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using frznUpload.Shared;

namespace frznUpload.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            meth().Wait();

            Console.ReadLine();

        }

        static async Task meth()
        {
            Stopwatch stp = new Stopwatch();
            stp.Start();
            Client cli = new Client("fritzen.tk", 22340);

            string file = "key.key";

            if (!await cli.AuthWithKey(file))
            {
                while (!cli.IsAuthenticated)
                {
                    Console.WriteLine("name:");
                    string name = Console.ReadLine();
                    Console.WriteLine("pass:");
                    string password = Console.ReadLine();
                    await cli.AuthWithPass(name, password, file);
                }

            }

            stp.Stop();

            Console.WriteLine(cli.Name);
            Console.WriteLine(stp.ElapsedMilliseconds);

            Console.WriteLine("Upload a File:");
            //string path = @"C:\Users\fritzen\Downloads\DSC_0138.JPG";//
            string path = Console.ReadLine();

            //while (true)
            //{
                var up = await cli.UploadFile(path);

                while (true)
                {
                    if (up.Finished)
                    {
                        Console.WriteLine("Finish: " + up.Identifier);
                        break;
                    }

                    Console.WriteLine(Math.Round(up.Progress * 100, 2));

                    await Task.Delay(100);
                }

                var files = await cli.GetFiles();

                Console.WriteLine(string.Join("\n", files));

            //}
        }
    }
}
