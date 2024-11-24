using System.Security.Claims;
using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Interfaces.Repositories;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Identity;

namespace API.Controllers;

public class CardController(ICardRepository cardRepository, IMapper mapper, UserManager<AppUser> userManager, IHttpContextAccessor accessor) : BaseApiController {
  [HttpGet]
  public async Task<IActionResult> GetCards() {
    var cards = await cardRepository.GetAllAsync(x => true);
    var response = new BaseResponse(200, "Success", mapper.Map<List<CardGetDto>>(cards), []);
    return Ok(response);
  }

  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetCard(int id) {
    var card = await cardRepository.GetAsync(x => x.Id == id);
    var response = new BaseResponse(200, "Success", mapper.Map<CardGetDto>(card), []);
    return Ok(response);
  }

  [HttpPost]
  public async Task<IActionResult> CreateCard(CardPostDto card) {
    if (!ModelState.IsValid) {
      return BadRequest(ModelState);
    }

    var newCard = mapper.Map<Card>(card);

    var token = accessor.HttpContext!.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
    if (token == null) return Unauthorized(new BaseResponse(401, "Unauthorized", null, []));

    newCard.AppUserId = JwtHelper.GetClaimFromJwt(token, ClaimTypes.NameIdentifier)!;

    var user = await userManager.FindByIdAsync(newCard.AppUserId);
    if (user == null) return BadRequest(new BaseResponse(400, "Bad request", null, []));

    await cardRepository.AddAsync(newCard);
    var response = new BaseResponse(201, "Created", mapper.Map<CardGetDto>(newCard), []);
    return Ok(response);
  }

  [HttpDelete("{id:int}")]
  public async Task<IActionResult> DeleteCard(int id) {
    var card = await cardRepository.GetAsync(x => x.Id == id);
    if (card == null) {
      return NotFound();
    }

    await cardRepository.DeleteAsync(card);
    var response = new BaseResponse(204, "Deleted", null, []);
    return Ok(response);
  }
}