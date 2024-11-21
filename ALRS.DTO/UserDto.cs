namespace ALRS.DTO
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string? LoginWithIdentifier { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
    }
}
