// namespace MirRemake {
//     struct MT_GainItem : IMissionTarget {
//         public MissionTargetType m_TargetType { get { return MissionTargetType.GAIN_ITEM; } }
//         public bool m_IsFinished {
//             get {
//                 return SM_MyRole.s_instance.GetItemNumInBagById (m_targetItemId) >= m_targetItemNum;
//             }
//         }
//         public bool m_IsDirty {
//             get {
//                 var curNum = SM_MyRole.s_instance.GetItemNumInBagById (m_targetItemId);
//                 if (m_itemNumPre == curNum)
//                     return false;
//                 m_itemNumPre = curNum;
//                 return true;
//             }
//         }
//         public short m_MissionProgressTargetValue { get { return m_targetItemNum; } }
//         public short m_MissionProgressValue { get { return SM_MyRole.s_instance.GetItemNumInBagById (m_targetItemId); } set { } }
//         private short m_targetItemId;
//         private short m_targetItemNum;
//         private short m_itemNumPre;
//         public void Init (short targetItemId, short targetItemNum) {
//             m_targetItemId = targetItemId;
//             m_targetItemNum = targetItemNum;
//             m_itemNumPre = SM_MyRole.s_instance.GetItemNumInBagById (targetItemId);
//         }
//         public bool IsProgressChangedAfterGainOrLostItem (short itemId) {
//             return itemId == m_targetItemId;
//         }
//     }
// }