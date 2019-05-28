using System;
using System.Collections.Generic;
using System.Numerics;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;
using MirRemakeBackend.EntityManager;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 控制Monster的AI
    /// </summary>
    partial class GL_MonsterAction : GameLogicBase {
        private Dictionary<int, MFSM> m_mfsmDict = new Dictionary<int, MFSM> ();
        public GL_MonsterAction (INetworkService netService) : base (netService) {
            var monEn = EM_ActorUnit.s_instance.GetMonsterEn ();
            while (monEn.MoveNext ()) {
                var mfsm = new MFSM (monEn.Current.Value);
            }
        }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
    }
}