using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiModel.Entities
{
    public class ClientAsset : EntityBase, IAsset
    {
        public string Icon { get; set; }        
    }
}
