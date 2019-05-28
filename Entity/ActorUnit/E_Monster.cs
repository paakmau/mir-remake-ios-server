using System;
using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    class E_Monster : E_ActorUnit {
        private DE_Monster m_monsterDe;
        public override ActorUnitType m_ActorUnitType { get { return ActorUnitType.MONSTER; } }
        public Vector2 m_respawnPosition;
        private ValueTuple<DE_Skill, DE_SkillData>[] m_skillArr;
        // 技能冷却
        private List<ValueTuple<short, MyTimer.Time>> m_skillIdAndCoolDownList = new List<ValueTuple<short, MyTimer.Time>> ();
        // 怪物仇恨度哈希表
        private Dictionary<int, MyTimer.Time> m_networkIdAndHatredRefreshDict = new Dictionary<int, MyTimer.Time> ();
        public E_ActorUnit m_highestHatredTarget;
        public void Reset (int networkId, Vector2 pos, ValueTuple<DE_Skill, DE_SkillData>[] skillArr, DE_ActorUnit auDe, DE_Monster mDe) {
            base.Reset (auDe);
            m_monsterDe = mDe;
            m_networkId = networkId;
            m_position = pos;
            m_respawnPosition = pos;
            m_skillArr = skillArr;
            m_level = mDe.m_level;
        }
        private bool IsSkillValid (short skillId) {
            for (int i = 0; i < m_skillIdAndCoolDownList.Count; i++) {
                if (m_skillIdAndCoolDownList[i].Item1 == skillId)
                    return false;
            }
            return true;
        }
        public bool GetRandomValidSkill (out ValueTuple<DE_Skill, DE_SkillData> resSkill) {
            foreach (var skill in m_skillArr)
                if (IsSkillValid (skill.Item1.m_skillId)) {
                    resSkill = skill;
                    return true;
                }
            resSkill = default;
            return false;
        }
        public void SetSkillCoolDown (ValueTuple<DE_Skill, DE_SkillData> skill) {
            m_skillIdAndCoolDownList.Add (
                new ValueTuple<short, MyTimer.Time> (
                    skill.Item1.m_skillId, MyTimer.s_CurTime.Ticked (skill.Item2.m_coolDownTime)));
        }
        public List<ValueTuple<short, MyTimer.Time>> GetRawSkillCoolDownList () {
            return m_skillIdAndCoolDownList;
        }
        // public void FSMCastSkillBegin (E_Skill skill, SkillParam parm) {
        //     SM_ActorUnit.s_instance.NotifyApplyCastSkillBegin (this, skill, parm);
        // }
        // public void FSMCastSkillSettle (E_Skill skill, SkillParam parm) {
        //     m_skillIdAndCoolDownList.Add (new KeyValuePair<short, MyTimer.Time> (skill.m_id, MyTimer.s_CurTime.Ticked (skill.m_coolDownTime)));
        //     List<E_ActorUnit> unitList = skill.GetEffectTargets (this, parm);
        //     SM_ActorUnit.s_instance.NotifyApplyCastSkillSettle (this, skill, unitList);
        // }
        // public override void Tick (float dT) {
        //     base.Tick (dT);

        //     // 关于技能冷却
        //     for (int i = m_skillIdAndCoolDownList.Count - 1; i >= 0; i--)
        //         if (MyTimer.CheckTimeUp (m_skillIdAndCoolDownList[i].Value))
        //             m_skillIdAndCoolDownList.RemoveAt (i);

        //     // 关于仇恨
        //     var hatredEn = m_networkIdAndHatredRefreshDict.GetEnumerator ();
        //     MyTimer.Time maxHatredTime = MyTimer.s_CurTime;
        //     E_ActorUnit attacker = null;
        //     List<int> enemyNetIdToRemoveList = new List<int> ();
        //     while (hatredEn.MoveNext ()) {
        //         var target = EM_ActorUnit.GetActorUnitByNetworkId (hatredEn.Current.Key);
        //         // 更新掉线单位 与 仇恨结束
        //         if (target == null || MyTimer.CheckTimeUp (hatredEn.Current.Value)) {
        //             enemyNetIdToRemoveList.Add (hatredEn.Current.Key);
        //             continue;
        //         }
        //         if (hatredEn.Current.Value >= maxHatredTime) {
        //             // 寻找最高仇恨目标
        //             maxHatredTime = hatredEn.Current.Value;
        //             attacker = target;
        //         }
        //     }
        //     // 移除仇恨度降至0或以下的或掉线的目标
        //     for (int i = 0; i < enemyNetIdToRemoveList.Count; i++)
        //         m_networkIdAndHatredRefreshDict.Remove (enemyNetIdToRemoveList[i]);
        //     // 更新仇恨目标
        //     m_highestHatredTarget = attacker;

        //     // 关于MonsterFSM
        //     m_mFSM.Tick (dT);
        // }
        // protected override bool CalculateAndApplyEffectToSelf (int attackerNetId, E_Effect initEffect, out E_Status[] newStatusArr) {
        //     bool hit = base.CalculateAndApplyEffectToSelf (attackerNetId, initEffect, out newStatusArr);
        //     // 若命中
        //     if (hit) {
        //         // 计算仇恨
        //         MyTimer.Time hatred;
        //         if (!m_networkIdAndHatredRefreshDict.TryGetValue (attackerNetId, out hatred))
        //             hatred = MyTimer.s_CurTime;
        //         hatred.Tick (10f * (-(float) initEffect.m_deltaHp / (float) m_MaxHP - (float) initEffect.m_deltaMp * 0.5f / (float) m_MaxMP + (float) newStatusArr.Length * 0.1f));
        //         m_networkIdAndHatredRefreshDict[attackerNetId] = hatred;
        //     }
        //     return hit;
        // }
        // public override List<E_Item> DropLegacy () {
        //     return null;
        // }
    }
}