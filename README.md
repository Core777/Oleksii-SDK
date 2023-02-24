For install sdk - just find it in to NuGet and click Install.

Way of using is pretty simple, because it is an library with public class, so just create one and call needed methods.
Also, most of models are public, so they can be used.

Example of using:

var sdk = new SDK.MovieService();
var all = await sdk.GetAll(1, 200, 1);  //here you go, all movies collection are called

Also each method can take an a user specific token as parameter, just check method comments.
Project contains unitTests, so they can be runned from cli or dirrectly from IDE. Based on nUnit framework.