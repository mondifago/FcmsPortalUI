using Microsoft.EntityFrameworkCore;

namespace FcmsPortalUI.Data
{
    public class FcmsPortalUIContext : DbContext
    {
        public FcmsPortalUIContext(DbContextOptions<FcmsPortalUIContext> options)
            : base(options)
        { }

    }
}