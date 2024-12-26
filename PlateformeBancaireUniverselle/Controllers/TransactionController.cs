using Microsoft.AspNetCore.Mvc;
using BankingPlatformAPI.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TransactionController : ControllerBase
{
    private readonly BankingDbContext _context;

    public TransactionController(BankingDbContext context)
    {
        _context = context;
    }

    // Obtenir toutes les transactions
    [HttpGet]
    public IActionResult GetAllTransactions()
    {
        var transactions = _context.Transactions.ToList();
        return Ok(transactions);
    }

    // Obtenir les transactions pour un compte spécifique
    [HttpGet("ByAccount/{accountId}")]
    public IActionResult GetTransactionsByAccount(int accountId)
    {
        var transactions = _context.Transactions.Where(t => t.BankAccountId == accountId).ToList();
        return Ok(transactions);
    }

    // Ajouter une transaction
    [HttpPost]
    public IActionResult CreateTransaction(Transaction transaction)
    {
        var account = _context.BankAccounts.Find(transaction.BankAccountId);
        if (account == null)
        {
            return NotFound(new { Message = "Compte bancaire non trouvé" });
        }

        // Ajuster le solde en fonction du type de transaction
        if (transaction.Type == "Dépôt")
        {
            account.Balance += transaction.Amount;
        }
        else if (transaction.Type == "Retrait")
        {
            if (account.Balance < transaction.Amount)
            {
                return BadRequest(new { Message = "Solde insuffisant" });
            }
            account.Balance -= transaction.Amount;
        }

        _context.Transactions.Add(transaction);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetAllTransactions), new { id = transaction.Id }, transaction);
    }
    [HttpGet("Report/{accountId}")]
    public IActionResult GenerateReport(int accountId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] string format = "json")
    {
        if (!new[] { "json", "pdf" }.Contains(format.ToLower()))
        {
            return BadRequest(new { Message = "Format non supporté" });
        }

        var account = _context.BankAccounts.Find(accountId);
        if (account == null)
        {
            return NotFound(new { Message = "Compte bancaire non trouvé" });
        }

        // Filtrer les transactions pour la période donnée
        var transactions = _context.Transactions
            .Where(t => t.BankAccountId == accountId)
            .Where(t => (!startDate.HasValue || t.TransactionDate >= startDate) &&
                        (!endDate.HasValue || t.TransactionDate <= endDate))
            .ToList();

        // Générer le rapport
        var report = new
        {
            AccountId = account.Id,
            AccountNumber = account.AccountNumber,
            Balance = account.Balance,
            Transactions = transactions
        };

        // Exporter en JSON
        if (format.ToLower() == "json")
        {
            var jsonReport = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });

            // Sauvegarder le rapport sur disque
            var filePath = Path.Combine("wwwroot/reports", $"Rapport_Compte_{account.AccountNumber}.json");
            System.IO.File.WriteAllText(filePath, jsonReport);

            return Ok(new { Message = "Rapport généré et sauvegardé", FilePath = filePath });
        }

        // Exporter en PDF (simplifié pour l'exemple)
        if (format.ToLower() == "pdf")
        {
            var pdfContent = $"Rapport du compte {account.AccountNumber}\n" +
                             $"Solde: {account.Balance:C}\n" +
                             $"Transactions:\n";

            foreach (var transaction in transactions)
            {
                pdfContent += $"- {transaction.TransactionDate}: {transaction.Type} de {transaction.Amount:C}\n";
            }

            var pdfBytes = System.Text.Encoding.UTF8.GetBytes(pdfContent);

            // Sauvegarder le PDF sur disque
            var filePath = Path.Combine("wwwroot/reports", $"Rapport_Compte_{account.AccountNumber}.pdf");
            System.IO.File.WriteAllBytes(filePath, pdfBytes);

            return Ok(new { Message = "Rapport généré et sauvegardé", FilePath = filePath });
        }

        return BadRequest(new { Message = "Format non supporté" });
    }
    [HttpGet("Download/{fileName}")]
    public IActionResult DownloadReport(string fileName)
    {
        var filePath = Path.Combine("wwwroot/reports", fileName);
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound(new { Message = "Fichier introuvable" });
        }

        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        return File(fileBytes, "application/octet-stream", fileName);
    }


}


