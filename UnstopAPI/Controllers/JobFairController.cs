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
    [Route("api/[controller]")]
    [ApiController]
    public class JobFairController : Controller
    {
        protected APIResponse _response;
        private readonly IMapper _mapper;
        private readonly IJobFairRepository _jobFairRepository;
        private readonly ICompanyRepository _companyRepository;

        public JobFairController(IJobFairRepository jobFairRepository, ICompanyRepository companyRepository, IMapper mapper)
        {
            _response = new APIResponse();
            _mapper = mapper;
            _jobFairRepository = jobFairRepository;
            _companyRepository = companyRepository;
        }

        #region Get All Job Fairs

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Company, Candidate")]
        public async Task<ActionResult<APIResponse>> GetAllJobFairs()
        {
            try
            {
                IEnumerable<JobFair> jobFairList = await _jobFairRepository.GetAllAsync(includeProperties: "Company");

                _response.Result = _mapper.Map<List<JobFairDTO>>(jobFairList);
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


        #region Get Job Fair

        //[Authorize(Roles = "Company, Candidate")]
        [HttpGet("{id:int}", Name = "GetJobFair")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetJobFair(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                JobFair jobFair = await _jobFairRepository.GetAsync(x => x.JobFairId == id);

                if (jobFair == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<JobFairDTO>(jobFair);
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

         
        #region Create Job Fair

        [Authorize(Roles = "Company")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateJobFair([FromBody] JobFairDTO jobFairDto)
        {
            try
            {
                if (await _companyRepository.GetAsync(x => x.CompanyId == jobFairDto.CompanyId) == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                if (jobFairDto == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                JobFair jobFair = _mapper.Map<JobFair>(jobFairDto);

                await _jobFairRepository.CreateAsync(jobFair);

                _response.Result = _mapper.Map<JobFairDTO>(jobFair);
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetJobFair", new { id = jobFair.JobFairId }, _response);

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
