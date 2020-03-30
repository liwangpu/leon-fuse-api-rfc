namespace ApiModel.Enums
{

    /*
     * 从品牌商的角度:
     *      
     *               ---> 个人(1)
     *              |
     *      品牌商=>|
     *              |            --->本组织公共(2)
     *              |           |
     *               --->公共 =>|                    --->所有下级组织(3)
     *                          |                   |
     *                           --->包含下级组织 =>|
     *                                              |
     *                                               --->指定下级组织(4)
     *      
     *      
     */










    public enum ResourceTypeEnum
    {
        /// <summary>
        /// 私人的
        /// </summary>
        Personal = 0,
        /// <summary>
        /// 部门的资源,对部门完全开放
        /// </summary>
        Departmental = 100,
        /// <summary>
        /// 组织的资源,仅该组织人员可以查看,下级组织不能查看
        /// </summary>
        Organizational = 200,
        /// <summary>
        /// 组织资源,该组织以及下级组织可以查看
        /// </summary>
        Organizational_SubShare = 300,
        /// <summary>
        /// 资源文件不限制,完全开放状态,全平台共享,任何人可以看,但是仅自己和管理员能编辑
        /// </summary>
        NoLimit = -1,
    }
}
