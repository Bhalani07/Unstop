using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UnstopAPI.Models;
using UnstopAPI.Models.DTO;
using UnstopAPI.Repository;
using UnstopAPI.Repository.IRepository;

namespace UnstopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController : Controller
    {
        protected APIResponse _response;
        private readonly IMapper _mapper;
        private readonly ITemplateRepository _templateRepository;
        private readonly IElementRepository _elementRepository;

        public TemplateController(IMapper mapper, ITemplateRepository templateRepository, IElementRepository elementRepository)
        {
            _response = new();
            _mapper = mapper;
            _templateRepository = templateRepository;
            _elementRepository = elementRepository;

        }


        #region Get All Template

        [Authorize(Roles = "Company")]
        [HttpGet(Name = "AllTemplate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> GetAllTemplate()
        {
            try
            {
                IEnumerable<Template> templates = await _templateRepository.GetAllAsync();

                _response.Result = _mapper.Map<List<TemplateDTO>>(templates);
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


        #region Get Template

        [Authorize(Roles = "Company")]
        [HttpGet("{id:int}", Name = "GetTemplate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetTemplate(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                Template template = await _templateRepository.GetAsync(x => x.TemplateId == id);

                if (template == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                TemplateDTO templateDTO = _mapper.Map<TemplateDTO>(template);

                List<Element> elements = await _elementRepository.GetAllAsync(x => x.TemplateId == id);

                templateDTO.Elements = _mapper.Map<List<ElementDTO>>(elements);

                _response.Result = templateDTO;
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


        #region Create Template

        [Authorize(Roles = "Company")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateTemplate([FromBody] TemplateDTO templateDTO)
        {
            try
            {
                if (templateDTO == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                Template template = _mapper.Map<Template>(templateDTO);

                await _templateRepository.CreateAsync(template);

                if(templateDTO.Elements != null)
                {
                    if (await _elementRepository.GetAsync(x => x.TemplateId == template.TemplateId) != null)
                    {
                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        return BadRequest(_response);
                    }

                    foreach(var item in templateDTO.Elements)
                    {
                        item.TemplateId = template.TemplateId;
                        Element element = _mapper.Map<Element>(item);

                        await _elementRepository.CreateAsync(element);
                    }
                }

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetTemplate", new { id = template.TemplateId }, _response);

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
