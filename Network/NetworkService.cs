using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace MirRemake {
    class NetworkService : INetEventListener, INetworkService {
        public static NetworkService s_instance = new NetworkService ();
        private const int c_serverPort = 23333;
        private const int c_maxClientNum = 3000;
        private NetManager m_serverNetManager;
        private Dictionary<int, NetPeer> m_netIdAndPeerDict = new Dictionary<int, NetPeer> ();
        private Dictionary<int, int> m_peerIdAndNetworkIdDict = new Dictionary<int, int> ();
        private Dictionary<int, int> m_networkIdAndPeerIdDict = new Dictionary<int, int> ();
        private Dictionary<NetworkToServerDataType, IClientCommand> m_clientCommandDict = new Dictionary<NetworkToServerDataType, IClientCommand> ();
        private NetDataWriter m_writer = new NetDataWriter ();
        public void Init () {
            // 初始化LiteNet
            m_serverNetManager = new NetManager (this);
            m_serverNetManager.Start (c_serverPort);

            // 初始化命令模式
            m_clientCommandDict.Add (NetworkToServerDataType.INIT_PLAYER_ID, new CC_InitPlayerId ());
            m_clientCommandDict.Add (NetworkToServerDataType.SET_POSITION, new CC_SetPosition ());
            m_clientCommandDict.Add (NetworkToServerDataType.APPLY_CAST_SKILL_BEGIN, new CC_ApplyCastSkillBegin ());
            m_clientCommandDict.Add (NetworkToServerDataType.APPLY_CAST_SKILL_SETTLE, new CC_ApplyCastSkillSettle ());
            m_clientCommandDict.Add (NetworkToServerDataType.APPLY_CAST_SKILL_SING_CANCEL, new CC_ApplyCastSkillSingCancel ());
        }
        public void Tick () {
            m_serverNetManager.PollEvents ();
        }
        private void ReceiveClientCommand (NetPacketReader reader, int clientNetId) {
            IClientCommand command = m_clientCommandDict[(NetworkToServerDataType) reader.GetByte ()];
            command.Execute (reader, clientNetId);
        }
        public void SendServerCommand (IServerCommand command) {
            m_writer.Put ((byte) command.m_DataType);
            command.PutData (m_writer);
            for (int i=0; i<command.m_ToClientList.Count; i++)
                m_netIdAndPeerDict[command.m_ToClientList[i]].Send (m_writer, command.m_DeliveryMethod);
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
            int netId = SM_ActorUnit.s_instance.CommandAssignNetworkId ();
            m_peerIdAndNetworkIdDict[peer.Id] = netId;
            m_networkIdAndPeerIdDict[netId] = peer.Id;

            // 保存peer
            m_netIdAndPeerDict[netId] = peer;

            // 发送NetId
            SendServerCommand (new SC_InitSelfNetworkId(new List<int> {netId}, netId));
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
        /// private void NetworkSetSelfNetworkId (NetPeer client, int netId) {
        //     m_writer.Put ((byte) NetworkToClientDataType.INIT_SELF_NETWORK_ID);
        //     m_writer.Put (netId);
        //     client.Send (m_writer, DeliveryMethod.ReliableUnordered);
        //     m_writer.Reset ();
        // }
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
        /// public void NetworkSetSelfInfo (int clientNetId, short level, int exp, short[] skillIds, short[] skillLevels, int[] skillMasterlys) {
        //     NetPeer client = m_netIdAndPeerDict[clientNetId];

        //     m_writer.Put ((byte) NetworkToClientDataType.INIT_SELF_INFO);
        //     m_writer.Put (level);
        //     m_writer.Put (exp);
        //     m_writer.Put ((short) skillIds.Length);
        //     for (int i = 0; i < skillIds.Length; i++) {
        //         m_writer.Put (skillIds[i]);
        //         m_writer.Put (skillLevels[i]);
        //         m_writer.Put (skillMasterlys[i]);
        //     }
        //     client.Send (m_writer, DeliveryMethod.ReliableOrdered);
        //     m_writer.Reset ();
        // }
        /// <summary>
        /// 向一个Client发送它的视野中的某一类单位
        /// </summary>
        /// <param name="clientNetId">Client</param>
        /// <param name="actorUnitType">单位种类</param>
        /// <param name="otherNetIdList">其他单位NetId列表</param>
        /// public void NetworkSetOtherActorUnitInSight (int clientNetId, ActorUnitType actorUnitType, List<int> otherNetIdList) {
            // TODO: 修改为, 当视野变化的时候才会发送, 且只发送新增与离开的单位, 并发送新增单位的初始信息(FSM, HP, MP, Level, Status)
            // TODO: 区分玩家与怪物的接口(因为玩家需要得到装备)
            // TODO: 视野需要有上限
        //     NetPeer client = m_netIdAndPeerDict[clientNetId];
        //     m_writer.Put ((byte) NetworkToClientDataType.APPLY_OTHER_ACTOR_UNIT_IN_SIGHT);
        //     m_writer.Put ((byte) actorUnitType);
        //     m_writer.Put ((byte) otherNetIdList.Count);
        //     for (int i = 0; i < otherNetIdList.Count; i++) {
        //         m_writer.Put (otherNetIdList[i]);
        //     }
        //     client.Send (m_writer, DeliveryMethod.Sequenced);
        //     m_writer.Reset ();
        // }
        /// <summary>
        /// 向一个Client发送它的视野中其他单位的位置
        /// </summary>
        /// <param name="clientNetId"></param>
        /// <param name="otherIdList"></param>
        /// <param name="posList"></param>
        /// public void NetworkSetOtherPosition (int clientNetId, List<int> otherIdList, List<Vector2> posList) {
        //     NetPeer client = m_netIdAndPeerDict[clientNetId];
        //     m_writer.Put ((byte) NetworkToClientDataType.SET_OTHER_POSITION);
        //     m_writer.Put ((byte) otherIdList.Count);
        //     for (int i = 0; i < otherIdList.Count; i++) {
        //         m_writer.Put (otherIdList[i]);
        //         m_writer.PutVector2 (posList[i]);
        //     }
        //     client.Send (m_writer, DeliveryMethod.Sequenced);
        //     m_writer.Reset ();
        // }
        /// <summary>
        /// 向一个Client发送所有Unit的HP与MP
        /// </summary>
        /// <param name="clientNetId"></param>
        /// <param name="otherIdList"></param>
        /// <param name="attrList"></param>
        // public void NetworkSetAllHPAndMP (int clientNetId, List<int> allNetIdList, List<Dictionary<ActorUnitConcreteAttributeType, int>> attrList) {
        //     m_writer.Put ((byte) NetworkToClientDataType.SET_ALL_HP_AND_MP);
        //     m_writer.Put ((byte) allNetIdList.Count);
        //     for (int i = 0; i < allNetIdList.Count; i++) {
        //         m_writer.Put (allNetIdList[i]);
        //         m_writer.Put (attrList[i][ActorUnitConcreteAttributeType.CURRENT_HP]);
        //         m_writer.Put (attrList[i][ActorUnitConcreteAttributeType.MAX_HP]);
        //         m_writer.Put (attrList[i][ActorUnitConcreteAttributeType.CURRENT_MP]);
        //         m_writer.Put (attrList[i][ActorUnitConcreteAttributeType.MAX_MP]);
        //     }
        //     NetPeer client = m_netIdAndPeerDict[clientNetId];
        //     client.Send (m_writer, DeliveryMethod.ReliableSequenced);
        //     m_writer.Reset ();
        // }

        /// <summary>
        /// 对所有玩家发送视野内的施加effect事件  
        /// 数据包格式:  
        /// effectAnimId, short  
        /// statusNum, byte  
        /// unitHitNum, byte 命中的目标数量  
        /// hitNetIdAndstatusArrPairArr, (int, E_Status[])*unitHitNum 命中的目标(unitHitNum个)的NetId及其被附加的状态  
        /// unitNotHitNum, byte 未命中的目标数量
        /// notHitNetIdArr, (int)*unitNotHitNum 未被命中的目标NetId
        /// </summary>
        /// <param name="effectAnimId">effect动画Id</param>
        /// <param name="statusNum">effect造成的新增状态数量</param>
        /// <param name="allNetIdAndStatusArrPairArr">所有受到effect的Unit的NetId, 状态数组键值对数组</param>
        // public void NetworkSetAllEffectToAll (short effectAnimId, byte statusNum, KeyValuePair<int, E_Status[]>[] allNetIdAndStatusArrPairArr) {
        //     m_writer.Put ((byte) NetworkToClientDataType.APPLY_ALL_EFFECT);
        //     m_writer.Put (effectAnimId);
        //     m_writer.Put (statusNum);
        //     int unitHitNum = 0;
        //     if (statusNum != 0)
        //         foreach (var pair in allNetIdAndStatusArrPairArr)
        //             if (pair.Value != null)
        //                 unitHitNum++;
        //     m_writer.Put ((byte)unitHitNum);
        //     foreach (var pair in allNetIdAndStatusArrPairArr)
        //         if (pair.Value != null) {
        //             m_writer.Put(pair.Key);
        //             foreach (var status in pair.Value)
        //                 m_writer.PutE_Status(status);
        //         }
        //     m_writer.Put ((byte)(allNetIdAndStatusArrPairArr.Length - unitHitNum));
        //     foreach (var pair in allNetIdAndStatusArrPairArr)
        //         if (pair.Value == null)
        //             m_writer.Put(pair.Key);
        //     foreach (var clientPeer in m_netIdAndPeerDict.Values)
        //         clientPeer.Send(m_writer, DeliveryMethod.ReliableOrdered);
        //     m_writer.Reset ();
        // }

        // public void NetworkSetAllDeadToAll (int killerNetId, int deadNetId) {
        //     m_writer.Put ((byte) NetworkToClientDataType.APPLY_ALL_DEAD);
        //     m_writer.Put (killerNetId);
        //     m_writer.Put (deadNetId);
        //     var peerEn = m_netIdAndPeerDict.Values.GetEnumerator();
        //     while(peerEn.MoveNext())
        //         peerEn.Current.Send (m_writer, DeliveryMethod.ReliableUnordered);
        //     m_writer.Reset ();
        // }
    }
}