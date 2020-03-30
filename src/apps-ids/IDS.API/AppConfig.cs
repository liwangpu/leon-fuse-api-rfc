using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDS.API
{
    public class AppConfig
    {
        public SoftwareProviderSettings SoftwareProviderSettings { get; set; }
    }

    public class SoftwareProviderSettings
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public string Phone { get; set; }
    }
}
