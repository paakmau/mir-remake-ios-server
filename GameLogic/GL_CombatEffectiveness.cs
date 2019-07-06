using MirRemakeBackend.Entity;
using MirRemakeBackend.Network;
using System;
namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 管理战斗力计算 与 获取
    /// </summary>
    class GL_CombatEffectiveness : GameLogicBase {
        public static GL_CombatEffectiveness s_instance;
        public GL_CombatEffectiveness (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) { }
        public override void NetworkTick () { }
        public void CommandGetEffectivenessChange (int netId) {
            // TODO: 客户端要求获取战斗力
        }
        public void NotifyCombatEffectivenessChange (E_Character unit) {
            double res=0;
            switch(unit.m_Occupation){
                case OccupationType.WARRIOR:
                    res=Math.Pow(unit.m_Attack,1.5);
                    break;
                case OccupationType.ROGUE:
                    res=3*Math.Pow(unit.m_Attack,1.5);
                    break;
                case OccupationType.MAGE:
                    res=0.8*Math.Pow(unit.m_Intelligence,1.5);
                    break;
                case OccupationType.TAOIST:
                    res=1.6*Math.Pow(unit.m_Intelligence,1.5);
                    break;
            }
            res=res+(unit.m_MaxHp/40f);
            res=res+(unit.m_MaxMp/50f);
            res=res+0.3*(unit.m_Defence+unit.m_Resistance)*(unit.m_Defence+unit.m_Resistance);
            res=res+unit.m_Level*unit.m_Level*unit.m_Tenacity/100;
            res=res*unit.m_HitRate*unit.m_DodgeRate/10000;
            res=res+unit.m_DeltaHpPerSecond*unit.m_DeltaMpPerSecond;
            unit.m_combatEffectiveness=(int)res;
        }
    }
}