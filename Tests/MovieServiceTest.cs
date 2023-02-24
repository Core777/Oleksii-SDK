using SDK;

namespace Tests
{
    [TestFixture]
    public class MovieServiceTest
    {
        private readonly MovieService _movieService = new MovieService();

        #region GetAll
        [Test]
        public async Task GetAll_FailedAuthKey_Failed()
        {
            var movies = await _movieService.GetAll(null, null, null, "someTestUserToken");

            Assert.IsFalse(movies.Success);
            Assert.IsTrue(movies.Message?.Contains("Unauthorized"));
        }

        [Test]
        public async Task GetAll_DefaultKeyWithOutParameters_Success()
        {
            var movies = await _movieService.GetAll();

            Assert.IsTrue(movies.Success);
            Assert.IsNotEmpty(movies.Docs);
            Assert.IsTrue(movies.Docs.Count > 0);
        }

        //When offset setted, page doesn't use, so we would have 0 page each time we set some offset
        [Test]
        [TestCaseSource(nameof(PaginationCases))]
        public async Task GetAll_DefaultKeyWithParametersWitOffset_Success(int? page, int? limit, int? offset, int? expPage, int? expLimit, int? expOffset)
        {
            var movies = await _movieService.GetAll(page, limit, offset);

            Assert.IsTrue(movies.Success);
            Assert.IsNotEmpty(movies.Docs);
            Assert.IsTrue(movies.Docs.Count <= expLimit);
            Assert.IsTrue(movies.Page == expPage);
            Assert.IsTrue(movies.Limit == expLimit);
            Assert.IsTrue(movies.Offset == expOffset);
        }
        #endregion

        #region GetById
        [Test]
        public async Task GetById_FailedAuthKey_Failed()
        {
            var movies = await _movieService.GetById("some id", "someTestUserToken");

            Assert.IsFalse(movies.Success);
            Assert.IsTrue(movies.Message?.Contains("Unauthorized"));
        }

        [Test]
        public async Task GetById_NotExistenId_FailedMessage()
        {
            var movies = await _movieService.GetById("NotExistenId");

            Assert.IsFalse(movies.Success);
            Assert.IsTrue(movies.Message?.Contains("There is no movie with id"));
        }

        [Test]
        public async Task GetById_Success_OnlyOneItem()
        {
            var movies = await _movieService.GetAll();
            var movie = await _movieService.GetById(movies.Docs.First().Id);

            Assert.IsTrue(movie.Success);
            Assert.IsTrue(movie.Docs.Count == 1);
        }
        #endregion

        #region GetQuote
        [Test]
        public async Task GetQuote_FailedAuthKey_Failed()
        {
            var movies = await _movieService.GetQuote("some id", null, null, null, "someTestUserToken");

            Assert.IsFalse(movies.Success);
            Assert.IsTrue(movies.Message?.Contains("Unauthorized"));
        }

        [Test]
        public async Task GetQuote_NotLOTRMovie_Failed()
        {
            var movies = await _movieService.GetAll();
            var quotes = await _movieService.GetQuote(movies.Docs.First().Id, null, null, null);

            Assert.IsFalse(quotes.Success);
            Assert.IsTrue(quotes.Message?.Contains("There is no quotes for requested movie. Check, that movie is from the LOTR trilogy!"));
        }

        [Test]
        [TestCaseSource(nameof(PaginationCases))]
        public async Task GetQuote_LOTRMovie_Success(int? page, int? limit, int? offset, int? expPage, int? expLimit, int? expOffset)
        {
            var movies = await _movieService.GetAll();
            var quotes = await _movieService.GetQuote(movies.Docs.First(x => x.Name == "The Two Towers").Id, page, limit, offset);

            Assert.IsTrue(quotes.Success);
            Assert.IsNotEmpty(quotes.Docs);
            Assert.IsTrue(quotes.Docs.Count <= expLimit);
            Assert.IsTrue(quotes.Page == expPage);
            Assert.IsTrue(quotes.Limit == expLimit);
            Assert.IsTrue(quotes.Offset == expOffset);
        }
        #endregion

        #region GetRandomMovie
        [Test]
        public async Task GetRandomMovie_FailedAuthKey_Failed()
        {
            var movies = await _movieService.GetRandomMovie("someTestUserToken");

            Assert.IsFalse(movies.Success);
            Assert.IsTrue(movies.Message?.Contains("Unauthorized"));
        }       
        
        [Test]
        public async Task GetRandomMovie_HaveSome_OneRandomMovie()
        {
            var movies = await _movieService.GetRandomMovie();

            Assert.IsTrue(movies.Success);
            Assert.IsTrue(movies.Docs.Count == 1);
        }
        #endregion

        #region DoesMovieScoreHigherThen
        [Test]
        public async Task DoesMovieScoreHigherThen_FailedAuthKey_Failed()
        {
            var movies = await _movieService.DoesMovieScoreHigherThen("someID", 3, "someTestUserToken");

            Assert.IsFalse(movies.Success);
            Assert.IsTrue(movies.Message?.Contains("Unauthorized"));
        }     
        
        [Test]
        public async Task DoesMovieScoreHigherThen_WrongId_Failed()
        {
            var movies = await _movieService.DoesMovieScoreHigherThen("NotExistenId", 3);

            Assert.IsFalse(movies.Success);
            Assert.IsTrue(movies.Message?.Contains("There is no movie with id"));
        }       
        
        [Test]
        public async Task DoesMovieScoreHigherThen_ReallyHighButValid_Success()
        {
            var movies = await _movieService.GetAll();
            var movieScore = await _movieService.DoesMovieScoreHigherThen(movies.Docs.First().Id, double.MaxValue);

            Assert.IsTrue(movieScore.Success);
            Assert.IsTrue(movieScore.Message?.Contains("Sorry, but it is not("));
        }
        #endregion

        static object[] PaginationCases = {
            new object[] {null, 5, 3, 0, 5, 3 },
            new object[] {82, 5, 3, 0, 5, 3},
            new object[] {2, 5, null, 2, 5, 0 },
            new object[] { null, null, null, 1, 1000, 0}
        };
    }
}