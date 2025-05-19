using System.ComponentModel.DataAnnotations;

namespace MyUni.Models
{
    public class UnicardEnDto
    {
        public string Url_en { get; set; }
        public string Title_en { get; set; }
        public string MainText_en { get; set; }
        public string History_en { get; set; }
        public string ForPupil_en { get; set; }
        public string ScholarshipAndFunding_en { get; set; }
        public string ExchangePrograms_en { get; set; }
        public string Labs_en { get; set; }
        public string StudentsLife_en { get; set; }
        public string PaymentMethods_en { get; set; }
        public ICollection<Event_en> Events_en { get; set; }
        public ICollection<Section_en> Sections_en { get; set; }
        public ICollection<Section2_en> Sections2_en { get; set; }
        public ICollection<ArchevitiSavaldebuloSagani_en> ArchevitiSavaldebuloSaganebi_en { get; set; }




        public class Event_en
        {
            public string Url_en { get; set; }
            public string Title_en { get; set; }
            public string Text_en { get; set; }
            public string Time_en { get; set; }
            public string Link_en { get; set; }
        }

        public class Section_en
        {
            public string Title_en { get; set; }
            public ICollection<Programname_en> ProgramNames_en { get; set; }
        }
        public class Section2_en
        {
            public string Title_en { get; set; }
            public ICollection<SavaldebuloSagnebi_en> SavaldebuloSagnebi_en { get; set; }
        }
        public class ArchevitiSavaldebuloSagani_en
        {
            public string Title_en { get; set; }
            public ICollection<ArchevitiSavaldebuloSagnebi_en> ArchevitiSavaldebuloSagnebi_en { get; set; }
        }

        public class SavaldebuloSagnebi_en
        {
            public string SagnisSaxeli_en { get; set; }
            public string Koeficienti_en { get; set; }
            public string MinimaluriZgvari_en { get; set; }
            public string Prioriteti_en { get; set; }
            public string AdgilebisRaodenoba_en { get; set; }

        }
        public class ArchevitiSavaldebuloSagnebi_en
        {
            public string SagnisSaxeli_en { get; set; }
            public string Koeficienti_en { get; set; }
            public string MinimaluriZgvari_en { get; set; }
            public string Prioriteti_en { get; set; }
            public string AdgilebisRaodenoba_en { get; set; }

        }
        public class Programname_en
        {
            public string ProgramName_en { get; set; }
            public string Jobs_en { get; set; }
            public string SwavlebisEna_en { get; set; }
            public string Kvalifikacia_en { get; set; }
            public string Dafinanseba_en { get; set; }
            public string KreditebisRaodenoba_en { get; set; }
            public string AdgilebisRaodenoba_en { get; set; }
            public string Fasi_en { get; set; }
            public string Kodi_en { get; set; }
            public string ProgramisAgwera_en { get; set; }
        }
    }
}

