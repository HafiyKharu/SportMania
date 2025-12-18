using SportMania.Models;
using SportMania.Models.Requests;

namespace SportMania.Handlers.Interface
{
    public interface IToyyibPayHandler
    {
        Task<(bool IsSuccess, string Result)> CreateBillAsync(RequestToyyibPay request);
        string GetCategoryCode(string planName);
        RequestToyyibPay BuildRequest(
            string categoryCode,
            string billName,
            string billDescription,
            int billAmount,
            string returnUrl,
            string externalReferenceNo,
            string customerEmail,
            string customerPhone,
            Plan plan,
            Key key);
    }
}