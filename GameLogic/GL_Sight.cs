using System.Collections.Generic;
using MirRemakeBackend.Entity;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    class GL_Sight : GameLogicBase {
        /// <summary>
        /// 每一帧计算的视野数
        /// </summary>
        private const int c_handleSightNumPerTick = 10;
        private const float c_sightRadius = 100f;
        public GL_Sight (INetworkService netService) : base (netService) {
            Messenger.AddListener<int, int> ("CommandInitCharacterId", CommandInitCharacterId);
            Messenger.AddListener<int> ("CommandRemoveCharacter", CommandRemoveCharacter);
            Messenger.AddListener<int> ("NotifyActorUnitDisappear", NotifyActorUnitDisappear);
            Messenger.AddListener<E_ActorUnit> ("NotifyActorUnitRespawn", NotifyActorUnitRespawn);
        }
        public override void Tick (float dT) {
            for (int i = 0; i < c_handleSightNumPerTick; i++) {
                // 获取循环队列中第一个角色并计算它的视野
                int charNetId = 0;
                if (!EM_Sight.s_instance.TryGetNextCharacterNetworkIdToGetSight (out charNetId))
                    return;
                var charObj = EM_ActorUnit.s_instance.GetCharacterByNetworkId (charNetId);
                var charSight = EM_Sight.s_instance.GetRawActorUnitsInSightByNetworkId (charNetId);
                charSight.Clear ();
                var unitEn = EM_Sight.s_instance.GetActorUnitVisibleEnumerator ();
                while (unitEn.MoveNext ()) {
                    if (unitEn.Current == charObj) continue;
                    if ((charObj.m_position - unitEn.Current.m_position).LengthSquared() > c_sightRadius * c_sightRadius) continue;
                    charSight.Add (unitEn.Current);
                }
            }
        }
        public override void NetworkTick () { }
        public void CommandInitCharacterId (int netId, int charId) {
            EM_Sight.s_instance.InitCharacterSight (netId);
        }
        public void CommandRemoveCharacter (int netId) {
            EM_Sight.s_instance.RemoveCharacterSight (netId);
        }
        public void NotifyActorUnitDisappear (int netId) {
            EM_Sight.s_instance.SetUnitInvisible (netId);
        }
        public void NotifyActorUnitRespawn (E_ActorUnit unit) {
            EM_Sight.s_instance.SetUnitVisible (unit);
        }
    }
}