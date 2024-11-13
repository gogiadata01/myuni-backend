namespace MyUni.Models  // Ensure it's this or a similar namespace
{
    public class UniversityVisit
    {
        public int Id { get; set; }
        public string UniversityName { get; set; }
        public string UserId { get; set; }
        public DateTime VisitDate { get; set; }
    }
}
