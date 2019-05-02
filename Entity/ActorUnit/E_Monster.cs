using System.Collections.Generic;
using UnityEngine;

namespace MirRemake {
    class E_Monster : E_ActorUnit {
        public override ActorUnitType m_ActorUnitType { get { return ActorUnitType.Monster; } }
        // 怪物仇恨度哈希表
        private Dictionary<E_ActorUnit, float> m_networkIdAndHatredDict = new Dictionary<E_ActorUnit, float> ();
        public E_Monster (int networkId, int monsterId, Vector2 pos) {
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

        }
        protected override bool CalculateAndApplyEffect (E_ActorUnit attacker, E_Effect initEffect, out E_Status[] newStatusArr) {
            bool hit = base.CalculateAndApplyEffect (attacker, initEffect, out newStatusArr);
            // 若命中
            if (hit) {
                // 计算仇恨
                float hatred = 0f;
                m_networkIdAndHatredDict.TryGetValue (attacker, out hatred);
                hatred += initEffect.m_deltaHP + initEffect.m_deltaMP * 0.5f + newStatusArr.Length * 0.1f;
                m_networkIdAndHatredDict[attacker] = hatred;
            }
            return hit;
        }
        public override List<E_Item> DropLegacy () {
            return null;
        }
    }
}