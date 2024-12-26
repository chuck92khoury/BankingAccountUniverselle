


public class BankAccount
{
    public int Id { get; set; } // Clé primaire
    public string AccountNumber { get; set; }
    public DateTime OpenDate { get; set; }
    public decimal Balance { get; set; } = 1000.00m; // Solde initial par défaut
    public List<Transaction> Transactions { get; set; } = new List<Transaction>();
}
