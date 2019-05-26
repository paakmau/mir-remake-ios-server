using System;
using System.Buffers;
using System.Collections.Generic;
using MirRemakeBackend.DataEntity;
using MirRemakeBackend.Entity;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Network;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.GameLogic {
    partial class GL_BattleSettle : GameLogicBase {
        public void EffectSettle (E_ActorUnit caster, DE_Effect effectDe, IReadOnlyList<E_ActorUnit> targetList) {
            for (int i = 0; i < targetList.Count; i++) {
                Messenger.Broadcast<DE_Effect, E_ActorUnit, E_ActorUnit> ("NotifyApplyEffect", effectDe, caster, targetList[i]);
                // TODO: 向Client发Effect
            }
        }
    }
}