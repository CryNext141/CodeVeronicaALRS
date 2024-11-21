namespace ALRS.DTO
{
    public class CreateAlertDto
    {
        public int AlertStatus { get; set; }

        public string CrimeLocation { get; set; }
        public string CrimeDate { get; set; }
        public CreateAlertVictimDto Victim { get; set; }
        public CreateAlertAbductorDto Abductor { get; set; }


        public class CreateAlertVictimDto
        {
            public string VictimName { get; set; }
            public int VictimAge { get; set; }
            public string VictimSex { get; set; }
            public string VictimHair { get; set; }
            public string VictimClothing { get; set; }
        }

        public class CreateAlertAbductorDto
        {
            public string AbductorName { get; set; }
            public int AbductorAge { get; set; }
            public string AbductorSex { get; set; }
            public string AbductorHair { get; set; }
            public string AbductorClothing { get; set; }
            public string AbductorVehicle { get; set; }
        }
    }
}
