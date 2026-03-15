using SportMania.Handlers.Interface;
using SportMania.Models;
using SportMania.Models.Requests;
using SportMania.Repository.Interface;
using SportMania.Services.Interface;

namespace SportMania.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IPlanRepository _planRepository;
        private readonly IKeyService _keyService;
        private readonly IToyyibPayHandler _toyyibPayHandler;
        private readonly IConfiguration _configuration;

        public TransactionService(
            ITransactionRepository transactionRepository,
            ICustomerRepository customerRepository,
            IPlanRepository planRepository,
            IKeyService keyService,
            IToyyibPayHandler toyyibPayHandler,
            IConfiguration configuration)
        {
            _transactionRepository = transactionRepository;
            _customerRepository = customerRepository;
            _planRepository = planRepository;
            _keyService = keyService;
            _toyyibPayHandler = toyyibPayHandler;
            _configuration = configuration;
        }

        public async Task<(bool IsSuccess, string Result)> InitiatePaymentAsync(RequestTransaction req, string phone, string returnUrl)
        {
            try
            {
                // Generate key with guild ID and correct duration from the plan
                var guildIdString = _configuration["Discord:DefaultGuildId"] ?? throw new Exception("Default Guild ID not configured. Please set 'Discord:DefaultGuildId' in configuration.");

                // Create pending transaction
                var transaction = new Transaction
                {
                    Customer = await _customerRepository.GetCustomerByEmailAsync(req.Email)
                               ?? await _customerRepository.CreateCustomerAsync(new Customer { Email = req.Email }),
                    Plan = await _planRepository.GetByIdAsync(req.PlanId) ?? throw new Exception("Plan not found."),
                    PaymentStatus = "Pending",
                };
                transaction.Amount = transaction.Plan.Price.ToString();
                
                if (!int.TryParse(transaction.Plan.Duration, out var duration))
                {
                    throw new FormatException("Invalid plan duration format.");
                }
                
                transaction.Key = await _keyService.GenerateKeyAsync(ulong.Parse(guildIdString), req.PlanId, duration);
                var createdTransaction = await _transactionRepository.CreateTransactionAsync(transaction);

                // Prepare ToyyibPay request
                var categoryCode = _toyyibPayHandler.GetCategoryCode(transaction.Plan.Name);
                var finalReturnUrl = $"{returnUrl}?transactionId={createdTransaction.TransactionId}";
                var billAmount = ConvertPriceToCents(transaction.Plan.Price);

                var toyyibPayRequest = _toyyibPayHandler.BuildRequest(
                    categoryCode: categoryCode,
                    billName: transaction.Plan.Name,
                    billDescription: $"Subscription for {transaction.Plan.Name}",
                    billAmount: billAmount,
                    returnUrl: finalReturnUrl,
                    externalReferenceNo: createdTransaction.TransactionId.ToString(),
                    customerEmail: transaction.Customer.Email,
                    customerPhone: phone ?? "0123456789",
                    plan: transaction.Plan,
                    key: transaction.Key
                );

                // Call ToyyibPay API
                return await _toyyibPayHandler.CreateBillAsync(toyyibPayRequest);
            }
            catch (Exception ex)
            {
                return (false, $"Error initiating payment: {ex.Message}");
            }
        }

        public async Task<Transaction> ProcessPaymentCallbackAsync(Guid transactionId, string statusId)
        {
            try
            {
                var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);
                if (transaction == null)
                    throw new KeyNotFoundException($"Transaction with ID {transactionId} not found.");

                transaction.PaymentStatus = statusId switch
                {
                    "1" => "Success",
                    _ => "Failed",
                };
                await _transactionRepository.UpdateTransactionAsync(transaction);
                return transaction;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error processing payment callback: {ex.Message}");
            }
        }

        private static int ConvertPriceToCents(string price)
        {
            decimal priceValue = 0;
            decimal.TryParse(price.Replace("RM", "").Trim(), out priceValue);
            return (int)(priceValue * 100);
        }
    }
}