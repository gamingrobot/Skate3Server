﻿using Microsoft.EntityFrameworkCore;
using Skateboard3Server.Data.Models;

namespace Skateboard3Server.Data
{
    public class Skateboard3Context : DbContext
    {
        public Skateboard3Context(DbContextOptions<Skateboard3Context> options)
            : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
