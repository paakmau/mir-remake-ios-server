using System;
using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Entity;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.GameLogic {
    partial class GL_MonsterAction : GameLogicBase {
        enum MFSMStateType : byte {
            AUTO_MOVE,
            AUTO_BATTLE,
            CAST_SING_AND_FRONT,
            CAST_BACK,
            FAINT,
            DEAD
        }

        class MFSM {
            public MFSMStateBase m_curState;
            private E_Monster m_monster;
            public MFSM (E_Monster monster) {
                m_monster = monster;
                m_curState = new MFSMS_Dead ();
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
            /// /// </summary>
            public abstract MFSMStateType m_Type { get; }
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
                if (self.m_IsDead)
                    return new MFSMS_Dead ();
                if (self.m_IsFaint)
                    return new MFSMS_Faint ();
                if (self.m_highestHatredTarget != null)
                    return new MFSMS_AutoBattle ();
                return null;
            }
            public override void OnExit (E_Monster self, MFSMStateType nextType) { }
        }

        class MFSMS_AutoBattle : MFSMStateBase {
            public override MFSMStateType m_Type { get { return MFSMStateType.AUTO_BATTLE; } }
            public override void OnEnter (E_Monster self, MFSMStateType prevType) { }
            public override void OnTick (E_Monster self, float dT) {
                if (self.m_highestHatredTarget != null) {
                    var dir = self.m_highestHatredTarget.m_position - self.m_position;
                    var deltaP = Vector2.Normalize (dir) * self.m_Speed * dT / 100f;
                    if (deltaP.LengthSquared () >= dir.LengthSquared ())
                        deltaP = dir;
                    self.m_position = self.m_position + deltaP;
                }
            }
            public override MFSMStateBase GetNextState (E_Monster self) {
                if (self.m_IsDead)
                    return new MFSMS_Dead ();
                if (self.m_IsFaint)
                    return new MFSMS_Faint ();
                if (self.m_highestHatredTarget == null)
                    return new MFSMS_AutoMove ();
                if (self.m_IsSilent)
                    return null;
                // 尝试对仇恨最高的目标释放技能
                ValueTuple<DE_Skill, DE_SkillData> skill;
                if (EM_Skill.s_instance.GetMonsterRandomValidSkill(self.m_networkId, self.m_MonsterId, out skill)) {
                    var spg = SkillParamGeneratorBase.s_spgDict[skill.Item1.m_skillAimType];
                    if (spg.InCastRange (self, skill.Item2.m_castRange, self.m_highestHatredTarget)) {
                        SkillParam sp = spg.GetSkillParam (self, self.m_highestHatredTarget);
                        var castState = new MFSMS_CastSingAndFront ();
                        castState.Reset (skill, sp);
                        return castState;
                    }
                }
                return null;
            }
            public override void OnExit (E_Monster self, MFSMStateType nextType) { }
        }
        class MFSMS_CastSingAndFront : MFSMStateBase {
            public override MFSMStateType m_Type { get { return MFSMStateType.CAST_SING_AND_FRONT; } }
            private ValueTuple<DE_Skill, DE_SkillData> m_skill;
            private SkillParam m_skillParam;
            private float m_timer;
            public void Reset (ValueTuple<DE_Skill, DE_SkillData> skill, SkillParam parm) {
                m_skill = skill;
                m_skillParam = parm;
                m_timer = skill.Item2.m_singTime + skill.Item2.m_castFrontTime;
            }
            public override void OnEnter (E_Monster self, MFSMStateType prevType) {
                GL_MonsterAction.s_instance.MFSMCastSkillBegin (self.m_networkId, m_skill.Item1.m_skillId);
            }
            public override void OnTick (E_Monster self, float dT) {
                m_timer -= dT;
            }
            public override MFSMStateBase GetNextState (E_Monster self) {
                if (self.m_IsDead)
                    return new MFSMS_Dead ();
                if (self.m_IsFaint)
                    return new MFSMS_Faint ();
                // 咏唱结束
                if (m_timer <= 0f) {
                    var castBackState = new MFSMS_CastBack ();
                    castBackState.Reset (m_skill.Item2.m_castBackTime);
                    return castBackState;
                }
                // 被沉默
                if (self.m_IsSilent)
                    return new MFSMS_AutoBattle ();
                return null;
            }
            public override void OnExit (E_Monster self, MFSMStateType nextType) {
                if (nextType == MFSMStateType.CAST_BACK) {
                    GL_MonsterAction.s_instance.MFSMSkillSettle (self, m_skill.Item1, m_skill.Item2, m_skillParam);
                }
            }
        }
        class MFSMS_CastBack : MFSMStateBase {
            public override MFSMStateType m_Type { get { return MFSMStateType.CAST_BACK; } }
            private float m_timer;
            public void Reset (float backTime) { m_timer = backTime; }
            public override void OnEnter (E_Monster self, MFSMStateType prevType) { }
            public override void OnTick (E_Monster self, float dT) {
                m_timer -= dT;
            }
            public override MFSMStateBase GetNextState (E_Monster self) {
                if (self.m_IsDead)
                    return new MFSMS_Dead ();
                if (self.m_IsFaint)
                    return new MFSMS_Faint ();
                // 后摇结束
                if (m_timer <= 0)
                    return new MFSMS_AutoBattle ();
                return null;
            }
            public override void OnExit (E_Monster self, MFSMStateType nextType) { }
        }
        class MFSMS_Faint : MFSMStateBase {
            public override MFSMStateType m_Type { get { return MFSMStateType.FAINT; } }
            public override void OnEnter (E_Monster self, MFSMStateType prevType) { }
            public override void OnTick (E_Monster self, float dT) { }
            public override MFSMStateBase GetNextState (E_Monster self) {
                if (self.m_IsDead)
                    return new MFSMS_Dead ();
                if (self.m_IsFaint)
                    return new MFSMS_Faint ();
                return null;
            }
            public override void OnExit (E_Monster self, MFSMStateType nextType) { }
        }
        class MFSMS_Dead : MFSMStateBase {
            public override MFSMStateType m_Type { get { return MFSMStateType.DEAD; } }
            private const float c_respawnTimeMin = 10f;
            private const float c_respawnTimeMax = 15f;
            private float m_timer;
            public override void OnEnter (E_Monster self, MFSMStateType prevType) {
                m_timer = MyRandom.NextFloat (c_respawnTimeMin, c_respawnTimeMax);
                GL_MonsterAction.s_instance.MFSMDead (self);
            }
            public override void OnTick (E_Monster self, float dT) {
                m_timer -= dT;
            }
            public override MFSMStateBase GetNextState (E_Monster self) {
                if (m_timer <= 0)
                    return new MFSMS_AutoMove ();
                return null;
            }
            public override void OnExit (E_Monster self, MFSMStateType nextType) {
                self.m_position = self.m_respawnPosition;
                GL_MonsterAction.s_instance.MFSMRespawn (self);
            }
        }
    }
}