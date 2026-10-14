using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;

namespace SurveySystem.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await context.Database.MigrateAsync();

            string[] roles = { "Admin", "Analyst", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "admin@survey.local";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Системний Адміністратор",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            var analystEmail = "analyst@survey.local";
            var analystUser = await userManager.FindByEmailAsync(analystEmail);
            if (analystUser == null)
            {
                analystUser = new ApplicationUser
                {
                    UserName = analystEmail,
                    Email = analystEmail,
                    FullName = "Олена Аналітик",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(analystUser, "Analyst123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(analystUser, "Analyst");
                }
            }

            if (!await context.Surveys.AnyAsync())
            {
                var survey1 = new Survey
                {
                    Title = "Оцінка якості дистанційного навчання",
                    Description = "Опитування серед студентів щодо зручності онлайн-платформ та якості матеріалів.",
                    Category = "Освіта",
                    IsPublic = true,
                    IsApproved = true,
                    IsActive = true,
                    AuthorId = analystUser?.Id ?? adminUser.Id,
                    Questions = new List<Question>
                    {
                        new Question
                        {
                            Text = "Якою платформою ви користуєтесь найчастіше?",
                            Type = QuestionType.SingleChoice,
                            Order = 1,
                            Options = new List<Option>
                            {
                                new Option { Text = "Microsoft Teams" },
                                new Option { Text = "Zoom" },
                                new Option { Text = "Google Meet" }
                            }
                        },
                        new Question
                        {
                            Text = "Наскільки ви задоволені якістю лекцій?",
                            Type = QuestionType.SingleChoice,
                            Order = 2,
                            Options = new List<Option>
                            {
                                new Option { Text = "Повністю задоволений" },
                                new Option { Text = "Частково задоволений" },
                                new Option { Text = "Не задоволений" }
                            }
                        }
                    }
                };

                var survey2 = new Survey
                {
                    Title = "Внутрішнє опитування IT-компанії (Приватне)",
                    Description = "Збір відгуків працівників щодо нового робочого графіка.",
                    Category = "HR та Корпоративна культура",
                    IsPublic = false,
                    IsApproved = false,
                    IsActive = true,
                    AuthorId = analystUser?.Id ?? adminUser.Id,
                    Questions = new List<Question>
                    {
                        new Question
                        {
                            Text = "Який формат роботи вам підходить найбільше?",
                            Type = QuestionType.SingleChoice,
                            Order = 1,
                            Options = new List<Option>
                            {
                                new Option { Text = "Повністю віддалений (Remote)" },
                                new Option { Text = "Гібридний (Hybrid)" },
                                new Option { Text = "Робота в офісі (Office)" }
                            }
                        }
                    }
                };

                context.Surveys.AddRange(survey1, survey2);
                await context.SaveChangesAsync();
            }
        }
    }
}