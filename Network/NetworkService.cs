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
        private Dictionary<int, NetPeer> m_netIdAndPeerDict = new Dictionary<int, NetPeer> ();
        private Dictionary<int, int> m_peerIdAndNetworkIdDict = new Dictionary<int, int> ();
        private Dictionary<int, int> m_networkIdAndPeerIdDict = new Dictionary<int, int> ();
        private Dictionary<NetworkSendDataType, IClientCommand> m_clientCommandDict = new Dictionary<NetworkSendDataType, IClientCommand> ();
        private NetDataWriter m_writer = new NetDataWriter ();
        public void Init () {
            // 初始化LiteNet
            m_serverNetManager = new NetManager (this);
            m_serverNetManager.Start (c_serverPort);

            // 初始化命令模式
            m_clientCommandDict.Add (NetworkSendDataType.SEND_PLAYER_ID, new CC_SendPlayerId ());
            m_clientCommandDict.Add (NetworkSendDataType.SEND_POSITION, new CC_SendPosition ());
            m_clientCommandDict.Add (NetworkSendDataType.APPLY_CAST_SKILL, new CC_ApplyCastSkill ());
            m_clientCommandDict.Add (NetworkSendDataType.APPLY_ACTIVE_ENTER_FSM_STATE, new CC_ApplyActiveEnterFSMState ());
        }
        public void Tick () {
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
            IClientCommand command = m_clientCommandDict[(NetworkSendDataType) reader.GetByte ()];
            command.Execute (reader, m_peerIdAndNetworkIdDict[peer.Id]);
            reader.Recycle ();
        }
        public void OnNetworkReceiveUnconnected (IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }
        public void OnPeerConnected (NetPeer peer) {
            // 超过连接上限
            if (m_netIdAndPeerDict.Count >= c_maxClientNum)
                return;

            // 分配NetId
            int netId = SM_ActorUnit.s_instance.CommandAssignNetworkId ();
            m_peerIdAndNetworkIdDict[peer.Id] = netId;
            m_networkIdAndPeerIdDict[netId] = peer.Id;

            // 保存peer
            m_netIdAndPeerDict[netId] = peer;

            // 发送NetId
            NetworkSetSelfNetworkId (peer, netId);
            Console.WriteLine (peer.Id + "连接成功");
        }
        public void OnPeerDisconnected (NetPeer peer, DisconnectInfo disconnectInfo) {
            var netId = m_peerIdAndNetworkIdDict[peer.Id];
            m_netIdAndPeerDict.Remove (netId);
            m_peerIdAndNetworkIdDict.Remove (peer.Id);
            m_networkIdAndPeerIdDict.Remove (netId);
            SM_ActorUnit.s_instance.CommandRemoveCharacter (netId);
            Console.WriteLine (peer.Id + "断开连接, 客户终端: " + peer.EndPoint + ", 断线原因: " + disconnectInfo.Reason);
        }
        /// <summary>
        /// 向Client发送为它分配的NetId
        /// </summary>
        /// <param name="client"></param>
        /// <param name="netId"></param>
        private void NetworkSetSelfNetworkId (NetPeer client, int netId) {
            m_writer.Put ((byte) NetworkReceiveDataType.SET_SELF_NETWORK_ID);
            m_writer.Put (netId);
            client.Send (m_writer, DeliveryMethod.ReliableUnordered);
            m_writer.Reset ();
        }
        /// <summary>
        /// 玩家进入游戏发送自身PlayerId后, 根据PlayerId从数据库获取玩家信息之后, 发送给玩家的Client  
        /// 包括等级, 经验, 已学技能列表  
        /// TODO: 装备, 物品, 加点  
        /// </summary>
        /// <param name="clientNetId">新接入的Client</param>
        /// <param name="level">等级</param>
        /// <param name="exp">经验值</param>
        /// <param name="skillIds">已学技能Id列表</param>
        /// <param name="skillLevels">已学技能等级列表</param>
        /// <param name="skillMasterlys">已学技能熟练度列表</param>
        public void NetworkSetSelfInfo (int clientNetId, short level, int exp, short[] skillIds, short[] skillLevels, int[] skillMasterlys) {
            NetPeer client = m_netIdAndPeerDict[clientNetId];

            m_writer.Put ((byte) NetworkReceiveDataType.SET_SELF_INIT_INFO);
            m_writer.Put (level);
            m_writer.Put (exp);
            m_writer.Put ((short) skillIds.Length);
            for (int i = 0; i < skillIds.Length; i++) {
                m_writer.Put (skillIds[i]);
                m_writer.Put (skillLevels[i]);
                m_writer.Put (skillMasterlys[i]);
            }
            client.Send (m_writer, DeliveryMethod.ReliableOrdered);
            m_writer.Reset ();
        }
        /// <summary>
        /// 向一个Client发送它的视野中的某一类单位
        /// </summary>
        /// <param name="clientNetId">Client</param>
        /// <param name="actorUnitType">单位种类</param>
        /// <param name="otherNetIdList">其他单位NetId列表</param>
        public void NetworkSetOtherActorUnitInSight (int clientNetId, ActorUnitType actorUnitType, List<int> otherNetIdList) {
            NetPeer client = m_netIdAndPeerDict[clientNetId];
            m_writer.Put ((byte) NetworkReceiveDataType.SET_OTHER_ACTOR_UNIT_IN_SIGHT);
            m_writer.Put ((byte) actorUnitType);
            m_writer.Put ((byte) otherNetIdList.Count);
            for (int i = 0; i < otherNetIdList.Count; i++) {
                m_writer.Put (otherNetIdList[i]);
            }
            client.Send (m_writer, DeliveryMethod.Sequenced);
            m_writer.Reset ();
        }
        /// <summary>
        /// 向一个Client发送它的视野中其他单位的位置
        /// </summary>
        /// <param name="clientNetId"></param>
        /// <param name="otherIdList"></param>
        /// <param name="posList"></param>
        public void NetworkSetOtherPosition (int clientNetId, List<int> otherIdList, List<Vector2> posList) {
            NetPeer client = m_netIdAndPeerDict[clientNetId];
            m_writer.Put ((byte) NetworkReceiveDataType.SET_OTHER_POSITION);
            m_writer.Put ((byte) otherIdList.Count);
            for (int i = 0; i < otherIdList.Count; i++) {
                m_writer.Put (otherIdList[i]);
                m_writer.PutVector2 (posList[i]);
            }
            client.Send (m_writer, DeliveryMethod.Sequenced);
            m_writer.Reset ();
        }
        /// <summary>
        /// 向一个Client发送所有Unit的HP与MP
        /// </summary>
        /// <param name="clientNetId"></param>
        /// <param name="otherIdList"></param>
        /// <param name="attrList"></param>
        public void NetworkSetAllHPAndMP (int clientNetId, List<int> allIdList, List<Dictionary<ActorUnitConcreteAttributeType, int>> attrList) {
            NetPeer client = m_netIdAndPeerDict[clientNetId];
            m_writer.Put ((byte) NetworkReceiveDataType.SET_ALL_HP_AND_MP);
            m_writer.Put ((byte) allIdList.Count);
            for (int i = 0; i < allIdList.Count; i++) {
                m_writer.Put (allIdList[i]);
                m_writer.Put (attrList[i][ActorUnitConcreteAttributeType.CURRENT_HP]);
                m_writer.Put (attrList[i][ActorUnitConcreteAttributeType.MAX_HP]);
                m_writer.Put (attrList[i][ActorUnitConcreteAttributeType.CURRENT_MP]);
                m_writer.Put (attrList[i][ActorUnitConcreteAttributeType.MAX_MP]);
            }
            client.Send (m_writer, DeliveryMethod.ReliableSequenced);
            m_writer.Reset ();
        }

        /// <summary>
        /// 向其他所有视野内的Client发送Unit的FSMState
        /// </summary>
        /// <param name="otherNetId"></param>
        /// <param name="aEState"></param>
        public void NetworkSetSelfFSMStateToOther (int selfNetId, FSMActiveEnterState aEState) {
            m_writer.Put ((byte) NetworkReceiveDataType.APPLY_OTHER_FSM_STATE);
            m_writer.Put (selfNetId);
            m_writer.PutFSMAEState (aEState);
            foreach (var clientPair in m_netIdAndPeerDict) {
                if (clientPair.Key != selfNetId)
                    clientPair.Value.Send (m_writer, DeliveryMethod.ReliableSequenced);
            }
            m_writer.Reset ();
        }

        /// <summary>
        /// 对所有玩家发送视野内的施加effect事件  
        /// 数据包格式:  
        /// effectAnimId, byte  
        /// statusNum, byte  
        /// unitHitNum, byte 命中的目标数量  
        /// hitNetIdAndstatusArrPairArr, (int, E_Status[])*unitHitNum 命中的目标(unitHitNum个)的NetId及其被附加的状态  
        /// unitNotHitNum, byte 未命中的目标数量
        /// notHitNetIdArr, (int)*unitNotHitNum 未被命中的目标NetId
        /// </summary>
        /// <param name="effectAnimId">effect动画Id</param>
        /// <param name="statusNum">effect造成的新增状态数量</param>
        /// <param name="allNetIdAndStatusArrPairArr">所有受到effect的Unit的NetId, 状态数组键值对数组</param>
        public void NetworkSetAllEffectToAll (short effectAnimId, byte statusNum, KeyValuePair<int, E_Status[]>[] allNetIdAndStatusArrPairArr) {
            m_writer.Put ((byte) NetworkReceiveDataType.APPLY_ALL_EFFECT);
            m_writer.Put (effectAnimId);
            m_writer.Put (statusNum);
            int unitHitNum = 0;
            if (statusNum != 0)
                foreach (var pair in allNetIdAndStatusArrPairArr)
                    if (pair.Value != null)
                        unitHitNum++;
            m_writer.Put ((byte)unitHitNum);
            foreach (var pair in allNetIdAndStatusArrPairArr)
                if (pair.Value != null) {
                    m_writer.Put(pair.Key);
                    foreach (var status in pair.Value)
                        m_writer.PutE_Status(status);
                }
            m_writer.Put ((byte)(allNetIdAndStatusArrPairArr.Length - unitHitNum));
            foreach (var pair in allNetIdAndStatusArrPairArr)
                if (pair.Value == null)
                    m_writer.Put(pair.Key);
            foreach (var clientPeer in m_netIdAndPeerDict.Values)
                clientPeer.Send(m_writer, DeliveryMethod.ReliableOrdered);
            m_writer.Reset ();
        }

        public void NetworkSetAllDeadToAll (int killerNetId, List<int> deadNetIdList) {
            if (deadNetIdList.Count == 0) return;
            m_writer.Put ((byte) NetworkReceiveDataType.APPLY_ALL_DEAD);
            m_writer.Put (killerNetId);
            m_writer.Put ((byte) deadNetIdList.Count);
            for(int i=0; i<deadNetIdList.Count; i++)
                m_writer.Put (deadNetIdList[i]);
            var peerEn = m_netIdAndPeerDict.Values.GetEnumerator();
            while(peerEn.MoveNext())
                peerEn.Current.Send (m_writer, DeliveryMethod.ReliableUnordered);
            m_writer.Reset ();
        }

        public void NetworkConfirmAcceptingMission(int netId, short missionId) {
            NetPeer client = m_netIdAndPeerDict[netId];
            m_writer.Put((byte)NetworkSendDataType.ACCEPT_MISSION);
            m_writer.Put(missionId);
            client.Send(m_writer, DeliveryMethod.Unreliable);
            m_writer.Reset();
        }

        public void NetworkConfirmDeliveringMission(int netId, short missionId, bool isCompleted) {
            NetPeer client = m_netIdAndPeerDict[netId];
            m_writer.Put((byte)NetworkSendDataType.DELIVERING_MISSION);
            m_writer.Put(isCompleted);
            m_writer.Put(missionId);
            client.Send(m_writer, DeliveryMethod.Unreliable);
            m_writer.Reset();
        }

        public void NetworkConfirmMissionFailed(int netId, short missionId) {
            NetPeer client = m_netIdAndPeerDict[netId];
            m_writer.Put((byte)NetworkSendDataType.CANCEL_MISSION);
            m_writer.Put(missionId);
            client.Send(m_writer, DeliveryMethod.Unreliable);
            m_writer.Reset();
        }
    }
}