namespace AStar.Dev.Code.Generators.ConsoleApp.Models;

//[GenerateToString]
public sealed class CreateCustomerCommand
{
    public Address Address { get; set; } = new();

    public IList<Address> Addresses { get; set; } = new List<Address>();

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string PostOrZipCode { get; set; } = string.Empty;

    public string PostOrZipCode2 { get; set; } = string.Empty;

//    [Mask]
    public string MaskedProperty { get; set; } = "A long string to confirm masking... Hope it works.";

//    [Redact]
    public string RedactedProperty { get; set; } = "A long string to confirm redacting... Hope it works.";

//    [Ignore]
    public string IgnoredProperty { get; set; } = "A long string to confirm ignoring... Hope it works.";
}
