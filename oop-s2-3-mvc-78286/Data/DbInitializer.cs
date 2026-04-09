using College.Domain.Models;
using Microsoft.AspNetCore.Identity;
using oop_s2_3_mvc_78286.Data;

namespace oop_s2_3_mvc_78286.Data
{
    public static class DbInitializer
    {
        public static async Task SeedData(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<Role> roleManager)
        {
            context.Database.EnsureCreated();

            // 1. Seed Roles
            string[] roleNames = { "Administrator", "Faculty", "Student" };
            foreach (var name in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(name))
                    await roleManager.CreateAsync(new Role(name));
            }

            // 2. Seed Identity Users AND Assign Roles
            // Note: CreateUser now takes the role as the 4th parameter
            var adminUser = await CreateUser(userManager, "admin@college.com", "Admin123!", "Administrator");
            var facultyUser = await CreateUser(userManager, "teacher@college.com", "Teacher123!", "Faculty");
            var student1 = await CreateUser(userManager, "student1@college.com", "Student123!", "Student");
            var student2 = await CreateUser(userManager, "student2@college.com", "Student123!", "Student");

            // 3. Seed Profiles (Linking to Identity IDs)
            if (!context.StaffProfiles.Any())
            {
                var staff = new StaffProfile { Name = "Dr. Smith", RoleName = "Faculty", IdentityUserID = facultyUser.Id };
                context.StaffProfiles.Add(staff);

                context.StudentProfiles.AddRange(
                    new StudentProfile { Name = "John Doe", RoleName = "Student", IdentityUserID = student1.Id },
                    new StudentProfile { Name = "Jane Miller", RoleName = "Student", IdentityUserID = student2.Id }
                );
                await context.SaveChangesAsync();
            }

            // --- Remaining Seeding Logic (Branches, Courses, etc.) stays the same ---
            
            // 4. Seed Branches
            if (!context.Branches.Any())
            {
                for (int i = 1; i <= 5; i++)
                {
                    context.Branches.Add(new Branch { Name = $"Campus {i}", Street = $"{i} College Road", City = "Dublin", Eircode = $"D0{i}X12{i}" });
                }
                await context.SaveChangesAsync();
            }

            var branch = context.Branches.First();

            // 5. Seed Courses
            // 5. Seed Courses (5 entries)
            if (!context.Courses.Any())
            {
                // Use the current date as a starting point
                DateOnly today = DateOnly.FromDateTime(DateTime.Now);

                for (int i = 1; i <= 5; i++)
                {
                    context.Courses.Add(new Course
                    {
                        Name = $"Course {i}",
                        BranchId = branch.Id,
                        // Convert DateTime to DateOnly
                        StartDate = today,
                        EndDate = today.AddYears(1)
                    });
                }
                await context.SaveChangesAsync();
            }

            var course = context.Courses.First();
            var staffMember = context.StaffProfiles.First();

            // 6. Seed Modules
            if (!context.Modules.Any())
            {
                for (int i = 1; i <= 5; i++)
                {
                    var module = new Module { Title = $"Module {i}" };
                    module.Courses.Add(course);
                    module.StaffProfiles.Add(staffMember);
                    context.Modules.Add(module);
                }
                await context.SaveChangesAsync();
            }

            // 7. Seed Enrolments, Attendance, Assignments
            var student = context.StudentProfiles.First();
            var moduleSample = context.Modules.First();

            if (!context.Attendances.Any())
            {
                for (int i = 1; i <= 5; i++)
                {
                    context.Attendances.Add(new Attendance { StudentProfileId = student.Id, ModuleId = moduleSample.Id, SessionDate = DateTime.Now.AddDays(-i), IsPresent = i % 2 == 0 });
                    context.Assignments.Add(new Assignment { Title = $"Assignment {i}", MaxScore = 100, ModuleId = moduleSample.Id });
                }
                await context.SaveChangesAsync();
            }

            // 8. Seed Assignment Results
            if (!context.AssignmentResults.Any())
            {
                var assignment = context.Assignments.First();
                context.AssignmentResults.Add(new AssignmentResults { AssignmentId = assignment.Id, StudentProfileId = student.Id, Result = 85.50m });
                await context.SaveChangesAsync();
            }
        }

        private static async Task<IdentityUser> CreateUser(UserManager<IdentityUser> userManager, string email, string password, string role)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                var result = await userManager.CreateAsync(user, password);
                
                if (result.Succeeded)
                {
                    // This is the CRITICAL part: Adding the user to the specific role
                    await userManager.AddToRoleAsync(user, role);
                }
            }
            return user;
        }
    }
}