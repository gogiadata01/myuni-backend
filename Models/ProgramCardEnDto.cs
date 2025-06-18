using System.ComponentModel.DataAnnotations;

namespace MyUni.Models
{
    public class ProgramCardEnDto
    {
        [Required]
        public ICollection<FieldEnDto> Fields_en { get; set; }

        public class FieldEnDto
        {
            public string FieldName_en { get; set; }
            public ICollection<ProgramNames_enDto> ProgramNames_en { get; set; }
        }
        public class ProgramNames_enDto
        {
            public string ProgramName_en { get; set; }
            public ICollection<CheckBoxes_enDto> CheckBoxes_en { get; set; }

        }
        public class CheckBoxes_enDto
        {
            public string CheckBoxName_en { get; set; }
        }

    }
}