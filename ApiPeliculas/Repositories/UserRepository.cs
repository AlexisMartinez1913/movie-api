using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XSystem.Security.Cryptography;

namespace ApiPeliculas.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _bd;
        private string _secretPassword;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        public UserRepository(ApplicationDbContext bd, IConfiguration config, UserManager<AppUser>userManager, RoleManager<IdentityRole> roleManager,
            IMapper mapper)
        {
            _bd = bd;
            //referencia al apisettinngs
            _secretPassword = config.GetValue<string>("ApiSettings:Secret");

            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        //cambios usando .NET Identity
        public AppUser GetUser(string userId)
        {
            //con dbcontextet
            //return _bd.User.FirstOrDefault(u => u.Id == userId);
            //con .NET Identity
            return _bd.AppUser.FirstOrDefault(u => u.Id == userId);
        }

        public ICollection<AppUser> GetUsers()
        {
            return _bd.AppUser.OrderBy(u => u.UserName).ToList();
        }

        public bool IsUniqueUser(string user)
        {
            var userbd = _bd.AppUser.FirstOrDefault(u => u.UserName == user);
            if (userbd == null)
            {
                return true;
            }
            return false;
            

        }

        //metodo para el login

        public async Task<UserLoginAnswerDto> Login(UserLoginDto userLoginDto)
        {
            //comparar nombre de usuario y el password encriptado
            //var encryptedPassword = _GetMd5(userLoginDto.Password);
            //var user = _bd.User.FirstOrDefault(
            //    u => u.UserName.ToLower() == userLoginDto.UserName.ToLower()
            //    && u.Password == encryptedPassword
            //    );
            var user = _bd.AppUser.FirstOrDefault(
                u => u.UserName == userLoginDto.UserName.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(user, userLoginDto.Password);
            //validamos si el usuario no existe con la combinacion de user y password correcta
            if (user == null || isValid == false)
            {
                return new UserLoginAnswerDto()
                {
                    Token = "",
                    user = null
                };
            }

            //aqui si existe el user entonces podemos procesar el login
            var roles = await _userManager.GetRolesAsync(user);

            var handleToken = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretPassword);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = handleToken.CreateToken(tokenDescriptor);
            UserLoginAnswerDto userLoginAnswerDto = new UserLoginAnswerDto()
            {
                Token = handleToken.WriteToken(token),
                user = _mapper.Map<UserDataDto>(user)
            };
            return userLoginAnswerDto;

        }

        //metodo para el registro

        public async Task<UserDataDto> Register(UserRegistryDto userRegistryDto)
        {
            //password encriptado
            //var encryptedPassword = _GetMd5(userRegistryDto.Password);
            AppUser user = new AppUser()
            {
                UserName = userRegistryDto.UserName,
                Email = userRegistryDto.UserName,
                NormalizedEmail = userRegistryDto.UserName.ToUpper(),
                Name = userRegistryDto.Name

            };
            //enviar user y password
            var result = await _userManager.CreateAsync(user, userRegistryDto.Password);
            //validar
            if (result.Succeeded)
            {
                //se procede a crear roles en la aplicación si no existen previamente
                if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                {
                    await _roleManager.CreateAsync(new IdentityRole("admin"));
                    await _roleManager.CreateAsync(new IdentityRole("registered"));
                }

                await _userManager.AddToRoleAsync(user, "admin");
                var userReturned = _bd.AppUser.FirstOrDefault(u => u.UserName == userRegistryDto.UserName);

                ////opcion 1 

                //return new UserDataDto()
                //{
                //    Id = userReturned.Id,
                //    UserName = userReturned.UserName,
                //    Name = userReturned.Name
                //};

                //opcion 2
                return _mapper.Map<UserDataDto>(userReturned);
            }
            //agregar user
            //_bd.User.Add(user);
            //await _bd.SaveChangesAsync();
            //user.Password = encryptedPassword;
            //return user;

            return new UserDataDto();
        }

        private string _GetMd5(string password)
        {
            //metodo de encriptacion de contraseñas md5
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(password);
            data = x.ComputeHash(data);
            string response = "";
            for (int i = 0; i < data.Length; i++)

                response += data[i].ToString("x2").ToLower();
            return response;

        }
    }
}
