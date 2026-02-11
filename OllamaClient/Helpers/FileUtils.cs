using Newtonsoft.Json;
using OllamaClient.Dto;
using System.IO;

namespace OllamaClient.Helpers
{
    internal class FileUtils
    {
        private const string SETTINGS_FILE_NAME = "settings.json";

        internal static (FileCheckHelper, string?) GetFileContentBase64(string filePath)
        {
            var fileCheck = ValidateFilePath(filePath);
            if (!fileCheck.CheckResult)
            {
                return (fileCheck, null);
            }

            byte[] bytes = File.ReadAllBytes(filePath);
            string file = Convert.ToBase64String(bytes);

            return (fileCheck, file);
        }

        private static FileCheckHelper ValidateFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return new FileCheckHelper(false, "Пустое имя файла");
            }

            if (!File.Exists(filePath))
            {
                return new FileCheckHelper(false, $"Файл не найден по пути {filePath}");
            }

            return new FileCheckHelper(true, null);
        }

        private static string GetSettingsFilePath()
        {
            return $@"{AppContext.BaseDirectory}{SETTINGS_FILE_NAME}";
        }

        internal static (FileCheckHelper, SettingsDto?) ReadSettings()
        {
            string settingsFilePath = GetSettingsFilePath();

            var fileCheck = ValidateFilePath(settingsFilePath);
            if (!fileCheck.CheckResult)
            {
                return (fileCheck, null);
            }

            try
            {
                string jsonString = File.ReadAllText(settingsFilePath);

                // Deserialize the JSON string into a Product object
                SettingsDto settings = JsonConvert.DeserializeObject<SettingsDto>(jsonString);

                return (new FileCheckHelper(true, null), settings);
            }
            catch (Exception ex)
            {
                return (new FileCheckHelper(false, ex.Message), null);
            }
        }

        internal static void SaveSettings(SettingsDto settings)
        {
            string settingsFilePath = GetSettingsFilePath();

            using (StreamWriter file = File.CreateText(settingsFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                // serialize object directly into file stream
                serializer.Serialize(file, settings);
            }
        }
    }
}
