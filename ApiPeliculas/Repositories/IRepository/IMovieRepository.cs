using ApiPeliculas.Models;

namespace ApiPeliculas.Repository.IRepository
{
    public interface IMovieRepository
    {
        ICollection<Movie> GetMovies();

        Movie GetMovie(int movieId);
        bool ExistsMovie(string name);
        bool ExistsMovie(int id);
        bool CreateMovie(Movie movie);
        bool UpdateMovie(Movie movie);
        bool DeleteMovie(Movie movie);


        //metodos para buscar peliculas en categoria y buscar peli por nombre
        ICollection<Movie> GetMoviesInCategories(int categoryId);
        ICollection<Movie> searchMovie(string name);

        bool SaveMovie();


    }
}
