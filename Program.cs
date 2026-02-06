using System.Text;

var playbackUrl = GetPlaybackStreamUrl();
Console.WriteLine($"Playback URL: {playbackUrl}");
string GetPlaybackStreamUrl()
{
    var baseUrl = $"rtsp://10.1.2.151:1935";
    var contentPath = $"live/VC02.stream";
    var queryParamPrefix = "wowzatoken";
    var playbackClientIP = "10.1.2.151";
    var startTime = new DateTimeOffset(DateTime.Now.AddSeconds(5)).ToUnixTimeSeconds();
    var endTime = new DateTimeOffset(DateTime.Now.AddSeconds(120)).ToUnixTimeSeconds();
    var customParams = $"VC01";
    var sharedSecret = "7757dac1e7754623";

    var startTimeQuery = $"{queryParamPrefix}starttime={startTime}";
    var endTimeQuery = $"{queryParamPrefix}endtime={endTime}";
    var customParamQuery = $"{queryParamPrefix}CustomParameter={customParams}";

    // Sort parameters in alphanumeric order for hashing
    var paramsToSort = new[] { playbackClientIP, sharedSecret, customParamQuery, endTimeQuery, startTimeQuery };
    Array.Sort(paramsToSort);
    var sortedParams = string.Join("&", paramsToSort);
    var paramToHash = $"{contentPath}?{sortedParams}";

    Console.WriteLine($"Param to hash: {paramToHash}");
    var hash = ComputeSha256Hash(paramToHash);
    var hashQuery = $"{queryParamPrefix}hash={hash}";


    var streamUrl = $"{baseUrl}/{contentPath}?{startTimeQuery}&{endTimeQuery}&{customParamQuery}&{hashQuery}";
    return streamUrl;
}

static string ComputeSha256Hash(string queryParams)
{
    // Create a SHA256   
    var bytes = Encoding.UTF8.GetBytes(queryParams);
    using (var sha256 = System.Security.Cryptography.SHA256.Create())
    {
        var hash = sha256.ComputeHash(bytes);
        var token = Convert.ToBase64String(hash);
        token = token.Replace('+', '-');
        token = token.Replace('/', '_');
        return token;
    }
}