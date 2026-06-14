using DogBreedAi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

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

        [HttpGet]
        public IActionResult GetAllDogs()
        {
            return Ok(new[]
            {
               new
               {
                    Id = 1,
                    Breed = "Golden Retriever",
                    Description = "Friendly and intelligent"
                },

                new
                {
                    Id = 2,
                    Breed = "Labrador",
                    Description = "Loyal and energetic"

                }
            });


        }

        [HttpGet("{id}")]
        public IActionResult GetDog(int id)
        {
            if (id == 1)
            {
                return Ok(new
                {
                    Id = 1,
                    Breed = "Golden Retriever",
                    Description = "Friendly and intelligent"
                });
            }

            if (id == 2)
            {
                return Ok(new
                {
                    Id = 2,
                    Breed = "Labrador",
                    Description = "Loyal and energetic"
                });
            }

            return NotFound();
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
                "https://router.huggingface.co/hf-inference/models/wesleyacheng/dog-breeds-multiclass-image-classification-with-vit",
                content);

            var result = await response.Content.ReadAsStringAsync();

            return Ok(result);
        }
    }
}  
