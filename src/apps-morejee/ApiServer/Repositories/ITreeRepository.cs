using ApiModel;
using ApiServer.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Repositories
{
    public interface ITreeRepository<T>
         where T : class, ITree, new()
    {
        ApiDbContext _DbContext { get; }
        Task AddNewNode(T data);
        Task AddChildNode(T data);
        Task AddSiblingNode(T data, string sibling);
        IQueryable<T> GetDescendantNode(T node, List<string> nodeTypes, bool includeCurrentNode = false);
        Task<IQueryable<T>> GetAncestorNode(T node, List<string> nodeTypes, bool includeCurrentNode = false);
        Task MoveNode(T data, string newParentNodeId);
    }
}
