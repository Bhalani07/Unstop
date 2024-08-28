using Microsoft.AspNetCore.Mvc;
using System.Net;
using UnstopAPI.Models;
using UnstopAPI.Models.DTO;
using UnstopAPI.Repository.IRepository;

namespace UnstopAPI.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class EmailController : Controller
    {
        protected APIResponse _response;
        private readonly IEmailSenderRepository _emailSenderRepository;

        public EmailController(IEmailSenderRepository emailSenderRepository)
        {
            _response = new();
            _emailSenderRepository = emailSenderRepository;
        }


        #region Email

        [HttpPost]
        public async Task<ActionResult<APIResponse>> SendEmail(EmailRequestModel emailRequest)
        {
            try
            {
                bool success = await _emailSenderRepository.SendEmailAsync(emailRequest);

                if(!success)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.InternalServerError;
                    _response.ErrorMessages.Add("Internal Server Error");

                    return _response;
                }

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
