using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;

namespace WebAPIClient
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static string hostname, port, command, filename;
        static void Main(string[] args)
        {
            if (args.Length == 4)
            {
                hostname = args[0];
                port = args[1];
                command = args[2];
                filename = args[3];

                GetRequest(hostname, filename, port).Wait();
            }
        }

        protected static async Task GetRequest(string url, string file, string portN) {
            string fullPath = url + ":" + portN + "/" + file;
            Console.WriteLine(fullPath);
            try 
            {
                HttpResponseMessage response = await client.GetAsync(fullPath);
                response.EnsureSuccessStatusCode();
                Console.WriteLine("Success");
            }
            catch(HttpRequestException e) 
            {
                Console.WriteLine("Failure");
                Console.WriteLine(e);
            }
            response.Dispose();
            //using (HttpResponseMessage response = await client.GetAsync(fullPath)) {
            //    using (HttpContent content = response.Content) {
            //        HttpContentHeaders headers = content.Headers;
            //        Console.WriteLine(headers);
            //    }
            //}
        } 

    }
}
