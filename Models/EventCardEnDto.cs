public class EventCardEnDto
{
    public string Url_en { get; set; }
    public string Title_en { get; set; }
    public string Text_en { get; set; }
    public string Time_en { get; set; }
    public bool isFeatured_en { get; set; }
    public string Link_en { get; set; }
    public int Numbering_en { get; set; }
    public string Description_en { get; set; }
    public string saregistracioForma_en { get; set; }

    public List<EventTypeEnDto> Types_en { get; set; }
}

public class EventTypeEnDto
{
    public string Type_en { get; set; }
}
