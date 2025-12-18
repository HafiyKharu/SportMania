using Microsoft.AspNetCore.Mvc;
using SportMania.Models;
using SportMania.Repository.Interface;
using SportMania.Services.Interface;
using SportMania.Models.Requests;
using System.Net.Http;
using System.Text.Json;

[Route("[controller]")]
public class TransactionController : Controller
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IPlanRepository _planRepository;
    private readonly IKeyService _keyService;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public TransactionController(
        ITransactionRepository transactionRepository,
        ICustomerRepository customerRepository,
        IPlanRepository planRepository,
        IKeyService keyService,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {
        _transactionRepository = transactionRepository;
        _customerRepository = customerRepository;
        _planRepository = planRepository;
        _keyService = keyService;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Step 1: User selects a plan. This action shows a page to enter an email.
    /// </summary>
    [HttpGet("Create/{planId}")]
    public async Task<IActionResult> Create(Guid planId)
    {
        var plan = await _planRepository.GetByIdAsync(planId);
        var key = await _keyService.GenerateKeyAsync();
        if (plan == null)
        {
            return NotFound("Plan not found.");
        }

        var model = new RequestTransaction { PlanId = planId };
        // You would have a view that displays plan details and an email input field.
        // For example: return View(model);
        return Ok($"This is the page to enter your email for Plan: {plan.Name}. Submit to /Transaction/InitiatePayment.");
    }

    /// <summary>
    /// Step 2: User submits their email. A pending transaction is created,
    /// and the user is redirected to a (mock) payment gateway.
    /// </summary>
    [HttpPost("InitiatePayment")]
    public async Task<IActionResult> InitiatePayment([FromForm] RequestTransaction req, [FromForm] string Phone)
    {
        if (!ModelState.IsValid){ return BadRequest(ModelState);}

        var customer = await _customerRepository.GetCustomerByEmailAsync(req.Email)
                       ?? await _customerRepository.CreateCustomerAsync(new Customer { Email = req.Email });

        var plan = await _planRepository.GetByIdAsync(req.PlanId);
        if (plan == null) return NotFound("Plan not found.");
        Key key = await _keyService.GenerateKeyAsync();

        // Create a pending transaction before sending to payment gateway
        var transaction = new Transaction
        {
            Customer = customer,
            Plan = plan,
            Amount = plan.Price.ToString(),
            Key = key,
            PaymentStatus = "Pending",
        };

        var createdTransaction = await _transactionRepository.CreateTransactionAsync(transaction);

        // --- ToyyibPay Integration Start ---
        
        // 1. Determine Category Code
        string categoryCode = "";
        if (plan.Name.Contains("Season", StringComparison.OrdinalIgnoreCase)) 
            categoryCode = _configuration["ToyyibPay:CategoryCodes:Seasonal"];
        else if (plan.Name.Contains("Daily", StringComparison.OrdinalIgnoreCase)) 
            categoryCode = _configuration["ToyyibPay:CategoryCodes:Daily"];
        else if (plan.Name.Contains("Monthly", StringComparison.OrdinalIgnoreCase)) 
            categoryCode = _configuration["ToyyibPay:CategoryCodes:Monthly"];
        else if (plan.Name.Contains("Weekly", StringComparison.OrdinalIgnoreCase)) 
            categoryCode = _configuration["ToyyibPay:CategoryCodes:Weekly"];
        
        // 2. Prepare API Parameters
        var returnUrl = Url.Action("PaymentCallback", "Transaction", new { transactionId = createdTransaction.TransactionId }, Request.Scheme);
        
        // Calculate price in cents (assuming plan.Price is in RM)
        decimal priceValue = 0;
        decimal.TryParse(plan.Price.Replace("RM", "").Trim(), out priceValue);
        var billPriceCents = (int)(priceValue * 100);

        // Truncate strings to meet API limits
        string billName = plan.Name.Length > 30 ? plan.Name.Substring(0, 30) : plan.Name;
        string billDescription = $"Subscription for {plan.Name}";
        if (billDescription.Length > 100) billDescription = billDescription.Substring(0, 100);

        var formData = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("userSecretKey", _configuration["ToyyibPay:UserSecretKey"]),
            new KeyValuePair<string, string>("categoryCode", categoryCode),
            new KeyValuePair<string, string>("billName", billName),
            new KeyValuePair<string, string>("billDescription", billDescription),
            new KeyValuePair<string, string>("billPriceSetting", "1"), // 1 = Fixed Amount
            new KeyValuePair<string, string>("billPayorInfo", "1"),    // 1 = Require Payer Info
            new KeyValuePair<string, string>("billAmount", billPriceCents.ToString()),
            new KeyValuePair<string, string>("billReturnUrl", returnUrl),
            new KeyValuePair<string, string>("billCallbackUrl", returnUrl),
            new KeyValuePair<string, string>("billExternalReferenceNo", createdTransaction.TransactionId.ToString()),
            new KeyValuePair<string, string>("billTo", customer.Email),
            new KeyValuePair<string, string>("billEmail", customer.Email),
            new KeyValuePair<string, string>("billPhone", Phone ?? "0123456789"),
            new KeyValuePair<string, string>("billSplitPayment", "0"),
            new KeyValuePair<string, string>("billPaymentChannel", "0"), // 0 = FPX
            new KeyValuePair<string, string>("billContentEmail", "Thank you for purchasing!"),
            new KeyValuePair<string, string>("billChargeToCustomer", "0") // 0 = Charge FPX fee to customer
        };

        // 3. Call ToyyibPay API
        var client = _httpClientFactory.CreateClient();
        // Updated URL to dev.toyyibpay.com for sandbox
        var request = new HttpRequestMessage(HttpMethod.Post, "https://dev.toyyibpay.com/index.php/api/createBill")
        {
            Content = new FormUrlEncodedContent(formData)
        };

        var response = await client.SendAsync(request);
        var responseString = await response.Content.ReadAsStringAsync();

        // 4. Parse Response to get BillCode
        // Response format: [{"BillCode":"..."}]
        try 
        {
            using var doc = JsonDocument.Parse(responseString);
            var root = doc.RootElement;
            
            // Success case: It is an array containing the BillCode
            if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
            {
                var billCode = root[0].GetProperty("BillCode").GetString();
                // Updated URL to dev.toyyibpay.com for sandbox
                return Redirect($"https://dev.toyyibpay.com/{billCode}");
            }
            // Error case: It might be an object with an error message
            else 
            {
                return BadRequest($"ToyyibPay API Error: {responseString}");
            }
        }
        catch (Exception ex)
        {
            // Fallback or log error
            return BadRequest($"Error parsing ToyyibPay response: {ex.Message}. Raw Response: {responseString}");
        }
    }

    /// <summary>
    /// Step 3: Callback from the payment gateway.
    /// This action processes the payment result.
    /// </summary>
    [HttpGet("PaymentCallback")]
    public async Task<IActionResult> PaymentCallback(Guid transactionId, string status_id, string billcode, string order_id)
    {
        var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);
        if (transaction == null)
        {
            return NotFound("Transaction not found.");
        }

        // ToyyibPay status_id: 1 = Success, 2 = Pending, 3 = Fail
        bool success = status_id == "1";

        if (success)
        {
            // Payment was successful
            transaction.PaymentStatus = "Success";

            // Generate a unique key for the user
            var newKey = await _keyService.GenerateKeyAsync();
            transaction.Key = newKey; // Assign the generated key to the transaction

            await _transactionRepository.UpdateTransactionAsync(transaction);

            // Redirect to a success page showing the user their key
            return RedirectToAction("PaymentComplete", "Transaction", new { transactionId = transaction.TransactionId });
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

    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        var transactions = await _transactionRepository.GetAllTransactionsAsync();
        return View(transactions);
    }
    [HttpGet("PaymentComplete")]
    public async Task<IActionResult> PaymentComplete(Guid transactionId, bool success = true)
    {
        var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);
        return View(transaction);
    }
}