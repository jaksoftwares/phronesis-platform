using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phronesis.Application.Common.Interfaces;

namespace Phronesis.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("initiate")]
    public async Task<IActionResult> InitiatePayment([FromBody] PaymentRequest request, CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        request.UserId = userId; // Override with authenticated user

        var result = await _paymentService.InitiatePaymentAsync(request, cancellationToken);
        
        if (!result.Success)
        {
            return BadRequest(new { Message = result.ErrorMessage });
        }

        return Ok(result);
    }

    [HttpGet("status/{transactionId}")]
    public async Task<IActionResult> GetPaymentStatus(Guid transactionId, CancellationToken cancellationToken)
    {
        var isSuccess = await _paymentService.VerifyPaymentAsync(transactionId, cancellationToken);
        return Ok(new { TransactionId = transactionId, IsSuccessful = isSuccess });
    }
}
