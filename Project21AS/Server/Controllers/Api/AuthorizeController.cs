
using Project21AS.Dto.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project21AS.DataContext.Models;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;

namespace AsseProject21AStManagement.Server.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public AuthorizeController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginParameters parameters)
        {
            //var user = await _userManager.FindByNameAsync(parameters.UserName);
            var user = await _userManager.FindByEmailAsync(parameters.UserEmail);
            if (user == null) return BadRequest("User does not exist");
            if (!user.IsActive)
            {
                return BadRequest("Your account is deactivated. Login denied.");
            }

            var singInResult = await _signInManager.CheckPasswordSignInAsync(user, parameters.Password, false);
            if (!singInResult.Succeeded) return BadRequest("Invalid password");

            await _signInManager.SignInAsync(user, parameters.RememberMe);

            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterParameters parameters)
        {
            var user = new ApplicationUser
            {
                UserName = parameters.UserName,
                Email = parameters.Email,
                AllowedBatches = parameters.AllowedBatches,
                MaxStudentPerBatch = parameters.MaxStudentPerBatch,
                IsActive = parameters.IsActive

            };

            var userCount = await _userManager.Users.CountAsync();

            if (userCount == 0)
            {
                var adminRole = await _roleManager.FindByNameAsync("Admin");
                if (adminRole == null)
                {
                    adminRole = new IdentityRole<Guid>("Admin");
                    var createAdminRoleResult = await _roleManager.CreateAsync(adminRole);
                    if (!createAdminRoleResult.Succeeded)
                        return BadRequest(createAdminRoleResult.Errors.FirstOrDefault()?.Description);
                }

                var result = await _userManager.CreateAsync(user, parameters.Password);
                if (!result.Succeeded)
                    return BadRequest(result.Errors.FirstOrDefault()?.Description);

                var addAdminRoleResult = await _userManager.AddToRoleAsync(user, "Admin");
                if (!addAdminRoleResult.Succeeded)
                    return BadRequest(addAdminRoleResult.Errors.FirstOrDefault()?.Description);
            }
            else
            {
                var userRole = await _roleManager.FindByNameAsync("User");
                if (userRole == null)
                {
                    userRole = new IdentityRole<Guid>("User");
                    var createUserRoleResult = await _roleManager.CreateAsync(userRole);
                    if (!createUserRoleResult.Succeeded)
                        return BadRequest(createUserRoleResult.Errors.FirstOrDefault()?.Description);
                }

                var result = await _userManager.CreateAsync(user, parameters.Password);
                if (!result.Succeeded)
                    return BadRequest(result.Errors.FirstOrDefault()?.Description);

                var addUserRoleResult = await _userManager.AddToRoleAsync(user, "User");
                if (!addUserRoleResult.Succeeded)
                    return BadRequest(addUserRoleResult.Errors.FirstOrDefault()?.Description);
            }

            return await Login(new LoginParameters
            {
                //UserName = parameters.UserName,
                UserEmail = parameters.Email,
                Password = parameters.Password
            });
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UserInfo()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var userInfo = new UserInfo
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                UserName = user.UserName,
                ExposedClaims = User.Claims
                    .ToDictionary(c => c.Type, c => c.Value),
                Roles = userRoles.ToList(),
                AllowedBatches = user.AllowedBatches,
                MaxStudentPerBatch = user.MaxStudentPerBatch
            };

            return Ok(userInfo);
        }



        private UserInfo BuildUserInfo()
        {
            return new UserInfo
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                UserName = User.Identity.Name,
                ExposedClaims = User.Claims
                    //Optionally: filter the claims you want to expose to the client
                    //.Where(c => c.Type == "test-claim")
                    .ToDictionary(c => c.Type, c => c.Value)
            };
        }

        //[Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            var userViewModels = users.Select(u => new UserViewModel
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Role = _userManager.GetRolesAsync(u).Result.FirstOrDefault(),
                IsActive = u.IsActive,  
                AllowedBatches = u.AllowedBatches,
                MaxStudentPerBatch = u.MaxStudentPerBatch
            }).ToList();

            return Ok(userViewModels);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateUserRole(UserViewModel userViewModel)
        {
            var user = await _userManager.FindByIdAsync(userViewModel.Id.ToString());

            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            // Remove all roles from the user
            var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!removeRolesResult.Succeeded)
            {
                return BadRequest(removeRolesResult.Errors.FirstOrDefault()?.Description);
            }

            // Add the new role to the user
            var role = await _roleManager.FindByNameAsync(userViewModel.Role);
            if (role == null)
            {
                role = new IdentityRole<Guid>(userViewModel.Role);
                var createRoleResult = await _roleManager.CreateAsync(role);
                if (!createRoleResult.Succeeded)
                {
                    return BadRequest(createRoleResult.Errors.FirstOrDefault()?.Description);
                }
            }

            // Update IsActive and AllowedBatches
            user.IsActive = userViewModel.IsActive;
            user.AllowedBatches = userViewModel.AllowedBatches;
            user.MaxStudentPerBatch = userViewModel.MaxStudentPerBatch;

            var addRoleResult = await _userManager.AddToRoleAsync(user, userViewModel.Role);
            if (!addRoleResult.Succeeded)
            {
                return BadRequest(addRoleResult.Errors.FirstOrDefault()?.Description);
            }

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> UserCount()
        {
            var userCount = await _userManager.Users.CountAsync();

            return Ok(new { Count = userCount });
        }


        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return Ok(roles);
        }


        /////////
        ///
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ResetPassword resetPassword)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                if (user == null)
                {
                    return BadRequest("User not found");
                }

                // Check if the current password is correct
                var passwordCheck = await _userManager.CheckPasswordAsync(user, resetPassword.CurrentPassword);
                if (!passwordCheck)
                {
                    return BadRequest("Invalid current password");
                }

                // Change the user's password
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, resetPassword.CurrentPassword, resetPassword.NewPassword);

                if (changePasswordResult.Succeeded)
                {
                    return Ok("Password changed successfully");
                }
                else
                {
                    return BadRequest("Failed to change password");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return BadRequest($"Failed to change password: {ex.Message}");
            }
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateUserDetails(UserDetailsUpdateParameters updateParameters)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                if (user == null)
                {
                    return BadRequest("User not found");
                }

                // Update username and email if provided
                if (!string.IsNullOrEmpty(updateParameters.NewUserName))
                {
                    user.UserName = updateParameters.NewUserName;
                }

                if (!string.IsNullOrEmpty(updateParameters.NewEmail))
                {
                    // Check if the new email is unique
                    var existingUserWithEmail = await _userManager.FindByEmailAsync(updateParameters.NewEmail);
                    if (existingUserWithEmail != null && existingUserWithEmail.Id != user.Id)
                    {
                        return BadRequest("Email is already in use by another user");
                    }

                    user.Email = updateParameters.NewEmail;
                }

                // Update the user in the database
                var updateResult = await _userManager.UpdateAsync(user);

                if (updateResult.Succeeded)
                {
                    return Ok("User details updated successfully");
                }
                else
                {
                    return BadRequest(updateResult.Errors.FirstOrDefault()?.Description ?? "Failed to update user details");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                return BadRequest($"Failed to update user details: {ex.Message}");
            }
        }


        [HttpPost]
        [Authorize]
        //[Authorize(Roles = "Admin")] // Ensure only admins can access this endpoint
        public async Task<IActionResult> RequestPasswordResetByEmail(ResetPasswordByAdmin resetPasswordByAdmin)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordByAdmin.Email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordByAdmin.NewPassword);

            if (result.Succeeded)
            {
                return Ok("Password reset successfully");
            }
            else
            {
                // Failed to reset password, handle errors
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest($"Failed to reset password: {errors}");
            }
        }

    }

}

