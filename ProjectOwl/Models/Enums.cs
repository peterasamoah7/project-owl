namespace ProjectOwl.Models
{
    public enum Issue
    {
        LocalPayments = 1,
        Remittance,
        BalanceTransfer,
        MissingCard,
        General,
        Maintenance,
        Uncategorized
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
        Done
    }

    public enum State
    {
        Done,
        Error
    }
}
