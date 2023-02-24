
using System.Text.Json.Serialization;

namespace SDK.Models
{
    public class Movie
    {
        [JsonPropertyName("_id")]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public int RuntimeInMinutes { get; set; }
        public double BudgetInMillions { get; set; }
        public double BoxOfficeRevenueInMillions { get; set; }
        public int AcademyAwardNominations { get; set; }
        public int AcademyAwardWins { get; set; }
        public double RottenTomatoesScore { get; set; }
    }
}
