using System;
using RestSharp;

namespace Render {
    class Program {
        static void Main(string[] args) {
            //init restsharp
            var client = new RestClient("https://shranime.org");
            var request = new RestRequest(Method.POST);
            IRestResponse response = client.Execute(request); //call at anytime in code
            
            Console.WriteLine(response.Content.ToString());
        }
    }
}