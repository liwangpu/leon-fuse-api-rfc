using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
namespace ApiModel.Entities
{
    public class Media : EntityBase, IListable, IDTOTransfer<MediaDTO>
    {
        public string Icon { get; set; }
        public string FileAssetId { get; set; }
        public string Rotation { get; set; }
        public string Location { get; set; }
        public string SolutionId { get; set; }
        public string Type { get; set; }
        public List<MediaShareResource> MediaShareResources { get; set; }

        [NotMapped]
        public string Server { get; set; }
        [NotMapped]
        public string FileAssetUrl { get; set; }

        public MediaDTO ToDTO()
        {
            var dto = new MediaDTO();
            dto.Id = Id;
            dto.Name = Name;
            dto.Description = Description;
            dto.Creator = Creator;
            dto.Modifier = Modifier;
            dto.CreatedTime = CreatedTime;
            dto.ModifiedTime = ModifiedTime;
            dto.CreatorName = CreatorName;
            dto.ModifierName = ModifierName;
            dto.Rotation = Rotation;
            dto.Location = Location;
            dto.FileAssetId = FileAssetId;
            dto.SolutionId = SolutionId;
            dto.OrganizationId = OrganizationId;
            dto.Type = Type;
            dto.CategoryName = CategoryName;
            dto.FileAssetId = FileAssetId;
            dto.FileAssetUrl = FileAssetUrl;
            dto.Icon = IconFileAssetUrl;
            dto.IconAssetId = Icon;
            if (MediaShareResources != null && MediaShareResources.Count > 0)
            {
                var list = new List<MediaShareResourceDTO>();
                for (int idx = MediaShareResources.Count - 1; idx >= 0; idx--)
                {
                    var curItem = MediaShareResources[idx];
                    curItem.Type = Type;
                    curItem.Rotation = Rotation;
                    curItem.Location = Location;
                    curItem.FileAssetId = FileAssetId;
                    //curItem.IconFileAsset = IconFileAsset;
                    //curItem.FileAsset = FileAsset;
                    curItem.Icon = IconFileAssetUrl;
                    curItem.IconAssetId = Icon;
                    curItem.FileAssetId = FileAssetId;
                    curItem.FileAssetUrl = FileAssetUrl;
                    curItem.Url = Server;
                    list.Add(curItem.ToDTO());
                }
                dto.MediaShares = list.OrderByDescending(x => x.CreatedTime).ToList();
            }
            return dto;
        }
    }

    public class MediaShareResource : EntityBase, IListable, IDTOTransfer<MediaShareResourceDTO>
    {
        private string _Url;
        public long StartShareTimeStamp { get; set; }
        public long StopShareTimeStamp { get; set; }
        public string Password { get; set; }

        public Media Media { get; set; }
        public string MediaId { get; set; }

        [NotMapped]
        public string Url
        {
            set
            {
                _Url = value;
            }
            get
            {
                return $"{_Url}?t={Type}&id={Id}";
            }
        }
        [NotMapped]
        public string Type { get; set; }
        [NotMapped]
        public string Rotation { get; set; }
        [NotMapped]
        public string Location { get; set; }
        [NotMapped]
        public string Icon { get; set; }
        [NotMapped]
        public string IconAssetId { get; set; }
        [NotMapped]
        public string FileAssetId { get; set; }
        [NotMapped]
        public string FileAssetUrl { get; set; }

        public MediaShareResourceDTO ToDTO()
        {
            var dto = new MediaShareResourceDTO();
            dto.Id = Id;
            dto.Name = Name;
            dto.Description = Description;
            dto.Creator = Creator;
            dto.Modifier = Modifier;
            dto.CreatedTime = CreatedTime;
            dto.ModifiedTime = ModifiedTime;
            dto.Type = Type;
            dto.Rotation = Rotation;
            dto.Location = Location;
            dto.StartShareTimeStamp = StartShareTimeStamp;
            dto.StopShareTimeStamp = StopShareTimeStamp;
            dto.FileAssetId = FileAssetId;
            dto.Password = Password;
            dto.Icon = Icon;
            dto.Url = Url;
            dto.Icon = IconFileAssetUrl;
            dto.IconAssetId = Icon;
            dto.FileAssetUrl = FileAssetUrl;
            return dto;
        }
    }

    public class MediaShareResourceDTO : EntityBase
    {
        public string Icon { get; set; }
        public string FileAssetId { get; set; }
        public string Rotation { get; set; }
        public string Location { get; set; }
        public long StartShareTimeStamp { get; set; }
        public long StopShareTimeStamp { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string IconAssetId { get; set; }
        public string FileAssetUrl { get; set; }
    }

    public class MediaDTO : EntityBase,IListable
    {
        public string FileAssetId { get; set; }
        public string IconAssetId { get; set; }
        public string FileAssetUrl { get; set; }
        public string Rotation { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string SolutionId { get; set; }
        public List<MediaShareResourceDTO> MediaShares { get; set; }
        public string Icon { get; set; }
        

    }
}
