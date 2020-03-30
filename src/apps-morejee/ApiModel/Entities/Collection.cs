using System.ComponentModel.DataAnnotations.Schema;

namespace ApiModel.Entities
{
    public class Collection : EntityBase, IListable
    {
        public string TargetId { get; set; }
        public string Type { get; set; }
        public string Folder { get; set; }
        [NotMapped]
        public string Icon { get; set; } 
    }
}
