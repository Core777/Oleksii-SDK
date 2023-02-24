// See https://aka.ms/new-console-template for more information

Console.WriteLine("Hello, World!");
var sdk = new SDK.MovieService();
var all = await sdk.GetAll(1, 200, 1);
var byId = await sdk.GetById(all.Docs[0].Id);
var quotes = await sdk.GetQuote(all.Docs[5].Id, 2, 300, 500);
var random = await sdk.GetRandomMovie();
var score = await sdk.DoesMovieScoreHigherThen(all.Docs[4].Id, 70.72);

Console.WriteLine(random.Docs.First().Name);
Console.WriteLine(quotes.Message);
Console.WriteLine(score.Message);

Console.WriteLine("END");