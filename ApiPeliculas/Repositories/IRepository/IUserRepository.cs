using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;

namespace ApiPeliculas.Repository.IRepository
{
    public interface IUserRepository
    {
        ICollection<AppUser> GetUsers();
        AppUser GetUser(string userId);
        bool IsUniqueUser(string user);
        Task<UserLoginAnswerDto> Login(UserLoginDto userLoginDto);
        Task<UserDataDto> Register(UserRegistryDto userRegistryDto);

    }
}
