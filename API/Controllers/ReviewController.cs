using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Interfaces.Repositories;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Core.Interfaces.Services;
using static System.Net.Mime.MediaTypeNames;

namespace API.Controllers;

public class ReviewController(IReviewRepository reviewRepository, IMapper mapper, IHttpContextAccessor accessor, UserManager<AppUser> userManager, IProductRepository productRepository, IAIService aiService) : BaseApiController {
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

  [HttpGet("product/{id:int}")]
  public async Task<IActionResult> GetReviewsByProduct(int id) {
    var reviews = await reviewRepository.GetAllAsync(x => x.ProductId == id);
    var response = new BaseResponse(200, "Success", mapper.Map<List<ReviewGetDto>>(reviews), []);
    return Ok(response);
  }

  [HttpGet("product/{id:int}/ai")]
  public async Task<IActionResult> Summarize(int id) {
    var reviews = await reviewRepository.GetAllAsync(x => x.ProductId == id);

    var reviewGetDtos = mapper.Map<List<ReviewGetDto>>(reviews);

    var combinedReviews = string.Join("\n", reviewGetDtos.Select(r => r.Text));

    var userInput = $"Summarize these reviews: {combinedReviews}";

    var systemPrompt = "You are an expert in summarizing product reviews. After receiving all reviews, provide a concise summary in 1-2 sentences. Respond only with the summary, and do not include any additional text. If no reviews provided, just send EMPTY response.";

    var prompt = $"{systemPrompt}\n{userInput}";

    var payload = new {
      contents = new[]
      {
        new
        {
          parts = new[]
          {
            new { text = prompt }
          }
        }
      }
    };

    var response = await aiService.SendToAIAsync(payload);

    if (response == null)
      return BadRequest(new BaseResponse(400, "Bad request", null, []));

    return Ok(new BaseResponse(200, "Success", response, []));
  }
}