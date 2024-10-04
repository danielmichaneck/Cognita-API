using Cognita_Infrastructure.Models.Dtos;
using Cognita_Shared.Dtos.Document;
using Cognita_Shared.Enums;
using System.Security.Claims;
using System.Security.Principal;

namespace Cognita_API.Controllers
{
    internal static class ControllerUtil
    {
        internal static async Task<DocumentDto?> AddDocument(IFormFile file)
        {
            if (file.Length == 0 ||
                file.Length > 20971520 ||
                file.ContentType != "application/pdf")
            {
                return null;
            }

            var path = Path.Combine(Directory.GetCurrentDirectory().ToString(),
                $"uploaded_file_{Guid.NewGuid()}.pdf");

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return new DocumentDto() {
                DocumentName = file.Name,
                UploadingTime = DateTime.Now,
                FilePath = path
            };
        }

        internal static async Task<UserWithRoleDto?> GetUserWithRole(ClaimsIdentity identity)
        {
            UserRole? role = null;
            int id = 0;
            bool idSet = false;

            List<Claim> claims = identity.Claims.ToList();

            foreach (Claim claim in claims)
            {
                if (claim.Type == ClaimTypes.Role)
                {
                    if (claim.Value == "Admin")
                    {
                        role = UserRole.Teacher;
                    }
                    else if (claim.Value == "User")
                    {
                        role = UserRole.Student;
                    }
                }
                else if (claim.Type == ClaimTypes.NameIdentifier)
                {
                    id = int.Parse(claim.Value);
                    idSet = true;
                }
            }

            if (!idSet || role is null) return null;

            return new UserWithRoleDto()
            {
                Id = id,
                Role = (UserRole)role
            };
        }
    }
}
