using System.Collections.Generic;
using UnityEngine;

namespace MirRemake {
    class E_Monster : E_ActorUnit {
        public override ActorUnitType m_ActorUnitType { get { return ActorUnitType.Monster; } }
        private MFSM m_mFSM;
        private E_Skill[] m_skillArr;
        // 怪物仇恨度哈希表
        private Dictionary<int, MyTimer.Time> m_networkIdAndHatredRefreshDict = new Dictionary<int, MyTimer.Time> ();
        public E_ActorUnit m_highestHatredTarget;
        public E_Monster (int networkId, int monsterId, Vector2 pos) {
            m_mFSM = new MFSM (new MFSMS_Free (this));

            m_networkId = networkId;
            m_position = pos;

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
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.SPEED, 700);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.CRITICAL_RATE, 150);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.CRITICAL_BONUS, 150);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.HIT_RATE, 150);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.DODGE_RATE, 150);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.FAINT, 0);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.SILENT, 0);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.IMMOBILE, 0);
            // TODO: 并把等级等发送到客户端

            m_skillArr = new E_Skill[2] { new E_Skill (0), new E_Skill (1) };
        }
        /// <summary>
        /// 获得自身的随机一个技能
        /// </summary>
        /// <returns></returns>
        public E_Skill GetRandomSkill () {
            int i = MyRandom.NextInt (0, m_skillArr.Length);
            return m_skillArr[i];
        }
        public override void Tick (float dT) {
            base.Tick (dT);

            var hatredEn = m_networkIdAndHatredRefreshDict.GetEnumerator ();
            MyTimer.Time maxHatredTime = MyTimer.s_CurTime;
            E_ActorUnit attacker = null;
            List<int> enemyNetIdToRemoveList = new List<int> ();
            while (hatredEn.MoveNext ()) {
                var target = SM_ActorUnit.s_instance.GetActorUnitByNetworkId (hatredEn.Current.Key);
                // 更新掉线单位 与 仇恨结束
                if (target == null || MyTimer.CheckTimeUp (hatredEn.Current.Value)) {
                    enemyNetIdToRemoveList.Add (hatredEn.Current.Key);
                    continue;
                }
                if (hatredEn.Current.Value >= maxHatredTime) {
                    // 寻找最高仇恨目标
                    maxHatredTime = hatredEn.Current.Value;
                    attacker = target;
                }
            }
            // 移除仇恨度降至0或以下的或掉线的目标
            for (int i = 0; i < enemyNetIdToRemoveList.Count; i++)
                m_networkIdAndHatredRefreshDict.Remove (enemyNetIdToRemoveList[i]);

            // 更新仇恨目标
            m_highestHatredTarget = attacker;

            // MonsterFSM的Tick
            m_mFSM.Tick (dT);
        }
        protected override bool CalculateAndApplyEffect (int attackerNetId, E_Effect initEffect, out E_Status[] newStatusArr) {
            bool hit = base.CalculateAndApplyEffect (attackerNetId, initEffect, out newStatusArr);
            // 若命中
            if (hit) {
                // 计算仇恨
                MyTimer.Time hatred;
                if (!m_networkIdAndHatredRefreshDict.TryGetValue (attackerNetId, out hatred))
                    hatred = MyTimer.s_CurTime;
                hatred.Tick (10f * (-(float) initEffect.m_deltaHP / (float) m_MaxHP - (float) initEffect.m_deltaMP * 0.5f / (float) m_MaxMP + (float) newStatusArr.Length * 0.1f));
                m_networkIdAndHatredRefreshDict[attackerNetId] = hatred;
            }
            return hit;
        }
        public override List<E_Item> DropLegacy () {
            return null;
        }
    }
}