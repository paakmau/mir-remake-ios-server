using System.Numerics;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.GameLogic {
    partial class GL_MonsterAction {
        private class MFSM {
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
                m_curState = m_autoMove;
                m_curState.OnEnter (monster, MFSMStateType.DEAD);
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

        enum MFSMStateType : byte {
            AUTO_MOVE,
            AUTO_BATTLE,
            CAST_FRONT,
            CAST_BACK,
            FAINT,
            DEAD
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
                    Vector2 dirNorm;
                    if (dir.LengthSquared () <= float.Epsilon)
                        dirNorm = Vector2.UnitX;
                    else dirNorm = Vector2.Normalize (dir);
                    var deltaP = dirNorm * self.m_Speed * dT * 0.01f;
                    // Console.WriteLine ("MFSMS_AutoMove 每帧位移" + deltaP);
                    if (deltaP.LengthSquared () >= dir.LengthSquared ())
                        deltaP = dir;
                    self.m_position = self.m_position + deltaP;
                    if ((self.m_position - m_targetPos).LengthSquared () <= 0.01f) {
                        m_moveTimeLeft = MyRandom.NextFloat (5f, 10f);
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
                Vector2 dirNorm;
                if (dir.LengthSquared () <= float.Epsilon)
                    dirNorm = Vector2.UnitX;
                else dirNorm = Vector2.Normalize (dir);
                var deltaP = dirNorm * self.m_Speed * dT / 100f;
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
                self.Respawn ();
                GL_MonsterAction.s_instance.MFSMRespawn (self);
            }
        }
    }
}