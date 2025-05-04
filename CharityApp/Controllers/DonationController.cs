using CharityApp.Models;
using CharityApp.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using System;

namespace CharityApp.Controllers
{
    public class DonationController : Controller
    {
        private readonly CharityDbContext _context;
        private readonly StripeSettings _stripeSettings;

        public DonationController(IOptions<StripeSettings> stripeOptions, CharityDbContext context)
        {
            _stripeSettings = stripeOptions.Value;
            _context = context;
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
        public IActionResult CreateCheckoutSession([FromBody] DonationRequest request)
        {
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
                    FailedAttempts = 0,
                    VpnUsed = false,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                };

                _context.Donations.Add(donation);
                _context.SaveChanges();

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
    }
}
