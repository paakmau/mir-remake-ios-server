using System.Collections.Generic;
using UnityEngine;

using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    partial class GL_Battle {
        private INetworkService m_networkService;
        private TargetStage m_targetStage = new TargetStage ();
        public GL_Battle (INetworkService networkService) {
            m_networkService = networkService;
            Messenger.AddListener<int, E_EquipmentItem> ("NotifyUseEquipmentItem", NotifyUseEquipmentItem);
            Messenger.AddListener<int, E_ConsumableItem> ("NotifyUseComsumableItem", NotifyUseComsumableItem);
            Messenger.AddListener<int, E_Skill, SkillParam> ("NotifySkillSettle", NotifySkillSettle);
        }
        void NotifyUseEquipmentItem (int netId, E_EquipmentItem equipment) {
        }
        void NotifyUseComsumableItem (int netId, E_ConsumableItem consumable) {
        }
        void NotifySkillSettle (int netId, E_Skill skill, SkillParam parm) {
        }
    }
}