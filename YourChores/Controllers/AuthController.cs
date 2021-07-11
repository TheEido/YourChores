using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using YourChores.ApiModels;
using YourChores.Data.Models;

namespace YourChores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration,SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult<ApiResponse<RegisterApiModel.Response>>> Register(RegisterApiModel.Request requestModel)
        {
            var user = new ApplicationUser()
            {
                UserName = requestModel.UserName,
                Email = requestModel.Email
            };

            var result = await _userManager.CreateAsync(user, requestModel.Password);
            var responseModel = new ApiResponse<RegisterApiModel.Response>();
            if (result.Succeeded)
            {
                responseModel.Response = new RegisterApiModel.Response()
                {
                    UserName = requestModel.UserName,
                    Email = requestModel.Email
                };

                return Ok(responseModel);
            }
               
            responseModel.Errors.AddRange(result.Errors.Select(e => e.Description));

            return responseModel;
        }


        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<ApiResponse<LoginApiModel.Response>>> Login(LoginApiModel.Request requestModel)
        {
            var responseModel = new ApiResponse<LoginApiModel.Response>();

            ApplicationUser user;

            if (requestModel.UserNameOrEmail.Contains("@"))
            {
                user = await _userManager.FindByEmailAsync(requestModel.UserNameOrEmail);
            }
            else
            {
                user = await _userManager.FindByNameAsync(requestModel.UserNameOrEmail);
            }

            if (user == null)
            {
                responseModel.AddError("This user is not exist");
                return responseModel;
            }

            var result = await _signInManager.PasswordSignInAsync(user, requestModel.Password, false, false);

            if (result.Succeeded)
            {
                responseModel.Response = new LoginApiModel.Response()
                {
                    Token = GenerateJwt(user)
                };

                return Ok(responseModel);
            }

            responseModel.AddError("This user is not exist");
            return responseModel;
        }


        //End point to check the token and generate new one if valid, and let the user in
        [HttpPost]
        [Authorize]
        [Route("TokenLogin")]
        public async Task<ActionResult<ApiResponse<LoginApiModel.Response>>> TokenLogin()
        {
            var responseModel = new ApiResponse<LoginApiModel.Response>();

            ApplicationUser user;

            user = await _userManager.FindByNameAsync(User.Identity.Name);

            responseModel.Response = new LoginApiModel.Response()
            {
                Token = GenerateJwt(user)
            };

            return Ok(responseModel);


        }


        //End point to set the first and last name for a user
        [HttpPost]
        [Authorize]
        [Route("ChangeName")]
        public async Task<ActionResult<ApiResponse>> ChangeName(ChangeNameApiModel.Request requestModel)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            user.FirstName = requestModel.FirstName;
            user.LastName = requestModel.LastName;

            await _userManager.UpdateAsync(user);

            var responseModel = new ApiResponse();
            return Ok(responseModel);
        }


        //End point to Change password for a user
        [HttpPost]
        [Authorize]
        [Route("ChangePassword")]
        public async Task<ActionResult<ApiResponse>> ChangePassword(ChangePasswordApiModel.Request requestModel)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var result = await _userManager.ChangePasswordAsync(user, requestModel.OldPassword, requestModel.NewPassword);
            var responseModel = new ApiResponse();
            if (result.Succeeded)
            {
                return Ok(responseModel);
            }

            responseModel.Errors = result.Errors.Select(error => error.Description).ToList();
            return responseModel;
        }


        //End point to Get User Info
        [HttpGet]
        [Authorize]
        [Route("GetUserInfo")]
        public async Task<ActionResult<ApiResponse<UserInfoApiModel.Response>>> GetUserInfo()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            
            var responseModel = new ApiResponse<UserInfoApiModel.Response>();

            if (user != null)
            {
                responseModel.Response = new UserInfoApiModel.Response()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                };

                return Ok(responseModel);
            }

            responseModel.AddError("this user is not exist");
            return responseModel;
        }




        private string GenerateJwt(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var Credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                new[] { 
                    new Claim(ClaimTypes.Name, user.UserName)
                },
                expires: DateTime.UtcNow.AddDays(120),
                signingCredentials: Credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
