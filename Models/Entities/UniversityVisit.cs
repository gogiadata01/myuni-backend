using System;
using System.ComponentModel.DataAnnotations;

namespace MyUni.Models.Entities // Updated to match other entities
{
    public class UniversityVisit
    {
        [Key]
        public int Id { get; set; }

        public string UniversityName { get; set; }

        public string UserId { get; set; }

        public DateTime VisitDate { get; set; }
    }
}
