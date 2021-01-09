using System;
using System.IO;
using RestSharp;

namespace Render {
    class Program
    {
        public static string os = null;
        public static bool running = true;
        static void Main(string[] args)
        {
            //get os
            if (OperatingSystem.IsWindows()) {
                os = "windows";
            }
            if (OperatingSystem.IsLinux()) {
                os = "linux";
            } else {
                Console.WriteLine("Unknown OS or unsupported");
            }
            //make dir
            string path = "none";
            if (os == "windows") {
                path = @"C:\img\";
            } else {
                path = "/home/img/";
            }
            DirectoryInfo di = Directory.CreateDirectory(path);
            //init loop
            while (running) {
                var client = new RestClient("http://url.com?requestFrame?=" + os); //should respond with frame number and send linux or windows command to start correct blender version
                var request = new RestRequest(Method.POST);
                IRestResponse response = client.Execute(request); //call at anytime in code
                Console.WriteLine(response.ToString()); // response should be something like "1:blender -b *.blend -o output/ -a -f" the -f at the end is because i add the frame number to the end of the string
                string[] responce2 = response.ToString().Split(':');
                Console.WriteLine(responce2[0]);
                Console.WriteLine(responce2[1] + " " + responce2[0] + ".png");
                //run responce2[1]+" "+responce2[0]] which should be the command to run blender
                request.AddParameter("file", path + responce2[0] + ".png");
                var client1 = new RestClient("http://url.com?sendFrame?frameNum=" + responce2[0]);
                var request1 = new RestRequest(Method.POST);
                IRestResponse response1 = client.Execute(request1);
            }
        }
    }
}
