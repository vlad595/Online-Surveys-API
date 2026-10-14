using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace SurveySystem.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) {}

        public DbSet<Survey> Surveys { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Response> SurveyResponses { get; set; }
        public DbSet<Answer> Answers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Survey>().HasOne(s => s.Author).WithMany(u => u.CreatedSurveys).HasForeignKey(s => s.AuthorId).OnDelete(DeleteBehavior.SetNull);
 
            builder.Entity<Response>().HasOne(r => r.Respondent).WithMany(u => u.SurveyResponses).HasForeignKey(r => r.RespondentId).OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Answer>().HasOne(a => a.Question).WithMany(q => q.Answers).HasForeignKey(a => a.QuestionId).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Answer>().HasOne(a => a.SelectedOption).WithMany().HasForeignKey(a => a.SelectedOptionId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}