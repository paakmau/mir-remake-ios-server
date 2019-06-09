using System;
using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 控制Monster的AI
    /// </summary>
    class GL_MonsterAction : GameLogicBase {
        public static GL_MonsterAction s_instance;
        enum MFSMStateType : byte {
            AUTO_MOVE,
            AUTO_BATTLE,
            CAST_FRONT,
            CAST_BACK,
            FAINT,
            DEAD
        }
        class MFSM {
            public MFSMStateBase m_curState;
            public MFSMS_AutoMove m_autoMove;
            public MFSMS_AutoBattle m_autoBattle;
            public MFSMS_CastFront m_castFront;
            public MFSMS_CastBack m_castBack;
            public MFSMS_Faint m_faint;
            public MFSMS_Dead m_dead;
            private E_Monster m_monster;
            public MFSM (E_Monster monster) {
                m_monster = monster;
                m_autoMove = new MFSMS_AutoMove (this);
                m_autoBattle = new MFSMS_AutoBattle (this);
                m_castFront = new MFSMS_CastFront (this);
                m_castBack = new MFSMS_CastBack (this);
                m_faint = new MFSMS_Faint (this);
                m_dead = new MFSMS_Dead (this);
                m_curState = m_dead;
            }
            public void Tick (float dT) {
                m_curState.OnTick (m_monster, dT);
                MFSMStateBase nextState = m_curState.GetNextState (m_monster);
                if (nextState != null) {
                    // 自动转移
                    m_curState.OnExit (m_monster, nextState.m_Type);
                    nextState.OnEnter (m_monster, m_curState.m_Type);
                    m_curState = nextState;
                }
            }
        }
        abstract class MFSMStateBase {
            /// <summary>
            /// 状态的类别
            /// </summary>
            public abstract MFSMStateType m_Type { get; }
            public MFSM m_mfsm;
            public MFSMStateBase (MFSM mfsm) {
                m_mfsm = mfsm;
            }
            public abstract void OnEnter (E_Monster self, MFSMStateType prevType);
            /// <summary>
            /// 每帧调用, 可对unit施加影响
            /// </summary>
            /// <param name="self"></param>
            /// <param name="dT"></param>
            public abstract void OnTick (E_Monster self, float dT);
            /// <summary>
            /// 每帧调用, 判断需要转移到的下一个状态  
            /// </summary>
            /// <returns>
            /// 若需要退出, 则返回下一个状态  
            /// 否则返回null  
            /// </returns>
            public abstract MFSMStateBase GetNextState (E_Monster self);
            public abstract void OnExit (E_Monster self, MFSMStateType nextType);
        }
        class MFSMS_AutoMove : MFSMStateBase {
            public override MFSMStateType m_Type { get { return MFSMStateType.AUTO_MOVE; } }
            private float m_moveTimeLeft;
            private Vector2 m_targetPos;
            public MFSMS_AutoMove (MFSM mfsm) : base (mfsm) { }
            public override void OnEnter (E_Monster self, MFSMStateType prevType) {
                m_moveTimeLeft = 0f;
                m_targetPos = self.m_position;
            }
            public override void OnTick (E_Monster self, float dT) {
                if (m_moveTimeLeft > 0f)
                    m_moveTimeLeft -= dT;
                else {
                    var dir = m_targetPos - self.m_position;
                    var deltaP = Vector2.Normalize (dir) * self.m_Speed * dT / 100f;
                    if (deltaP.LengthSquared () >= dir.LengthSquared ())
                        deltaP = dir;
                    self.m_position = self.m_position + deltaP;
                    if ((self.m_position - m_targetPos).LengthSquared () <= 0.01f) {
                        m_moveTimeLeft = MyRandom.NextFloat (3f, 6f);
                        m_targetPos = self.m_respawnPosition + new Vector2 (MyRandom.NextFloat (0f, 2.5f), MyRandom.NextFloat (0f, 2.5f));
                    }
                }
            }
            public override MFSMStateBase GetNextState (E_Monster self) {
                if (self.m_IsDead) return m_mfsm.m_dead;
                if (self.m_IsFaint) return m_mfsm.m_faint;
                if (self.m_HighestHatredTargetNetId != -1)
                    return m_mfsm.m_autoBattle;
                return null;
            }
            public override void OnExit (E_Monster self, MFSMStateType nextType) { }
        }
        class MFSMS_AutoBattle : MFSMStateBase {
            public override MFSMStateType m_Type { get { return MFSMStateType.AUTO_BATTLE; } }
            public MFSMS_AutoBattle (MFSM mfsm) : base (mfsm) { }
            public override void OnEnter (E_Monster self, MFSMStateType prevType) { }
            public override void OnTick (E_Monster self, float dT) {
                if (self.m_HighestHatredTargetNetId == -1)
                    return;
                var unit = EM_Sight.s_instance.GetUnitVisibleByNetworkId (self.m_HighestHatredTargetNetId);
                if (unit == null)
                    return;
                var dir = unit.m_position - self.m_position;
                var deltaP = Vector2.Normalize (dir) * self.m_Speed * dT / 100f;
                if (deltaP.LengthSquared () >= dir.LengthSquared ())
                    deltaP = dir;
                self.m_position = self.m_position + deltaP;
            }
            public override MFSMStateBase GetNextState (E_Monster self) {
                if (self.m_IsDead) return m_mfsm.m_dead;
                if (self.m_IsFaint) return m_mfsm.m_faint;
                if (self.m_HighestHatredTargetNetId == -1)
                    return m_mfsm.m_autoMove;
                if (self.m_IsSilent)
                    return null;
                var unit = EM_Sight.s_instance.GetUnitVisibleByNetworkId (self.m_HighestHatredTargetNetId);
                if (unit == null)
                    return null;
                // 尝试对仇恨最高的目标释放技能
                E_MonsterSkill skill;
                if (EM_MonsterSkill.s_instance.GetRandomValidSkill (self.m_networkId, self.m_MonsterId, out skill)) {
                    var spg = SkillParamGeneratorBase.s_spgDict[skill.m_AimType];
                    if (spg.InCastRange (self, skill.m_CastRange, unit)) {
                        SkillParam sp = spg.GetSkillParam (self, unit);
                        var castState = m_mfsm.m_castFront;
                        castState.Reset (skill, sp);
                        return castState;
                    }
                }
                return null;
            }
            public override void OnExit (E_Monster self, MFSMStateType nextType) { }
        }
        class MFSMS_CastFront : MFSMStateBase {
            public override MFSMStateType m_Type { get { return MFSMStateType.CAST_FRONT; } }
            private E_MonsterSkill m_skill;
            private SkillParam m_skillParam;
            private float m_timer;
            public MFSMS_CastFront (MFSM mfsm) : base (mfsm) { }
            public void Reset (E_MonsterSkill skill, SkillParam parm) {
                m_skill = skill;
                m_skillParam = parm;
                m_timer = skill.m_CastFrontTime;
            }
            public override void OnEnter (E_Monster self, MFSMStateType prevType) {
                GL_MonsterAction.s_instance.MFSMCastSkillBegin (self.m_networkId, m_skill, m_skillParam);
            }
            public override void OnTick (E_Monster self, float dT) {
                m_timer -= dT;
            }
            public override MFSMStateBase GetNextState (E_Monster self) {
                if (self.m_IsDead) return m_mfsm.m_dead;
                if (self.m_IsFaint) return m_mfsm.m_faint;
                // 咏唱结束
                if (m_timer <= 0f) {
                    var castBackState = m_mfsm.m_castBack;
                    castBackState.Reset (m_skill.m_CastBackTime);
                    return castBackState;
                }
                // 被沉默
                if (self.m_IsSilent)
                    return m_mfsm.m_autoBattle;
                return null;
            }
            public override void OnExit (E_Monster self, MFSMStateType nextType) {
                if (nextType == MFSMStateType.CAST_BACK) {
                    GL_MonsterAction.s_instance.MFSMSkillSettle (self, m_skill, m_skillParam);
                }
            }
        }
        class MFSMS_CastBack : MFSMStateBase {
            public override MFSMStateType m_Type { get { return MFSMStateType.CAST_BACK; } }
            private float m_timer;
            public MFSMS_CastBack (MFSM mfsm) : base (mfsm) { }
            public void Reset (float backTime) { m_timer = backTime; }
            public override void OnEnter (E_Monster self, MFSMStateType prevType) { }
            public override void OnTick (E_Monster self, float dT) {
                m_timer -= dT;
            }
            public override MFSMStateBase GetNextState (E_Monster self) {
                if (self.m_IsDead) return m_mfsm.m_dead;
                if (self.m_IsFaint) return m_mfsm.m_faint;
                // 后摇结束
                if (m_timer <= 0)
                    return m_mfsm.m_autoBattle;
                return null;
            }
            public override void OnExit (E_Monster self, MFSMStateType nextType) { }
        }
        class MFSMS_Faint : MFSMStateBase {
            public override MFSMStateType m_Type { get { return MFSMStateType.FAINT; } }
            public override void OnEnter (E_Monster self, MFSMStateType prevType) { }
            public override void OnTick (E_Monster self, float dT) { }
            public MFSMS_Faint (MFSM mfsm) : base (mfsm) { }
            public override MFSMStateBase GetNextState (E_Monster self) {
                if (self.m_IsDead) return m_mfsm.m_dead;
                if (!self.m_IsFaint) return m_mfsm.m_autoBattle;
                return null;
            }
            public override void OnExit (E_Monster self, MFSMStateType nextType) { }
        }
        class MFSMS_Dead : MFSMStateBase {
            public override MFSMStateType m_Type { get { return MFSMStateType.DEAD; } }
            private const float c_respawnTimeMin = 10f;
            private const float c_respawnTimeMax = 15f;
            private float m_timer;
            public MFSMS_Dead (MFSM mfsm) : base (mfsm) { }
            public override void OnEnter (E_Monster self, MFSMStateType prevType) {
                m_timer = MyRandom.NextFloat (c_respawnTimeMin, c_respawnTimeMax);
                GL_MonsterAction.s_instance.MFSMDead (self);
            }
            public override void OnTick (E_Monster self, float dT) {
                m_timer -= dT;
            }
            public override MFSMStateBase GetNextState (E_Monster self) {
                if (m_timer <= 0)
                    return m_mfsm.m_autoMove;
                return null;
            }
            public override void OnExit (E_Monster self, MFSMStateType nextType) {
                self.m_position = self.m_respawnPosition;
                GL_MonsterAction.s_instance.MFSMRespawn (self);
            }
        }
        abstract class SkillParamGeneratorBase {
            public static Dictionary<SkillAimType, SkillParamGeneratorBase> s_spgDict = new Dictionary<SkillAimType, SkillParamGeneratorBase> () { { SkillAimType.AIM_CIRCLE, new SPG_AimCircle () } };
            public abstract SkillAimType m_AimType { get; }
            public virtual bool InCastRange (E_Unit self, float castRange, E_Unit aimedTarget) {
                return (self.m_position - aimedTarget.m_position).LengthSquared () <= castRange * castRange;
            }
            /// <summary>
            /// 根据目标完善技能参数
            /// </summary>
            /// <returns></returns>
            public abstract SkillParam GetSkillParam (E_Unit self, E_Unit aimedTarget);
        }
        class SPG_AimCircle : SkillParamGeneratorBase {
            public override SkillAimType m_AimType { get { return SkillAimType.AIM_CIRCLE; } }
            public override SkillParam GetSkillParam (E_Unit self, E_Unit aimedTarget) {
                return new SkillParam (m_AimType, aimedTarget, Vector2.Zero, Vector2.Zero);
            }
        }
        private Dictionary<int, MFSM> m_mfsmDict = new Dictionary<int, MFSM> ();
        public GL_MonsterAction (INetworkService netService) : base (netService) {
            var monEn = EM_Unit.s_instance.GetMonsterEn ();
            while (monEn.MoveNext ()) {
                var mfsm = new MFSM (monEn.Current.Value);
                m_mfsmDict.Add (monEn.Current.Key, mfsm);
            }
        }
        public override void Tick (float dT) {
            var monEn = EM_Unit.s_instance.GetMonsterEn ();
            while (monEn.MoveNext ()) {
                // 处理仇恨消失
                var hatredEn = monEn.Current.Value.m_hatredRefreshDict.GetEnumerator ();
                var hTarRemoveList = new List<int> ();
                while (hatredEn.MoveNext ()) {
                    // 仇恨时间到
                    if (MyTimer.CheckTimeUp (hatredEn.Current.Value)) {
                        hTarRemoveList.Add (hatredEn.Current.Key);
                        continue;
                    }
                    // 仇恨目标下线或死亡
                    var tar = EM_Sight.s_instance.GetUnitVisibleByNetworkId (hatredEn.Current.Key);
                    if (tar == null || tar.m_IsDead) {
                        hTarRemoveList.Add (hatredEn.Current.Key);
                        continue;
                    }
                }
                for (int i = 0; i < hTarRemoveList.Count; i++)
                    monEn.Current.Value.m_hatredRefreshDict.Remove (hTarRemoveList[i]);
            }
            // fsm控制AI
            var mfsmEn = m_mfsmDict.GetEnumerator ();
            while (mfsmEn.MoveNext ())
                mfsmEn.Current.Value.Tick (dT);
        }
        public override void NetworkTick () { }
        public void MFSMCastSkillBegin (int netId, E_MonsterSkill skill, SkillParam sp) {
            m_networkService.SendServerCommand (SC_ApplyOtherCastSkillBegin.Instance (
                EM_Sight.s_instance.GetCharacterInSightNetworkId (netId, false),
                netId,
                skill.m_skillId,
                sp.GetNo ()));
        }
        public void MFSMSkillSettle (E_Monster monster, E_MonsterSkill skill, SkillParam sp) {
            GL_BattleSettle.s_instance.NotifySkillSettle (monster, skill, sp);
        }
        public void MFSMDead (E_Monster monster) { }
        public void MFSMRespawn (E_Monster monster) { }
    }
}