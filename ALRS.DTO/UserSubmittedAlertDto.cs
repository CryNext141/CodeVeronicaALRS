public class UserAlertDto
{
    public string VictimName { get; set; }
    public int VictimAge { get; set; }
    public string CrimeLocation { get; set; }
    public string CrimeDate { get; set; }
}

public class KidnapperDto
{
    public string KidnapperName { get; set; } = "Unknown";
    public int? KidnapperAge { get; set; } = 0;
    public string KidnapperSex { get; set; } = "Unknown";
    public string KidnapperClothes { get; set; } = "Unknown";
    public string KidnapperVehicle { get; set; } = "Unknown";
}