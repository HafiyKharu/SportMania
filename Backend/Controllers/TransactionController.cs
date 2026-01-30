using Microsoft.AspNetCore.Mvc;
using SportMania.Models.Requests;
using SportMania.Repository.Interface;
using SportMania.Services.Interface;

[Route("[controller]")]
public class TransactionController : Controller
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

    [HttpPost("InitiatePayment")]
    public async Task<IActionResult> InitiatePayment([FromForm] RequestTransaction req, [FromForm] string Phone)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var baseUrl = Url.Action("PaymentCallback", "Transaction", null, Request.Scheme);

            if (string.IsNullOrEmpty(baseUrl))
                return StatusCode(500, "Could not generate payment callback URL.");

            var (isSuccess, result) = await _transactionService.InitiatePaymentAsync(req, Phone, baseUrl);

            return isSuccess ? Redirect(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("PaymentCallback")]
    public async Task<IActionResult> PaymentCallback(Guid transactionId, string status_id)
    {
        try
        {
            var transaction = await _transactionService.ProcessPaymentCallbackAsync(transactionId, status_id);

            if (transaction == null)
                return NotFound("Transaction not found.");

            return transaction.PaymentStatus == "Success"
                ? RedirectToAction("PaymentComplete", "Transaction", new { transactionId = transaction.TransactionId })
                : RedirectToAction("PaymentFailed", "Transaction", new { transactionId = transaction.TransactionId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var transactions = await _transactionRepository.GetAllTransactionsAsync();
            return View(transactions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("PaymentComplete/{transactionId}")]
    public async Task<IActionResult> PaymentComplete(Guid transactionId)
    {
        try
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);

            if (transaction == null)
                return NotFound("Transaction not found.");

            return View(transaction);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("PaymentFailed/{transactionId}")]
    public async Task<IActionResult> PaymentFailed(Guid transactionId)
    {
        try
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);

            if (transaction == null)
                return NotFound("Transaction not found.");

            return View(transaction);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}