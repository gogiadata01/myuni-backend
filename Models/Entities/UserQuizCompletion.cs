using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace MyUni.Models.Entities
{
    public class UserQuizCompletion
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CompletedDate { get; set; }
    }
}
