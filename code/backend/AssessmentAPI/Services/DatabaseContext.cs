﻿using Microsoft.EntityFrameworkCore;

namespace AssessmentAPI.Services
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }
    }
}
