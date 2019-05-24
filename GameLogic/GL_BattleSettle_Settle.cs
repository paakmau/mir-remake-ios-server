using System.Buffers;
using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Entity;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.GameLogic {
    partial class GL_BattleSettle : GameLogicBase {
        private struct Effect {
            public DE_Effect m_effectDe;
            public int m_deltaHp;
            public int m_deltaMp;
            public List<E_Status> m_statusAttached;
            public Effect (DE_Effect effectDe, int casterNetId) {
                m_effectDe = effectDe;
                m_deltaHp = effectDe.m_deltaHp;
                m_deltaMp = effectDe.m_deltaMp;
                m_statusAttached = new List<E_Status> ();
                for (int i = 0; i < effectDe.m_statusIdAndValueAndTimeList.Count; i++) {
                    var info = effectDe.m_statusIdAndValueAndTimeList[i];
                    DE_Status statusDe = DEM_Status.s_instance.GetStatusById (info.Item1);
                    E_Status statusObj = EM_Status.s_instance.AssignStatus ();
                    statusObj.Reset (statusDe, info.Item1, info.Item2, info.Item3, casterNetId);
                }
            }
            /// <summary>
            /// 根据释放者修改Effect
            /// </summary>
            public void CalcWithCaster (E_ActorUnit caster) {
                
            }
            /// <summary>
            /// 根据作用目标属性修改Effect
            /// </summary>
            public void CalcWithTarget (E_ActorUnit target) {

            }
        }
        private const int c_effectPoolSize = 100;
        public void EffectSettle (E_ActorUnit caster, DE_Effect effectDe, IReadOnlyList<E_ActorUnit> targetList) {
            Effect effect = new Effect (effectDe, caster.m_networkId);
            for (int i=0; i<targetList.Count; i++) {

            }
        }
    }
}