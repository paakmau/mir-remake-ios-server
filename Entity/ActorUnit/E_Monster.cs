using System.Collections.Generic;
using UnityEngine;

namespace MirRemake {
    class E_Monster : E_ActorUnit {
        public override ActorUnitType m_ActorUnitType { get { return ActorUnitType.Monster; } }
        // private FSM m_fSM;
        // 怪物仇恨度哈希表
        private Dictionary<int, float> m_networkIdAndHatredDict = new Dictionary<int, float> ();
        public E_Monster (int networkId, int monsterId, Vector2 pos) {
            // m_fSM = new FSM (new FSMS_Free (this));

            m_networkId = networkId;
            SetPosition (pos);

            // TODO: 从数据库获取怪物等级技能属性等
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.MAX_HP, 1500);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.MAX_MP, 1500);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.CURRENT_HP, 1500);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.CURRENT_MP, 1500);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.DELTA_HP_PER_SECOND, 5);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.DELTA_MP_PER_SECOND, 5);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.ATTACK, 150);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.MAGIC, 150);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.DEFENCE, 150);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.RESISTANCE, 150);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.TENACITY, 150);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.SPEED, 150);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.CRITICAL_RATE, 150);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.CRITICAL_BONUS, 150);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.HIT_RATE, 150);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.DODGE_RATE, 150);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.FAINT, 0);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.SILENT, 0);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.IMMOBILE, 0);
            // TODO: 并把等级等发送到客户端
        }
        public override void Tick (float dT) {
            base.Tick (dT);

            var hatredEn = m_networkIdAndHatredDict.GetEnumerator ();
            float maxHatred = float.MinValue;
            int attackerNetId = 0;
            List<int> enemyNetIdToRemoveList = new List<int> ();
            while (hatredEn.MoveNext ()) {
                // 更新仇恨度及其移除列表
                float newHatred = m_networkIdAndHatredDict[hatredEn.Current.Key] - dT * 0.15f;
                m_networkIdAndHatredDict[hatredEn.Current.Key] = newHatred;
                if (newHatred <= 0f)
                    enemyNetIdToRemoveList.Add (hatredEn.Current.Key);
                else if (hatredEn.Current.Value > maxHatred) {
                    // 寻找最高仇恨目标
                    maxHatred = hatredEn.Current.Value;
                    attackerNetId = hatredEn.Current.Key;
                }
            }
            // 移除仇恨度降至0或以下的目标
            for (int i = 0; i < enemyNetIdToRemoveList.Count; i++)
                m_networkIdAndHatredDict.Remove (enemyNetIdToRemoveList[i]);

            // 有仇恨目标
            if (maxHatred != float.MinValue) {

            }
        }
        protected override bool CalculateAndApplyEffect (int attackerNetId, E_Effect initEffect, out E_Status[] newStatusArr) {
            bool hit = base.CalculateAndApplyEffect (attackerNetId, initEffect, out newStatusArr);
            // 若命中
            if (hit) {
                // 计算仇恨
                float hatred = 0f;
                m_networkIdAndHatredDict.TryGetValue (attackerNetId, out hatred);
                hatred += initEffect.m_deltaHP / m_MaxHP + initEffect.m_deltaMP * 0.5f / m_MaxMP + newStatusArr.Length * 0.1f;
                m_networkIdAndHatredDict[attackerNetId] = hatred;
            }
            return hit;
        }
        public override List<E_Item> DropLegacy () {
            return null;
        }
    }
}