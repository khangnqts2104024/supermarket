using Microsoft.EntityFrameworkCore;

namespace SuperMarket_DataAccess.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions options):base(options)
        {

        }
    }
}