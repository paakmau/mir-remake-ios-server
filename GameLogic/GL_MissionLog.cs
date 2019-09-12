using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理消息
    /// </summary>
    class GL_MissionLog : GameLogicBase {
        public static GL_MissionLog s_instance;
        public GL_MissionLog (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) {
            EM_MissionLog.s_instance.NextTick ();
        }
        public override void NetworkTick () { }

        public void NotifyLogGainItem (int netId, short itemId, short realStoreNum) {
            CreateLog (MissionTargetType.GAIN_ITEM, netId, itemId, realStoreNum);
        }

        public void NotifyLogLvUpSkill (int netId, short skillId, short skillLv) {
            CreateLog (MissionTargetType.LEVEL_UP_SKILL, netId, skillId, skillLv);
        }

        public void NotifyLogKillMonster (int netId, short monId) {
            CreateLog (MissionTargetType.KILL_MONSTER, netId, monId);
        }

        public void NotifyLogTalkToNpc (int netId, short misId, short misTarId) {
            CreateLog (MissionTargetType.TALK_TO_NPC, netId, misId, misTarId);
        }

        public void NotifyLogChargeAdequatelyOnline (int netId, int totalChargedmoney) {
            CreateLog (MissionTargetType.CHARGE_ADEQUATELY, netId, totalChargedmoney);
        }

        public void NotifyLogChargeAdequatelyOffline (int charId, int totalChargedmoney) {
            EM_MissionLog.s_instance.CreateLogOffline (MissionTargetType.CHARGE_ADEQUATELY, charId, totalChargedmoney);
        }

        private void CreateLog (MissionTargetType type, int netId, int parm1 = 0, int parm2 = 0, int parm3 = 0) {
            var logs = EM_MissionLog.s_instance.GetRawLogsCurTick ();
            logs.Add (EM_MissionLog.s_instance.CreateLog (type, netId, parm1, parm2, parm3));
        }
    }
}