namespace EuropeanStudentCard.DTO;

public class CreateCardDto
{
    public int StudentId { get; set; }

    public string CardNumber { get; set; }
    public string DisplayName {get; set;}

    public string PersonIdentifier { get; set; } 

    public string IssuerIdentifier { get; set; }

    public string CardStatusType { get; set; } 
}