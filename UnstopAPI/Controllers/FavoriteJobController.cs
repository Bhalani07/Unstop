using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class FavoriteJobController : Controller
    {
        protected APIResponse _response;
        private readonly IJobRepository _jobRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFavoriteJobRepository _favoriteJobRepository;
        private readonly IMapper _mapper;

        public FavoriteJobController(IJobRepository jobRepository, IMapper mapper, IUserRepository userRepository, IFavoriteJobRepository favoriteJobRepository)
        {
            _response = new();
            _jobRepository = jobRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _favoriteJobRepository = favoriteJobRepository;

        }


        #region Get All Favorite Jobs

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult<APIResponse>> GetAllFavoriteJobs(
                int userId,
                [FromQuery(Name = "searchFavorite")] string searchJob,
                int pageSize = 5,
                int pageNumber = 1
            )
        {
            try
            {
                Expression<Func<FavoriteJob, bool>> filter = x => x.UserId == userId;

                if (!string.IsNullOrEmpty(searchJob))
                {
                    filter = filter.AndAlso(x => x.Job.Title.ToLower().Contains(searchJob.ToLower()));
                }

                IEnumerable<FavoriteJob> favoriteList = await _favoriteJobRepository.GetAllAsync(filter: filter, pageNumber: pageNumber, pageSize: pageSize, includeProperties: "Job,User");

                int totalRecords = await _favoriteJobRepository.GetTotalRecordsAsync(filter: filter);

                Pagination pagination = new()
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
                };

                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));
                _response.Result = _mapper.Map<List<FavoriteDTO>>(favoriteList);
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


        #region Get Favorite Job

        [Authorize(Roles = "Candidate")]
        [HttpGet("{id:int}", Name = "GetFavoriteJob")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetFavoriteJob(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                FavoriteJob favorite = await _favoriteJobRepository.GetAsync(x => x.FavoriteId == id);

                if (favorite == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<FavoriteDTO>(favorite);
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


        #region Create Favorite Job

        [Authorize(Roles = "Candidate")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateFavoriteJob([FromBody] FavoriteDTO favoriteDTO)
        {
            try
            {
                if (await _userRepository.GetAsync(x => x.Id == favoriteDTO.UserId) == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                if (await _jobRepository.GetAsync(x => x.JobId == favoriteDTO.JobId) == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                FavoriteJob favoriteJob = _mapper.Map<FavoriteJob>(favoriteDTO);

                await _favoriteJobRepository.CreateAsync(favoriteJob);

                _response.Result = _mapper.Map<FavoriteDTO>(favoriteJob);
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetFavoriteJob", new { id = favoriteJob.FavoriteId }, _response);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);

                return _response;
            }
        }

        #endregion


        #region Check Favorite 

        [Authorize(Roles = "Candidate")]
        [HttpGet("{jobId:int}/{userId:int}", Name = "CheckFavorite")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> CheckFavorite(int jobId, int userId)
        {
            try
            {
                if (jobId != 0 && userId != 0)
                {
                    if (await _favoriteJobRepository.GetAsync(x => x.JobId == jobId && x.UserId == userId) == null)
                    {
                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.BadRequest;
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


        #region Remove Favorite Job

        [Authorize(Roles = "Candidate")]
        [HttpDelete("{jobId:int}/{userId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> RemoveFavoriteJob(int jobId, int userId)
        {
            try
            {
                if (await _userRepository.GetAsync(x => x.Id == userId) == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                if (await _jobRepository.GetAsync(x => x.JobId == jobId) == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                FavoriteJob favorite = await _favoriteJobRepository.GetAsync(x => x.JobId == jobId && x.UserId == userId);

                if(favorite == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                await _favoriteJobRepository.DeleteAsync(favorite);

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
