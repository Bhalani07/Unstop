using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.InteropServices;
using UnstopAPI.Extentions;
using UnstopAPI.Models;
using UnstopAPI.Models.DTO;
using UnstopAPI.Repository;
using UnstopAPI.Repository.IRepository;

namespace UnstopAPI.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class PreferenceController : Controller
    {
        protected APIResponse _response;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IUserPreferenceRepository _userPreferenceRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IJobRepository _jobRepository;

        public PreferenceController(IMapper mapper, IUserRepository userRepository, IUserPreferenceRepository userPreferenceRepository, ICompanyRepository companyRepository, IJobRepository jobRepository)
        {
            _response = new();
            _mapper = mapper;
            _userRepository = userRepository;
            _userPreferenceRepository = userPreferenceRepository;
            _companyRepository = companyRepository;
            _jobRepository = jobRepository;
        }


        #region Get Preferences

        [Authorize(Roles = "Candidate")]
        [HttpGet("{id:int}", Name = "GetPreference")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetPreference(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                UserPreference userPreferences = await _userPreferenceRepository.GetAsync(x => x.UserId == id);

                if (userPreferences == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<UserPreferenceDTO>(userPreferences);
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


        #region Create Preference 

        [Authorize(Roles = "Candidate")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreatePreference([FromBody] UserPreferenceDTO userPreferenceDTO)
        {
            try
            {
                if (await _userRepository.GetAsync(x => x.Id == userPreferenceDTO.UserId) == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                if (userPreferenceDTO == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                UserPreference userPreferences = _mapper.Map<UserPreference>(userPreferenceDTO);

                await _userPreferenceRepository.CreateAsync(userPreferences);

                _response.Result = _mapper.Map<UserPreferenceDTO>(userPreferences);
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetPreference", new { id = userPreferences.UserId }, _response);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);

                return _response;
            }
        }

        #endregion


        #region Update Preference

        [Authorize(Roles = "Candidate")]
        [HttpPut("{id:int}", Name = "UpdatePreference")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> UpdatePreference(int id, [FromBody] UserPreferenceDTO userPreferenceDTO)
        {
            try
            {
                if (id == 0 || userPreferenceDTO == null || id != userPreferenceDTO.PreferenceId)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                if (await _userPreferenceRepository.GetAsync(x => x.PreferenceId == id, tracked: false) == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                UserPreference userPreference = _mapper.Map<UserPreference>(userPreferenceDTO);

                await _userPreferenceRepository.UpdateAsync(userPreference);

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
