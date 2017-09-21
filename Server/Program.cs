using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Program
    {
        public static UInt32 clientId = 0;

        private static byte[] _buffer = new byte[1024];
        private static Dictionary<int, ConnectedClient> _clientSockets = new Dictionary<int, ConnectedClient>();
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        static void Main(string[] args)
        {
            SetupServer();
            Console.ReadLine();
        }

        private static void SetupServer()
        {
            Console.WriteLine("Setting up server...");
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 100));
            _serverSocket.Listen(5);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            Console.WriteLine("Server is running!");

        }

        private static void AcceptCallback(IAsyncResult AR)
        {
            
            Socket socket = _serverSocket.EndAccept(AR);

            _clientSockets.Add(socket.GetHashCode(), new ConnectedClient(clientId, socket));
            Console.WriteLine("Client connected Socket hash: " + socket.GetHashCode() + " Client ID: " + _clientSockets[socket.GetHashCode()]._clientId);

            clientId++;  

            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallback), socket);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        //Callback when recieving data
        private static void RecieveCallback(IAsyncResult AR)
        {
            try
            {
                Socket socket = (Socket)AR.AsyncState;
                int recieved = socket.EndReceive(AR);

                if (recieved == 0)
                {
                    return;
                }

                byte[] dataBuff = new byte[recieved];
                Array.Copy(_buffer, dataBuff, recieved);

                byte[] temp_msg = new byte[2];
                byte[] data_rec = new byte[dataBuff.Length - 4];
                byte[] packet_len = new byte[2];

                if (dataBuff.Length >= 4)
                {
                    Array.Copy(dataBuff, temp_msg, 2);
                    Array.Copy(dataBuff, 2, packet_len, 0, 2);
                    Array.Copy(dataBuff, 4, data_rec, 0, dataBuff.Length - 4);
                }

                //Extracting packetLen and messageNo
                UInt16 packetLen = BitConverter.ToUInt16(packet_len, 0);
                UInt16 messageNo = BitConverter.ToUInt16(temp_msg, 0);

                Console.WriteLine("-------------------------------------------------------------------");
                Console.WriteLine("Processing Client ID: " + _clientSockets[socket.GetHashCode()]._clientId + " reqest!");
                PrintRecievedMessage(messageNo, packetLen, data_rec);
                Console.WriteLine("\nFull byte dump: " + ServerMethods.PrintBytes(dataBuff));


                byte[] data_send = new byte[512];
                //Process messages
                switch (messageNo)
                {
                    case MessageNO.USERNAME_SET:
                        string name = Encoding.ASCII.GetString(data_rec);
                        UInt16 result = _clientSockets[socket.GetHashCode()].SetUserName(name);
                        data_send = ServerMethods.ErrorCodePacket(MessageNO.USERNAME_RESPONSE, result);
                        if(result == 1)
                        {
                            Console.WriteLine("Username OK! Client ID: " + _clientSockets[socket.GetHashCode()]._clientId + " UserName set to: " + name);
                        }
                        else
                        {
                            Console.WriteLine("Username BAD!");
                        }
                        break;

                    default:
                        Console.WriteLine("Unknown messageNo");
                        break;
                }

                //All packet handling happens here!
                if (data_send.Length > 0)
                {
                    Console.WriteLine("\nSending data:");
                    Console.WriteLine(ServerMethods.PrintBytes(data_send));
                    socket.BeginSend(data_send, 0, data_send.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                }
                Console.WriteLine("-------------------------------------------------------------------");

                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(RecieveCallback), socket);
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void PrintRecievedMessage(UInt16 messageNO, UInt16 packetLength, byte[] data)
        {
            Console.WriteLine("Recieved data: [Note Message Number and Packet Length take 4bytes!]");
            Console.WriteLine(string.Format("Message number: {0} Packet Length: {1}\n", messageNO, packetLength));
            Console.WriteLine(string.Format("Data\nHex:{0}\nString:\n{1}", ServerMethods.PrintBytes(data), Encoding.ASCII.GetString(data)));
        }

        private static void SendCallback(IAsyncResult AR)
        {
            try
            {
                Socket clientSocket = (Socket)AR.AsyncState;
                clientSocket.EndSend(AR);
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
