﻿namespace MyUni.Models.Entities
{
    public class EventCard
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Time { get; set; }
        public bool isFeatured { get; set; }
        public string Link { get; set; }
        public int Numbering { get; set; }
        public string Description  { get; set; }

//new element
	public string saregistracioForma { get; set; }
        public ICollection<EventType> Types { get; set; }


        public class EventType
        {
            public int Id { get; set; }
            public string Type { get; set; }
        }
    }

}
