using Microsoft.AspNetCore.Mvc;
using SportMania.Repository.Interface;
using System;
using System.Threading.Tasks;

namespace SportMania.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ITransactionRepository _transactionRepository;

        public PaymentController(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        [HttpGet]
        public async Task<IActionResult> PaymentComplete(Guid transactionId)
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);
            if (transaction == null || transaction.PaymentStatus != "Success")
            {
                return RedirectToAction("Index", "Home");
            }

            return View(transaction);
        }
    }
}
