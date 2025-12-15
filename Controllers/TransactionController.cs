using Microsoft.AspNetCore.Mvc;
using SportMania.Models;
using SportMania.Repository.Interface;
using SportMania.Services.Interface;
using System;
using System.Threading.Tasks;

// A simple ViewModel to capture user input
public class TransactionInitiationViewModel
{
    public Guid PlanId { get; set; }
    public string Email { get; set; } = string.Empty;
}

[Route("[controller]")]
public class TransactionController : Controller
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IPlanRepository _planRepository; // Assuming you have a Plan repository
    private readonly IKeyService _keyService;

    public TransactionController(
        ITransactionRepository transactionRepository,
        ICustomerRepository customerRepository,
        IPlanRepository planRepository,
        IKeyService keyService)
    {
        _transactionRepository = transactionRepository;
        _customerRepository = customerRepository;
        _planRepository = planRepository;
        _keyService = keyService;
    }

    /// <summary>
    /// Step 1: User selects a plan. This action shows a page to enter an email.
    /// </summary>
    [HttpGet("Create/{planId}")]
    public async Task<IActionResult> Create(Guid planId)
    {
        var plan = await _planRepository.GetByIdAsync(planId);
        if (plan == null)
        {
            return NotFound("Plan not found.");
        }

        var model = new TransactionInitiationViewModel { PlanId = planId };
        // You would have a view that displays plan details and an email input field.
        // For example: return View(model);
        return Ok($"This is the page to enter your email for Plan: {plan.Name}. Submit to /Transaction/InitiatePayment.");
    }

    /// <summary>
    /// Step 2: User submits their email. A pending transaction is created,
    /// and the user is redirected to a (mock) payment gateway.
    /// </summary>
    [HttpPost("InitiatePayment")]
    public async Task<IActionResult> InitiatePayment([FromForm] TransactionInitiationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Return to the form with validation errors
            // For example: return View("Create", model);
            return BadRequest(ModelState);
        }

        // Find or create the customer by email
        var customer = await _customerRepository.GetCustomerByEmailAsync(model.Email)
                       ?? await _customerRepository.CreateCustomerAsync(new Customer { Email = model.Email });

        var plan = await _planRepository.GetByIdAsync(model.PlanId);
        if (plan == null) return NotFound("Plan not found.");

        // Create a pending transaction before sending to payment gateway
        var transaction = new Transaction
        {
            Customer = customer,
            Plan = plan,
            Amount = plan.Price.ToString(), // Assuming Plan has a Price property
            PaymentStatus = "Pending",
        };

        var createdTransaction = await _transactionRepository.CreateTransactionAsync(transaction);

        // TODO: Redirect to the actual payment gateway with transaction details.
        // The return URL for the gateway should point to our PaymentCallback action.
        // For now, we'll simulate a successful payment by redirecting directly.
        return RedirectToAction("PaymentComplete", "Payment", new { transactionId = createdTransaction.TransactionId, success = true });
    }

    /// <summary>
    /// Step 3: Callback from the payment gateway.
    /// This action processes the payment result.
    /// </summary>
    [HttpGet("PaymentCallback")]
    public async Task<IActionResult> PaymentCallback(Guid transactionId, bool success)
    {
        var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);
        if (transaction == null)
        {
            return NotFound("Transaction not found.");
        }

        if (success)
        {
            // Payment was successful
            transaction.PaymentStatus = "Success";

            // Generate a unique key for the user
            var newKey = await _keyService.GenerateKeyAsync();
            transaction.Key = newKey; // Assign the generated key to the transaction

            await _transactionRepository.UpdateTransactionAsync(transaction);

            // Redirect to a success page showing the user their key
            return RedirectToAction("PaymentComplete", "Payment", new { transactionId = transaction.TransactionId });
        }
        else
        {
            // Payment failed
            transaction.PaymentStatus = "Failed";
            await _transactionRepository.UpdateTransactionAsync(transaction);

            // Redirect to a failure page
            // For example: return View("Failure", transaction);
            return Ok("Payment Failed. Please try again.");
        }
    }

    /// <summary>
    /// An action to view all transactions for monitoring purposes.
    /// </summary>
    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        var transactions = await _transactionRepository.GetAllTransactionsAsync();
        // Return a view that lists all transactions
        // For example: return View(transactions);
        return View(transactions);
    }
}