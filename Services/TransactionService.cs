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

        public TransactionService(
            ITransactionRepository transactionRepository,
            ICustomerRepository customerRepository,
            IPlanRepository planRepository,
            IKeyService keyService,
            IToyyibPayHandler toyyibPayHandler)
        {
            _transactionRepository = transactionRepository;
            _customerRepository = customerRepository;
            _planRepository = planRepository;
            _keyService = keyService;
            _toyyibPayHandler = toyyibPayHandler;
        }

        public async Task<(bool IsSuccess, string Result)> InitiatePaymentAsync(RequestTransaction req, string phone, string returnUrl)
        {
            try
            {
                // Get or create customer
                var customer = await _customerRepository.GetCustomerByEmailAsync(req.Email)
                               ?? await _customerRepository.CreateCustomerAsync(new Customer { Email = req.Email });

                // Get plan
                var plan = await _planRepository.GetByIdAsync(req.PlanId);
                if (plan == null) 
                    return (false, "Plan not found.");

                // Parse the duration from the plan
                if (!int.TryParse(plan.Duration, out int durationDays))
                {
                    // Handle cases where duration might not be a valid number, maybe default or log an error
                    return (false, $"Invalid duration format for plan {plan.Name}.");
                }

                // Generate key with guild ID and correct duration from the plan
                var key = await _keyService.GenerateKeyAsync(req.GuildId, req.PlanId, durationDays);

                // Create pending transaction
                var transaction = new Transaction
                {
                    Customer = customer,
                    Plan = plan,
                    Amount = plan.Price.ToString(),
                    Key = key,
                    PaymentStatus = "Pending",
                };

                var createdTransaction = await _transactionRepository.CreateTransactionAsync(transaction);

                // Prepare ToyyibPay request
                var categoryCode = _toyyibPayHandler.GetCategoryCode(plan.Name);
                var finalReturnUrl = $"{returnUrl}?transactionId={createdTransaction.TransactionId}";
                var billAmount = ConvertPriceToCents(plan.Price);

                var toyyibPayRequest = _toyyibPayHandler.BuildRequest(
                    categoryCode: categoryCode,
                    billName: plan.Name,
                    billDescription: $"Subscription for {plan.Name}",
                    billAmount: billAmount,
                    returnUrl: finalReturnUrl,
                    externalReferenceNo: createdTransaction.TransactionId.ToString(),
                    customerEmail: customer.Email,
                    customerPhone: phone ?? "0123456789",
                    plan: plan,
                    key: key
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