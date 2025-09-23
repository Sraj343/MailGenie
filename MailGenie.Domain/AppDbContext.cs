using MailGenie.Domain.Model.Mail;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailGenie.Domain
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<EmailTemplate> Mailtemplates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmailTemplate>().HasKey(e => e.TemplateId);
            modelBuilder.Entity<EmailTemplate>().ToTable("EmailTemplate");
        }

    }
}
