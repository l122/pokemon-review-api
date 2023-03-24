using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : Controller
    {
        private readonly IOwnerRepository ownerRepository;
        private readonly IPokemonRepository pokemonRepository;
        private readonly ICountryRepository countryRepository;
        private readonly IMapper mapper;

        public OwnerController(IOwnerRepository ownerRepository, IPokemonRepository pokemonRepository, ICountryRepository countryRepository, IMapper mapper)
        {
            this.ownerRepository = ownerRepository;
            this.pokemonRepository = pokemonRepository;
            this.countryRepository = countryRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        public IActionResult GetOwners()
        {
            var result = this.mapper.Map<List<OwnerDto>>(this.ownerRepository.GetAll());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(result);
        }

        [HttpGet("{ownerId}")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]

        public IActionResult GetOwner(int ownerId)
        {
            if (!this.ownerRepository.Exists(ownerId))
            {
                return NotFound();
            }

            var result = this.mapper.Map<OwnerDto>(this.ownerRepository.Get(ownerId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(result);
        }

        [HttpGet("{pokeId}/owners")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        [ProducesResponseType(400)]
        public IActionResult GetOwnersOfAPokemon(int pokeId)
        {
            if (!this.pokemonRepository.Exists(pokeId))
            {
                return NotFound();
            }

            var result = this.mapper.Map<List<OwnerDto>>(this.ownerRepository.GetOwnersOfAPokemon(pokeId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(result);
        }

        [HttpGet("{ownerId}/pokemon")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonByOwner(int ownerId)
        {
            if (!this.ownerRepository.Exists(ownerId))
            {
                return NotFound();
            }

            var result = this.mapper.Map<List<PokemonDto>>(this.ownerRepository.GetPokemonByOwner(ownerId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult CreateOwner([FromBody] OwnerDto inOwner, [FromQuery] int countryId)
        {
            if (inOwner is null)
            {
                return BadRequest(ModelState);
            }

            var duplicate = ownerRepository.GetAll()
                    .FirstOrDefault(
                p => p.FirstName.Trim().Equals(inOwner.FirstName.TrimEnd(), StringComparison.InvariantCultureIgnoreCase)
                    && p.LastName.Trim().Equals(inOwner.LastName.TrimEnd(), StringComparison.InvariantCultureIgnoreCase));

            if (duplicate is not null)
            {
                ModelState.AddModelError("", "Owner already exists");
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!this.countryRepository.Exists(countryId))
            {
                return NotFound();
            }

            var ownerMap = this.mapper.Map<Owner>(inOwner);
            ownerMap.Country = this.countryRepository.Get(countryId);

            if (!this.ownerRepository.Create(ownerMap))
            {
                ModelState.AddModelError("", "Error in Create.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return Ok("A new owner is successfully created.");
        }

        [HttpPut("{ownerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult Update(int ownerId, [FromBody] OwnerDto ownerDto)
        {
            if (ownerDto is null
                || ownerId != ownerDto.Id
                || !ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!this.ownerRepository.Exists(ownerId))
            {
                return NotFound();
            }

            var mapped = this.mapper.Map<Owner>(ownerDto);
            if (!this.ownerRepository.Update(mapped))
            {
                ModelState.AddModelError("", "Error in update.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return Ok($"The owner #{ownerId} is updated.");
        }

        [HttpDelete("{ownerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult Delete(int ownerId)
        {
            if (!this.ownerRepository.Exists(ownerId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var owner = this.ownerRepository.Get(ownerId);
            if (!this.ownerRepository.Delete(owner))
            {
                ModelState.AddModelError("", "Error in Delete.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return Ok($"The owner id#{ownerId} is deleted.");
        }
    }
}
