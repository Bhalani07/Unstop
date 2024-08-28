using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Linq.Expressions;
using System.Net;
using System.Text.Json;
using UnstopAPI.Extentions;
using UnstopAPI.Models;
using UnstopAPI.Models.DTO;
using UnstopAPI.Repository.IRepository;

namespace UnstopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : Controller
    {
        protected APIResponse _response;
        private readonly IMapper _mapper;
        private readonly IJobRepository _jobRepository;
        private readonly IUserRepository _userRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly ICandidateRepository _candidateRepository;

        public ApplicationController(IMapper mapper, IJobRepository jobRepository, IUserRepository userRepository, IApplicationRepository applicationRepository, ICandidateRepository candidateRepository)
        {
            _response = new();
            _mapper = mapper;
            _jobRepository = jobRepository;
            _userRepository = userRepository;
            _applicationRepository = applicationRepository;
            _candidateRepository = candidateRepository; 

        }


        #region Get All Applications

        [Authorize(Roles = "Company, Candidate")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> GetAllApplications(
                string userRole,
                int userId,
                int jobId,
                [FromQuery(Name = "searchApplicants")] string searchApplicants,
                [FromQuery(Name = "searchApplication")] string searchApplication,
                [FromQuery(Name = "status")] List<string> statusList,
                [FromQuery(Name = "sortOrder")] string sortOrder = "asc",
                int pageNumber = 1,
                int pageSize = 5
            )
        {
            try
            {
                Expression<Func<Application, bool>> filter = null;

                if(userRole == "Candidate")
                {
                    filter = x => x.CandidateId == userId;
                }

                if(userRole == "Company")
                {
                    filter = x => x.Job.UserId == userId;

                    if(jobId != 0)
                    {
                        filter = x => x.JobId == jobId;
                    }
                }

                if (!string.IsNullOrEmpty(searchApplicants))
                {
                    filter = filter.AndAlso(x => x.Candidate.FullName.ToLower().Contains(searchApplicants.ToLower()));
                }

                if (!string.IsNullOrEmpty(searchApplication))
                {
                    filter = filter.AndAlso(x => x.Job.Title.ToLower().Contains(searchApplication.ToLower()));  
                }

                if (statusList != null && statusList.Count > 0)
                {
                    filter = filter.AndAlso(x => statusList.Contains(x.Status));
                }

                Func<IQueryable<Application>, IOrderedQueryable<Application>> orderBy = null;
                if (!string.IsNullOrEmpty(sortOrder))
                {
                    switch (sortOrder.ToLower())
                    {
                        case "asc":
                            orderBy = (Func<IQueryable<Application>, IOrderedQueryable<Application>>)(q => q.OrderBy(j => j.AppliedDate));
                            break;

                        case "desc":
                            orderBy = (Func<IQueryable<Application>, IOrderedQueryable<Application>>)(q => q.OrderByDescending(j => j.AppliedDate));
                            break;
                    }
                }

                IEnumerable<Application> applicationList = await _applicationRepository.GetAllAsync(filter: filter, includeProperties: "Job,Candidate", pageNumber: pageNumber, pageSize: pageSize, orderBy: orderBy);

                int totalApplications = await _applicationRepository.GetTotalRecordsAsync(filter: filter);

                Pagination pagination = new()
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalApplications / pageSize)
                };

                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));
                _response.Result = _mapper.Map<List<ApplicationDTO>>(applicationList);
                _response.PaginationData = pagination;
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


        #region Get Application

        [Authorize(Roles = "Company, Candidate")]
        [HttpGet("{id:int}", Name = "GetApplication")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetApplication(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                Application application = await _applicationRepository.GetAsync(x => x.ApplicationId == id, includeProperties: "Candidate,Job");

                if (application == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<ApplicationDTO>(application);
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


        #region Check Application

        [Authorize(Roles = "Candidate")]
        [HttpGet("{jobId:int}/{userId:int}", Name = "CheckApplication")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> CheckApplication(int jobId, int userId)
        {
            try
            {
                if (jobId != 0 && userId != 0)
                {
                    if (await _applicationRepository.GetAsync(x => x.JobId == jobId && x.CandidateId == userId) != null)
                    {
                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.ErrorMessages.Add("Already Applied");

                        return BadRequest(_response);
                    }
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


        #region Create Application

        [Authorize(Roles = "Candidate")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateApplication([FromBody] ApplicationDTO applicationDTO)
        {
            try
            {
                if (await _candidateRepository.GetAsync(x => x.CandidateId == applicationDTO.CandidateId) == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                if (await _jobRepository.GetAsync(x => x.JobId == applicationDTO.JobId) == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                if (applicationDTO == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                applicationDTO.Status = "Applied";

                Application application = _mapper.Map<Application>(applicationDTO);

                await _applicationRepository.CreateAsync(application);

                _response.Result = _mapper.Map<ApplicationDTO>(application);
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetApplication", new { id = application.ApplicationId }, _response);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);

                return _response;
            }
        }

        #endregion


        #region Update Application
           
        [Authorize(Roles = "Company, Candidate")]
        [HttpPut("{id:int}", Name = "UpdateApplication")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> UpdateApplication(int id, [FromBody] ApplicationDTO applicationDTO)
        {
            try
            {
                if (id == 0 || applicationDTO == null || id != applicationDTO.ApplicationId)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                if (await _applicationRepository.GetAsync(x => x.ApplicationId == id, tracked: false) == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                Application application = _mapper.Map<Application>(applicationDTO);

                await _applicationRepository.UpdateAsync(application);

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
