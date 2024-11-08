using System.IO;
using Newtonsoft.Json;

namespace Flow.Launcher.Plugin.ShortlinkPlugin;
public class SettingsManager
{
    private const string SettingsFilePath = "settings.json";

    public Settings LoadSettings()
    {
        if (!File.Exists(SettingsFilePath))
        {
            return new Settings();
        }

        string json = File.ReadAllText(SettingsFilePath);
        return JsonConvert.DeserializeObject<Settings>(json);
    }

    public void SaveSettings(Settings settings)
    {
        string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
        File.WriteAllText(SettingsFilePath, json);
    }
}
