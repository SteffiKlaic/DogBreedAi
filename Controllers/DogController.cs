using DogBreedAi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace DogBreedAi.Controllers
{
    [ApiController]
    [Route("api/dogs")]
    public class DogController : Controller
    {
        private readonly IConfiguration _configuration;

        public DogController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var token = _configuration["HuggingFace:ApiToken"];

            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    token);

            using var stream = file.OpenReadStream();
            using var content = new StreamContent(stream);

            content.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            var response = await httpClient.PostAsync(
                "https://router.huggingface.co/hf-inference/models/google/vit-base-patch16-224",
                content);

            var result = await response.Content.ReadAsStringAsync();

            var description = await GenerateDescription("malinois");
            
            return Ok(new
            {
                Description = description
            });
        }
        public async Task<string> GenerateDescription(string breed)
        {
            var token = _configuration["HuggingFace:ApiToken"];

            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    token);

            var prompt = $"Describe the characteristics of a {breed} dog breed, with max 4 sentences.";

            var requestBody = new
            {
                messages = new[]
              {
            new
            {
                role = "user",
                content = prompt
            }
        },
                model = "mistralai/Mistral-7B-Instruct-v0.2:featherless-ai",
                stream = false
            };

            var json =
                JsonSerializer.Serialize(requestBody);

            using var content =
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");


            var response = await httpClient.PostAsync(
                "https://router.huggingface.co/v1/chat/completions", content);

            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);

            return result;
        }
    }
}
