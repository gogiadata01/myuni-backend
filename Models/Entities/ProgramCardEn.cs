
namespace MyUni.Models.Entities
{
    public class ProgramCardEn
    {
        public int Id { get; set; }
        public ICollection<FieldEn> Fields_en { get; set; }

        public class FieldEn
        {
            public int Id { get; set; }
            public string FieldName_en { get; set; }
            public ICollection<ProgramNamesEn> ProgramNames_en { get; set; }
        }

        public class ProgramNamesEn
        {
            public int Id { get; set; }
            public string ProgramName_en { get; set; }
            public ICollection<CheckBoxesEn> CheckBoxes_en { get; set; }
        }

        public class CheckBoxesEn
        {
            public int Id { get; set; }
            public string CheckBoxName_en { get; set; }
        }
    }
}

