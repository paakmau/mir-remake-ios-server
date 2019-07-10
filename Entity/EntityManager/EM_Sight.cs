using System.Collections.Generic;

namespace MirRemakeBackend.Entity {
    /// <summary>
    /// 管理场景中Character的视野信息  
    /// 存放场景中所有能够显示的单位  
    /// </summary>
    class EM_Sight : EntityManagerBase {
        public static EM_Sight s_instance;
        private Dictionary<int, E_Unit> m_netIdAndUnitVisibleDict = new Dictionary<int, E_Unit> ();
        /// <summary>
        /// 每个单位视野范围内的角色NetId (包括自身, 若为角色)
        /// </summary>
        private Dictionary<int, HashSet<int>> m_unitInSightCharacterDict = new Dictionary<int, HashSet<int>> ();
        /// <summary>
        /// 每个角色视野中的单位 (不包括自身)
        /// </summary>
        private Dictionary<int, List<E_Unit>> m_characterSightDict = new Dictionary<int, List<E_Unit>> ();
        /// <summary>
        /// 为角色初始化视野, 并加入可视单位
        /// </summary>
        public void InitCharacter (E_Character charObj) {
            m_characterSightDict.TryAdd (charObj.m_networkId, new List<E_Unit> ());
            m_netIdAndUnitVisibleDict.TryAdd (charObj.m_networkId, charObj);
            m_unitInSightCharacterDict.TryAdd (charObj.m_networkId, new HashSet<int> { charObj.m_networkId });
        }
        public void InitMonster (E_Monster mon) {
            m_netIdAndUnitVisibleDict.Add (mon.m_networkId, mon);
            m_unitInSightCharacterDict.TryAdd (mon.m_networkId, new HashSet<int> { });
        }
        /// <summary>
        /// 移除一个角色的视野信息, 并移除可视单位
        /// </summary>
        public void RemoveCharacter (int netId) {
            // 移除角色在其他单位中的视野 (仅inSightChar)
            var charSight = GetCharacterRawSight (netId);
            if (charSight == null) return;
            for (int i = 0; i < charSight.Count; i++) {
                var unitInSightChar = GetRawUnitInSightCharacter (charSight[i].m_networkId);
                if (unitInSightChar == null) continue;
                unitInSightChar.Remove (netId);
            }
            // 移除该角色视野
            m_characterSightDict.Remove (netId);
            m_netIdAndUnitVisibleDict.Remove (netId);
        }
        /// <summary>
        /// 获取角色视野内的单位 (不包括自身)  
        /// 可读写  
        /// </summary>
        public List<E_Unit> GetCharacterRawSight (int netId) {
            List<E_Unit> res;
            m_characterSightDict.TryGetValue (netId, out res);
            return res;
        }
        public HashSet<int> GetRawUnitInSightCharacter (int netId) {
            HashSet<int> res;
            m_unitInSightCharacterDict.TryGetValue (netId, out res);
            return res;
        }
        /// <summary>
        /// 获取一个单位视野内的角色的netId
        /// </summary>
        public List<int> GetInSightCharacterNetworkId (int netId, bool includeSelf) {
            var units = GetRawUnitInSightCharacter (netId);
            if (units == null) return null;
            var res = new List<int> ();
            var en = units.GetEnumerator ();
            while (en.MoveNext ()) {
                // 检查是否包含自身
                if (en.Current == netId && !includeSelf)
                    continue;
                res.Add (en.Current);
            }
            return res;
        }
        /// <summary>
        /// 根据NetId获取可视单位
        /// </summary>
        public E_Unit GetUnitVisibleByNetworkId (int netId) {
            E_Unit res = null;
            m_netIdAndUnitVisibleDict.TryGetValue (netId, out res);
            return res;
        }
        /// <summary>
        /// 获取所有可视单位的迭代器
        /// </summary>
        public Dictionary<int, E_Unit>.ValueCollection.Enumerator GetUnitVisibleEnumerator () {
            return m_netIdAndUnitVisibleDict.Values.GetEnumerator ();
        }
    }
}