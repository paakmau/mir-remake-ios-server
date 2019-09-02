using System.Collections.Generic;
using MirRemakeBackend.DynamicData;

namespace MirRemakeBackend.Entity {
    class EM_Wallet {
        public static EM_Wallet s_instance;
        private IDDS_CharacterWallet m_dds;
        private Dictionary < int, (long, long) > m_walletDict = new Dictionary < int, (long, long) > ();
        public EM_Wallet (IDDS_CharacterWallet dds) { m_dds = dds; }
        public (long, long) InitCharacter (int netId, int charId) {
            (long, long) res;
            if (m_walletDict.TryGetValue (netId, out res))
                return res;
            DDO_CharacterWallet wallet;
            if (!m_dds.GetCharacterWalletByCharacterId (charId, out wallet)) return (-1, -1);
            res = (wallet.m_virtualCy, wallet.m_chargeCy);
            m_walletDict[netId] = res;
            return res;
        }
        public void RemoveCharacter (int netId) {
            m_walletDict.Remove (netId);
        }
        public (long, long) GetWallet (int netId) {
            (long, long) res;
            if (!m_walletDict.TryGetValue (netId, out res))
                return (-1, -1);
            return res;
        }
        public (long, long) CharacterUpdateVirtualCy (int netId, int charId, long dVirtualCy) {
            (long, long) res;
            if (!m_walletDict.TryGetValue (netId, out res))
                return (-1, -1);
            res.Item1 += dVirtualCy;
            m_walletDict[netId] = res;
            m_dds.UpdateCharacterWallet (new DDO_CharacterWallet (charId, res.Item1, res.Item2));
            return res;
        }
        public (long, long) CharacterUpdateChargeCy (int netId, int charId, long dChargeCy) {
            (long, long) res;
            if (!m_walletDict.TryGetValue (netId, out res))
                return (-1, -1);
            res.Item2 += dChargeCy;
            m_walletDict[netId] = res;
            m_dds.UpdateCharacterWallet (new DDO_CharacterWallet (charId, res.Item1, res.Item2));
            return res;
        }
        public void CharacterUpdateVirtualCy (int charId, long dVirtualCy) {
            DDO_CharacterWallet wallet;
            bool suc = m_dds.GetCharacterWalletByCharacterId (charId, out wallet);
            if (!suc) return;
            wallet.m_virtualCy += dVirtualCy;
            m_dds.UpdateCharacterWallet (wallet);
        }
        public void CharacterUpdateChargeCy (int charId, long dChargeCy) {
            DDO_CharacterWallet wallet;
            bool suc = m_dds.GetCharacterWalletByCharacterId (charId, out wallet);
            if (!suc) return;
            wallet.m_chargeCy += dChargeCy;
            m_dds.UpdateCharacterWallet (wallet);
        }
    }
}