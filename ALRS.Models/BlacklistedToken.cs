namespace ALRS.Models
{
    public class BlacklistedToken
    {
        public int Id { get; set; }
        public string TokenId { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
