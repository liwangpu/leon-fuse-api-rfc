using ApiModel.Entities;

namespace ApiModel
{
    public class ClientAssetEntity : EntityBase,IListable
    {
        public string PackageName { get; set; }
        public string UnCookedAssetId { get; set; }
        public string Icon { get; set; }
    }
}
