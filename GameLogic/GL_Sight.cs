using System.Collections.Generic;
using MirRemakeBackend.Entity;
using MirRemakeBackend.EntityManager;
using MirRemakeBackend.Network;

namespace MirRemakeBackend.GameLogic {
    /// <summary>
    /// 每帧计算视野
    /// </summary>
    class GL_Sight : GameLogicBase {
        public static GL_Sight s_instance;
        private const float c_sightRadius = 100f;
        private List<int> t_intList = new List<int> ();
        public GL_Sight (INetworkService netService) : base (netService) { }
        public override void Tick (float dT) {
            var en = EM_ActorUnit.s_instance.GetCharacterEnumerator ();
            while (en.MoveNext ()) {
                int charNetId = en.Current.Key;
                var charObj = en.Current.Value;
                var charSight = EM_Sight.s_instance.GetRawCharacterSight (charNetId);
                charSight.Clear ();
                var unitEn = EM_Sight.s_instance.GetActorUnitVisibleEnumerator ();
                while (unitEn.MoveNext ()) {
                    // 若在视野范围外
                    if ((charObj.m_position - unitEn.Current.m_position).LengthSquared () > c_sightRadius * c_sightRadius)
                        continue;
                    charSight.Add (unitEn.Current);
                }
            }
        }
        public override void NetworkTick () {
            
        }
    }
}