namespace MyUni.Models.Entities
{
    public class UniversityVisit
    {
        public int Id { get; set; }
        public string UniversityName { get; set; }
        public string UserId { get; set; }
        public DateTime VisitDate { get; set; }
    }
}
