using MirRemakeBackend.Network;
using MirRemakeBackend.Entity;
using MirRemakeBackend.EntityManager;

namespace MirRemakeBackend.GameLogic {
    class GL_Effect : GameLogicBase {
        public GL_Effect (INetworkService netService) : base (netService) {
            Messenger.AddListener<Effect, E_ActorUnit> ("NotifyApplyEffect", NotifyApplyEffect);
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        /// <summary>
        /// 对目标的属性和状态添加影响
        /// </summary>
        /// <param name="target"></param>
        public void NotifyApplyEffect (Effect effect, E_ActorUnit target) {
            if (effect.m_hit) {
                target.m_CurHp += effect.m_deltaHp;
                target.m_CurMp += effect.m_deltaMp;
                EM_Status.s_instance.AddStatus (target.m_networkId, effect.m_statusIdAndValueAndTimeAndCasterNetIdArr);
            }
        }
    }
}