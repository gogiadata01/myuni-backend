namespace MyUni.Models.Entities
{
    public class EventCardEn
    {
        public int Id { get; set; }
        public string Url_en { get; set; }
        public string Title_en { get; set; }
        public string Text_en { get; set; }
        public string Time_en { get; set; }
        public bool isFeatured_en { get; set; }
        public string Link_en { get; set; }
        public int Numbering_en { get; set; }
        public string Description_en  { get; set; }

//new element
	public string saregistracioForma_en { get; set; }
        public ICollection<EventType_en> Types_en { get; set; }


        public class EventType_en
        {
            public int Id { get; set; }
            public string Type_en { get; set; }
        }
    }

}
