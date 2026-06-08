using DogBreedAi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DogBreedAi.Controllers
{
    [ApiController]
    [Route("api/dogs")]
    public class DogController : Controller
    {
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

        [HttpPost]
        public IActionResult CreateDog([FromBody] Dog dog)
        {
            return Ok(dog);
        }
    }
}  
