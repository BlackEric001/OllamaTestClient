using System.IO;

namespace OllamaClient.Helpers
{
    internal class FileUtils
    {
        public static (bool, string) GetFileContentBase64(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return (false,"Пустое имя файла");
            }

            if (!File.Exists(filePath))
            {
                return (false, $"Файл не найден по пути {filePath}");
            }

            byte[] bytes = File.ReadAllBytes(filePath);
            string file = Convert.ToBase64String(bytes);

            return (true, file);
        }
    }
}
