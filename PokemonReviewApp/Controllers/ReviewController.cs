using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : Controller
    {
        private readonly IReviewRepository reviewRepository;
        private readonly IPokemonRepository pokemonRepository;
        private readonly IReviewerRepository reviewerRepository;
        private readonly IMapper mapper;

        public ReviewController(
            IReviewRepository reviewRepository,
            IPokemonRepository pokemonRepository,
            IReviewerRepository reviewerRepository,
            IMapper mapper)
        {
            this.reviewRepository = reviewRepository;
            this.pokemonRepository = pokemonRepository;
            this.reviewerRepository = reviewerRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        public IActionResult GetReviews()
        {
            var result = this.mapper.Map<List<ReviewDto>>(this.reviewRepository.GetAll());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(result);
        }

        [HttpGet("{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetReview(int reviewId)
        {
            if (!this.reviewRepository.Exists(reviewId))
            {
                return NotFound();
            }

            var result = this.mapper.Map<ReviewDto>(this.reviewRepository.Get(reviewId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(result);
        }

        [HttpGet("{pokeId}/reviews")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewsOfAPokemon(int pokeId)
        {
            if (!this.pokemonRepository.Exists(pokeId))
            {
                return NotFound();
            }

            var result = this.mapper.Map<List<ReviewDto>>(this.reviewRepository.GetReviewsOfAPokemon(pokeId));
        
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
        public IActionResult CreateReview(
            [FromQuery] int reviewerId,
            [FromQuery] int pokeId,
            [FromBody] ReviewDto review)
        {
            if (review is null)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!this.reviewerRepository.Exists(reviewerId)
                || !this.pokemonRepository.Exists(pokeId))
            {
                return NotFound();
            }

            var reviewMap = this.mapper.Map<Review>(review);
            reviewMap.Pokemon = this.pokemonRepository.Get(pokeId);
            reviewMap.Reviewer = this.reviewerRepository.Get(reviewerId);

            if (!this.reviewRepository.Create(reviewMap))
            {
                ModelState.AddModelError("", "Error in Create.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return Ok("A review is created.");
        }

        [HttpPut("{reviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult Update(int reviewId, [FromBody] ReviewDto reviewDto)
        {
            if (reviewDto is null
                || reviewDto.Id != reviewId
                || !ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!this.reviewerRepository.Exists(reviewId))
            {
                return NotFound();
            }

            var mapped = this.mapper.Map<Review>(reviewDto);
            if (!this.reviewRepository.Update(mapped))
            {
                ModelState.AddModelError("", "Error in update.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return Ok($"The review #{reviewId} is udpated.");
        }

        [HttpDelete("{reviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult Delete(int reviewId)
        {
            if (!this.reviewRepository.Exists(reviewId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var found = this.reviewRepository.Get(reviewId);
            if (!this.reviewRepository.Delete(found))
            {
                ModelState.AddModelError("", "Error in Delete.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return Ok($"The review #{reviewId} is deleted.");
        }
    }
}
