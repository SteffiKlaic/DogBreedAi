using Microsoft.AspNetCore.Mvc;

namespace DogBreedAi.Controllers
{
    [ApiController]
    [Route("api/dogs")]
    public class DogController : Controller
    {
        [HttpGet]
        public IActionResult Test()
        {
            return Ok(new
            {
                Breed = "Golden Retriever",
                Description = "Friendly and intelligent"
            });
        }
    }
}
