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
using UnstopAPI.Repository;
using UnstopAPI.Repository.IRepository;

namespace UnstopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : Controller
    {
        protected APIResponse _response;
        private readonly IMapper _mapper;
        private readonly IJobRepository _jobRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICompanyRepository _companyRepository;

        public JobController(IJobRepository jobRepository, IMapper mapper, IUserRepository userRepository, ICompanyRepository companyRepository)
        {
            _response = new();
            _jobRepository = jobRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _companyRepository = companyRepository;
        }


        #region Get All Jobs

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Company, Candidate")]
        public async Task<ActionResult<APIResponse>> GetAllJobs(
                string userRole,
                int userId,
                [FromQuery(Name = "searchJob")] string searchJob,
                [FromQuery(Name = "jobType")] string jobType,
                [FromQuery(Name = "jobTime")] string jobTime,
                [FromQuery(Name = "workingDays")] int workingDays,
                [FromQuery(Name = "sortBy")] string sortBy = null,
                [FromQuery(Name = "sortOrder")] string sortOrder = "asc",
                [FromQuery(Name = "jobStatus")] string jobStatus = "all",
                [FromBody] UserPreferenceDTO userPreferenceDTO = null,
                int pageSize = 5,
                int pageNumber = 1
            )
        {
            try
            {
                Expression<Func<Job, bool>> filter = x => true;

                if (userRole == "Company")
                {
                    filter = x => x.UserId == userId;
                }

                if(jobStatus == "active")
                {
                    filter = filter.AndAlso(x => x.LastDate >= DateTime.UtcNow);
                }

                if (jobStatus == "inactive")
                {
                    filter = filter.AndAlso(x => x.LastDate <= DateTime.UtcNow);
                }

                if (!string.IsNullOrEmpty(searchJob))
                {
                    filter = filter.AndAlso(x => x.Title.ToLower().Contains(searchJob.ToLower()));  
                }

                if (jobType != null)
                {
                    filter = filter.AndAlso(x => x.JobType != null && x.JobType.ToLower().Contains(jobType.ToLower()));
                }

                if (jobTime != null)
                {
                    filter = filter.AndAlso(x => x.JobTiming != null && x.JobTiming.ToLower().Contains(jobTime.ToLower()));
                }

                if (workingDays != 0)
                {
                    filter = filter.AndAlso(x => x.WorkingDays == workingDays);
                }

                if(userPreferenceDTO != null)
                {
                    Expression<Func<Company, bool>> companyFilter = x => true;

                    if (userPreferenceDTO.Industry != null && userPreferenceDTO.Industry.Count > 0)
                    {
                        companyFilter = companyFilter.AndAlso(x => userPreferenceDTO.Industry.Contains(x.Industry));
                    }

                    if (userPreferenceDTO.CompanySize != null && userPreferenceDTO.CompanySize.Count > 0)
                    {
                        companyFilter = companyFilter.AndAlso(x => userPreferenceDTO.CompanySize.Contains(x.CompanySize));
                    }

                    IEnumerable<Company> companyList = await _companyRepository.GetAllAsync(filter: companyFilter, includeProperties: "User");

                    List<int> userIds = companyList.Select(x => x.UserId).ToList();

                    if (userPreferenceDTO.JobType != null && userPreferenceDTO.JobType.Count > 0)
                    {
                        filter = filter.AndAlso(x => userPreferenceDTO.JobType.Contains(x.JobType));
                    }

                    if (userPreferenceDTO.JobTime != null && userPreferenceDTO.JobTime.Count > 0)
                    {
                        filter = filter.AndAlso(x => userPreferenceDTO.JobTime.Contains(x.JobTiming));
                    }

                    filter = filter.AndAlso(x => userIds.Contains(x.UserId));

                }

                Func<IQueryable<Job>, IOrderedQueryable<Job>> orderBy = null;
                if (!string.IsNullOrEmpty(sortBy))
                {
                    switch (sortBy.ToLower())
                    {
                        case "minsalary":
                            orderBy = sortOrder == "asc" ? (Func<IQueryable<Job>, IOrderedQueryable<Job>>)(q => q.OrderBy(j => j.MinSalary)) : (q => q.OrderByDescending(j => j.MinSalary));
                            break;

                        case "maxsalary":
                            orderBy = sortOrder == "asc" ? (Func<IQueryable<Job>, IOrderedQueryable<Job>>)(q => q.OrderBy(j => j.MaxSalary)) : (q => q.OrderByDescending(j => j.MaxSalary));
                            break;
                    }
                }

                IEnumerable<Job> jobList = await _jobRepository.GetAllAsync(filter: filter, pageNumber: pageNumber, pageSize: pageSize, orderBy: orderBy, includeProperties: "User");

                int totalRecords = await _jobRepository.GetTotalRecordsAsync(filter: filter);

                Pagination pagination = new()
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
                };

                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));
                _response.Result = _mapper.Map<List<JobDTO>>(jobList);
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


        #region Get Job

        [Authorize(Roles = "Company, Candidate")]
        [HttpGet("{id:int}", Name = "GetJob")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetJob(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                Job job = await _jobRepository.GetAsync(x => x.JobId == id);

                if (job == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<JobDTO>(job);
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


        #region Create Job

        [Authorize(Roles = "Company")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateJob([FromBody] JobDTO jobDto)
        {
            try
            {
                if (await _userRepository.GetAsync(x => x.Id == jobDto.UserId) == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                if (jobDto == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                Job job = _mapper.Map<Job>(jobDto);

                await _jobRepository.CreateAsync(job);

                _response.Result = _mapper.Map<JobDTO>(job);
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetJob", new { id = job.JobId }, _response);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);

                return _response;
            }
        }

        #endregion


        #region Delete Job

        [Authorize(Roles = "Company")]
        [HttpDelete("{id:int}", Name = "DeleteJob")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteJob(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                Job job = await _jobRepository.GetAsync(x => x.JobId == id);

                if (job == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                await _jobRepository.DeleteAsync(job);

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


        #region Update Job

        [Authorize(Roles = "Company")]
        [HttpPut("{id:int}", Name = "UpdateJob")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> UpdateJob(int id, [FromBody] JobDTO jobDTO)
        {
            try
            {
                if (id == 0 || jobDTO == null || id != jobDTO.JobId)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                if (await _jobRepository.GetAsync(x => x.JobId == id, tracked: false) == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                Job job = _mapper.Map<Job>(jobDTO);

                await _jobRepository.UpdateAsync(job);

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


        [Authorize(Roles = "Company")]
        [HttpPatch("{id:int}", Name = "UpdateJobPartial")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> UpdateJobPartial(int id, [FromBody] JsonPatchDocument<JobDTO> patchDoc)
        {
            try
            {
                if (patchDoc == null || id == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var jobEntity = await _jobRepository.GetAsync(x => x.JobId == id, tracked: false);
                if (jobEntity == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                var jobDTO = _mapper.Map<JobDTO>(jobEntity);

                patchDoc.ApplyTo(jobDTO, ModelState);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _mapper.Map(jobDTO, jobEntity);

                var updatedProperties = patchDoc.Operations
                    .Select(op => op.path.Trim('/'))
                    .ToList();

                var propertyExpressions = new List<Expression<Func<Job, object>>>();
                foreach (var propertyName in updatedProperties)
                {
                    var parameter = Expression.Parameter(typeof(Job));
                    var property = Expression.Property(parameter, propertyName);
                    var lambda = Expression.Lambda<Func<Job, object>>(Expression.Convert(property, typeof(object)), parameter);
                    propertyExpressions.Add(lambda);
                }

                await _jobRepository.UpdatePartialAsync(jobEntity, propertyExpressions.ToArray());

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
