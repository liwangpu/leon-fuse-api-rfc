namespace ApiModel.Entities
{
    /// <summary>
    /// 摆放在场景中的对象的数据
    /// </summary>
    public class AreaItem
    {
        /// <summary>
        /// 在场景中次对象的名称（场景内唯一）
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 使用的产品的ID
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 位置向量，格式为 x,y,z，单位厘米，使用Vector3类来解析。 比如 12,-13.56,208.2992. null或者string.empty则表示x=y=z=0。
        /// </summary>
        public string LocationVec { get; set; }
        /// <summary>
        /// 朝向（旋转量），格式为 x,y,z，使用Vector3类来解析，单位为角度(-360 to 360)。null或者string.empty则表示x=y=z=0。
        /// </summary>
        public string RotationVec { get; set; }
        /// <summary>
        /// 缩放，格式为 x,y,z，使用Vector3类来解析。 1=不缩放，1.5=放大到1.5倍，0.5=缩小一倍，-1=翻转，0=缩放到看不见。 null或者string.empty表示 x=y=z=1，不缩放
        /// </summary>
        public string ScaleVec { get; set; }
        /// <summary>
        /// 材质改动，空字符串表示没有材质改动。componentId1=matId1;componentId2=matId2表示每一个组件的材质配置
        /// </summary>
        public string Materials { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 父对象名，用来记录对象层级关系，比如子对象，物品组
        /// </summary>
        public string ParentName { get; set; }
    }
}
