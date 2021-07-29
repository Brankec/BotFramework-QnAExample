using System.Text.Json;
using Tensorflow;

namespace BotFramework_QnAExample.Utils.Settings
{
    public static class SettingsManager
    {
        private static SettingsModel _settings;
        
        public static void LoadSettings()
        {
            var settingsFileText = ResourceFiles.GetFileText("appsettings.json");
            _settings = JsonSerializer.Deserialize<SettingsModel>(settingsFileText);
        }

        public static SettingsModel GetSettings()
        {
            return _settings;
        }
    }
}