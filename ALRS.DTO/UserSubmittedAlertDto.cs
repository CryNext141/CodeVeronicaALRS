public class UserAlertDto
{
    public string CrimeLocation { get; set; }
    public string CrimeDate { get; set; }
    public string VictimLook { get; set; }
}

public class KidnapperDto
{
    public string KidnapperName { get; set; } = "Unknown";
    public int? KidnapperAge { get; set; } = 0;
    public string KidnapperSex { get; set; } = "Unknown";
    public string KidnapperLook { get; set; } = "Unknown";
    public string KidnapperVehicle { get; set; } = "Unknown";
}