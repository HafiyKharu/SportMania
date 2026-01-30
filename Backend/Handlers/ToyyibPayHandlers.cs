using System.Text.Json;
using SportMania.Handlers.Interface;
using SportMania.Models.Requests;
using SportMania.Models;

namespace SportMania.Handlers
{
    public class ToyyibPayHandler : IToyyibPayHandler
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Sandbox URL for testing, change to "https://toyyibpay.com" for production
        private const string BASE_URL = "https://dev.toyyibpay.com";
        private const string CREATE_BILL_ENDPOINT = "/index.php/api/createBill";

        public ToyyibPayHandler(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<(bool IsSuccess, string Result)> CreateBillAsync(RequestToyyibPay request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{BASE_URL}{CREATE_BILL_ENDPOINT}")
                {
                    Content = new FormUrlEncodedContent(request.ToFormData())
                };

                using var response = await client.SendAsync(httpRequest);
                var responseString = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(responseString);
                var root = doc.RootElement;

                if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                {
                    var billCode = root[0].GetProperty("BillCode").GetString();
                    return (true, $"{BASE_URL}/{billCode}");
                }
                
                System.Diagnostics.Debug.WriteLine($"ToyyibPay API Error: {responseString}");
                return (false, $"ToyyibPay API Error: {responseString}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ToyyibPay API Error: {ex}");
                return (false, $"Error calling ToyyibPay API: {ex.Message}");
            }
        }

        public string GetCategoryCode(string planName)
        {
            return planName.ToLower() switch
            {
                var name when name.Contains("season") => _configuration["ToyyibPay:CategoryCodes:Seasonal"] ?? string.Empty,
                var name when name.Contains("daily") => _configuration["ToyyibPay:CategoryCodes:Daily"] ?? string.Empty,
                var name when name.Contains("monthly") => _configuration["ToyyibPay:CategoryCodes:Monthly"] ?? string.Empty,
                var name when name.Contains("weekly") => _configuration["ToyyibPay:CategoryCodes:Weekly"] ?? string.Empty,
                _ => string.Empty
            };
        }

        public RequestToyyibPay BuildRequest(
            string categoryCode,
            string billName,
            string billDescription,
            int billAmount,
            string returnUrl,
            string externalReferenceNo,
            string customerEmail,
            string customerPhone,
            Plan plan,
            Key key)
        {
            var emailContent = $"Thank you for purchasing {plan.Name}! Your redemption key: {key.LicenseKey}";

            if (string.IsNullOrWhiteSpace(categoryCode)){throw new ArgumentException("ToyyibPay category code must be provided and cannot be empty.", nameof(categoryCode));}
            var userSecretKey = _configuration["ToyyibPay:UserSecretKey"];
            if (string.IsNullOrWhiteSpace(userSecretKey)){throw new InvalidOperationException("ToyyibPay configuration is missing or invalid: 'ToyyibPay:UserSecretKey' must be configured.");}

            return new RequestToyyibPay
            {
                UserSecretKey = userSecretKey,
                CategoryCode = categoryCode,
                BillName = TruncateString(billName, 30),
                BillDescription = TruncateString(billDescription, 100),
                BillPriceSetting = 1,
                BillPayorInfo = 1,
                BillAmount = billAmount,
                BillReturnUrl = returnUrl,
                BillCallbackUrl = returnUrl,
                BillExternalReferenceNo = externalReferenceNo,
                BillTo = customerEmail,
                BillEmail = customerEmail,
                BillPhone = customerPhone,
                BillSplitPayment = 0,
                BillPaymentChannel = 0,
                BillContentEmail = emailContent,
                BillChargeToCustomer = "0"
            };
        }

        private static string TruncateString(string value, int maxLength)
        {
            return string.IsNullOrEmpty(value)
                ? string.Empty
                : value.Length > maxLength ? value[..maxLength] : value;
        }
    }
}