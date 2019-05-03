namespace MirRemake {
    class E_Material : E_Item {
        public E_Material(short materialId) {
            m_id = materialId;
            m_type = ItemType.MATERIAL;
            // TODO:从数据库获取数据初始化
        }
    }
}