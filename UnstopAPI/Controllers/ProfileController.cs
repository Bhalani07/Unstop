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
    [Route("/api/[controller]")]
    [ApiController]
    public class ProfileController : Controller
    {
        protected APIResponse _response;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly ICandidateRepository _candidateRepository;
        private readonly ICompanyRepository _companyRepository;

        public ProfileController(IMapper mapper, IUserRepository userRepository, ICandidateRepository candidateRepository, ICompanyRepository companyRepository)
        {
            _response = new();
            _mapper = mapper;
            _userRepository = userRepository;
            _candidateRepository = candidateRepository;
            _companyRepository = companyRepository;

        }


        #region Get All Candidate

        [HttpGet(Name = "AllCandidate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Company")]
        public async Task<ActionResult<APIResponse>> GetAllCandidate()
        {
            try
            {
                IEnumerable<Candidate> candidateList = await _candidateRepository.GetAllAsync(includeProperties: "User");

                _response.Result = _mapper.Map<List<CandidateDTO>>(candidateList);
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


        #region Get Candidate

        [Authorize(Roles = "Company, Candidate")]
        [HttpGet("candidate/{id:int}", Name = "GetCandidate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetCandidate(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                Candidate candidate = await _candidateRepository.GetAsync(x => x.UserId == id, includeProperties: "User");

                if (candidate == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<CandidateDTO>(candidate);
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


        #region Update Candidate

        [Authorize(Roles = "Candidate")]
        [HttpPut("candidate/{id:int}", Name = "UpdateCandidate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> UpdateCandidate(int id, [FromBody] CandidateDTO candidateDTO)
        {
            try
            {
                if (id == 0 || candidateDTO == null || id != candidateDTO.CandidateId)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                if (await _userRepository.GetAsync(x => x.Id == candidateDTO.UserId) == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                if (await _candidateRepository.GetAsync(x => x.CandidateId == id, tracked: false) == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                Candidate candidate = _mapper.Map<Candidate>(candidateDTO);

                if(candidate.FullName != null && candidate.Gender != null && candidate.PhoneNumber != null && candidate.Address != null) 
                {
                    candidate.IsProfileCompleted = true;
                }

                await _candidateRepository.UpdateAsync(candidate);

                _response.Result = _mapper.Map<CandidateDTO>(candidate);
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


        #region Get Company

        [HttpGet("company/{id:int}", Name = "GetCompany")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetCompany(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                Company company = await _companyRepository.GetAsync(x => x.UserId == id, includeProperties: "User");

                if (company == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<CompanyDTO>(company);
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


        #region Update Company

        [Authorize(Roles = "Company")]
        [HttpPut("company/{id:int}", Name = "UpdateCompany")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> UpdateCompany(int id, [FromBody] CompanyDTO companyDTO)
        {
            try
            {
                if (id == 0 || companyDTO == null || id != companyDTO.CompanyId)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                if (await _userRepository.GetAsync(x => x.Id == companyDTO.UserId) == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                if (await _companyRepository.GetAsync(x => x.CompanyId == id, tracked: false) == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                Company oldCompany = await _companyRepository.GetAsync(x => x.CompanyId == id, tracked: false);
                Company newCompany = _mapper.Map<Company>(companyDTO);

                if(oldCompany != null)
                {
                    if (oldCompany.Logo.Length > 0)
                    {
                        newCompany.Logo = oldCompany.Logo;
                        newCompany.LogoFileName = oldCompany.LogoFileName;
                        newCompany.LogoContentType = oldCompany.LogoContentType;
                    }
                }

                await _companyRepository.UpdateAsync(newCompany);

                _response.Result = _mapper.Map<CompanyDTO>(newCompany);
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


        #region Create Company

        [HttpPost("user")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateCompany([FromBody] CompanyDTO companyDTO)
        {
            try
            {
                if (await _userRepository.GetAsync(x => x.Id == companyDTO.UserId, tracked: false) == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                if (companyDTO == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                Company company = _mapper.Map<Company>(companyDTO);

                await _companyRepository.CreateAsync(company);

                _response.Result = _mapper.Map<CompanyDTO>(company);
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetCompany", new { id = company.CompanyId }, _response);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);

                return _response;
            }
        }

        #endregion


        #region Get User

        [HttpGet("user/{id:int}", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetUser(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                User user = await _userRepository.GetAsync(x => x.Id == id);

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
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
    }
}
