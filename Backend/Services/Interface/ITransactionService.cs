using SportMania.Models;
using SportMania.Models.Requests;

namespace SportMania.Services.Interface
{
    public interface ITransactionService
    {
        Task<(bool IsSuccess, string Result)> InitiatePaymentAsync(RequestTransaction req, string phone, string returnUrl);
        Task<Transaction> ProcessPaymentCallbackAsync(Guid transactionId, string statusId);
    }
}