namespace REAC.EntityFrameworkCore.Seed.Integration
{
    public static class CitizenIntegration
    {
        public static List<CitizenIntegrationDto> GetCitizen()
        {
            return new List<CitizenIntegrationDto>()
            {
                new CitizenIntegrationDto()
                {
                    Id = "1090000001",
                    IdVersion ="4",
                    FirstNameAr ="احمد",
                    FullNameAr ="احمد عبدالله صالح الأحمد",
                    Gender ="M",
                    Nationality =113,
                    DobG = new DateTime(1991,1,20),
                    IdExpiryDateG = DateTime.Now.AddMonths(DateTime.Now.Second + 5)

                },
                new CitizenIntegrationDto()
                {
                    Id = "1090000002",
                    FirstNameAr ="خالد",
                    FullNameAr ="خالد حسن محمد الأحمد",
                    Gender ="M",
                    Language ="A",
                    Nationality =113,
                    DobG = new DateTime(1991,1,20),
                    IdExpiryDateG = DateTime.Now.AddMonths(DateTime.Now.Second + 5)
                },
                new CitizenIntegrationDto()
                {
                    Id = "1090000003",
                    FirstNameAr ="حسن",
                    FullNameAr ="حسن خالد محمد ابراهيم",
                    Gender ="M",
                    Nationality =113,
                    DobG = new DateTime(1991,1,20),
                    IdExpiryDateG = DateTime.Now.AddMonths(DateTime.Now.Second + 5)
                },
                new CitizenIntegrationDto()
                {
                    Id = "1090000004",
                    FirstNameAr ="ياسر",
                    FullNameAr ="ياسر خالد ياسر ابراهيم",
                    Gender ="M",
                    Nationality =113,
                    DobG = new DateTime(1991, 1, 20),
                    IdExpiryDateG = DateTime.Now.AddMonths(DateTime.Now.Second + 5),
                },
                new CitizenIntegrationDto()
                {
                    Id = "1090000005",
                    FirstNameAr ="محمد",
                    FullNameAr ="محمد خالد محمد محمد",
                    Gender ="M",
                    Nationality =113,
                    DobG = new DateTime(1991, 1, 20),
                    IdExpiryDateG = DateTime.Now.AddMonths(DateTime.Now.Second + 5),
                },
                new CitizenIntegrationDto()
                {
                    Id = "1090000006",
                    FirstNameAr ="حسين",
                    FullNameAr ="حسين خالد حسن ابراهيم",
                    Gender ="M",
                    Nationality =113,
                    DobG = new DateTime(1991, 1, 20),
                    IdExpiryDateG = DateTime.Now.AddMonths(DateTime.Now.Second + 5),
                },
                new CitizenIntegrationDto()
                {
                    Id = "1090000007",
                    FirstNameAr ="يوسف",
                    FullNameAr ="يوسف خالد يوسف ابراهيم",
                    Gender ="M",
                    Nationality =113,
                    DobG = new DateTime(1991,1,20),
                    IdExpiryDateG = DateTime.Now.AddMonths(DateTime.Now.Second + 5)
                },
                new CitizenIntegrationDto()
                {
                    Id = "1090000008",
                    FirstNameAr ="سيد",
                    FullNameAr ="سيد خالد سيد ابراهيم",
                    Gender ="M",
                    Nationality =113,
                    DobG = new DateTime(1991,1,20),
                    IdExpiryDateG = DateTime.Now.AddMonths(DateTime.Now.Second + 5)
                },
                new CitizenIntegrationDto()
                {
                    Id = "1090000009",
                    FirstNameAr ="فهد",
                    FullNameAr ="فهد محمد فهد ابراهيم",
                    Gender ="M",
                    Nationality =113,
                    DobG = new DateTime(1991,1,20),
                    IdExpiryDateG = DateTime.Now.AddMonths(DateTime.Now.Second + 5)
                },
                new CitizenIntegrationDto()
                {
                    Id = "1090000010",
                    FirstNameAr ="حمزه",
                    FullNameAr ="حمزه خالد حسن ابراهيم",
                    Gender ="M",
                    Nationality =113,
                    DobG = new DateTime(1991,1,20),
                    IdExpiryDateG = DateTime.Now.AddMonths(DateTime.Now.Second + 5)
                }

            };
        }

    }
    public class CitizenIntegrationDto
    {
        public string Id { get; set; } = null!; //     "id": "1090000001",
        public string IdVersion { get; set; } = null!; // 4,
        public string FirstNameAr { get; set; } = null!; //public string احمد",
        public string FullNameAr { get; set; } = null!; //public string احمد عبدالله صالح الأحمد",
        public string Gender { get; set; } = null!; //public string M",
        public string Language { get; set; } = null!; //public string A",
        public int Nationality { get; set; }// 113,
        public DateTime DobG { get; set; } //public string 1931-05-18",
        public DateTime IdExpiryDateG { get; set; } //public string 1931-05-18",

    }
}
