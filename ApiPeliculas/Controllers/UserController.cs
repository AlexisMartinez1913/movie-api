using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiPeliculas.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserRepository _userRepository;
        protected AnswerApi _answerApi;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IMapper mapper) 
        {
            _userRepository = userRepository;
            _mapper = mapper;
            this._answerApi = new();
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetUsers()
        {
            var listUsers = _userRepository.GetUsers();

            var listUserDto = new List<UserDto>();
            foreach (var user in listUsers)
            {
                listUserDto.Add(_mapper.Map<UserDto>(user));
            }
            return Ok(listUserDto);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{userId}", Name = "GetUserById")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUser(string userId)
        {
            var itemUser = _userRepository.GetUser(userId);

            if (itemUser == null)
            {
                return NotFound();
            }

            var itemUserDto = _mapper.Map<UserDto>(itemUser);
            return Ok(itemUserDto);
        }

        [AllowAnonymous]
        //metodo para registro del user
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> RegisterUser([FromBody] UserRegistryDto userRegistryDto)
        {
            bool validateUserNameUnique = _userRepository.IsUniqueUser(userRegistryDto.UserName);
            if (!validateUserNameUnique)
            {
                _answerApi.StatusCode = HttpStatusCode.BadRequest;
                _answerApi.IsSuccess = false;
                _answerApi.ErrorMessages.Add("username already exist");
                return BadRequest(_answerApi);
            }
            var user = await _userRepository.Register(userRegistryDto);
            if (user == null)
            {
                _answerApi.StatusCode = HttpStatusCode.BadRequest;
                _answerApi.IsSuccess = false;
                _answerApi.ErrorMessages.Add("error in the register");
                return BadRequest(_answerApi);
            }
            _answerApi.StatusCode = HttpStatusCode.OK;
            _answerApi.IsSuccess = true;
            return Ok(_answerApi);
        }

        [AllowAnonymous]
        //metodo para crear LOGIN
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> LoginUser([FromBody] UserLoginDto userLoginDto)
        {
            var answerLogin = await _userRepository.Login(userLoginDto);
            
            if (answerLogin.user == null || string.IsNullOrEmpty(answerLogin.Token))
            {
                _answerApi.StatusCode = HttpStatusCode.BadRequest;
                _answerApi.IsSuccess = false;
                _answerApi.ErrorMessages.Add("username or password are incorrects");
                return BadRequest(_answerApi);
            }
            
           
            _answerApi.StatusCode = HttpStatusCode.OK;
            _answerApi.IsSuccess = true;
            _answerApi.Result = answerLogin;
            return Ok(_answerApi);
        }
    }
}
