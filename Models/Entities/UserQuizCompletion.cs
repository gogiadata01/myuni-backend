using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace MyUni.Models.Entities
{
    public class UserQuizCompletion
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime CompletedDate { get; set; }
    }
}
