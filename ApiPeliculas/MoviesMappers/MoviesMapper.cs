using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using AutoMapper;

namespace ApiPeliculas.MoviesMapper
{
    public class MoviesMapper : Profile
    {
        public MoviesMapper()
        {
            //automapper de la categoria y el dto
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, CreateCategoryDto>().ReverseMap();
            CreateMap<Movie, MovieDto>().ReverseMap();
            CreateMap<AppUser, UserDataDto>().ReverseMap();
            CreateMap<AppUser, UserDto>().ReverseMap();

        }
    }
}
