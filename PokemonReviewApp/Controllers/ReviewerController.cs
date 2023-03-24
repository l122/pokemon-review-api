using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewerController : Controller
    {
        private readonly IReviewerRepository reviewerRepository;
        private readonly IMapper mapper;

        public ReviewerController(IReviewerRepository reviewerRepository, IMapper mapper)
        {
            this.reviewerRepository = reviewerRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
        public IActionResult GetReviewers()
        {
            var result = this.mapper.Map<List<ReviewerDto>>(this.reviewerRepository.GetAll());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(result);
        }

        [HttpGet("{reviewerId}")]
        [ProducesResponseType(200, Type = typeof(Reviewer))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewer(int reviewerId)
        {
            if (!this.reviewerRepository.Exists(reviewerId))
            {
                return NotFound();
            }

            var result = this.mapper.Map<ReviewerDto>(this.reviewerRepository.Get(reviewerId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(result);
        }

        [HttpGet("{reviewerId}/reviews")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewsByReviewer(int reviewerId)
        {
            if (!this.reviewerRepository.Exists(reviewerId))
            {
                return NotFound();
            }

            var result = this.mapper.Map<List<ReviewDto>>(this.reviewerRepository.GetReviewsByReviewer(reviewerId));

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
        [ProducesResponseType(422)]
        public IActionResult CreateReviewer([FromBody] ReviewerDto reviewerDto)
        {
            if (reviewerDto is null)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var duplicate = reviewerRepository.GetAll()
                .FirstOrDefault(p => p.FirstName.Trim()
                .Equals(reviewerDto.FirstName.Trim(), StringComparison.InvariantCultureIgnoreCase)
                    && p.LastName.Trim()
                    .Equals(reviewerDto.LastName.Trim(), StringComparison.InvariantCultureIgnoreCase));

            if (duplicate is not null)
            {
                ModelState.AddModelError("", "The reviewer already exists.");
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ModelState);
            }

            var reviewer = this.mapper.Map<Reviewer>(reviewerDto);
            if (!this.reviewerRepository.Create(reviewer))
            {
                ModelState.AddModelError("", "Error in Create.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return Ok("A new reviewer is created successfully.");
        }

        [HttpPut("{reviewerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult Update(int reviewerId, [FromBody] ReviewerDto reviewerDto)
        {
            if (reviewerDto is null
                || reviewerId != reviewerDto.Id
                || !ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!this.reviewerRepository.Exists(reviewerId))
            {
                return NotFound();
            }

            var mapped = this.mapper.Map<Reviewer>(reviewerDto);
            if (!this.reviewerRepository.Update(mapped))
            {
                ModelState.AddModelError("", "Error in update.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return Ok($"The reviewer #{reviewerId} is updated.");
        }

        [HttpDelete("{reviewerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult Delete(int reviewerId)
        {
            if (!this.reviewerRepository.Exists(reviewerId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var found = this.reviewerRepository.Get(reviewerId);
            if (!this.reviewerRepository.Delete(found))
            {
                ModelState.AddModelError("", "Error in Delete.");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return Ok($"The reviewer id#{reviewerId} is deleted.");
        }
    }
}
