using HFYStorySorter.Data;
using HFYStorySorter.Models;
using Microsoft.EntityFrameworkCore;

namespace HFYStorySorter.Services
{
    public class PostFetcherService : BackgroundService
    {
        private readonly ILogger<PostFetcherService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ServiceStatus _status;


        //https://www.reddit.com/dev/api/
        //todo make the subbreddit url configurable
        private const string SubredditUrl = "https://www.reddit.com/r/hfy/new.json?limit=50";

        public PostFetcherService(IServiceProvider serviceProvider,
                              ILogger<PostFetcherService> logger,
                              IHttpClientFactory httpClientFactory,
                              ServiceStatus status)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _status = status;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PostFetcherService running");
            _status.IsPostFetcherRunning = true;

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await FetchNewPostsAsync(stoppingToken);

                    //todo make the time configurable
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching new posts");
            }
            finally
            {
                _logger.LogInformation("PostFetcherService is stopping");
                _status.IsPostFetcherRunning = false;
            }
        }

        private async Task FetchNewPostsAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching new posts from subreddit");

            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "HFYStorySorter/1.0");

            string? after = null;
            bool keepFetching = true;
            int newPostsCount = 0;

            while (keepFetching && !cancellationToken.IsCancellationRequested)
            {
                var url = SubredditUrl + (after != null ? $"&after={after}" : "");
                var response = await httpClient.GetFromJsonAsync<RedditResponse>(url, cancellationToken);

                if (response?.Data?.Children == null || response.Data.Children.Count == 0)
                {
                    _logger.LogInformation("No new posts found in subreddit");
                    break;
                }

                foreach (var child in response.Data.Children)
                {
                    var postData = child.Data;

                    bool exists = await dbContext.Posts.AnyAsync(p => p.RedditId == postData.Id, cancellationToken);

                    //only works if subreddit sorted by new
                    if (exists)
                    {
                        _logger.LogInformation("Post {RedditId} already exists. Stopping further processing.", postData.Id);
                        keepFetching = false;
                        break;
                    }
                    var x = DateTimeOffset.FromUnixTimeSeconds((long)postData.Created_Utc).UtcDateTime;
                    Console.WriteLine(x);


                    var post = new Post
                    {
                        RedditId = postData.Id,
                        Title = postData.Title,
                        Content = postData.Selftext,
                        Author = postData.Author,
                        CreatedUtc = DateTimeOffset.FromUnixTimeSeconds((long)postData.Created_Utc).UtcDateTime,
                        IsProcessed = false
                    };

                    await dbContext.Posts.AddAsync(post, cancellationToken);
                    newPostsCount++;
                    _logger.LogInformation("New post added: {Title}", post.Title);
                }

                after = response.Data.After;
                if (string.IsNullOrEmpty(after))
                {
                    _logger.LogInformation("No more new posts found");
                    break;
                }

                await dbContext.SaveChangesAsync(cancellationToken);
            }

            if (newPostsCount > 0)
            {
                _logger.LogInformation("{Count} new posts saved to database", newPostsCount);
            }
            else
            {
                _logger.LogInformation("no new posts found");
            }
        }

        private class RedditResponse
        {
            public RedditData Data { get; set; } = new();
        }

        private class RedditData
        {
            public List<RedditChild> Children { get; set; } = new();
            public string? After { get; set; }
        }
        private class RedditChild
        {
            public RedditPostData Data { get; set; } = new();
        }


        //todo add author
        private class RedditPostData
        {
            public string Id { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Selftext { get; set; } = string.Empty;
            public string Author { get; set; } = string.Empty;
            public double Created_Utc { get; set; }
        }
    }
}
