using Newtonsoft.Json;
using OllamaClient.Dto;
using OllamaClient.Helpers;
using Refit;
using System.Net.Http;

namespace OllamaClient.OllamaUtils
{
    /// <summary>
    /// https://github.com/ollama/ollama/blob/main/docs/api.md
    /// </summary>
    static class OllamaApiClient
    {
        /// <summary>
        /// http://localhost:11434/api/tags
        /// </summary>
        /// <returns></returns>
        internal static async Task<string> GetLocalModelsListAsync()
        {
            IOllamaApiClient client = GetOllamaClient();
            var result = await client.GetLocalModelsAsync();

            return FormatJson(result);
        }

        private static IOllamaApiClient GetOllamaClient()
        {
            string baseAddress = "http://localhost:11434/api";
            var httpClient = new HttpClient(new HttpLoggingHandler()) { BaseAddress = new Uri(baseAddress), Timeout = TimeSpan.FromMinutes(40) };
            var client = RestService.For<IOllamaApiClient>(httpClient);

            return client;
        }

        private static string FormatJson(string unformattedJson)
        {
            object? jsonObject = JsonConvert.DeserializeObject(unformattedJson);
            string formattedJson = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
            return formattedJson;
        }

        /// <summary>
        /// Сделать асинхру что бы прога не висела ожидая ответа
        /// </summary>
        /// <param name="model"></param>
        /// <param name="prompt"></param>
        /// <returns></returns>
        internal static async Task<BaseResultDto> SendPromptAsync(string model, string prompt, string[]? images)
        {
            if (string.IsNullOrEmpty(model))
                return new BaseResultDto(false, "Не выбрана модель. Запрос не будет отправлен.");

            if (string.IsNullOrEmpty(prompt))
                return new BaseResultDto(false, "Промт пустой. Запрос не будет отправлен.");

            try
            {
                IOllamaApiClient client = GetOllamaClient();
                var result = await client.GenerateAsync(new PromptDto(model, prompt, false, images, "json", new OptionsDto { Temperature = 0.0}));
                return new BaseResultDto(true, FormatJson(result));
            }
            catch (Exception ex)
            {
                return new BaseResultDto(false, $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
