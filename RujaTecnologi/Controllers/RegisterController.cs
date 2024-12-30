using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SeñorPolfavol.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Invalid user data.");
            }

            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Crear los roles si no existen
                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                }
                if (!await _roleManager.RoleExistsAsync("Writer"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Writer"));
                }

                // Verificar si existe un usuario con rol Admin
                var adminExists = await _userManager.GetUsersInRoleAsync("Admin");

                if (!adminExists.Any())
                {
                    // No hay Admin, asignar rol Admin al primer usuario
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    // Asignar rol Writer a los demás usuarios
                    await _userManager.AddToRoleAsync(user, "Writer");
                }

                return Ok(new { message = "User registered successfully.", userId = user.Id });
            }

            return BadRequest(result.Errors);
        }

        [HttpGet("current-user")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            // Obtiene el ID del usuario autenticado
            var userId = User?.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated." });
            }

            // Busca el usuario en la base de datos
            var user = await _userManager.FindByNameAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Obtiene los roles del usuario
            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                UserId = user.Id,
                Email = user.Email,
                Roles = roles
            });
        }

        [HttpGet("all-users-with-roles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsersWithRoles(int page = 1, int pageSize = 10)
        {
            var query = _userManager.Users.AsQueryable();

            // Obtener usuarios paginados
            var paginatedUsers = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var userRolesList = new List<object>();

            foreach (var user in paginatedUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRolesList.Add(new
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = roles
                });
            }

            // Construir resultado paginado
            var result = new
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = query.Count(),
                TotalPages = (int)Math.Ceiling(query.Count() / (double)pageSize),
                Items = userRolesList
            };

            return Ok(result);
        }


        [HttpPut("update-user-roles/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRoles(string userId, [FromBody] UpdateUserModel model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Actualizar email si se proporciona
            if (!string.IsNullOrEmpty(model.Email))
            {
                user.Email = model.Email;
                user.UserName = model.Email;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    return BadRequest(updateResult.Errors);
                }
            }

            // Manejar roles
            if (model.Roles != null && model.Roles.Any())
            {
                // Obtener roles actuales
                var currentRoles = await _userManager.GetRolesAsync(user);

                // Eliminar roles actuales
                var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeRolesResult.Succeeded)
                {
                    return BadRequest(removeRolesResult.Errors);
                }

                // Crear roles que no existan
                foreach (var roleName in model.Roles)
                {
                    if (!await _roleManager.RoleExistsAsync(roleName))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                // Agregar nuevos roles
                var addRolesResult = await _userManager.AddToRolesAsync(user, model.Roles);
                if (!addRolesResult.Succeeded)
                {
                    return BadRequest(addRolesResult.Errors);
                }
            }

            return Ok(new { message = "User updated successfully." });
        }


        [HttpDelete("delete-user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Eliminar roles asignados
            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeRolesResult.Succeeded)
            {
                return BadRequest(removeRolesResult.Errors);
            }

            // Eliminar usuario
            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                return BadRequest(deleteResult.Errors);
            }

            return Ok(new { message = "User deleted successfully." });
        }

        [HttpGet("all-roles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllRoles(int page = 1, int pageSize = 10)
        {
            var query = _roleManager.Roles.AsQueryable();

            // Obtener roles paginados
            var paginatedRoles = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Construir resultado paginado
            var result = new
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = query.Count(),
                TotalPages = (int)Math.Ceiling(query.Count() / (double)pageSize),
                Items = paginatedRoles.Select(role => new { role.Id, role.Name }).ToList()
            };

            return Ok(result);
        }
    }

    public class RegisterModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public List<string> Roles { get; set; } // Nuevo campo para roles
    }

    public class UpdateUserModel
    {
        public string Email { get; set; } // Nuevo email del usuario (opcional)
        public List<string> Roles { get; set; } // Lista de roles que el usuario debe tener
    }


}


