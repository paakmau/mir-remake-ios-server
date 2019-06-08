using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using MirRemakeBackend.GameLogic;

namespace MirRemakeBackend.Network {
    interface INetworkService {
        void SendServerCommand (ServerCommandBase command);
    }
    class NetworkService : INetEventListener, INetworkService {
        private const int c_serverPort = 23333;
        private const int c_maxClientNum = 3000;
        private NetManager m_serverNetManager;
        private Dictionary<int, NetPeer> m_netIdAndPeerDict = new Dictionary<int, NetPeer> ();
        private Dictionary<int, int> m_peerIdAndNetworkIdDict = new Dictionary<int, int> ();
        private Dictionary<int, int> m_networkIdAndPeerIdDict = new Dictionary<int, int> ();
        private Dictionary<NetworkToServerDataType, IClientCommand> m_clientCommandDict = new Dictionary<NetworkToServerDataType, IClientCommand> ();
        private NetDataWriter m_writer = new NetDataWriter ();
        public NetworkService () {
            // 初始化LiteNet
            m_serverNetManager = new NetManager (this);
            m_serverNetManager.Start (c_serverPort);
            // 实例化所有实现了ClientCommand接口的class
            var ccType = typeof (IClientCommand);
            var ccImplTypes = AppDomain.CurrentDomain.GetAssemblies ().SelectMany (s => s.GetTypes ()).Where (p => p.IsClass && ccType.IsAssignableFrom (p));
            foreach (var type in ccImplTypes) {
                IClientCommand cc = type.GetConstructor (Type.EmptyTypes).Invoke (null) as IClientCommand;
                m_clientCommandDict.Add (cc.m_DataType, cc);
            }
        }
        public void Tick () {
            m_serverNetManager.PollEvents ();
        }
        private void ReceiveClientCommand (NetPacketReader reader, int clientNetId) {
            IClientCommand command = m_clientCommandDict[(NetworkToServerDataType) reader.GetByte ()];
            command.Execute (reader, clientNetId);
        }
        public void SendServerCommand (ServerCommandBase command) {
            m_writer.Put ((byte) command.m_DataType);
            command.PutData (m_writer);
            Console.WriteLine (command.m_DataType);
            for (int i = 0; i < command.m_toClientList.Count; i++)
                m_netIdAndPeerDict[command.m_toClientList[i]].Send (m_writer, command.m_DeliveryMethod);
            m_writer.Reset ();
        }
        public void OnConnectionRequest (ConnectionRequest request) {
            request.AcceptIfKey ("client");
        }
        public void OnNetworkError (IPEndPoint endPoint, SocketError socketError) {
            Console.WriteLine ("网络错误, 客户终端: " + endPoint + ", 错误信息: " + socketError);
        }
        public void OnNetworkLatencyUpdate (NetPeer peer, int latency) { }
        public void OnNetworkReceive (NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod) {
            ReceiveClientCommand (reader, m_peerIdAndNetworkIdDict[peer.Id]);
            reader.Recycle ();
        }
        public void OnNetworkReceiveUnconnected (IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }
        public void OnPeerConnected (NetPeer peer) {
            // 超过连接上限
            if (m_netIdAndPeerDict.Count >= c_maxClientNum)
                return;
            // 分配NetId
            int netId = GameLogicInit.s_instance.AssignNetworkId ();

            // 索引并保存peer
            m_peerIdAndNetworkIdDict[peer.Id] = netId;
            m_networkIdAndPeerIdDict[netId] = peer.Id;
            m_netIdAndPeerDict[netId] = peer;
            // 发送NetId
            SendServerCommand (SC_InitSelfNetworkId.Instance (new List<int> { netId }, netId));
            Console.WriteLine (peer.Id + "连接成功");
        }
        public void OnPeerDisconnected (NetPeer peer, DisconnectInfo disconnectInfo) {
            var netId = m_peerIdAndNetworkIdDict[peer.Id];
            m_netIdAndPeerDict.Remove (netId);
            m_peerIdAndNetworkIdDict.Remove (peer.Id);
            m_networkIdAndPeerIdDict.Remove (netId);
            // 移除角色
            GameLogicInit.s_instance.CommandRemoveCharacter (netId);
            Console.WriteLine (peer.Id + "断开连接, 客户终端: " + peer.EndPoint + ", 断线原因: " + disconnectInfo.Reason);
        }
    }
}