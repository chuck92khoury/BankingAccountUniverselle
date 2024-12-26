public abstract class Client
{
    public int Id { get; set; } // Clé primaire
    public string Name { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
}

public class IndividualClient : Client
{
    public string FirstName { get; set; }
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; }
}

public class ProfessionalClient : Client
{
    public string Siret { get; set; }
    public string LegalStatus { get; set; }
    public string HeadquartersAddress { get; set; }
}
