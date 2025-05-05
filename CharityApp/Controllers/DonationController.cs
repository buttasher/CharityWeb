using CharityApp.Models;
using CharityApp.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using System.Net.Http;
using System.Text.Json;

namespace CharityApp.Controllers
{
    public class DonationController : Controller
    {
        private readonly CharityDbContext _context;
        private readonly StripeSettings _stripeSettings;
        private readonly HttpClient _httpClient;
        private readonly FraudPredictionService _fraudService;

        public DonationController(IOptions<StripeSettings> stripeOptions, CharityDbContext context, FraudPredictionService fraudService)
        {
            _stripeSettings = stripeOptions.Value;
            _context = context;
            _httpClient = new HttpClient();
            _fraudService = fraudService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.StripePublishableKey = _stripeSettings.PublishableKey;
            ViewBag.UserName = HttpContext.Session.GetString("FullName") ?? "";
            ViewBag.Email = HttpContext.Session.GetString("Email") ?? "";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] DonationRequest request)
        {
            string userIp = HttpContext.Connection.RemoteIpAddress?.ToString();

            int recentFailedAttempts = _context.Donations
                .Where(d => d.IpAddress == userIp && d.Amount == 0)
                .Count();

            bool vpnUsed = await IsUsingVpn(userIp);

            try
            {
                var donation = new Donation
                {
                    UserName = request.UserName ?? "Guest",
                    Email = request.Email ?? "unknown@example.com",
                    Amount = request.Amount,
                    DonationTime = DateTime.Now,
                    Country = request.Country ?? "Unknown",
                    IsFirstTimeDonor = request.IsFirstTimeDonor,
                    Device = HttpContext.Request.Headers["User-Agent"].ToString(),
                    FailedAttempts = recentFailedAttempts,
                    VpnUsed = vpnUsed,
                    IpAddress = userIp
                };

                _context.Donations.Add(donation);
                _context.SaveChanges();

                // Call AI API
                var fraudResult = await _fraudService.PredictFraudAsync(donation);

                if (fraudResult.fraud)
                {
                    donation.IsFraud = true;
                    donation.FraudFlags = string.Join("; ", fraudResult.flags);
                    donation.FraudConfidence = fraudResult.confidence;
                    donation.FraudMethod = fraudResult.method;
                    _context.SaveChanges();
                }
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = request.Amount * 100,
                                Currency = "usd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = "Charity Donation"
                                }
                            },
                            Quantity = 1
                        }
                    },
                    Mode = "payment",
                    SuccessUrl = Url.Action("Success", "Donation", null, Request.Scheme),
                    CancelUrl = Url.Action("Cancel", "Donation", null, Request.Scheme),
                };

                var service = new SessionService();
                Session session = service.Create(options);

                return Json(new { id = session.Id });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Cancel()
        {
            return View("Index");
        }

        // 🛡 VPN Detection Method using ipapi.co
        private async Task<bool> IsUsingVpn(string ip)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"https://ipapi.co/{ip}/json/");
                var json = JsonDocument.Parse(response);
                if (json.RootElement.TryGetProperty("privacy", out JsonElement privacy))
                {
                    if (privacy.TryGetProperty("vpn", out JsonElement vpn))
                    {
                        return vpn.GetBoolean();
                    }
                }
            }
            catch
            {
                // Optional: log failure or fallback
            }
            return false;
        }
    }
}
