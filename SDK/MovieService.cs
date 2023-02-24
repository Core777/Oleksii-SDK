using SDK.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SDK
{
    public class MovieService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private const string DefaultToken = "We5aU3D7jr335YNADt4e";
        private const string AllMovies = "/v2/movie";
        private const string BaseUrl = "https://the-one-api.dev";

        /// <summary>
        /// Get all existen movies from the-one-api, with pagination.
        /// </summary>
        /// <param name="page"> current page (1 if not setted) </param>
        /// <param name="limit"> items per page limit (0 if not setted) </param>
        /// <param name="offset"> items offset (0 if not setted) </param>
        /// <param name="userToken"> the-one-api AccessToken can be setted manually </param>
        /// <returns>SDKResponse with movies list and some request info in it</returns>
        public async Task<SDKResponse<Movie>> GetAll(int? page = null, int? limit = null, int? offset = null, string? userToken = null)
        {
            SDKResponse<Movie> movies = new SDKResponse<Movie>();

            try
            {
                movies = await GetTyped<SDKResponse<Movie>>(AllMovies, userToken, page, limit, offset);
            }
            catch (Exception ex)
            {
                movies.Message = ex.Message;
                movies.Success = false;
                return movies;
            }

            return movies;
        }

        /// <summary>
        /// Get movie by sended identifier.
        /// </summary>
        /// <param name="id"> movie unique identifier</param>
        /// <param name="userToken"> the-one-api AccessToken can be setted manually </param>
        /// <returns>SDKResponse with movie and some request info in it</returns>
        public async Task<SDKResponse<Movie>> GetById(string id, string? userToken = null)
        {
            SDKResponse<Movie> movie = new SDKResponse<Movie>();

            try
            {
                movie = await GetTyped<SDKResponse<Movie>>($"{AllMovies}/{id}", userToken);
            }
            catch (Exception ex)
            {
                movie.Message = ex.Message;
                movie.Success = false;
                return movie;
            }

            if (movie.Docs.Count == 0)
            {
                movie.Message = $"There is no movie with id {id}";
                movie.Success = false;
                return movie;
            }

            return movie;
        }

        /// <summary>
        /// Get quote from choosen movie by identifier, with pagination. Pay attention - working only for the LotR trilogy!
        /// </summary>
        /// <param name="id"> movie unique identifier</param>
        /// <param name="page"> current page (1 if not setted) </param>
        /// <param name="limit"> items per page limit (0 if not setted) </param>
        /// <param name="offset"> items offset (0 if not setted) </param>
        /// <param name="userToken"> the-one-api AccessToken can be setted manually </param>
        /// <returns>SDKResponse with quotes list and some request info in it</returns>
        public async Task<SDKResponse<Quote>> GetQuote(string id, int? page = null, int? limit = null, int? offset = null, string? userToken = null)
        {
            SDKResponse<Quote> quotes = new SDKResponse<Quote>();

            try
            {
                quotes = await GetTyped<SDKResponse<Quote>>($"{AllMovies}/{id}/quote", userToken, page, limit, offset);
            }
            catch (Exception ex)
            {
                quotes.Message = ex.Message;
                quotes.Success = false;
                return quotes;
            }

            if (quotes.Docs.Count == 0)
            {
                quotes.Message = "There is no quotes for requested movie. Check, that movie is from the LOTR trilogy!";
                quotes.Success = false;
                return quotes;
            }

            return quotes;
        }

        /// <summary>
        /// Get one random movie from list of all.
        /// </summary>
        /// <param name="userToken"> the-one-api AccessToken can be setted manually </param>
        /// <returns>SDKResponse with one random movie in list</returns>
        public async Task<SDKResponse<Movie>> GetRandomMovie(string? userToken = null)
        {
            SDKResponse<Movie> movies = new SDKResponse<Movie>();

            try
            {
                movies = await GetTyped<SDKResponse<Movie>>($"{AllMovies}", userToken);
            }
            catch (Exception ex)
            {
                movies.Message = ex.Message;
                movies.Success = false;
                return movies;
            }

            if (movies.Docs.Count == 0)
            {
                movies.Message = "Sorry, but movies list is empty for some reason(";
                movies.Success = false;
                return movies;
            }

            Random rnd = new Random();
            movies.Docs = new List<Movie> { movies.Docs[rnd.Next(movies.Docs.Count)] };

            return movies;
        }

        /// <summary>
        /// Check does sended movie score higher then you think)
        /// </summary>
        /// <param name="id"> movie unique identifier</param>
        /// <param name="score"> number of score, you think, higher then rooten tomatoes score  </param>
        /// <param name="userToken"> the-one-api AccessToken can be setted manually </param>
        /// <returns>SDKResponse with movie and answer in Message prop</returns>
        public async Task<SDKResponse<Movie>> DoesMovieScoreHigherThen(string id, double score, string? userToken = null)
        {
            SDKResponse<Movie> movie = new SDKResponse<Movie>();

            try
            {
                movie = await GetTyped<SDKResponse<Movie>>($"{AllMovies}/{id}", userToken);
            }
            catch (Exception ex)
            {
                movie.Message = ex.Message;
                movie.Success = false;
                return movie;
            }

            if (movie.Docs.Count == 0)
            {
                movie.Message = $"There is no movie with id {id}";
                movie.Success = false;
                return movie;
            }

            var movieModel = movie.Docs.First();

            if (movieModel.RottenTomatoesScore > score)
                movie.Message = "You are right! Rotten tomatoes score is higher!";
            else if (movieModel.RottenTomatoesScore < score)
                movie.Message = "Sorry, but it is not(";
            else
                movie.Message = "Wow! There are equal!";

            return movie;
        }

        #region PRIVATE
        private async Task<T> GetTyped<T>(string path, string? userToken = null, int? page = null, int? limit = null, int? offset = null)
        {
            if (page != null)
                path += $"?page={page}";

            if (limit != null)
                path += path.Contains('?') ? $"&limit={limit}" : $"?limit={limit}";

            if (offset != null)
                path += path.Contains('?') ? $"&offset={offset}" : $"?offset={offset}";

            var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}{path}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", string.IsNullOrEmpty(userToken) ? DefaultToken : userToken);
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"API call failed! Problem is: {response.StatusCode}");

            var result = await response.Content.ReadFromJsonAsync<T>();

            if (result == null)
                throw new Exception("Sorry, but have some null, where it shouldn't be! Please try again");

            return result;
        }
        #endregion

    }
}
