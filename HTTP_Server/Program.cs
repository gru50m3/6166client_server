/*
    Authors: Taylor Conners, Stephen Stroud
*/


using System;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;

namespace HTTP_Server
{
    class Program
    {
        public static Server server;
        static void Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(CancelKeyPress);

            Console.WriteLine("Starting server on port: " + args[0]);
            server = new Server(int.Parse(args[0]));
            //server = new Server(8080);
            server.StartServer();
        }

        static void CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Terminating Execution...");
            if (e.SpecialKey == ConsoleSpecialKey.ControlC || e.SpecialKey == ConsoleSpecialKey.ControlBreak)
            {
                server.GetHttpListener().Stop();
                //Console.WriteLine(server.GetHttpListener().IsListening);
                server.GetHttpListener().Close();
            }
        }
    }

    public class Server {

        public const String WEB_DIR = "/contents/web";
        public const int BUFFER_SIZE = 2048;
        public int portN;

        private HttpListener http_listener;
        private Thread serverThread;

        public Server(int port) {
            portN = port;
            http_listener = new HttpListener();
        }

        public void StartServer() {
            serverThread = new Thread(new ThreadStart(HttpRun));
            serverThread.Start();
        }

        public Thread GetThread()
        {
            return serverThread;
        }

        public HttpListener GetHttpListener()
        {
            return http_listener;
        }


        private void HttpRun()
        {
            var prefix = "http://localhost:" + portN.ToString() + "/";
            http_listener.Prefixes.Add(prefix);
            http_listener.Start();

            while (http_listener.IsListening)
            {
                Console.WriteLine("waiting for connection...");
                if (!http_listener.IsListening) break;
                HttpListenerContext context = http_listener.GetContext();
                ProcessContext(context);

            }
            Console.WriteLine("Not listening..");
            http_listener.Close();
        }

        private void ProcessContext(HttpListenerContext context)
        {

            Console.WriteLine("client connected");
            HttpListenerRequest req = context.Request;
            HttpListenerResponse resp = context.Response;

            if (req.HttpMethod == "GET")
            {
                String file = Directory.GetCurrentDirectory() + Server.WEB_DIR + req.RawUrl;
                FileInfo _file = new FileInfo(file);
                if (_file.Exists)
                {
                    Console.WriteLine("Retrieving requested file: " + _file.Name + "...");
                    //Return requested file
                    FileStream _fileStream = _file.OpenRead();
                    BinaryReader b_reader = new BinaryReader(_fileStream);
                    Byte[] d = new Byte[_fileStream.Length];
                    
                    b_reader.Read(d, 0, d.Length);
                    _fileStream.Close();

                    Console.WriteLine("Sending response...");
                    resp.StatusCode = 200;
                    resp.KeepAlive = false;
                    resp.ContentLength64 = d.Length;
                    Console.WriteLine("\n");
                    resp.OutputStream.Write(d, 0, d.Length);
                }
                else
                {
                    //File does not exist
                    byte[] b = Encoding.UTF8.GetBytes("404 Not Found");
                    resp.StatusCode = 404;
                    resp.KeepAlive = false;
                    resp.ContentLength64 = b.Length;
                    resp.OutputStream.Write(b, 0, b.Length);
                }
            }
            if (req.HttpMethod == "PUT")
            {
                try
                {
                    Console.WriteLine("Writing file: " + context.Request.Headers.Get(0) + " to the server...");
                    Byte[] incBytes = new Byte[BUFFER_SIZE];
                    BinaryReader reader = new BinaryReader(context.Request.InputStream);
                    reader.Read(incBytes, 0, incBytes.Length);

                    string dataReceived = Encoding.ASCII.GetString(incBytes, 0, incBytes.Length);
                    string fileExtension = context.Request.ContentType.Split('/')[1];
                    Console.WriteLine(dataReceived);

                    File.WriteAllBytes(Directory.GetCurrentDirectory() + "/contents/web/" + context.Request.Headers.Get(0), incBytes);

                    byte[] b = Encoding.UTF8.GetBytes("200 OK");
                    resp.StatusCode = 200;
                    resp.KeepAlive = false;
                    resp.ContentLength64 = b.Length;
                    resp.OutputStream.Write(b, 0, b.Length);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            if (req.HttpMethod != "PUT" && req.HttpMethod != "GET")
            {
                byte[] b = Encoding.UTF8.GetBytes("400 Bad Request");
                resp.StatusCode = 400;
                resp.KeepAlive = false;
                resp.ContentLength64 = b.Length;
                resp.OutputStream.Write(b, 0, b.Length);
            }
            resp.Close();
        }

    }



}
