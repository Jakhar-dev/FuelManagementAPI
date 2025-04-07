public class AccountViewModel
{
    public string CustomerName { get; set; }
    public string CustomerPhone { get; set; }
    public string CustomerAddress { get; set; }
    public string? Description { get; set; }
    public DateTime? Date { get; set; } = default(DateTime?);
}
