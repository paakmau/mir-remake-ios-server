using System;
using System.Text;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MirRemake {
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("HelloWorld");
            TestUdp();
        }
        static void TestUdp() {
            int port = 2333;

            EventBasedNetListener serverListener = new EventBasedNetListener();
            NetManager server = new NetManager(serverListener);
            bool temp = server.Start(port);
            
            serverListener.ConnectionRequestEvent += request => {
                request.AcceptIfKey("client");
            };

            serverListener.PeerConnectedEvent += peer => {
                Console.WriteLine("新的连接, 用户ip: " + (peer.EndPoint));
                NetDataWriter writer = new NetDataWriter();
                writer.Put("Hello, client");
                peer.Send(writer, DeliveryMethod.ReliableOrdered);
            };

            serverListener.NetworkReceiveEvent += (peer, reader, method) => {
                string clientMessage = ((NetworkSendDataType)reader.GetByte()).ToString();
                Console.WriteLine(clientMessage);
            };
            
            serverListener.PeerDisconnectedEvent += (peer, info) => {
                Console.WriteLine("连接断开, 用户ip: " + (peer.EndPoint) + info);
            };

            while(!Console.KeyAvailable) {
                server.PollEvents();
                Thread.Sleep(15);
            }
            server.Stop();
        }
    }
}
