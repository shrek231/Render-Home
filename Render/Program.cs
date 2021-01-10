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
    class Program {
        public static string blendpath = null;
        public static string sys = null;
        public static OperatingSystem os;
        public static bool running = true;
        static void Main(string[] args) {
            os = Environment.OSVersion;//get os
            string ostring = os.ToString();
            string[] osplit = ostring.Split(' ');
            if (osplit[0] == "Unix") {
                sys = "linux";
            } if (osplit[0] == "Microsoft") {
                sys = "windows"; 
            } 
            string path = "none"; //make dir
            if (sys == "windows") {
                path = AppDomain.CurrentDomain.BaseDirectory+@"img\";
                blendpath = AppDomain.CurrentDomain.BaseDirectory+@"ble\";
            } else {
                path = AppDomain.CurrentDomain.BaseDirectory+"img/";
                blendpath = AppDomain.CurrentDomain.BaseDirectory+"ble/";
            } if (Directory.Exists(path)) {
                Console.WriteLine("");
            } else {
                DirectoryInfo dire = Directory.CreateDirectory(path);
                DirectoryInfo di = Directory.CreateDirectory(blendpath);
            }
            int timeout = 1000; //start loop
            int fail = 0;
            while (running) {
                if (fail > 3) {
                    Console.WriteLine("no new frames, waiting 30 seconds.");
                    timeout = 30000;
                } 
                Thread.Sleep(timeout);
                Console.WriteLine("Requesting new frame");
                var client = new RestClient("https://BlenderRenderServer.youtubeadminist.repl.co/requestFrame");
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request); //call at anytime in code
                WebClient Client = new WebClient ();
                Client.DownloadFile("https://BlenderRenderServer.youtubeadminist.repl.co/getBlend", blendpath+"render.blend");
                if (response.Content == "-1") {
                    Console.WriteLine("No new frames");
                    fail += 1;
                } else {
                    fail = 0;
                    Console.WriteLine("\nGot new frame "+response.Content);
                } if (sys == "windows") {
                    Process process = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/C blender.exe -b " + blendpath + "render.blend -o " + path + " -f " + response.Content;
                    process.StartInfo = startInfo;
                    process.Start();
                } else {
                    ProcessStartInfo procStartInfo = new ProcessStartInfo("/bin/bash","-c blender -b " + blendpath + "render.blend -o " + path + " -f " + response.Content);
                    procStartInfo.RedirectStandardOutput = true;
                    procStartInfo.UseShellExecute = false;
                    procStartInfo.CreateNoWindow = true;
                    Process proc = new Process();
                    proc.StartInfo = procStartInfo;
                    proc.Start();
                } try {
                    Console.WriteLine("Trying to send frame");
                    if (sys == "windows") {
                        Process process = new Process();
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.FileName = "cmd.exe";
                        startInfo.Arguments = "/C curl -F "+response.Content+"=@"+path+response.Content+".png"+" blenderrenderserver.youtubeadminist.repl.co/sendFrame";
                        process.StartInfo = startInfo;
                        process.Start();
                    } else {
                        ProcessStartInfo procStartInfo = new ProcessStartInfo("/bin/bash","-c curl -F "+response.Content+"=@"+path+response.Content+".png"+" blenderrenderserver.youtubeadminist.repl.co/sendFrame");
                        procStartInfo.RedirectStandardOutput = true;
                        procStartInfo.UseShellExecute = false;
                        procStartInfo.CreateNoWindow = true;
                        Process proc = new Process();
                        proc.StartInfo = procStartInfo;
                        proc.Start();
                    }
                } catch(Exception e){
                    Console.WriteLine(e);
                }
            }
        }
    }
}