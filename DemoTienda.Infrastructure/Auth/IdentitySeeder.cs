using Microsoft.AspNetCore.Identity;

namespace DemoTienda.Infrastructure.Auth
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            var roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            var adminEmail = "admin@demotienda.local";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser is null)
            {
                adminUser = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Administrador DemoTienda"
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");

                if (result.Succeeded)
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                else
                    throw new Exception("No se pudo crear el usuario administrador inicial");
            }
        }

    }
}
