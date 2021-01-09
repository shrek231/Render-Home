using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using RestSharp;
using RestSharp.Extensions;

namespace Render {
    class Program
    {
        public static string blendpath = null;
        public static string sys = null;
        public static OperatingSystem os;
        public static bool running = true;
        static void Main(string[] args)
        {
            //get os
            os = Environment.OSVersion;
            string ostring = os.ToString();
            string[] osplit = ostring.Split(' ');
            if (osplit[0] == "Unix") {
                sys = "linux";
            }
            if (osplit[0] == "Microsoft") {
                sys = "windows";
            }
            //make dir
            string path = "none";
            if (sys == "windows") {
                path = AppDomain.CurrentDomain.BaseDirectory+@"img";
                blendpath = AppDomain.CurrentDomain.BaseDirectory+@"ble";
            } else {
                path = AppDomain.CurrentDomain.BaseDirectory+"img";
                blendpath = AppDomain.CurrentDomain.BaseDirectory+"ble";
            }

            DirectoryInfo dire = Directory.CreateDirectory(path);
            DirectoryInfo di = Directory.CreateDirectory(blendpath);
            //init loop
            while (running) {
                Thread.Sleep(1000);
                var client = new RestClient("https://BlenderRenderServer.youtubeadminist.repl.co/requestFrame");
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request); //call at anytime in code
                WebClient Client = new WebClient ();
                Client.DownloadFile("https://BlenderRenderServer.youtubeadminist.repl.co/getBlend", blendpath);
                Console.WriteLine("\ngot new frame "+response.Content);
                if (sys == "linux") {
                    ProcessStartInfo procStartInfo = new ProcessStartInfo("/bin/bash","blender -b "+blendpath+"render.blend -o "+path+" -f "+response.Content.ToString());
                    procStartInfo.RedirectStandardOutput = true;
                    procStartInfo.UseShellExecute = false;
                    procStartInfo.CreateNoWindow = true;
                    Process proc = new Process();
                    proc.StartInfo = procStartInfo;
                    proc.Start();
                } else {
                    Process process = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "\"C:\\Program Files\\Blender Foundation\\Blender 2.91\\blender.exe\" -b "+blendpath+"render.blend -o "+path+" -f "+response.Content.ToString(); //add blender to path
                    process.StartInfo = startInfo;
                    process.Start();
                }
                Console.WriteLine("requesting new frame");
                request.AddParameter("file", path + response.Content.ToString() + ".png");
                var client1 = new RestClient("https://BlenderRenderServer.youtubeadminist.repl.co/sendFrame" + response.Content.ToString());
                var request1 = new RestRequest(Method.POST);
                IRestResponse response1 = client.Execute(request1);
            }
        }
    }
}