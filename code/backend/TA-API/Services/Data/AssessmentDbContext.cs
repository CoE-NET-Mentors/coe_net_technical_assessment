using Microsoft.EntityFrameworkCore;
using TA_API.Models.Data;

namespace TA_API.Services.Data
{
    public class AssessmentDbContext : DbContext
    {
        public AssessmentDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<CharacterEntity> Characters { get; set; }
    }
}
