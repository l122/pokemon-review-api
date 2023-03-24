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
    public class CountryController : Controller
    {
        private readonly ICountryRepository countryRepository;
        private readonly IMapper mapper;

        public CountryController(ICountryRepository countryRepository, IMapper mapper)
        {
            this.countryRepository = countryRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
        public IActionResult GetCountries()
        {
            var result = this.mapper.Map<List<CountryDto>>(this.countryRepository.GetAll());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(result);
        }

        [HttpGet("{countryId}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]
        public IActionResult GetCountry(int countryId)
        {
            if (!this.countryRepository.Exists(countryId))
            {
                return NotFound();
            }

            var result = this.mapper.Map<CountryDto>(this.countryRepository.Get(countryId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(result);
        }

        [HttpGet("{ownerId}/country")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]
        public IActionResult GetCountryByOwner(int ownerId)
        {
            var result = this.mapper.Map<CountryDto>(this.countryRepository.GetCountryByOwner(ownerId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(result);
        }

        [HttpGet("{countryId}/owners")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        [ProducesResponseType(400)]
        public IActionResult GetOwnersFromACountry(int countryId)
        {
            if (!this.countryRepository.Exists(countryId))
            {
                return NotFound();
            }
            
            var result = this.mapper.Map<List<OwnerDto>>(this.countryRepository.GetOwnersFromACountry(countryId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCountry([FromBody] CountryDto arg)
        {
            if (arg is null)
            {
                return BadRequest(ModelState);
            }

            var duplicate = countryRepository.GetAll()
                    .FirstOrDefault(p => p.Name.Trim().Equals(arg.Name.TrimEnd(), StringComparison.InvariantCultureIgnoreCase));

            if (duplicate is not null)
            {
                ModelState.AddModelError("", "Country already exists");
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var countryMap = this.mapper.Map<Country>(arg);

            if (!this.countryRepository.Create(countryMap))
            {
                ModelState.AddModelError("", "Error in Create.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return Ok("A new Country is successfully created.");
        }

        [HttpPut("{countryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Update(int countryId, [FromBody] CountryDto countryDto)
        {
            if (countryDto is null)
            {
                return BadRequest();
            }

            if (countryId != countryDto.Id)
            {
                return BadRequest();
            }

            if (!this.countryRepository.Exists(countryId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var country = this.mapper.Map<Country>(countryDto);
            if (!this.countryRepository.Update(country))
            {
                base.ModelState.AddModelError("", "Error in Update.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return Ok($"The country #{countryId} is updated.");
        }

        [HttpDelete("{countryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult Delete(int countryId)
        {
            if (!this.countryRepository.Exists(countryId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var country = this.countryRepository.Get(countryId);
            if (!this.countryRepository.Delete(country))
            {
                ModelState.AddModelError("", "Error in Delete.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }
            
            return Ok($"The country id#{countryId} is deleted.");
        }
    }
}
