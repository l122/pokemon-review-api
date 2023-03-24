using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IMapper mapper;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        public IActionResult GetCategories()
        {
            var result = this.mapper.Map<List<CategoryDto>>(this.categoryRepository.GetAll());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
    
            return Ok(result);
        }

        [HttpGet("{categoryId}")]
        [ProducesResponseType(200, Type = typeof(Category))]
        [ProducesResponseType(400)]
        public IActionResult GetCategory(int categoryId)
        {
            if (!this.categoryRepository.Exists(categoryId))
            {
                return NotFound();
            }

            var result = this.mapper.Map<CategoryDto>(this.categoryRepository.Get(categoryId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(result);
        }

        [HttpGet("{categoryId}/pokemon")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonByCategory(int categoryId)
        {
            if (!this.categoryRepository.Exists(categoryId))
            {
                return NotFound();
            }

            var result = this.mapper.Map<List<PokemonDto>>(this.categoryRepository.GetPokemonByCategory(categoryId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCategory([FromBody] CategoryDto inputCategory)
        {
            if (inputCategory is null)
            {
                return BadRequest(ModelState);
            }

            var duplicate = categoryRepository.GetAll()
                    .FirstOrDefault(p => p.Name.Trim().Equals(inputCategory.Name.TrimEnd(), StringComparison.InvariantCultureIgnoreCase));

            if (duplicate is not null)
            {
                ModelState.AddModelError("", "Category already exists");
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryMap = this.mapper.Map<Category>(inputCategory);

            if (!this.categoryRepository.Create(categoryMap))
            {
                ModelState.AddModelError("", "Error in Create");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return Ok("A new inputCategory is successfully created.");
        }

        [HttpPut("{categoryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult Update(int categoryId, [FromBody] CategoryDto categoryDto)
        {
            if (categoryDto is null)
            {
                return BadRequest(base.ModelState);
            }

            if (categoryDto.Id != categoryId)
            {
                return BadRequest(ModelState);
            }

            if (!this.categoryRepository.Exists(categoryId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var category = this.mapper.Map<Category>(categoryDto);
            if (!this.categoryRepository.Update(category))
            {
                ModelState.AddModelError("", "Error in Update.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return Ok($"The category #{categoryId} is updated.");
        }

        [HttpDelete("{categoryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult Delete(int categoryId)
        {
            if (!this.categoryRepository.Exists(categoryId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var category = this.categoryRepository.Get(categoryId);
            if (!this.categoryRepository.Delete(category))
            {
                ModelState.AddModelError("", "Error in Delete.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return Ok($"The category #{categoryId} is deleted.");
        }
    }
}
