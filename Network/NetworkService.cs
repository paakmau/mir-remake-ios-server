using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MirRemake {
    class NetworkService : INetEventListener {
        public static NetworkService s_instance = new NetworkService ();
        private const int c_serverPort = 23333;
        private const int c_maxClientNum = 3000;
        private NetManager m_serverNetManager;
        private Dictionary<int, NetPeer> m_networkIdAndPeerDict = new Dictionary<int, NetPeer> ();
        private Dictionary<int, E_ActorUnit> m_networkIdAndActorUnitDict = new Dictionary<int, E_ActorUnit> ();
        private Dictionary<NetworkSendDataType, IClientCommand> m_clientCommand = new Dictionary<NetworkSendDataType, IClientCommand>();
        private NetDataWriter m_writer = new NetDataWriter();
        public void Init () {
            // 初始化LiteNet
            m_serverNetManager = new NetManager (this);
            m_serverNetManager.Start (c_serverPort);

            // 初始化命令模式
            m_clientCommand.Add(NetworkSendDataType.SEND_POSITION, new CC_SendPosition());
            m_clientCommand.Add(NetworkSendDataType.APPLY_CAST_SKILL, new CC_ApplyCastSkill());
            m_clientCommand.Add(NetworkSendDataType.APPLY_ACTIVE_ENTER_FSM_STATE, new CC_ApplyActiveEnterFSMState());
        }
        public void Tick (float dT) {
            m_serverNetManager.PollEvents();
        }
        public void OnConnectionRequest (ConnectionRequest request) {
            request.AcceptIfKey ("client");
        }
        public void OnNetworkError (IPEndPoint endPoint, SocketError socketError) {
            Console.WriteLine ("网络错误, 客户终端: " + endPoint + ", 错误信息: " + socketError);
        }
        public void OnNetworkLatencyUpdate (NetPeer peer, int latency) { }
        public void OnNetworkReceive (NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod) {
            IClientCommand command = m_clientCommand[(NetworkSendDataType)reader.GetByte()];
            command.SetData(reader);
            command.Execute(m_networkIdAndActorUnitDict[peer.Id]);
        }
        public void OnNetworkReceiveUnconnected (IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }
        public void OnPeerConnected (NetPeer peer) {
            if (m_networkIdAndPeerDict.Count >= c_maxClientNum)
                return;
            m_networkIdAndPeerDict.Add(peer.Id, peer);
            m_writer.Put((byte)NetworkReceiveDataType.SET_SELF_NETWORK_ID);
            m_writer.Put(peer.Id);
            peer.Send(m_writer, DeliveryMethod.ReliableOrdered);
        }
        public void OnPeerDisconnected (NetPeer peer, DisconnectInfo disconnectInfo) {
            m_networkIdAndPeerDict.Remove(peer.Id);
            m_networkIdAndActorUnitDict.Remove(peer.Id);
            Console.WriteLine (peer.Id + "断开连接, 客户终端: " + peer.EndPoint + ", 断线信息: " + disconnectInfo);
        }

        public E_ActorUnit GetActorUnitByNetworkId(int networkId) {
            return m_networkIdAndActorUnitDict[networkId];
        }
    }
}