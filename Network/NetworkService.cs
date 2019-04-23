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
        private Dictionary<NetworkSendDataType, IClientCommand> m_clientCommand = new Dictionary<NetworkSendDataType, IClientCommand> ();
        private NetDataWriter m_writer = new NetDataWriter ();
        public void Init () {
            // 初始化LiteNet
            m_serverNetManager = new NetManager (this);
            m_serverNetManager.Start (c_serverPort);

            // 初始化命令模式
            m_clientCommand.Add (NetworkSendDataType.SEND_PLAYER_ID, new CC_SendPlayerId ());
            m_clientCommand.Add (NetworkSendDataType.SEND_POSITION, new CC_SendPosition ());
            m_clientCommand.Add (NetworkSendDataType.APPLY_CAST_SKILL, new CC_ApplyCastSkill ());
            m_clientCommand.Add (NetworkSendDataType.APPLY_ACTIVE_ENTER_FSM_STATE, new CC_ApplyActiveEnterFSMState ());
        }
        public void Tick (float dT) {
            m_serverNetManager.PollEvents ();
        }
        public void OnConnectionRequest (ConnectionRequest request) {
            request.AcceptIfKey ("client");
        }
        public void OnNetworkError (IPEndPoint endPoint, SocketError socketError) {
            Console.WriteLine ("网络错误, 客户终端: " + endPoint + ", 错误信息: " + socketError);
        }
        public void OnNetworkLatencyUpdate (NetPeer peer, int latency) { }
        public void OnNetworkReceive (NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod) {
            IClientCommand command = m_clientCommand[(NetworkSendDataType) reader.GetByte ()];
            command.Execute (reader, peer.Id);
            reader.Recycle ();
        }
        public void OnNetworkReceiveUnconnected (IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }
        public void OnPeerConnected (NetPeer peer) {
            if (m_networkIdAndPeerDict.Count >= c_maxClientNum)
                return;
            m_networkIdAndPeerDict[peer.Id] = peer;
            NetworkEntityManager.s_instance.AddCharacter(peer.Id);
            NetworkSetSelfNetworkId (peer);
            Console.WriteLine (peer.Id + "连接成功");
        }
        public void OnPeerDisconnected (NetPeer peer, DisconnectInfo disconnectInfo) {
            m_networkIdAndPeerDict.Remove (peer.Id);
            NetworkEntityManager.s_instance.RemoveCharacter(peer.Id);
            Console.WriteLine (peer.Id + "断开连接, 客户终端: " + peer.EndPoint + ", 断线原因: " + disconnectInfo.Reason);
        }

        private void NetworkSetSelfNetworkId (NetPeer client) {
            m_writer.Put ((byte) NetworkReceiveDataType.SET_SELF_NETWORK_ID);
            m_writer.Put (client.Id);
            client.Send (m_writer, DeliveryMethod.ReliableUnordered);
            m_writer.Reset ();
        }
        public void NetworkSetOtherActorUnitInSight (int clientNetId, List<int> otherIdList, List<ActorUnitType> typeList) {
            m_writer.Put ((byte) NetworkReceiveDataType.SET_OTHER_ACTOR_UNIT_IN_SIGHT);
            NetPeer client = m_networkIdAndPeerDict[clientNetId];
            m_writer.Put ((byte) otherIdList.Count);
            for(int i=0; i<otherIdList.Count; i++) {
                m_writer.Put(otherIdList[i]);
                m_writer.Put((byte)typeList[i]);
            }
            client.Send(m_writer, DeliveryMethod.Sequenced);
            m_writer.Reset();
        }
        public void NetworkSetOtherPosition (int clientNetId, List<int> otherIdList, List<Vector2> posList) {
            m_writer.Put ((byte) NetworkReceiveDataType.SET_OTHER_POSITION);
            NetPeer client = m_networkIdAndPeerDict[clientNetId];
            m_writer.Put ((byte) otherIdList.Count);
            for (int i=0; i<otherIdList.Count; i++) {
                m_writer.Put (otherIdList[i]);
                m_writer.PutVector2 (posList[i]);
            }
            client.Send (m_writer, DeliveryMethod.Sequenced);
            m_writer.Reset ();
        }
        public void NetworkSetAllHPAndMP (int clientNetId, List<int> otherIdList, List<Dictionary<ActorUnitConcreteAttributeType, int>> attrList) {
            m_writer.Put ((byte) NetworkReceiveDataType.SET_ALL_HP_AND_MP);
            NetPeer client = m_networkIdAndPeerDict[clientNetId];
            m_writer.Put ((byte) otherIdList.Count);
            for(int i=0; i<otherIdList.Count; i++) {
                m_writer.Put (otherIdList[i]);
                m_writer.PutHPAndMP (attrList[i]);
            }
            client.Send (m_writer, DeliveryMethod.ReliableSequenced);
            m_writer.Reset ();
        }
        public void NetworkSetAllFSMState (int allNetId, FSMActiveEnterState aEState) {
            m_writer.Put ((byte) NetworkReceiveDataType.SET_ALL_FSM_STATE);
            m_writer.Put (allNetId);
            m_writer.PutFSMAEState (aEState);
            foreach (var clientPair in m_networkIdAndPeerDict)
                if (clientPair.Key != allNetId)
                    clientPair.Value.Send (m_writer, DeliveryMethod.ReliableSequenced);
            m_writer.Reset ();
        }
    }
}