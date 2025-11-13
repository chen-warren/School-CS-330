namespace Fall2025_Project3_wchen60.Models.ViewModels
{
    public class MovieDetailsViewModel
    {
        public Movie? Movie { get; init; }
        public List<Actor?> Actors { get; init; } = [];
        public List<(string Review, double Sentiment)> Reviews { get; init; } = [];
        public double AverageSentiment => Reviews.Any() ? Reviews.Average(r => r.Sentiment) : 0;
    }
}