namespace Fall2025_Project3_wchen60.Models.ViewModels
{
    public class ActorDetailsViewModel
    {
        public Actor? Actor { get; init; }
        public List<Movie?> Movies { get; init; } = [];
        public List<(string Username, string Tweet, double Sentiment)> Tweets { get; init; } = [];
        public double AverageSentiment => Tweets.Count != 0 ? Tweets.Average(t => t.Sentiment) : 0;
    }
}