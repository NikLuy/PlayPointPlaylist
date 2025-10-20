using Google.Apis.Services;
using Google.Apis.YouTube.v3;

namespace PlayPointPlaylist.Services;

public class YouTubeApiService
{
    private readonly YouTubeService _youtubeService;
    
    public YouTubeApiService(IConfiguration configuration)
    {
        var apiKey = configuration["YouTubeApi:ApiKey"];
        _youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            ApiKey = apiKey,
            ApplicationName = "PlayPointPlaylist"
        });
    }
    
    public async Task<List<VideoSearchResult>> SearchVideos(string query, int maxResults = 10)
    {
        var searchRequest = _youtubeService.Search.List("snippet");
        searchRequest.Q = query;
        searchRequest.MaxResults = maxResults;
        searchRequest.Type = "video";
        
        var searchResponse = await searchRequest.ExecuteAsync();
        var results = new List<VideoSearchResult>();
        
        // Get video IDs to fetch duration details
        var videoIds = string.Join(",", searchResponse.Items.Select(i => i.Id.VideoId));
        
        if (!string.IsNullOrEmpty(videoIds))
        {
            var videoRequest = _youtubeService.Videos.List("contentDetails,snippet");
            videoRequest.Id = videoIds;
            var videoResponse = await videoRequest.ExecuteAsync();
            
            foreach (var video in videoResponse.Items)
            {
                results.Add(new VideoSearchResult
                {
                    VideoId = video.Id,
                    Title = video.Snippet.Title,
                    Artist = video.Snippet.ChannelTitle,
                    DurationSeconds = ParseDuration(video.ContentDetails.Duration)
                });
            }
        }
        
        return results;
    }
    
    public async Task<VideoSearchResult?> GetVideoDetails(string videoId)
    {
        try
        {
            var videoRequest = _youtubeService.Videos.List("contentDetails,snippet");
            videoRequest.Id = videoId;
            var videoResponse = await videoRequest.ExecuteAsync();
            
            var video = videoResponse.Items.FirstOrDefault();
            if (video == null)
                return null;
            
            return new VideoSearchResult
            {
                VideoId = video.Id,
                Title = video.Snippet.Title,
                Artist = video.Snippet.ChannelTitle,
                DurationSeconds = ParseDuration(video.ContentDetails.Duration)
            };
        }
        catch
        {
            return null;
        }
    }
    
    private int ParseDuration(string isoDuration)
    {
        // Parse ISO 8601 duration (e.g., PT4M13S)
        var duration = System.Xml.XmlConvert.ToTimeSpan(isoDuration);
        return (int)duration.TotalSeconds;
    }
}

public class VideoSearchResult
{
    public string VideoId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
}
