using System;
using Message.Entity;
using Microsoft.EntityFrameworkCore;

namespace Message.Data
{
	public class MessageDbContext : DbContext
	{
        public MessageDbContext(DbContextOptions<MessageDbContext> options)
            : base(options){}

        public DbSet<Sobsh> Sobshes { get; set; }
    }
}

