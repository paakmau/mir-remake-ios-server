using System.Collections.Generic;
using MirRemakeBackend.DynamicData;
using MirRemakeBackend.Util;

namespace MirRemakeBackend.Entity {
    class EM_Wallet {
        public static EM_Wallet s_instance;
        const int c_vipCardPoolSize = 400;
        ObjectPool<E_VipCard> m_vipCardPool = new ObjectPool<E_VipCard> (c_vipCardPoolSize);
        private IDDS_CharacterWallet m_walletDds;
        private IDDS_CharacterVipCard m_vipCardDds;
        private Dictionary < int, (long, long) > m_walletDict = new Dictionary < int, (long, long) > ();
        private Dictionary<int, E_VipCard> m_vipCardDict = new Dictionary<int, E_VipCard> ();
        public EM_Wallet (IDDS_CharacterWallet walletDds, IDDS_CharacterVipCard vipCardDds) { m_walletDds = walletDds; m_vipCardDds = vipCardDds; }
        public (long, long) InitCharacter (int netId, int charId) {
            (long, long) res;
            E_VipCard vipCard;
            if (m_walletDict.TryGetValue (netId, out res) && m_vipCardDict.TryGetValue (netId, out vipCard))
                return res;
            DDO_CharacterWallet wallet;
            if (!m_walletDds.GetCharacterWalletByCharacterId (charId, out wallet)) return (-1, -1);
            DDO_CharacterVipCard vipCardDdo;
            if (!m_vipCardDds.GetCharacterVipCardByCharacterId (charId, out vipCardDdo)) return (-1, -1);
            res = (wallet.m_virtualCy, wallet.m_chargeCy);
            m_walletDict[netId] = res;
            vipCard = m_vipCardPool.GetInstance ();
            vipCard.Reset (vipCardDdo);
            m_vipCardDict[netId] = vipCard;
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

        public int UpdateTotalChargedMoney (int netId, int newChargedMoney) {
            E_VipCard vipCard;
            if (!m_vipCardDict.TryGetValue (netId, out vipCard))
                return -1;
            vipCard.m_chargeMoney += newChargedMoney;
            m_vipCardDds.UpdateCharacterVipCard (vipCard.GetDdo ());
            return vipCard.m_chargeMoney;
        }

        public (long, long) CharacterUpdateVirtualCy (int netId, int charId, long dVirtualCy) {
            (long, long) res;
            if (!m_walletDict.TryGetValue (netId, out res))
                return (-1, -1);
            res.Item1 += dVirtualCy;
            m_walletDict[netId] = res;
            m_walletDds.UpdateCharacterWallet (new DDO_CharacterWallet (charId, res.Item1, res.Item2));
            return res;
        }

        public (long, long) CharacterUpdateChargeCy (int netId, int charId, long dChargeCy) {
            (long, long) res;
            if (!m_walletDict.TryGetValue (netId, out res))
                return (-1, -1);
            res.Item2 += dChargeCy;
            m_walletDict[netId] = res;
            m_walletDds.UpdateCharacterWallet (new DDO_CharacterWallet (charId, res.Item1, res.Item2));
            return res;
        }
        public void CharacterUpdateVirtualCy (int charId, long dVirtualCy) {
            DDO_CharacterWallet wallet;
            bool suc = m_walletDds.GetCharacterWalletByCharacterId (charId, out wallet);
            if (!suc) return;
            wallet.m_virtualCy += dVirtualCy;
            m_walletDds.UpdateCharacterWallet (wallet);
        }
        public void CharacterUpdateChargeCy (int charId, long dChargeCy) {
            DDO_CharacterWallet wallet;
            bool suc = m_walletDds.GetCharacterWalletByCharacterId (charId, out wallet);
            if (!suc) return;
            wallet.m_chargeCy += dChargeCy;
            m_walletDds.UpdateCharacterWallet (wallet);
        }
    }
}