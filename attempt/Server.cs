using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ConsoleApp1
{
    class Server
    {
        public static void Main(String[] args)
        {
            TcpListener server = new TcpListener(IPAddress.Parse(GetLocalIPAddress()), int.Parse(args[0]));
            Console.WriteLine(GetLocalIPAddress() + "\nServer started.\n");
            while (true)
            {
                server.Start();
                Console.WriteLine("Step\n");
                using (var tcpClient = server.AcceptTcpClient())
                {
                    Console.WriteLine("Accepted connection from client\n");

                    using (var networkStream = tcpClient.GetStream())
                    {
                        byte[] buffer = new byte[tcpClient.ReceiveBufferSize];

                        int bytesRead = networkStream.Read(buffer, 0, tcpClient.ReceiveBufferSize);

                        string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                        if (dataReceived.Substring(0, dataReceived.IndexOf(" ")).Equals("PUT"))
                        {
                            int start;
                            if (dataReceived.LastIndexOf("\\") == -1)
                                start = 0;
                            else
                                start = dataReceived.LastIndexOf("\\");
                            using (var result = File.Create("C:\\" + dataReceived.Substring(start)))
                            {
                                Console.WriteLine("Client connected. Starting to receive the file");

                                var buffer2 = new byte[1024];
                                int bytesRead2;
                                while ((bytesRead2 = networkStream.Read(buffer2, 0, buffer2.Length)) > 0)
                                {
                                    result.Write(buffer2, 0, bytesRead2);
                                }
                                byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes("200 OK File Created");
                                //---send the text---
                                networkStream.Write(bytesToSend, 0, bytesToSend.Length);
                            }
                        }
                        else if (dataReceived.Substring(0, dataReceived.IndexOf(" ")).Equals("GET"))
                        {
                            byte[] fileSizeBytes = new byte[4];
                            int bytes = networkStream.Read(fileSizeBytes, 0, 4);
                            int dataLength = BitConverter.ToInt32(fileSizeBytes, 0);
                            int bytesLeft = dataLength;
                            byte[] data = new byte[dataLength];
                            bytesRead = 0;
                            int bufferSize = 1024;

                            while (bytesLeft > 0)
                            {
                                int curDataSize = Math.Min(bufferSize, bytesLeft);
                                if (tcpClient.Available < curDataSize)
                                    curDataSize = tcpClient.Available;

                                bytes = networkStream.Read(data, bytesRead, curDataSize);

                                bytesRead += curDataSize;
                                bytesLeft -= curDataSize;
                            }
                        }
                    }
                    Console.WriteLine("Stopped read loop");
                }
                server.Stop();
            }
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static void ReceiveFile(int portN)
        {

        }
    }
}
