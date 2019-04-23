using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace MirRemake {
    class NetworkService : INetEventListener {
        public static NetworkService s_instance = new NetworkService ();
        private const int c_serverPort = 23333;
        private const int c_maxClientNum = 3000;
        private NetManager m_serverNetManager;
        private Dictionary<int, NetPeer> m_networkIdAndPeerDict = new Dictionary<int, NetPeer> ();
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
            command.Execute(reader, peer.Id);
            reader.Recycle();
        }
        public void OnNetworkReceiveUnconnected (IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }
        public void OnPeerConnected (NetPeer peer) {
            if (m_networkIdAndPeerDict.Count >= c_maxClientNum)
                return;
            m_networkIdAndPeerDict.Add(peer.Id, peer);
            NetworkSetSelfNetworkId(peer);
        }
        public void OnPeerDisconnected (NetPeer peer, DisconnectInfo disconnectInfo) {
            m_networkIdAndPeerDict.Remove(peer.Id);
            // m_networkIdAndActorUnitDict.Remove(peer.Id);
            Console.WriteLine (peer.Id + "断开连接, 客户终端: " + peer.EndPoint + ", 断线信息: " + disconnectInfo);
        }

        private void NetworkSetSelfNetworkId(NetPeer client) {
            m_writer.Put((byte)NetworkReceiveDataType.SET_SELF_NETWORK_ID);
            m_writer.Put(client.Id);
            client.Send(m_writer, DeliveryMethod.ReliableOrdered);
            m_writer.Reset();
        }
        public void NetworkSetOtherPosition(int clientNetId, Tuple<int, Vector2>[] unitIdAndPos) {
            NetPeer client = m_networkIdAndPeerDict[clientNetId];
            m_writer.Put((byte)unitIdAndPos.Length);
            foreach(var unit in unitIdAndPos) {
                m_writer.Put(unit.Item1);
                m_writer.PutVector2(unit.Item2);
            }
            client.Send(m_writer, DeliveryMethod.Sequenced);
            m_writer.Reset();
        }
        public void NetworkSetAllHPAndMP(int clientNetId, Tuple<int, Dictionary<ActorUnitConcreteAttributeType, int>>[] untIdAndAttr) {
            NetPeer client = m_networkIdAndPeerDict[clientNetId];
            m_writer.Put((byte)untIdAndAttr.Length);
            foreach (var item in untIdAndAttr) {
                m_writer.Put(item.Item1);
                m_writer.PutHPAndMP(item.Item2);
            }
            client.Send(m_writer, DeliveryMethod.ReliableSequenced);
            m_writer.Reset();
        }
        public void NetworkSetAllFSMState(int clientNetId, Tuple<int, FSMActiveEnterState>[] unitIdAndAEState) {
            NetPeer client = m_networkIdAndPeerDict[clientNetId];
            m_writer.Put((byte)unitIdAndAEState.Length);
            foreach (var item in unitIdAndAEState) {
                m_writer.Put(item.Item1);
                m_writer.PutFSMAEState(item.Item2);
            }
            client.Send(m_writer, DeliveryMethod.Unreliable);
            m_writer.Reset();
        }
    }
}