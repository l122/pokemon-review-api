using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : Controller
    {
        private readonly IPokemonRepository pokemonRepository;
        private readonly IOwnerRepository ownerRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IMapper mapper;

        public PokemonController(IPokemonRepository pokemonRepository,
                                 IOwnerRepository ownerRepository,
                                 ICategoryRepository categoryRepository,
                                 IMapper mapper)
        {
            this.pokemonRepository = pokemonRepository;
            this.ownerRepository = ownerRepository;
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        public IActionResult GetPokemons()
        {
            var pokemons =  this.mapper.Map<List<PokemonDto>>(this.pokemonRepository.GetAll());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(pokemons);
        }

        [HttpGet("{pokeId}")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemon(int pokeId)
        {
            if (!this.pokemonRepository.Exists(pokeId))
            {
                return NotFound();
            }

            var pokemon = this.mapper.Map<PokemonDto>(this.pokemonRepository.Get(pokeId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(pokemon);
        }

        [HttpGet("{pokeId}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemanRating(int pokeId)
        {
            if (!this.pokemonRepository.Exists(pokeId))
            {
                return NotFound();
            }

            var rating = this.pokemonRepository.GetRating(pokeId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(rating);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int pokeId, [FromBody] PokemonDto pokemon)
        {
            if (pokemon is null)
            {
                return BadRequest(ModelState);
            }

            var duplicate = this.pokemonRepository.GetAll()
                .Where(p => p.Name.Trim().Equals(pokemon.Name.Trim(), StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();

            if (duplicate is not null)
            {
                ModelState.AddModelError("", "Pokemon already exists.");
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!this.ownerRepository.Exists(ownerId)
                || !this.categoryRepository.Exists(pokeId))
            {
                return NotFound();
            }

            var pokemonMap = this.mapper.Map<Pokemon>(pokemon);
            if (!this.pokemonRepository.Create(ownerId, pokeId, pokemonMap))
            {
                ModelState.AddModelError("", "Error in Create.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return Ok("A pokemon is successfully created.");
        }

        [HttpPut("{pokemonId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult Update(int pokemonId, [FromBody] PokemonDto pokemonDto)
        {
            if (pokemonDto is null
                || !ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!this.pokemonRepository.Exists(pokemonId))
            {
                return NotFound();
            }

            var mapped = this.mapper.Map<Pokemon>(pokemonDto);
            if (!this.pokemonRepository.Update(mapped))
            {
                ModelState.AddModelError("", "Error in Update.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return Ok($"The pokemon #{pokemonId} is updated.");
        }

        [HttpDelete("{pokeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult Delete(int pokeId)
        {
            if (!this.pokemonRepository.Exists(pokeId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var found = this.pokemonRepository.Get(pokeId);
            if (!this.pokemonRepository.Delete(found))
            {
                ModelState.AddModelError("", "Error in Delete.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return Ok($"The pokemon #{pokeId} is deleted.");
        }
    }
}
