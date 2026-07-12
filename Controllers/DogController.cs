using DogBreedAi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Reflection.Emit;
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

            Console.WriteLine(result);
            Console.WriteLine(file.ContentType);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(result);
            }

            using var imageDocument = JsonDocument.Parse(result);

            if (imageDocument.RootElement.ValueKind != JsonValueKind.Array)
            {
                return BadRequest("The image model did not return a prediction array.");
            }

            var breed = imageDocument.RootElement[0]
                .GetProperty("label")
                .GetString();

            var score = imageDocument.RootElement[0]
                .GetProperty("score")
                .GetDouble();

            var description = await GenerateDescription(breed);

            return Ok(new
            {
                Breed = breed,
                Confidence = score,
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
                model = "zai-org/GLM-5.2:together:fastest",
                stream = false
            };

            var json =
                JsonSerializer.Serialize(requestBody);

            using var content =
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");

            Console.WriteLine(json);
            var response = await httpClient.PostAsync(
                "https://router.huggingface.co/v1/chat/completions", content);

            var result = await response.Content.ReadAsStringAsync();


            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine((int)response.StatusCode);
                Console.WriteLine(result);
                return "The description service is currently unavailable.";
             
            }


            using var document = JsonDocument.Parse(result);

            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                return "The model did not return a valid response.";
            }

            if (!document.RootElement.TryGetProperty("choices", out var choices))
            {
                return "The model did not return a valid response.";
            }


            var description =
                choices[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

            return description;
        }
    }
}
