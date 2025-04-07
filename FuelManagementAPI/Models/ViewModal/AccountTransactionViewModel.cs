public class AccountTransactionViewModel
{

    public int AccountId { get; set; }
    public decimal Amount { get; set; }
    public string TransactionType { get; set; } // "Debit" or "Credit"
    public string? Description { get; set; }
    public DateTime TransactionDate { get; set; }
}
