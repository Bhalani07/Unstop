using IronQr;
using IronSoftware.Drawing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using Unstop.Models;
using Unstop.Models.DTO;
using Unstop.Services;
using Unstop.Services.IServices;
using Unstop_Utility;

namespace Unstop.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly IProfileService _profileService;
        private readonly IApplicationService _applicationService;
        private readonly IJobFairService _jobFairService;

        public HomeController(IAuthService authService, IEmailService emailService, IProfileService profileService, IApplicationService applicationService, IJobFairService jobFairService) 
        {
            _authService = authService;
            _emailService = emailService;
            _profileService = profileService;
            _applicationService = applicationService;
            _jobFairService = jobFairService;
        }


        #region Login

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestModel model = new();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestModel model)
        {
            APIResponse response = await _authService.LoginAsync<APIResponse>(model);

            if (response != null && response.IsSuccess)
            {
                LoginResponseModel loginResponse = JsonConvert.DeserializeObject<LoginResponseModel>(Convert.ToString(response.Result));

                ClaimsIdentity identity = new(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, loginResponse.User.Id.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Role, loginResponse.User.Role));

                ClaimsPrincipal principal = new(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                HttpContext.Session.SetString(StaticDetails.SessionToken, loginResponse.Token);
                HttpContext.Session.SetString(StaticDetails.SessionId, loginResponse.User.Id.ToString());   
                HttpContext.Session.SetString(StaticDetails.SessionRole, loginResponse.User.Role);
                HttpContext.Session.SetString(StaticDetails.SessionName, loginResponse.User.FirstName);

                if (loginResponse.User.Role == "Company")
                {
                    APIResponse companyResponse = await _profileService.GetCompanyAsync<APIResponse>(loginResponse.User.Id);

                    if (companyResponse.Result != null && companyResponse.IsSuccess)
                    {
                        TempData["success"] = "Login Successfull";
                        return RedirectToAction("Dashboard", "Company");
                    }

                    TempData["error"] = "Please Complete Registration Process From Email";
                    return RedirectToAction("Login", "Home");

                }
                else if (loginResponse.User.Role == "Candidate")
                {
                    TempData["success"] = "Login Successfull";
                    return RedirectToAction("Dashboard", "Candidate");
                }

                return RedirectToAction("AccessDenied", "Home");
            }

            //ModelState.AddModelError("Custom Error", response.ErrorMessages.FirstOrDefault());

            else if (response == null)
            {
                TempData["error"] = "Something Went Wrong";
            }
            else
            {
                TempData["error"] = response.ErrorMessages.FirstOrDefault();
            }

            return View(model);
        }

        #endregion


        #region Logout

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.SetString(StaticDetails.SessionToken, "");
            HttpContext.Session.SetString(StaticDetails.SessionId, "");
            HttpContext.Session.SetString(StaticDetails.SessionRole, "");
            HttpContext.Session.SetString(StaticDetails.SessionName, "");

            return RedirectToAction("Login", "Home");
        }

        #endregion


        #region Register

        [HttpGet]
        public IActionResult Register()
        {
            RegistrationModel model = new();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationModel model)
        {
            APIResponse response = await _authService.RegisterAsync<APIResponse>(model);

            if (response != null && response.IsSuccess)
            {
                if (model.Role == "Company")
                {
                    UserDTO user = JsonConvert.DeserializeObject<UserDTO>(Convert.ToString(response.Result));

                    string registrationLink = $"https://localhost:7002/Home/CompanyRegistration?userId={user.Id}";

                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "email", "CompanyRegistration.html");
                    string emailBody = await System.IO.File.ReadAllTextAsync(filePath);

                    emailBody = emailBody.Replace("[Company's First Name]", user.FirstName)
                                .Replace("[url]", registrationLink);

                    EmailRequestModel emailRequest = new()
                    {
                        To = user.Email,
                        Subject = "Complete Your Company Registration - Unstop",
                        IsBodyHtml = true,
                        Body = emailBody
                    };

                    await _emailService.SendEmailAsync<APIResponse>(emailRequest);

                    TempData["success"] = "Check Your Email to Proceed Further";
                }
                else
                {
                    TempData["success"] = "Registration Successfull";
                }

                return RedirectToAction("Login", "Home");
            }
            else if (response == null)
            {
                TempData["error"] = "Something Went Wrong";
            }
            else
            {
                TempData["error"] = response.ErrorMessages.FirstOrDefault();
            }

            return View(model);
        }

        [HttpGet]   
        public async Task<IActionResult> CompanyRegistration(int userId)
        {
            CompanyDTO model = new();

            APIResponse companyResponse = await _profileService.GetCompanyAsync<APIResponse>(userId);

            if (companyResponse.StatusCode == HttpStatusCode.NotFound)
            {
                APIResponse userResponse = await _profileService.GetUserAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), userId);

                if (userResponse != null && userResponse.IsSuccess)
                {
                    UserDTO user = JsonConvert.DeserializeObject<UserDTO>(Convert.ToString(userResponse.Result));

                    model.Email = user.Email;
                    model.UserId = userId;

                    return View(model);
                }
            }

            TempData["error"] = "Registration Already Done";

            return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompanyRegistration(CompanyDTO model)
        {
            APIResponse response = await _profileService.CreateUserAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), model);

            if (response != null && response.IsSuccess)
            {
                CompanyDTO company = JsonConvert.DeserializeObject<CompanyDTO>(Convert.ToString(response.Result));

                QrCode jobQr = QrWriter.Write($"{company.UserId}");
                string qrPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "QRs", $"{company.CompanyName}-Details.png");
                using (AnyBitmap qrBitmap = jobQr.Save())
                {
                    qrBitmap.SaveAs(qrPath);
                }

                TempData["success"] = "Registration Successfull";
                return RedirectToAction("Login", "Home");
            }
            else if (response == null)
            {
                TempData["error"] = "Something Went Wrong";
            }
            else
            {
                TempData["error"] = response.ErrorMessages.FirstOrDefault();
            }
            return View(model);
        }

        #endregion


        #region Access Denied

        public IActionResult AccessDenied()
        {
            return View();
        }

        #endregion


        #region Forgot Password

        public IActionResult Forgot()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Forgot(string email)
        {
            APIResponse forgotResponse = await _authService.ForgotAsync<APIResponse>(email);

            if (forgotResponse != null && forgotResponse.IsSuccess)
            {
                ForgotPasswordModel forgotPassword = JsonConvert.DeserializeObject<ForgotPasswordModel>(Convert.ToString(forgotResponse.Result));

                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "email", "OTPForForgot.html");
                string emailBody = await System.IO.File.ReadAllTextAsync(filePath);

                emailBody = emailBody.Replace("[Candidate's Email]", forgotPassword.Email)
                            .Replace("[OTP]", forgotPassword.OtpCode);

                EmailRequestModel emailRequest = new()
                {
                    To = forgotPassword.Email,
                    Subject = "Verification Code - Unstop",
                    IsBodyHtml = true,
                    Body = emailBody
                };

                await _emailService.SendEmailAsync<APIResponse>(emailRequest);

                TempData["success"] = "Verification Code sent to your Email Address";
                return RedirectToAction("ForgotOTP", new { forgotPassword.Email });
            }

            TempData["error"] = forgotResponse.ErrorMessages.FirstOrDefault();
            return View("Forgot");

        }

        #endregion


        #region OTP Verification

        public IActionResult ForgotOTP(string email)
        {
            ForgotPasswordModel model = new()
            {
                Email = email,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ForgotOTP(ForgotPasswordModel model)
        {
            APIResponse otpResponse = await _authService.ValidateOTPAsync<APIResponse>(model);

            if (otpResponse != null && otpResponse.IsSuccess)
            {
                ForgotPasswordModel forgotPassword = JsonConvert.DeserializeObject<ForgotPasswordModel>(Convert.ToString(otpResponse.Result));

                TempData["success"] = "Code Verified";
                return RedirectToAction("ResetPassword", new { forgotPassword.Email });
            }

            TempData["error"] = otpResponse.ErrorMessages.FirstOrDefault();
            return View("Forgot");
        }

        #endregion


        #region Reset Password

        public IActionResult ResetPassword(string email)
        {
            ForgotPasswordModel model = new()
            {
                Email = email,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ForgotPasswordModel model)
        {
            APIResponse resetResponse = await _authService.ResetPasswordAsync<APIResponse>(model);

            if (resetResponse != null && resetResponse.IsSuccess)
            {
                TempData["success"] = "Password Changed";
                return RedirectToAction("Login");
            }

            TempData["error"] = resetResponse.ErrorMessages.FirstOrDefault();
            return RedirectToAction("Login");
        }

        #endregion


        #region Resume

        public async Task<IActionResult> GetResume(int applicationId, int jobId)
        {
            APIResponse response = await _applicationService.GetAsync<APIResponse>(token: HttpContext.Session.GetString(StaticDetails.SessionToken), applicationId);

            ApplicationDTO application = JsonConvert.DeserializeObject<ApplicationDTO>(Convert.ToString(response.Result));

            return File(application.Resume, application.ResumeContentType, application.ResumeFileName);
        }

        #endregion


        #region Job Fair Information

        [HttpGet]
        public async Task<IActionResult> JobFairInformation(int jobFairId)
        {
            JobFairDTO model = new();

            APIResponse response = await _jobFairService.GetAsync<APIResponse>(jobFairId);

            if(response != null && response.IsSuccess)
            {
                model = JsonConvert.DeserializeObject<JobFairDTO>(Convert.ToString(response.Result));
            }

            return View(model);
        }

        #endregion

    }
}