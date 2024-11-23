using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Interfaces.Repositories;
using Infrastructure.Helpers;

namespace API.Controllers;

public class ReviewController(IReviewRepository reviewRepository, IMapper mapper) : BaseApiController {
  [HttpGet]
  public async Task<IActionResult> GetReviews() {
    var reviews = await reviewRepository.GetAllAsync(x => true);
    var response = new BaseResponse(200, "Success", mapper.Map<List<ReviewGetDto>>(reviews), []);
    return Ok(response);
  }

  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetReview(int id) {
    var review = await reviewRepository.GetAsync(x => x.Id == id);
    var response = new BaseResponse(200, "Success", mapper.Map<ReviewGetDto>(review), []);
    return Ok(response);
  }

  [HttpPost]
  public async Task<IActionResult> CreateReview(ReviewPostDto review) {
    if (!ModelState.IsValid) {
      return BadRequest(ModelState);
    }

    var newReview = mapper.Map<Review>(review);
    await reviewRepository.AddAsync(newReview);
    var response = new BaseResponse(201, "Created", mapper.Map<ReviewGetDto>(newReview), []);
    return Ok(response);
  }

  [HttpDelete("{id:int}")]
  public async Task<IActionResult> DeleteReview(int id) {
    var review = await reviewRepository.GetAsync(x => x.Id == id);
    if (review == null) {
      return NotFound();
    }

    await reviewRepository.DeleteAsync(review);
    var response = new BaseResponse(204, "Deleted", null, []);
    return Ok(response);
  }
}