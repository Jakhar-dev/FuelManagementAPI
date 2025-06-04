namespace FuelManagementAPI.ViewModels
{
    public class CustomerLedgerViewModel
    {
        public int AccountId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal Balance => TotalDebit - TotalCredit;
    }
}
