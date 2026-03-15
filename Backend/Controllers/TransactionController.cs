using Microsoft.AspNetCore.Mvc;
using SportMania.Models.Requests;
using SportMania.Repository.Interface;
using SportMania.Services.Interface;

namespace SportMania.Controllers;

[ApiController]
[Route("api/transactions")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly IPlanRepository _planRepository;
    private readonly ITransactionRepository _transactionRepository;

    public TransactionController(
        ITransactionService transactionService,
        IPlanRepository planRepository,
        ITransactionRepository transactionRepository)
    {
        _transactionService = transactionService;
        _planRepository = planRepository;
        _transactionRepository = transactionRepository;
    }

    [HttpPost("initiate-payment")]
    public async Task<IActionResult> InitiatePayment([FromBody] RequestInitiatePayment req)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Backend callback URL for processing payment status
            var callbackUrl = Url.Action("PaymentCallback", "Transaction", null, Request.Scheme);

            if (string.IsNullOrEmpty(callbackUrl))
                return StatusCode(500, "Could not generate payment callback URL.");

            var requestTransaction = new RequestTransaction
            {
                Email = req.Email,
                PlanId = req.PlanId
            };

            var (isSuccess, result) = await _transactionService.InitiatePaymentAsync(requestTransaction, req.Phone, callbackUrl);

            return isSuccess ? Ok(new { redirectUrl = result }) : BadRequest(new { error = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("payment-callback")]
    public async Task<IActionResult> PaymentCallback(Guid transactionId, string status_id)
    {
        try
        {
            var transaction = await _transactionService.ProcessPaymentCallbackAsync(transactionId, status_id);

            if (transaction == null)
                return NotFound("Transaction not found.");

            // Redirect to Frontend based on payment status
            var frontendUrl = transaction.PaymentStatus == "Success"
                ? $"http://localhost:5103/transactions/success/{transaction.TransactionId}"
                : $"http://localhost:5103/transactions/failed/{transaction.TransactionId}";

            return Redirect(frontendUrl);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var transactions = await _transactionRepository.GetAllTransactionsAsync();
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("{transactionId:guid}")]
    public async Task<IActionResult> GetById(Guid transactionId)
    {
        try
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);

            if (transaction == null)
                return NotFound("Transaction not found.");

            return Ok(transaction);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}