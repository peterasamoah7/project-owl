namespace ProjectOwl.Models
{
    public enum Issue
    {
        LocalPayments = 1,
        Remittance,
        BalanceTransfer,
        MissingCard,
        General,
        Maintenance
    }

    public enum Priority
    {
        High = 1,
        Medium, 
        Low, 
    }

    public enum AuditStatus
    {
        Pending = 1,
        Ongoing,
        Done
    }

}
