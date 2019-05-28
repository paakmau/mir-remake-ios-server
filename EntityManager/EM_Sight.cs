using System.Collections.Generic;
using MirRemakeBackend.Entity;

namespace MirRemakeBackend.EntityManager {
    /// <summary>
    /// 管理场景中Character的视野信息  
    /// 存放场景中所有能够显示的单位  
    /// </summary>
    class EM_Sight : EntityManagerBase {
        public static EM_Sight s_instance;
        private Dictionary<int, E_ActorUnit> m_networkIdAndActorUnitVisibleDict = new Dictionary<int, E_ActorUnit> ();
        /// <summary>
        /// 每个角色的视野信息
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, List<E_ActorUnit>> m_networkIdAndActorUnitListInSightDict = new Dictionary<int, List<E_ActorUnit>> ();
        /// <summary>
        /// 每个单位在玩家视野中的信息
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, List<E_Character>> m_networkIdAndCharacterListInSightDict = new Dictionary<int, List<E_Character>> ();
        /// <summary>
        /// 根据NetId获取他的视野内的单位 (包括自身)  
        /// 可以对其视野进行读写  
        /// </summary>
        public List<E_ActorUnit> GetRawActorUnitsInSightByNetworkId (int netId) {
            List<E_ActorUnit> res = null;
            m_networkIdAndActorUnitListInSightDict.TryGetValue (netId, out res);
            return res;
        }
        /// <summary>
        /// 根据NetId获取可视单位
        /// </summary>
        public E_ActorUnit GetActorUnitVisibleByNetworkId (int netId) {
            E_ActorUnit res = null;
            m_networkIdAndActorUnitVisibleDict.TryGetValue (netId, out res);
            return res;
        }
        /// <summary>
        /// 获取所有可视单位的迭代器
        /// </summary>
        public Dictionary<int, E_ActorUnit>.ValueCollection.Enumerator GetActorUnitVisibleEnumerator () {
            return m_networkIdAndActorUnitVisibleDict.Values.GetEnumerator ();
        }
        /// <summary>
        /// 为角色初始化视野
        /// </summary>
        public void InitCharacterSight (int netId) {
            m_networkIdAndActorUnitListInSightDict.Add (netId, new List<E_ActorUnit> ());
        }
        /// <summary>
        /// 移除一个角色的视野信息
        /// </summary>
        public void RemoveCharacterSight (int netId) {
            m_networkIdAndActorUnitListInSightDict.Remove (netId);
        }
        /// <summary>
        /// 添加可视单位
        /// </summary>
        public void SetUnitVisible (E_ActorUnit unit) {
            m_networkIdAndActorUnitVisibleDict[unit.m_networkId] = unit;
        }
        /// <summary>
        /// 移除可视单位
        /// </summary>
        public void SetUnitInvisible (int netId) {
            m_networkIdAndActorUnitVisibleDict.Remove (netId);
        }
    }
}