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
        private Dictionary<int, NetPeer> m_peerIdAndPeerDict = new Dictionary<int, NetPeer> ();
        private Dictionary<int, int> m_peerIdAndNetworkIdDict = new Dictionary<int, int> ();
        private Dictionary<int, int> m_networkIdAndPeerIdDict = new Dictionary<int, int> ();
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
            command.Execute (reader, m_peerIdAndNetworkIdDict[peer.Id]);
            reader.Recycle ();
        }
        public void OnNetworkReceiveUnconnected (IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }
        public void OnPeerConnected (NetPeer peer) {
            // 超过连接上限
            if (m_peerIdAndPeerDict.Count >= c_maxClientNum)
                return;
            
            // 保存peer
            m_peerIdAndPeerDict[peer.Id] = peer;

            // 实例化玩家角色并分配NetId
            int netId = SM_ActorUnit.s_instance.AddCharacter();
            m_peerIdAndNetworkIdDict[peer.Id] = netId;
            m_networkIdAndPeerIdDict[netId] = peer.Id;

            // 发送NetId
            NetworkSetSelfNetworkId (peer, netId);
            Console.WriteLine (peer.Id + "连接成功");
        }
        public void OnPeerDisconnected (NetPeer peer, DisconnectInfo disconnectInfo) {
            var netId = m_peerIdAndNetworkIdDict[peer.Id];
            m_peerIdAndPeerDict.Remove (peer.Id);
            m_peerIdAndNetworkIdDict.Remove (peer.Id);
            m_networkIdAndPeerIdDict.Remove (netId);
            SM_ActorUnit.s_instance.RemoveCharacter (netId);
            Console.WriteLine (peer.Id + "断开连接, 客户终端: " + peer.EndPoint + ", 断线原因: " + disconnectInfo.Reason);
        }
        private void NetworkSetSelfNetworkId (NetPeer client, int netId) {
            m_writer.Put ((byte) NetworkReceiveDataType.SET_SELF_NETWORK_ID);
            m_writer.Put (netId);
            client.Send (m_writer, DeliveryMethod.ReliableUnordered);
            m_writer.Reset ();
        }
        public void NetworkSetOtherActorUnitInSight (int clientNetId, ActorUnitType actorUnitType, List<int> otherIdList) {
            int peerId = m_networkIdAndPeerIdDict[clientNetId];
            m_writer.Put ((byte) NetworkReceiveDataType.SET_OTHER_ACTOR_UNIT_IN_SIGHT);
            NetPeer client = m_peerIdAndPeerDict[peerId];
            m_writer.Put ((byte) actorUnitType);
            m_writer.Put ((byte) otherIdList.Count);
            for (int i = 0; i < otherIdList.Count; i++) {
                m_writer.Put (otherIdList[i]);
            }
            client.Send (m_writer, DeliveryMethod.Sequenced);
            m_writer.Reset ();
        }
        public void NetworkSetOtherPosition (int clientNetId, List<int> otherIdList, List<Vector2> posList) {
            int peerId = m_networkIdAndPeerIdDict[clientNetId];
            m_writer.Put ((byte) NetworkReceiveDataType.SET_OTHER_POSITION);
            NetPeer client = m_peerIdAndPeerDict[peerId];
            m_writer.Put ((byte) otherIdList.Count);
            for (int i = 0; i < otherIdList.Count; i++) {
                m_writer.Put (otherIdList[i]);
                m_writer.PutVector2 (posList[i]);
            }
            client.Send (m_writer, DeliveryMethod.Sequenced);
            m_writer.Reset ();
        }
        public void NetworkSetAllHPAndMP (int clientNetId, List<int> otherIdList, List<Dictionary<ActorUnitConcreteAttributeType, int>> attrList) {
            int peerId = m_networkIdAndPeerIdDict[clientNetId];
            m_writer.Put ((byte) NetworkReceiveDataType.SET_ALL_HP_AND_MP);
            NetPeer client = m_peerIdAndPeerDict[peerId];
            m_writer.Put ((byte) otherIdList.Count);
            for (int i = 0; i < otherIdList.Count; i++) {
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
            foreach (var clientPair in m_peerIdAndPeerDict) {
                if (m_peerIdAndNetworkIdDict[clientPair.Key] != allNetId)
                    clientPair.Value.Send (m_writer, DeliveryMethod.ReliableSequenced);
            }
            m_writer.Reset ();
        }
    }
}