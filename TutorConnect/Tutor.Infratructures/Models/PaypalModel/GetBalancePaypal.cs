namespace Tutor.Infratructures.Models.PaypalModel
{
    public class GetBalancePayPal
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Signature { get; set; }
        public string BaseUrl { get; set; }
    }
}
