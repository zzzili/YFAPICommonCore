using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using EntityFrameworkCore.UserModel;

namespace EntityFrameworkCore
{
    public class MyDBContext : DbContext
    {
        public MyDBContext(DbContextOptions<MyDBContext> options)
         : base(options)
        {
        }

        public DbSet<User> User { get; set; }
    }
}
