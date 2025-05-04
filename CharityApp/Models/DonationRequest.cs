namespace CharityApp.Models
{
    public class DonationRequest
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public int Amount { get; set; }
    }
}
