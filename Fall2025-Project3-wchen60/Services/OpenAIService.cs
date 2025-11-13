using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Text.Json;
using System.Text.Json.Nodes;
using VaderSharp2;

namespace Fall2025_Project3_wchen60.Services
{
    public class OpenAiService
    {
        private readonly ChatClient _chatClient;

        public OpenAiService(IConfiguration config)
        {
            var endpoint = new Uri(config["OpenAI:Endpoint"]!);
            var key = new ApiKeyCredential(config["OpenAI:ApiKey"]!);
            var deployment = config["OpenAI:Deployment"] ?? "gpt-4.1-mini";

            var client = new AzureOpenAIClient(endpoint, key);
            _chatClient = client.GetChatClient(deployment);
        }

        public async Task<List<(string Review, double Sentiment)>> GetMovieReviewsAsync(string title, int year, string director)
        {
            var messages = new ChatMessage[]
            {
                new SystemChatMessage($"You represent a panel of 10 film critics with unique personalities. " +
                                      $"Respond with a JSON object containing an array of reviews. Each review should be 100 words or less."),
                new UserChatMessage($"Generate 10 reviews about '{title}' released in {year}, directed by {director}. " +
                                   $"Return a JSON object with this exact structure: {{\"reviews\": [\"review1\", \"review2\", ...]}}")
            };


            var result = await _chatClient.CompleteChatAsync(messages);
            string json = result.Value.Content[0].Text;

            var analyzer = new SentimentIntensityAnalyzer();
            var output = new List<(string, double)>();

            try
            {
                var jsonDoc = JsonDocument.Parse(json);
                if (jsonDoc.RootElement.TryGetProperty("reviews", out var reviewsArray))
                {
                    foreach (var reviewElement in reviewsArray.EnumerateArray())
                    {
                        string review = reviewElement.GetString() ?? "";
                        if (!string.IsNullOrWhiteSpace(review))
                        {
                            var sentiment = analyzer.PolarityScores(review).Compound;
                            output.Add((review, sentiment));
                        }
                    }
                }
            }
            catch (JsonException)
            {
                try
                {
                    var array = JsonNode.Parse(json)?.AsArray();
                    if (array != null)
                    {
                        foreach (var reviewNode in array)
                        {
                            string review = reviewNode?.ToString() ?? "";
                            if (!string.IsNullOrWhiteSpace(review))
                            {
                                var sentiment = analyzer.PolarityScores(review).Compound;
                                output.Add((review, sentiment));
                            }
                        }
                    }
                }
                catch{
                    Console.WriteLine("Error parsing JSON");
                }
            }

            return output;
        }

        public async Task<List<(string Username, string Tweet, double Sentiment)>> GetActorTweetsAsync(string actorName)
        {
            var messages = new ChatMessage[]
            {
                new SystemChatMessage($"You represent the X/Twitter social media API. Respond with a JSON array of objects. " +
                                     $"Each object must have exactly two fields: 'username' (string) and 'tweet' (string)."),
                new UserChatMessage($"Generate 20 tweets about actor {actorName} from a variety of fictional users. " +
                                   $"Return a JSON array with this exact structure: [{{\"username\": \"user1\", \"tweet\": \"tweet text\"}}, ...]")
            };

            var result = await _chatClient.CompleteChatAsync(messages);
            string json = result.Value.Content.FirstOrDefault()?.Text ?? "[]";

            var analyzer = new SentimentIntensityAnalyzer();
            var output = new List<(string, string, double)>();

            try
            {
                JsonArray? array = JsonNode.Parse(json)?.AsArray();
                if (array != null)
                {
                    foreach (var t in array)
                    {
                        string username = t?["username"]?.ToString() ?? "unknown";
                        string text = t?["tweet"]?.ToString() ?? "";
                        
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            double sentiment = analyzer.PolarityScores(text).Compound;
                            output.Add((username, text, sentiment));
                        }
                    }
                }
            }
            catch (JsonException){ 
                Console.WriteLine("Error parsing JSON");
            }

            return output;
        }
    }
}
