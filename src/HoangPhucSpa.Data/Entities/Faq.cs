namespace HoangPhucSpa.Data.Entities;

public class Faq : AuditableEntity
{
    public int Id { get; set; }
    public required string Question { get; set; }
    public required string AnswerHtml { get; set; }
    public string? AskerName { get; set; }
    public string? AskerEmail { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisible { get; set; } = true;
    public string? SearchText { get; set; }
    public byte[] RowVersion { get; set; } = [];
}
