/*
    Authors:Taylor Conners, Stephen Stroud 
*/

using System;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using System.IO;

namespace HTTP_client
{
    class Program
    {
        private static String hostname, port, command, filename;
        static void Main(string[] args)
        {
            using (HttpClient client = new HttpClient())
            {
                hostname = args[0]; port = args[1]; command = args[2].ToUpper(); filename = args[3];
                //IPHostEntry ipHost = Dns.GetHostEntry(hostname);

                if (command == "GET")
                {
                    string fullPath = hostname + ":" + port + "/" + filename;
                    GetRequest(fullPath, client).Wait();
                }

                if (command == "PUT".ToUpper())
                {
                    string fullPath = hostname + ":" + port + "/web";
                    string filePath = Directory.GetCurrentDirectory() + "\\files\\" + filename;
                    PutRequest(fullPath, filePath, client).Wait();
                }
            }
        }

        protected static async Task GetRequest(String path, HttpClient client)
        {
            using (HttpResponseMessage response = await client.GetAsync(path))
            {
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("200 OK");
                }
                using (HttpContent content = response.Content)
                {
                    string msg = await content.ReadAsStringAsync();
                    Console.WriteLine(msg);
                }
            }
        }

        protected static async Task PutRequest(String path, String filePath, HttpClient client)
        {

            FileInfo _file = new FileInfo(filePath);
            String extensionType = _file.Extension.Split('.')[1];
            
            Console.WriteLine(extensionType);
            if (_file.Exists)
            {
                FileStream stream = _file.OpenRead();
                HttpContent fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/" + extensionType);
                fileContent.Headers.Add("fileName", _file.Name);
                using (HttpResponseMessage response = await client.PutAsync(path, fileContent))
                {
                    
                    using (HttpContent content = response.Content)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("200 OK");
                        }
                        string msg = await content.ReadAsStringAsync();
                        Console.WriteLine(msg);
                    }
                }
                
            }
            
        }
    }
}
