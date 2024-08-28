using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Net;
using UnstopAPI.Models;
using UnstopAPI.Models.DTO;
using UnstopAPI.Repository;
using UnstopAPI.Repository.IRepository;

namespace UnstopAPI.Controllers
{
    [Route("/api/UserAuth")]
    [ApiController]
    public class UserController : Controller
    {
        protected APIResponse _response;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IOTPRepository _otpRepository;

        public UserController(IUserRepository userRepository, IMapper mapper, IOTPRepository otpRepository)
        {
            _userRepository = userRepository;
            this._response = new();
            this._mapper = mapper;
            _otpRepository = otpRepository;
        }


        #region Login

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> Login([FromBody] LoginRequestModel model)
        {
            try
            {
                LoginResponseModel loginResponse = await _userRepository.LoginAsync(model);

                if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Email or Password is Incorrect");

                    return BadRequest(_response);
                }

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = loginResponse;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);

                return _response;
            }
        }

        #endregion


        #region Register

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> Register([FromBody] RegistrationModel model)
        {
            try
            {
                bool emailExists = await _userRepository.UserExistAsync(model.Email!);

                if (emailExists)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Email already Exists");

                    return BadRequest(_response);
                }

                User user = await _userRepository.RegisterAsync(model);

                if (user == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Error while Registering");

                    return BadRequest(_response);
                }

                _response.Result = _mapper.Map<UserDTO>(user);
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);

                return _response;
            }
        }

        #endregion


        #region Forgot Password

        [HttpPost("forgot")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> ForgorPassword([FromBody] string email)
        {
            try
            {
                Expression<Func<User, bool>> filter = x => x.Email == email;

                User user = await _userRepository.GetAsync(filter: filter);

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add("Email does not exists");

                    return NotFound(_response);
                }

                string otpCode = new Random().Next(100000, 999999).ToString();

                ForgotPasswordModel forgotPassword = new()
                {
                    Email = user.Email,
                    OtpCode = otpCode,
                };

                Expression<Func<OTP, bool>> otpFilter = x => x.UserId == user.Id;

                OTP getOTP = await _otpRepository.GetAsync(filter: otpFilter, tracked: false);
                
                if (getOTP == null)
                {
                    OTP newOtp = new()
                    {
                        UserId = user.Id,
                        Code = otpCode,
                        ExpirationTime = DateTime.UtcNow.AddMinutes(1),
                    };

                    await _otpRepository.CreateAsync(newOtp);
                }
                else
                {
                    OTP updatedOtp = new()
                    {
                        OTPId = getOTP.OTPId,
                        UserId = user.Id,
                        Code = otpCode,
                        ExpirationTime = DateTime.UtcNow.AddMinutes(1),
                    };

                    await _otpRepository.UpdateAsync(updatedOtp);
                }

                _response.IsSuccess = true;
                _response.Result = forgotPassword;
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);

                return _response;
            }
        }

        #endregion


        #region Verify OTP

        [HttpPost("verify-otp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> VerifyOTP([FromBody] ForgotPasswordModel model)
        {
            try
            {
                Expression<Func<User, bool>> filter = x => x.Email == model.Email;

                User user = await _userRepository.GetAsync(filter: filter);

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add("Email does not exists");

                    return NotFound(_response);
                }

                Expression<Func<OTP, bool>> otpFilter = x => x.UserId == user.Id && x.Code == model.OtpCode && x.ExpirationTime >= DateTime.UtcNow;

                OTP getOTP = await _otpRepository.GetAsync(filter: otpFilter, tracked: false);

                if (getOTP == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add("Invalid or Expired OTP");

                    return NotFound(_response);
                }
                
                _response.IsSuccess = true;
                _response.Result = model;
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);

                return _response;
            }
        }

        #endregion


        #region Reset Password

        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> ResetPassword([FromBody] ForgotPasswordModel model)
        {
            try
            {
                Expression<Func<User, bool>> filter = x => x.Email == model.Email;

                User user = await _userRepository.GetAsync(filter: filter);

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add("Email does not exists");

                    return NotFound(_response);
                }

                user.PasswordHash = _userRepository.CreatePasswordHash(model.NewPassword);

                await _userRepository.UpdateAsync(user);

                _response.Result = _mapper.Map<UserDTO>(user);
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);

                return _response;
            }
        }

        #endregion

    }
}
