using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Net;
using UnstopAPI.Extentions;
using UnstopAPI.Models;
using UnstopAPI.Models.DTO;
using UnstopAPI.Repository.IRepository;

namespace UnstopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterviewController : Controller
    {
        protected APIResponse _response;
        private readonly IMapper _mapper;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IInterviewRepository _interviewRepository;

        public InterviewController(IMapper mapper, IApplicationRepository applicationRepository, IInterviewRepository interviewRepository)
        {
            _response = new APIResponse();
            _mapper = mapper;
            _applicationRepository = applicationRepository;
            _interviewRepository = interviewRepository;
        }


        #region Get All Interviews

        [Authorize(Roles = "Company, Candidate")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> GetAllInterviews(string userRole, int userId)
        {
            try
            {
                Expression<Func<Interview, bool>> filter = null;

                if (userRole == "Candidate")
                {
                    filter = x => x.Application.Candidate.UserId == userId;
                }

                if (userRole == "Company")
                {
                    filter = x => x.Application.Job.UserId == userId;
                }

                IEnumerable<Interview> interviewList = await _interviewRepository.GetAllAsync(filter: filter, includeProperties: "Application");

                _response.Result = _mapper.Map<List<InterviewDTO>>(interviewList);
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


        #region Get Interview

        [Authorize(Roles = "Company, Candidate")]
        [HttpGet("{id:int}", Name = "GetInterview")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetInterview(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                Interview interview = await _interviewRepository.GetAsync(x => x.InterviewId == id, includeProperties: "Application");

                if (interview == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<InterviewDTO>(interview);
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


        #region Create Interview

        [Authorize(Roles = "Company")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateInterview([FromBody] InterviewDTO interviewDTO)
        {
            try
            {
                if (await _applicationRepository.GetAsync(x => x.ApplicationId == interviewDTO.ApplicationId) == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                if (interviewDTO == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                Interview interview = _mapper.Map<Interview>(interviewDTO);

                await _interviewRepository.CreateAsync(interview);

                _response.Result = _mapper.Map<InterviewDTO>(interview);
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetInterview", new { id = interview.InterviewId }, _response);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);

                return _response;
            }
        }

        #endregion


        #region Update Interview

        [Authorize(Roles = "Company")]
        [HttpPut("{id:int}", Name = "UpdateInterview")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> UpdateInterview(int id, [FromBody] InterviewDTO interviewDTO)
        {
            try
            {
                if (id == 0 || interviewDTO == null || id != interviewDTO.InterviewId)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                if (await _interviewRepository.GetAsync(x => x.InterviewId == id, tracked: false) == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                Interview interview = _mapper.Map<Interview>(interviewDTO);

                await _interviewRepository.UpdateAsync(interview);

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.NoContent;
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
