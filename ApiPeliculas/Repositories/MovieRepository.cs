using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Repository
{
    public class MovieRepository : IMovieRepository

    {
        private readonly ApplicationDbContext _bd;

        public MovieRepository(ApplicationDbContext bd) 
        {
            _bd = bd;
        }
        public bool CreateMovie(Movie movie)
        {
            movie.DateCreation = DateTime.Now;
            _bd.Movie.Add(movie);
            return SaveMovie();
        }

        public bool DeleteMovie(Movie movie)
        {
            _bd.Movie.Remove(movie);
            return SaveMovie();
        }

        public bool ExistsMovie(string name)
        {
            bool value = _bd.Movie.Any(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
            return value;
        }

        public bool ExistsMovie(int id)
        {
            return _bd.Movie.Any(c => c.Id == id);
        }

        public Movie GetMovie(int movieId)
        {
            return _bd.Movie.FirstOrDefault(c => c.Id == movieId);
        }

        public ICollection<Movie> GetMovies()
        {
            //ordena por el nombre y crea una lista
            return _bd.Movie.OrderBy(c => c.Name).ToList();
        }

        public ICollection<Movie> GetMoviesInCategories(int categoryId)
        {
            return _bd.Movie.Include(ca => ca.Category).Where(ca => ca.categoryId == categoryId).ToList();
        }

        public bool SaveMovie()
        {
            return _bd.SaveChanges() >= 0 ? true : false;
        }

        public ICollection<Movie> searchMovie(string name)
        {
            //realizar consultas sobre una entidad
            IQueryable<Movie> query = _bd.Movie;
            if (!string.IsNullOrEmpty(name)) 
            {
                query = query.Where(e => e.Name.Contains(name) || e.Description.Contains(name));
            }
            return query.ToList();
        }

        public bool UpdateMovie(Movie movie)
        {
            movie.DateCreation = DateTime.Now;
            _bd.Movie.Update(movie);
            return SaveMovie();
        }
    }
}
