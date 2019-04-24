using System.Collections.Generic;

namespace MirRemake {
    class SM_Monster {
        public static SM_Monster s_instance = new SM_Monster();
        private Dictionary<int, E_Monster> m_netIdAndMonsterDict = new Dictionary<int, E_Monster>();
        public void Tick(float dT) {
            
        }
    }
}