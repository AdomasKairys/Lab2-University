using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CoursesAPI.Models;

namespace CoursesAPI.Data
{
    public class CoursesAPIContext : DbContext
    {
        public CoursesAPIContext (DbContextOptions<CoursesAPIContext> options)
            : base(options)
        {
        }

        public DbSet<CoursesAPI.Models.Course> Course { get; set; } = default!;
    }
}
