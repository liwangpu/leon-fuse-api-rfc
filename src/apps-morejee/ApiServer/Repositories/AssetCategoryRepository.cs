using ApiModel.Consts;
using ApiModel.Entities;
using ApiModel.Enums;
using ApiServer.Data;
using BambooCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Repositories
{
    public class AssetCategoryRepository : ListableRepository<AssetCategory, AssetCategoryDTO>
    {
        public ITreeRepository<AssetCategoryTree> _AssetCategoryTreeRepository { get; }
        public AssetCategoryRepository(ApiDbContext context, ITreeRepository<PermissionTree> permissionTreeRep, ITreeRepository<AssetCategoryTree> assetCategoryTreeRep)
            : base(context, permissionTreeRep)
        {
            _AssetCategoryTreeRepository = assetCategoryTreeRep;
        }

        /// <summary>
        /// 资源访问类型
        /// </summary>
        public override ResourceTypeEnum ResourceTypeSetting
        {
            get
            {
                return ResourceTypeEnum.Organizational;
            }
        }

        public override async Task<bool> CanReadAsync(string accid, string id)
        {
            return await Task.FromResult(true);
        }


        public async Task<AssetCategoryDTO> GetCategoryIsolateAsync(string rootCategoryId, string organId)
        {

            var rootCategoryTreeNode = await _DbContext.AssetCategoryTrees.FirstAsync(x => x.ObjId == rootCategoryId);
            var templist = new List<AssetCategory>();

            #region 查看根节点下节点有没有需要独立出来的
            {
                var tmpQ = from cat in _DbContext.AssetCategories
                           join tree in _DbContext.AssetCategoryTrees on cat.Id equals tree.ObjId
                           where tree.OrganizationId == organId && tree.NodeType == rootCategoryTreeNode.NodeType && tree.LValue >= rootCategoryTreeNode.LValue && tree.RValue <= rootCategoryTreeNode.RValue && cat.ActiveFlag == AppConst.I_DataState_Active
                           select cat;

                templist = await tmpQ.ToListAsync();

                //只有根节点才去讨论说里面的子节点要不要独立出来,所以只有根节点需要去除独立节点里的分类
                if (rootCategoryTreeNode.LValue == 1)
                {
                    var isolateCats = await tmpQ.Where(x => x.Isolate).ToListAsync();
                    foreach (var isolateCat in isolateCats)
                    {
                        var isolateNode = await _DbContext.AssetCategoryTrees.FirstAsync(x => x.ObjId == isolateCat.Id);
                        var excludeIds = await _DbContext.AssetCategoryTrees.Where(x => x.OrganizationId == organId && x.NodeType == isolateNode.NodeType && x.LValue >= isolateNode.LValue && x.RValue <= isolateNode.RValue).Select(x => x.ObjId).ToListAsync();
                        foreach (var excludeId in excludeIds)
                        {
                            for (var idx = templist.Count - 1; idx >= 0; idx--)
                            {
                                if (templist[idx].Id == excludeId)
                                    templist.RemoveAt(idx);
                            }
                        }
                    }
                }
            }
            #endregion

            var list = new LinkedList<AssetCategory>();
            foreach (var item in templist)
            {
                list.AddLast(item);
            }
            var root = await _DbContext.AssetCategories.FirstAsync(x => x.Id == rootCategoryId);
            var dto = root.ToDTO();
            FindChildren(list, dto);

            return dto;
        }

        /// <summary>
        ///  获取所有分类，已经整理为树结构
        /// </summary>
        /// <param name="type"></param>
        /// <param name="organId"></param>
        /// <returns></returns>
        public async Task<AssetCategoryDTO> GetCategoryAsync(string type, string organId)
        {
            //之所以采用这个方式,是因为tree表里面没有active标记,不清楚分类节点是否已经属于删除状态
            var rootCatTreeNodeQ = from tree in _DbContext.AssetCategoryTrees
                                   join cat in _DbContext.AssetCategories on tree.ObjId equals cat.Id
                                   where cat.ActiveFlag == AppConst.I_DataState_Active && cat.OrganizationId == organId && tree.NodeType == type && tree.LValue == 1
                                   select tree;
            var rootCatTreeNode = await rootCatTreeNodeQ.FirstAsync();

            var tmpQ = from cat in _DbContext.AssetCategories
                       join tree in _DbContext.AssetCategoryTrees on cat.Id equals tree.ObjId
                       where cat.ActiveFlag == AppConst.I_DataState_Active && tree.OrganizationId == organId && tree.NodeType == rootCatTreeNode.NodeType && tree.LValue >= rootCatTreeNode.LValue && tree.RValue <= rootCatTreeNode.RValue
                       select cat;

            var templist = await tmpQ.ToListAsync();


            LinkedList<AssetCategory> list = new LinkedList<AssetCategory>();
            foreach (var item in templist)
            {
                list.AddLast(item);
            }
            AssetCategoryDTO root = FindRoot(list);
            FindChildren(list, root);

            if (root == null)
            {
                AssetCategory rootNode = new AssetCategory();
                rootNode.Id = GuidGen.NewGUID();
                rootNode.ParentId = "";
                rootNode.OrganizationId = organId;
                rootNode.Type = type;
                rootNode.Name = type + "_root";
                rootNode.Icon = "";
                rootNode.Description = "auto generated node for " + type + ", do not need to display this node";
                rootNode.ResourceType = (int)ResourceTypeEnum.Organizational;
                _DbContext.AssetCategories.Add(rootNode);
                await _DbContext.SaveChangesAsync();

                #region 添加tree节点
                {
                    var oTree = new AssetCategoryTree();
                    oTree.NodeType = type;
                    oTree.Name = rootNode.Name;
                    oTree.ObjId = rootNode.Id;
                    oTree.OrganizationId = organId;
                    await _AssetCategoryTreeRepository.AddNewNode(oTree);
                }
                #endregion

                root = rootNode.ToDTO();
            }

            return root;
        }

        /// <summary>
        /// 获取扁平结构的分类信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="organId"></param>
        /// <returns></returns>
        public async Task<List<AssetCategoryDTO>> GetFlatCategory(string type, string organId)
        {
            //return await (_AssetCategoryTreeRepository as AssetCategoryTreeRepository).GetFlatCategory(type, organId);
            //之所以采用这个方式,是因为tree表里面没有active标记,不清楚分类节点是否已经属于删除状态
            var rootCatTreeNodeQ = from tree in _DbContext.AssetCategoryTrees
                                   join cat in _DbContext.AssetCategories on tree.ObjId equals cat.Id
                                   where cat.ActiveFlag == AppConst.I_DataState_Active && cat.OrganizationId == organId && tree.NodeType == type && tree.LValue == 1
                                   select tree;
            var rootCatTreeNode = await rootCatTreeNodeQ.FirstAsync();

            var tmpQ = from cat in _DbContext.AssetCategories
                       join tree in _DbContext.AssetCategoryTrees on cat.Id equals tree.ObjId
                       where cat.ActiveFlag == AppConst.I_DataState_Active && tree.OrganizationId == organId && tree.NodeType == rootCatTreeNode.NodeType && tree.LValue >= rootCatTreeNode.LValue && tree.RValue <= rootCatTreeNode.RValue
                       select cat;

            var templist = await tmpQ.ToListAsync();
            return templist.Select(x => x.ToDTO()).ToList();
        }

        /// <summary>
        /// 移动一个分类到另外一个分类下
        /// </summary>
        /// <param name="type"></param>
        /// <param name="catid"></param>
        /// <param name="targetCatId"></param>
        /// <returns>error message</returns>
        public async Task<string> MoveAsync(string type, string catid, string targetCatId)
        {
            AssetCategory cat = await _DbContext.AssetCategories.FirstOrDefaultAsync(d => d.Type == type && d.Id == catid);
            if (cat == null)
            {
                return "category " + catid + " not found"; // not found.
            }

            AssetCategory targetCat = await _DbContext.AssetCategories.FirstOrDefaultAsync(d => d.Type == type && d.Id == targetCatId);
            if (targetCat == null)
            {
                return "target category " + targetCatId + " not found"; // not found.
            }

            int childrenCount = await _DbContext.AssetCategories.Where(d => d.ParentId == targetCatId).CountAsync();

            cat.ParentId = targetCatId;
            cat.DisplayIndex = childrenCount; // index start from 0;
            await _DbContext.SaveChangesAsync();
            return "";
        }

        /// <summary>
        /// 转移一个分类下的所有资产到另外一个分类下
        /// </summary>
        /// <param name="type"></param>
        /// <param name="catid"></param>
        /// <param name="targetCatId"></param>
        /// <returns></returns>
        public async Task<string> TransferAsync(string type, string catid, string targetCatId)
        {
            AssetCategory cat = await _DbContext.AssetCategories.FirstOrDefaultAsync(d => d.Type == type && d.Id == catid);
            if (cat == null)
            {
                return "category " + catid + " not found"; // not found.
            }

            AssetCategory targetCat = await _DbContext.AssetCategories.FirstOrDefaultAsync(d => d.Type == type && d.Id == targetCatId);
            if (targetCat == null)
            {
                return "target category " + targetCatId + " not found"; // not found.
            }

            AssetCategory targetChildCat = await _DbContext.AssetCategories.FirstOrDefaultAsync(d => d.Type == type && d.ParentId == targetCatId);
            if (targetChildCat != null)
            {
                return "target category " + targetCatId + " have child category. ";
            }

            string tableName = "";
            if (type == "product")
                tableName = "Products";
            else if (type == "material")
                tableName = "Materials";

            string sql = string.Format("update \"{0}\" set \"CategoryId\"='{1}' where \"CategoryId\"='{2}'", tableName, targetCatId, catid);
            int rows = await _DbContext.Database.ExecuteSqlCommandAsync(sql);
            return "";
        }

        /// <summary>
        /// 设置一个分类在父级分类中的显示顺序，index的范围为0 - childrenCount - 1.非法的index会被自动矫正
        /// 返回父级分类对象。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="catid"></param>
        /// <param name="displayIndex"></param>
        /// <returns>返回父级分类对象</returns>
        public async Task<AssetCategoryDTO> SetDisplayIndex(string type, string catid, int displayIndex)
        {
            AssetCategory target = await _DbContext.AssetCategories.FirstOrDefaultAsync(d => d.Type == type && d.Id == catid);
            if (target == null)
            {
                return null; // not found.
            }
            AssetCategory parent = await _DbContext.AssetCategories.FirstOrDefaultAsync(d => d.Type == type && d.Id == target.ParentId);
            if (parent == null)
            {
                return null; // not found.
            }

            AssetCategoryDTO result = parent.ToDTO();

            List<AssetCategory> children = await _DbContext.AssetCategories.Where(d => d.ParentId == target.ParentId).OrderBy(d => d.DisplayIndex).ToListAsync();
            int total = children.Count;
            if (displayIndex < 0)
                displayIndex = 0;
            if (displayIndex >= total)
                displayIndex = total - 1;

            children.Remove(target);
            children.Insert(displayIndex, target);

            int index = 0;
            foreach (var item in children)
            {
                item.DisplayIndex = index++;
                result.Children.Add(item.ToDTO());
            }

            await _DbContext.SaveChangesAsync();

            return result;
        }

        AssetCategoryDTO FindRoot(LinkedList<AssetCategory> list)
        {
            foreach (var item in list)
            {
                if (string.IsNullOrEmpty(item.ParentId))
                {
                    return item.ToDTO();
                }
            }
            return null;
        }

        void FindChildren(LinkedList<AssetCategory> list, AssetCategoryDTO parent)
        {
            if (parent == null)
                return;
            var node = list.First;
            while (node != null)
            {
                var next = node.Next;

                var item = node.Value;
                if (item.ParentId == parent.Id)
                {
                    AssetCategoryDTO child = item.ToDTO();
                    parent.Children.Add(child);
                    list.Remove(node);
                }

                node = next;
            }

            foreach (var item in parent.Children)
            {
                FindChildren(list, item);
            }
        }


        #region SatisfyCreateAsync 判断数据是否满足存储规范
        /// <summary>
        /// 判断数据是否满足存储规范
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="data"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public override async Task SatisfyCreateAsync(string accid, AssetCategory data, ModelStateDictionary modelState)
        {
            if (!string.IsNullOrWhiteSpace(data.ParentId))
            {
                var pnode = await _DbContext.AssetCategories.FirstOrDefaultAsync(x => x.Id == data.ParentId);
                if (pnode == null)
                    modelState.AddModelError("ParentId", string.Format("没有找到ParentId为{0}的记录", data.ParentId));
            }
        }
        #endregion

        #region SatisfyUpdateAsync 判断数据是否满足更新规范
        /// <summary>
        /// 判断数据是否满足更新规范
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="data"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public override async Task SatisfyUpdateAsync(string accid, AssetCategory data, ModelStateDictionary modelState)
        {
            await Task.FromResult(string.Empty);
        }
        #endregion

        #region CreateAsync 创建分类信息
        /// <summary>
        /// 创建分类信息
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public override async Task CreateAsync(string accid, AssetCategory data)
        {
            await base.CreateAsync(accid, data);

            #region 添加tree节点
            {
                var pNode = await _DbContext.AssetCategoryTrees.Where(x => x.ObjId == data.ParentId && x.OrganizationId == data.OrganizationId).FirstOrDefaultAsync();
                if (pNode != null)
                {
                    var oTree = new AssetCategoryTree();
                    oTree.NodeType = data.Type;
                    oTree.Name = data.Name;
                    oTree.ObjId = data.Id;
                    oTree.OrganizationId = data.OrganizationId;
                    oTree.ParentId = pNode.Id;
                    await _AssetCategoryTreeRepository.AddChildNode(oTree);
                }
                else
                {
                    var oTree = new AssetCategoryTree();
                    oTree.NodeType = data.Type;
                    oTree.Name = data.Name;
                    oTree.ObjId = data.Id;
                    oTree.LValue = 1;
                    oTree.RValue = 2;
                    oTree.OrganizationId = data.OrganizationId;
                    oTree.RootOrganizationId = data.OrganizationId;
                    _DbContext.AssetCategoryTrees.Add(oTree);
                    await _DbContext.SaveChangesAsync();
                }
            }
            #endregion




        }
        #endregion
    }
}
