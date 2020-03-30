using System.ComponentModel.DataAnnotations;

namespace ApiModel.Entities
{
    public class SettingsItem
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
