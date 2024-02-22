using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Flow.Launcher.Plugin.TinyUrlPlugin;

public class TinyUrlPlugin : IAsyncPlugin
{
    private PluginInitContext _context;

    public async Task InitAsync(PluginInitContext context)
    {
        _context = context;
        await Task.CompletedTask;
    }

    public async Task<List<Result>> QueryAsync(Query query, CancellationToken token)
    {
        var input = query.Search;
        Result result;
        if (input == string.Empty)
            result = new Result { Title = "enter a url", IcoPath = "icons/icon.png" };
        else
            result = await ShortenUrl(input);

        return new List<Result> { result };
    }

    public async Task<Result> ShortenUrl(string longUrl)
    {
        HttpClient Client = new HttpClient();

        string apiUrl = $"https://tinyurl.com/api-create.php?url={longUrl}";
        HttpResponseMessage response = await Client.GetAsync(apiUrl);

        if (response.IsSuccessStatusCode)
        {
            string shortenedUrl = await response.Content.ReadAsStringAsync();
            return new Result
            {
                Title = "copy to clipboard",
                SubTitle = shortenedUrl,
                Action = copy =>
                {
                    _context.API.CopyToClipboard(shortenedUrl);
                    return true;
                },
                IcoPath = "icons/icon.png"
            };
        }
        else if (longUrl == string.Empty)
            return new Result { Title = "enter a url", IcoPath = "icons/icon.png" };
        else
            return new Result
            {
                Title = "invalid url",
                SubTitle = $"Failed to shorten URL. Status code: {response.StatusCode}",
                IcoPath = "icons/icon.png"
            };
    }
}