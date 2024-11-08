using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Flow.Launcher.Plugin.ShortlinkPlugin;

public class ShortlinkPlugin : IAsyncPlugin
{
    private PluginInitContext _context;
    private readonly Settings _settings;

    public ShortlinkPlugin()
    {
        var settingsManager = new SettingsManager();
        _settings = settingsManager.LoadSettings();
    }


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
            result = new Result { Title = "enter a url", IcoPath = "icon.png" };
        else
            result = await ShortenUrl(input);

        return new List<Result> { result };
    }

    public async Task<Result> ShortenUrl(string longUrl)
    {
        HttpClient Client = new HttpClient();

        // Set Default Headers Token
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _settings.ApiToken);

        string apiUrl = $"https://api.syntafin.de/v1/links/";

        // Create JSON-Content and set longUrl as param
        var content = new StringContent($"{{\"target\": \"{longUrl}\"}}", System.Text.Encoding.UTF8, "application/json");

        HttpResponseMessage response = await Client.PostAsync(apiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            string shortenedUrl = await response.Content.ReadAsStringAsync();
            return new Result
            {
                Title = "Copy to Clipboard",
                SubTitle = shortenedUrl,
                Action = copy =>
                {
                    _context.API.CopyToClipboard(shortenedUrl);
                    return true;
                },
                IcoPath = "icon.png"
            };
        }
        else if (longUrl == string.Empty)
            return new Result { Title = "enter a url", IcoPath = "icon.png" };
        else
            return new Result
            {
                Title = "Invalid URL",
                SubTitle = $"Failed to shorten URL. Status code: {response.StatusCode}",
                IcoPath = "icon.png"
            };
    }
}