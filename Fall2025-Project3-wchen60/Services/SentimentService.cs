using VaderSharp2;

namespace Fall2025_Project3_wchen60.Services
{
    public class SentimentService
    {
        private readonly SentimentIntensityAnalyzer _analyzer = new();

        public double GetAverageSentiment(IEnumerable<string> texts)
        {
            var scores = texts.Select(t => _analyzer.PolarityScores(t).Compound);
            return scores.Any() ? scores.Average() : 0;
        }
    }
}