using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Interfaces.Repositories;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Identity;
using Stripe;
using System.Security.Claims;

namespace API.Controllers;

public class PaymentController(IPaymentRepository paymentRepository, IMapper mapper, IOrderRepository orderRepository) : BaseApiController {
  [HttpGet]
  public async Task<IActionResult> GetPayments() {
    var payments = await paymentRepository.GetAllAsync(x => true);
    var response = new BaseResponse(200, "Success", mapper.Map<List<PaymentGetDto>>(payments), []);
    return Ok(response);
  }

  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetPayment(int id) {
    var payment = await paymentRepository.GetAsync(x => x.Id == id);
    var response = new BaseResponse(200, "Success", mapper.Map<PaymentGetDto>(payment), []);
    return Ok(response);
  }

  [HttpPost]
  public async Task<IActionResult> CreatePayment(PaymentPostDto payment) {
    if (!ModelState.IsValid) {
      return BadRequest(ModelState);
    }

    var newPayment = mapper.Map<Payment>(payment);

    var order = await orderRepository.GetAsync(x => x.Id == payment.OrderId);
    if (order == null) return BadRequest(new BaseResponse(400, "Bad request", null, []));

    await paymentRepository.AddAsync(newPayment);
    var response = new BaseResponse(201, "Created", mapper.Map<PaymentGetDto>(newPayment), []);
    return Ok(response);
  }

  [HttpDelete("{id:int}")]
  public async Task<IActionResult> DeletePayment(int id) {
    var payment = await paymentRepository.GetAsync(x => x.Id == id);
    if (payment == null) {
      return NotFound();
    }

    await paymentRepository.DeleteAsync(payment);
    var response = new BaseResponse(204, "Deleted", null, []);
    return Ok(response);
  }

  [HttpPost("create-intent")]
  public async Task<IActionResult> CreatePaymentIntent(PaymentIntentDto request) {
    try {
      var options = new PaymentIntentCreateOptions {
        Amount = request.Amount,
        Currency = request.Currency,
        PaymentMethodTypes = ["card"]
      };

      var service = new PaymentIntentService();
      var intent = await service.CreateAsync(options);

      return StatusCode(200, new BaseResponse(200, "Success", intent.ClientSecret, []));
    }
    catch (StripeException ex) {
      return StatusCode(400, new BaseResponse(400, ex.Message, null, []));
    }
    catch (Exception ex) {
      return StatusCode(500, new BaseResponse(500, ex.Message, null, []));
    }
  }
}