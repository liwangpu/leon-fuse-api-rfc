using System;

namespace ApiModel
{
    public interface IEntity : IData
    {
        string Description { get; set; }
        DateTime CreatedTime { get; set; }
        DateTime ModifiedTime { get; set; }
        string Creator { get; set; }
        string Modifier { get; set; }
        string CreatorName { get; set; }
        string ModifierName { get; set; }
        int ActiveFlag { get; set; }
        int ResourceType { get; set; }
        string OrganizationId { get; set; }
        string FolderName { get; set; }
        string CategoryId { get; set; }
        string CategoryName { get; set; }
    }
}
