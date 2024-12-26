public class Transaction
{
    public int Id { get; set; } // Clé primaire
    public string Type { get; set; } // Retrait, Dépôt, Paiement
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }

    // Clé étrangère vers BankAccount
    public int BankAccountId { get; set; }

    // Propriété de navigation
    public BankAccount BankAccount { get; set; }
}
