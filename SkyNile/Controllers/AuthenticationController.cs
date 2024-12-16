using BusinessLogic.Models;
using DataAccess.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using MimeKit;
using SkyNile.DTO;
using SkyNile.HelperModel;
using SkyNile.Services;
using System.Text;
using MailKit.Net.Smtp;
using MimeKit;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using Azure;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http.HttpResults;

namespace SkyNile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthService _authService;
        private readonly IMailingServices _mailingService;
        private readonly SignInManager<User> _signinmanager;

        public AuthenticationController(SignInManager<User> signinmanager, IMailingServices mailingService, IAuthService authService, UserManager<User> userManager)
        {
            _userManager = userManager;
            _authService = authService;
            _mailingService = mailingService;
            _signinmanager = signinmanager;
        }
        [HttpPost("/login")]
        public async Task<IActionResult> login([FromForm] Login model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.GetTokenAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);

        }

        [HttpPost("/register")]
        public async Task<IActionResult> register([FromForm] Register model)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            var user = await _userManager.FindByEmailAsync(model.Email);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", new { token, email = result.Email }, Request.Scheme);
            await _mailingService.SendMailAsync(model.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");

            return Ok("relogin after chack inbox to confirm mail :)");

        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return Ok("Email Verified Successfully");
                }
            }
            return BadRequest("invaild user");
        }

        [HttpPost("/ResendCodeConfirmEmail")]
        public async Task<IActionResult> ResendCodeConfirmEmail([FromForm] string mail)
        {
            var user = await _userManager.FindByEmailAsync(mail);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", new { token, email = mail }, Request.Scheme);

            await _mailingService.SendMailAsync(mail, "Confirm your email",
                        $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");

            return Ok("resend Succeeded");
        }

        [HttpPost("/ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromForm] Login l)
        {
            var user = await _userManager.FindByEmailAsync(l.Email);
            if (user == null)
            {
                return BadRequest("invaild user");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ResetPassword), "Authentication", new { mail = l.Email, newpassword = l.Password, token }, Request.Scheme);

            await _mailingService.SendMailAsync(l.Email, "Confirm ResetPassword",
                                   $"To Reset your Password <a href='{confirmationLink}'>clicking here</a>.");

            return Ok("Done");
        }

        [HttpGet("/ResetPassword")]
        public async Task<IActionResult> ResetPassword(string mail, string newpassword, string token)
        {
            var user = await _userManager.FindByEmailAsync(mail);
            if (user == null)
            {
                return BadRequest("invaild user");
            }
            var result = await _userManager.ResetPasswordAsync(user, token, newpassword);

            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return BadRequest(errors);
            }
            return Ok("ResetPassword Succeeded");

        }
    }
}