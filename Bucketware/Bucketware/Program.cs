using Bucketware.View;
using System;
using System.Windows.Forms;
using System.IO;
using System.Reflection.Emit;
using System.Drawing;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Runtime.InteropServices;
using Bucketware.PluginSystem;

namespace Bucketware
{
    static class Program
    {
        static string path = @"C:\Users\" + Environment.UserName + @"\AppData\Local\Growtopia\Growtopia.exe";
        static string dir = @"C:\Users\" + Environment.UserName + @"\AppData\Local\Growtopia";
        static string cachepath = @"C:\BCache";
        
        static void CheckGT()
        {
            Colorful.Console.WriteLine("[+] Checking GT Path", Color.Yellow);
            if (File.Exists(path))
                return;
            Colorful.Console.WriteLine("[!] You dont have gt installed on default path lets change the gt path on bucketware.", Color.Red);
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (!Directory.Exists(dir))
            {
                openFileDialog1.InitialDirectory = "c:\\";
            }
            else
            {
                openFileDialog1.InitialDirectory = dir;
            }
            openFileDialog1.Filter = "Exe files (*.exe)|*.exe";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedFileName = openFileDialog1.FileName;
                path = selectedFileName;
                var filecreator = File.Create(cachepath + @"\gtpath.bware");
                filecreator.Close();
                File.WriteAllText(cachepath + @"\gtpath.bware", selectedFileName);
                Colorful.Console.WriteLine("[+] Succesfully changed to " + path, Color.Yellow);

            }

        }
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        static void CheckIFGTopen()
        {
            Colorful.Console.WriteLine("[+] Killing Growtopia.exe if its already running..", Color.Yellow);
            try
            {
                //Taskill growtopia if already running...
                Process[] procs = Process.GetProcessesByName("Growtopia");
                foreach (Process p in procs) { p.Kill(); }
            }
            catch
            {
                //Growtopia is not running do nothing
            }
        }

        static void CheckUpdates()
        {
            //uPDATER CLOSED < Discord : Sahyui#1337 >
            Colorful.Console.WriteLine("[Updater] Closed. < Discord : Sahyui#1337 >", Color.Blue);
           /* WebClient client = new WebClient();
            //BWare.version = client.DownloadString("https://textbin.net/raw/j0fzqocots");
            if (!client.DownloadString("https://textbin.net/raw/j0fzqocots").Contains("4.0"))
            {
                Colorful.Console.WriteLine("[Updater] New version is avaible installing it..", Color.Blue);
                Thread.Sleep(2000);
                string downloadurl = client.DownloadString("https://textbin.net/raw/ycmitfymd9");
                client.DownloadFile(downloadurl, Application.StartupPath + "/NewBucketware.exe");
                Process.Start(Application.StartupPath + "/BucketUpdateHandler.exe");
                Application.Exit();
            }
            else
            {
                Colorful.Console.WriteLine("[Updater] Bucketware is running latest version", Color.Blue);
            }*/
        }
        [STAThread]
        static void Main()
        {
            Colorful.Console.Title = "Bucketware";
            CheckUpdates();
            if (File.Exists(cachepath + @"\gtpath.bware"))
            {
                Colorful.Console.WriteLine("[+] Succesfully Loaded GT Path: " + path, Color.Yellow);
                path = File.ReadAllText(cachepath + @"\gtpath.bware");
            }
            CheckGT();
            CheckIFGTopen();
            try
            {
                Process.Start(path);
            }
            catch
            {

            }
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
            //PluginHandler ph = new PluginHandler();
            //ph.LoadPlugins();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Bucket());
        }
    }
}
