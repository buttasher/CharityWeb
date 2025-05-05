using CharityApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Diagnostics;
using Microsoft.Data.SqlClient;

namespace CharityApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CharityDbContext _context;
        private readonly IConfiguration _configuration;

        // Injecting ApplicationDbContext to access the database
        public HomeController(ILogger<HomeController> logger, CharityDbContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Admin()
        {
            decimal totalAmount;
            
            string connectionString = _configuration.GetConnectionString("dbconn");
            // Total Sales Today
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT SUM(Amount) FROM dbo.Donations";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                var result = cmd.ExecuteScalar();
                totalAmount = result != DBNull.Value ? Convert.ToDecimal(result) : 0;
            }

            // Fetch donations data from the database
            var donations = _context.Donations
                                    .OrderByDescending(d => d.DonationTime) // Sorting donations by DonationTime
                                    .ToList(); // You can add pagination here if needed

            ViewBag.totalAmount = totalAmount;

            // Pass the donations to the Admin view
            return View(donations);
        }

        public IActionResult User()
        {
            return View();
        }

        public IActionResult Donate()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
