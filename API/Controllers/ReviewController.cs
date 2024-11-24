using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Interfaces.Repositories;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace API.Controllers;

public class ReviewController(IReviewRepository reviewRepository, IMapper mapper, IHttpContextAccessor accessor, UserManager<AppUser> userManager, IProductRepository productRepository) : BaseApiController {
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

    var token = accessor.HttpContext!.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
    if (token == null) return Unauthorized(new BaseResponse(401, "Unauthorized", null, []));

    newReview.AppUserId = JwtHelper.GetClaimFromJwt(token, ClaimTypes.NameIdentifier)!;

    var user = await userManager.FindByIdAsync(newReview.AppUserId);
    if (user == null) return BadRequest(new BaseResponse(400, "Bad request", null, []));

    var product = await productRepository.GetAsync(x => x.Id == review.ProductId);
    if (product == null) return BadRequest(new BaseResponse(400, "Bad request", null, []));

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