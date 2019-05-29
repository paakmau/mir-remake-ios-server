using System;
using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Entity;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 处理单位的属性变化
    /// </summary>
    class GL_ActorUnit : GameLogicBase {
        public static GL_ActorUnit s_instance;
        private float deltaTimeAfterLastSecond = 0f;
        public GL_ActorUnit (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) {
            // 根据状态处理具体属性的每秒变化
            deltaTimeAfterLastSecond += dT;
            if (deltaTimeAfterLastSecond >= 1.0f) {
                deltaTimeAfterLastSecond -= 1.0f;
                var en = EM_Sight.s_instance.GetActorUnitVisibleEnumerator ();
                while (en.MoveNext ()) {
                    if (en.Current.m_IsDead)
                        continue;
                    int newHP = en.Current.m_CurHp + en.Current.m_DeltaHpPerSecond;
                    int newMP = en.Current.m_CurMp + en.Current.m_DeltaMpPerSecond;
                    en.Current.m_CurHp = Math.Max (Math.Min (newHP, en.Current.m_MaxHp), 0);
                    en.Current.m_CurMp = Math.Max (Math.Min (newMP, en.Current.m_MaxMp), 0);
                }
            }
        }
        public override void NetworkTick () { }
    }
}