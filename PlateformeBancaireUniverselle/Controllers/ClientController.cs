using Microsoft.AspNetCore.Mvc;
using BankingPlatformAPI.Data;

[Route("api/[controller]")]
[ApiController]
public class ClientController : ControllerBase
{
    private readonly BankingDbContext _context;

    public ClientController(BankingDbContext context)
    {
        _context = context;
    }

    // Obtenir la liste de tous les clients
    [HttpGet]
    public IActionResult GetAllClients()
    {
        var clients = _context.Clients.ToList();
        return Ok(clients);
    }

    // Obtenir un client par ID
    [HttpGet("{id}")]
    public IActionResult GetClientById(int id)
    {
        var client = _context.Clients.Find(id);
        if (client == null)
        {
            return NotFound(new { Message = "Client non trouvé" });
        }
        return Ok(client);
    }

    // Ajouter un client
    [HttpPost]
    public IActionResult CreateClient(Client client)
    {
        _context.Clients.Add(client);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetClientById), new { id = client.Id }, client);
    }

    // Modifier un client
    [HttpPut("{id}")]
    public IActionResult UpdateClient(int id, Client updatedClient)
    {
        var client = _context.Clients.Find(id);
        if (client == null)
        {
            return NotFound(new { Message = "Client non trouvé" });
        }

        client.Name = updatedClient.Name;
        client.Address = updatedClient.Address;
        client.Email = updatedClient.Email;

        if (client is IndividualClient individualClient && updatedClient is IndividualClient updatedIndividualClient)
        {
            individualClient.FirstName = updatedIndividualClient.FirstName;
            individualClient.BirthDate = updatedIndividualClient.BirthDate;
            individualClient.Gender = updatedIndividualClient.Gender;
        }

        if (client is ProfessionalClient professionalClient && updatedClient is ProfessionalClient updatedProfessionalClient)
        {
            professionalClient.Siret = updatedProfessionalClient.Siret;
            professionalClient.LegalStatus = updatedProfessionalClient.LegalStatus;
            professionalClient.HeadquartersAddress = updatedProfessionalClient.HeadquartersAddress;
        }

        _context.SaveChanges();
        return NoContent();
    }

    // Supprimer un client
    [HttpDelete("{id}")]
    public IActionResult DeleteClient(int id)
    {
        var client = _context.Clients.Find(id);
        if (client == null)
        {
            return NotFound(new { Message = "Client non trouvé" });
        }

        _context.Clients.Remove(client);
        _context.SaveChanges();
        return NoContent();
    }
}
