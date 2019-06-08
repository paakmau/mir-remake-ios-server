using System.Collections.Generic;

namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 管理场景中Character的视野信息  
    /// 存放场景中所有能够显示的单位  
    /// </summary>
    class EM_Sight : EntityManagerBase {
        public static EM_Sight s_instance;
        private Dictionary<int, E_Unit> m_networkIdAndActorUnitVisibleDict = new Dictionary<int, E_Unit> ();
        /// <summary>
        /// 每个角色视野中的单位 (包括自身)
        /// </summary>
        private Dictionary<int, List<E_Unit>> m_characterSightDict = new Dictionary<int, List<E_Unit>> ();
        private List<int> t_intList = new List<int> ();
        /// <summary>
        /// 根据NetId获取角色视野内的单位 (包括自身)  
        /// 可以对其视野进行读写  
        /// </summary>
        public List<E_Unit> GetCharacterRawSight (int netId) {
            List<E_Unit> res = null;
            m_characterSightDict.TryGetValue (netId, out res);
            return res;
        }
        /// <summary>
        /// 获取一个角色视野内的角色的netId
        /// </summary>
        public List<int> GetCharacterInSightNetworkId (int netId, bool includeSelf) {
            var units = GetCharacterRawSight (netId);
            if (units == null) return null;
            t_intList.Clear ();
            for (int i = 0; i < units.Count; i++) {
                // 移除非玩家
                if (units[i].m_ActorUnitType != ActorUnitType.PLAYER)
                    continue;
                // 检查是否包含自身
                if (units[i].m_networkId == netId && !includeSelf)
                    continue;
                t_intList.Add (units[i].m_networkId);
            }
            return t_intList;
        }
        /// <summary>
        /// 根据NetId获取可视单位
        /// </summary>
        public E_Unit GetActorUnitVisibleByNetworkId (int netId) {
            E_Unit res = null;
            m_networkIdAndActorUnitVisibleDict.TryGetValue (netId, out res);
            return res;
        }
        /// <summary>
        /// 获取所有可视单位的迭代器
        /// </summary>
        public Dictionary<int, E_Unit>.ValueCollection.Enumerator GetActorUnitVisibleEnumerator () {
            return m_networkIdAndActorUnitVisibleDict.Values.GetEnumerator ();
        }
        /// <summary>
        /// 为角色初始化视野, 并加入可视单位
        /// </summary>
        public void InitCharacter (E_Character charObj) {
            m_characterSightDict.Add (charObj.m_networkId, new List<E_Unit> ());
            m_networkIdAndActorUnitVisibleDict.Add (charObj.m_networkId, charObj);
        }
        /// <summary>
        /// 移除一个角色的视野信息, 并移除可视单位
        /// </summary>
        public void RemoveCharacter (int netId) {
            m_characterSightDict.Remove (netId);
            m_networkIdAndActorUnitVisibleDict.Remove (netId);
        }
    }
}