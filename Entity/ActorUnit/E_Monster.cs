using System.Collections.Generic;
using UnityEngine;

namespace MirRemakeBackend {
    class E_Monster : E_ActorUnit {
        public override ActorUnitType m_ActorUnitType { get { return ActorUnitType.MONSTER; } }
        private MFSM m_mFSM;
        private E_Skill[] m_skillArr;
        private List<KeyValuePair<short, MyTimer.Time>> m_skillIdAndCoolDownList = new List<KeyValuePair<short, MyTimer.Time>> ();
        // 怪物仇恨度哈希表
        private Dictionary<int, MyTimer.Time> m_networkIdAndHatredRefreshDict = new Dictionary<int, MyTimer.Time> ();
        public E_ActorUnit m_highestHatredTarget;
        public E_Monster (int networkId, int monsterId, Vector2 pos) {
            m_mFSM = new MFSM (new MFSMS_AutoMove (this));

            m_networkId = networkId;
            m_Position = pos;

            // TODO: 从数据库获取怪物等级技能属性等, 并把等级等发送到客户端
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
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.TENACITY, 15);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.SPEED, 700);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.CRITICAL_RATE, 150);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.CRITICAL_BONUS, 150);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.HIT_RATE, 150);
            m_concreteAttributeDict.Add (ActorUnitConcreteAttributeType.DODGE_RATE, 150);
            m_specialAttributeDict.Add (ActorUnitSpecialAttributeType.FAINT, 0);
            m_specialAttributeDict.Add (ActorUnitSpecialAttributeType.SILENT, 0);
            m_specialAttributeDict.Add (ActorUnitSpecialAttributeType.IMMOBILE, 0);

            m_skillArr = new E_Skill[1] { new E_Skill (0)};
        }
        /// <summary>
        /// 获得自身的随机一个不在冷却的技能
        /// </summary>
        /// <returns></returns>
        public E_Skill GetRandomValidSkill () {
            int num = MyRandom.NextInt (0, m_skillArr.Length);
            E_Skill skill = m_skillArr[num];
            // 若技能正在冷却
            for (int i = 0; i<m_skillIdAndCoolDownList.Count; i++)
                if (m_skillIdAndCoolDownList[i].Key == skill.m_id)
                    return null;
            return skill;
        }
        public void FSMCastSkillBegin (E_Skill skill, SkillParam parm) {
            SM_ActorUnit.s_instance.NotifyApplyCastSkillBegin (this, skill, parm);
        }
        public void FSMCastSkillSettle (E_Skill skill, SkillParam parm) {
            m_skillIdAndCoolDownList.Add (new KeyValuePair<short, MyTimer.Time> (skill.m_id, MyTimer.s_CurTime.Ticked (skill.m_coolDownTime)));
            List<E_ActorUnit> unitList = skill.GetEffectTargets(this, parm);
            SM_ActorUnit.s_instance.NotifyApplyCastSkillSettle (this, skill, unitList);
        }
        public override void Tick (float dT) {
            base.Tick (dT);

            // 关于技能冷却
            for (int i = m_skillIdAndCoolDownList.Count - 1; i >= 0; i--)
                if (MyTimer.CheckTimeUp (m_skillIdAndCoolDownList[i].Value))
                    m_skillIdAndCoolDownList.RemoveAt (i);

            // 关于仇恨
            var hatredEn = m_networkIdAndHatredRefreshDict.GetEnumerator ();
            MyTimer.Time maxHatredTime = MyTimer.s_CurTime;
            E_ActorUnit attacker = null;
            List<int> enemyNetIdToRemoveList = new List<int> ();
            while (hatredEn.MoveNext ()) {
                var target = EM_ActorUnit.GetActorUnitByNetworkId (hatredEn.Current.Key);
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

            // 关于MonsterFSM
            m_mFSM.Tick (dT);
        }
        protected override bool CalculateAndApplyEffectToSelf (int attackerNetId, E_Effect initEffect, out E_Status[] newStatusArr) {
            bool hit = base.CalculateAndApplyEffectToSelf (attackerNetId, initEffect, out newStatusArr);
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