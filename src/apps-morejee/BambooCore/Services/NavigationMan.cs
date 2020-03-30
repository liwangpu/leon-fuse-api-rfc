using Microsoft.EntityFrameworkCore;

namespace BambooCore.Services
{


    public class NavigationMan
    {
        DbContext context;

        public NavigationMan(DbContext context)
        {
            this.context = context;
        }
        
    }
}
