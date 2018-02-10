using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;


namespace ConsoleApp1
{
    class Client
    {
        public static void Main(String[] args)
        {
            String host = args[0].Trim();
            int port = int.Parse(args[1]);
            String command = args[2].ToUpper();
            String file = args[3].Trim();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            var builder = new UriBuilder("https", ipHost.AddressList[0].ToString(), port);
            var uri = builder.Uri;
            Console.WriteLine("Pass");
            using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                Console.WriteLine("Pass2");
                client.Connect(ipHost.AddressList[0].ToString(), port);
                Console.WriteLine("Client connected.\n");
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
                request.Method = command.Trim();

                if (request.Method.Equals("GET"))
                {
                    String test = String.Empty;
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        Console.WriteLine("Pass3");
                        Stream dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        test = reader.ReadToEnd();
                        reader.Close();
                        dataStream.Close();
                    }
                    Console.WriteLine(test);
                }
                else if (request.Method.Equals("PUT"))
                {
                    Console.WriteLine("Hey");
                    int bufferSize = 1024;
                    byte[] bs = new byte[1024];
                    byte[] data = File.ReadAllBytes(file);
                    var netStream = new NetworkStream(client);
                    String text = command.Trim() + " " + file;
                    byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(text);
                    //---send the text---
                    Console.WriteLine("Sending : " + text);
                    netStream.Write(bytesToSend, 0, bytesToSend.Length);

                    // Build the package
                    byte[] dataLength = BitConverter.GetBytes(data.Length);
                    byte[] package = new byte[4 + data.Length];
                    dataLength.CopyTo(package, 0);
                    data.CopyTo(package, 4);

                    // Send to server
                    int bytesSent = 0;
                    int bytesLeft = package.Length;

                    while (bytesLeft > 0)
                    {

                        int nextPacketSize = (bytesLeft > bufferSize) ? bufferSize : bytesLeft;

                        netStream.Write(package, bytesSent, nextPacketSize);
                        bytesSent += nextPacketSize;
                        bytesLeft -= nextPacketSize;

                    }
                    Console.WriteLine("We did it! Maybe...");
                    int bytesRead = netStream.Read(bs, 0, bs.Length);
                    string dataReceived = Encoding.ASCII.GetString(bs, 0, bytesRead);
                    Console.WriteLine("Received : " + dataReceived);

                    netStream.Close();
                    client.Close();

                    //using (var myStream = new NetworkStream(client))
                    //{
                    //    using (StreamWriter streamWriter = new StreamWriter(myStream))
                    //    {
                    //        // request line
                    //        streamWriter.Write("POST " + file + " HTTP/1.0\r\n");

                    //        // headers
                    //        streamWriter.Write("Host: " + host + "\r\n");
                    //        streamWriter.Write("Content-Type: application/x-www-form-urlencoded\r\n");
                    //        streamWriter.Write("Content-Length: 32\r\n");
                    //        streamWriter.Write("\r\n");

                    //        // body
                    //        streamWriter.Write("\r\n");
                    //        streamWriter.Flush();
                    //    }
                    //}
                    //SendFile(file, ipHost.AddressList[0].ToString(), port);
                }
            }
        }

        //public static void SendFile(string file, string IPA, Int32 PortN)
        //{
        //    FileTransfer fileTransfer = new FileTransfer();
        //    fileTransfer.Name = "TestFile";
        //    fileTransfer.Content = System.Convert.ToBase64String(File.ReadAllBytes(file));
        //    System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(fileTransfer.GetType());
        //    TcpClient client = new TcpClient();
        //    client.Connect(IPAddress.Parse(IPA), PortN);
        //    Stream stream = client.GetStream();
        //    x.Serialize(stream, fileTransfer);
        //    client.Close();

        //    //int BufferSize = 1024;
        //    //byte[] SendingBuffer = null;
        //    //TcpClient client = null;
        //    //String text = "";
        //    //NetworkStream netstream = null;
        //    //try
        //    //{
        //    //    client = new TcpClient(IPA, PortN);
        //    //    text = "Connected to the Server...\n";
        //    //    netstream = client.GetStream();
        //    //    FileStream Fs = new FileStream(file, FileMode.Open, FileAccess.Read);
        //    //    int NoOfPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Fs.Length) / Convert.ToDouble(BufferSize)));
        //    //    int Maximum = NoOfPackets;
        //    //    int TotalLength = (int)Fs.Length, CurrentPacketLength = 0;
        //    //    for (int i = 0; i < NoOfPackets; i++)
        //    //    {
        //    //        if (TotalLength > BufferSize)
        //    //        {
        //    //            CurrentPacketLength = BufferSize;
        //    //            TotalLength = TotalLength - CurrentPacketLength;
        //    //        }
        //    //        else
        //    //            CurrentPacketLength = TotalLength;
        //    //        SendingBuffer = new byte[CurrentPacketLength];
        //    //        Fs.Read(SendingBuffer, 0, CurrentPacketLength);
        //    //        netstream.Write(SendingBuffer, 0, (int)SendingBuffer.Length);
        //    //    }
        //    //    text = text + "Sent " + Fs.Length.ToString() + " bytes to the server";
        //    //    Fs.Close();
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    Console.WriteLine(ex.Message);
        //    //}
        //    //finally
        //    //{
        //    //    netstream.Close();
        //    //    client.Close();
        //    //}
        //}
    }
}