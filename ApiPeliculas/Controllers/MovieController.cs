using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repository;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers
{
    [Route("api/movies")]
    [ApiController]
    public class MovieController : ControllerBase
    {

        private readonly IMovieRepository _movieRepository;

        private readonly IMapper _mapper;

        public MovieController(IMovieRepository movieRepository, IMapper mapper)
        {
            _movieRepository = movieRepository;
            _mapper = mapper;
        }


        [AllowAnonymous]
        [HttpGet]
        //respuestas
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //metodo para mostrar todas las peliculas
        public IActionResult GetMovies()
        {
            var listMovies = _movieRepository.GetMovies();
            var listMoviesDto = new List<MovieDto>();

            foreach (var movie in listMovies)
            {
                listMoviesDto.Add(_mapper.Map<MovieDto>(movie));
            }
            return Ok(listMoviesDto);
        }

        [AllowAnonymous]
        [HttpGet("{movieId:int}", Name = "GetMovie")]
        //respuestas
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        //metodo para traer categoria individual por id
        public IActionResult GetMovieById(int movieId)
        {
            var itemMovie = _movieRepository.GetMovie(movieId);

            //si el item es nulo retorna null
            if (itemMovie == null)
            {
                return NotFound();
            }
            //si no es nulo, itemMovieDto mapea a MovieDto enviandole el itemMovie

            var itemMovieDto = _mapper.Map<MovieDto>(itemMovie);
            return Ok(itemMovieDto);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        //respuestas
        [ProducesResponseType(201, Type = typeof(MovieDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //metodo para crear
        public IActionResult CreateMovie([FromBody]  MovieDto movieDto)
        {
            //validar campos requeridos
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (movieDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_movieRepository.ExistsMovie(movieDto.Name))
            {
                ModelState.AddModelError("", "The MOVIE already exist");
                return StatusCode(404, ModelState);
            }

            var movie = _mapper.Map<Movie>(movieDto);


            if (!_movieRepository.CreateMovie(movie))
            {
                ModelState.AddModelError("", $"The error {movie.Name}");
                return StatusCode(500, ModelState);
            }
            //se retorna la creacion del recurso
            return CreatedAtRoute("GetMovie", new { movieId = movie.Id }, movie);
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("{movieId:int}", Name = "UpdateMovie")]
        //respuestas
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateMovie(int movieId, [FromBody] MovieDto movieDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            

            var movie = _mapper.Map<Movie>(movieDto);

            if (!_movieRepository.UpdateMovie(movie))
            {
                ModelState.AddModelError("", $"Error update the register {movie.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }

        [Authorize(Roles = "admin")]
        //metodo para eliminar
        [HttpDelete("{movieId:int}", Name = "DeleteMovie")]
        //respuestas
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult DeleteMovie(int movieId)
        {
            //si la movie no existe
            if (!_movieRepository.ExistsMovie(movieId))
            {
                return NotFound();
            }

            var movie = _movieRepository.GetMovie(movieId);

            if (!_movieRepository.DeleteMovie(movie))
            {
                ModelState.AddModelError("", $"Error deleting the register {movie.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }

        [AllowAnonymous]
        //mostrar peliculas en categoria
        [HttpGet("GetMovieInCategory/{categoryId:int}")]
        public IActionResult GetMoviesInMovies(int categoryId)
        {
            var listMovies = _movieRepository.GetMoviesInCategories(categoryId);

            if (listMovies == null)
            {
                return NotFound();
            }

            var itemMovie = new List<MovieDto>();
            foreach (var item in listMovies)
            {
                itemMovie.Add(_mapper.Map<MovieDto>(item));
            }

            return Ok(itemMovie);
        }

        [AllowAnonymous]
        //buscar movie 
        [HttpGet("Search")]
        public IActionResult SearchMovie(string name)
        {
            try
            {
                var result = _movieRepository.searchMovie(name.Trim());
                if (result.Any())
                {
                    return Ok(result);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "error");
            }
            
           
        }
    }
}
