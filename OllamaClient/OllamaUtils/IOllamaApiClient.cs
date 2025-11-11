using OllamaClient.Dto;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OllamaClient.OllamaUtils
{
    
    interface IOllamaApiClient
    {
        /// <summary>
        /// http://localhost:11434/api/tags
        /// </summary>
        /// <returns></returns>
        [Get("/tags")]
        Task<string> GetLocalModelsAsync();

        /// <summary>
        /// http://localhost:11434/api/generate
        /// </summary>
        /// <returns></returns>
        [Post("/generate")]
        Task<string> GenerateAsync([Body]PromptDto prompt);
    }
}
