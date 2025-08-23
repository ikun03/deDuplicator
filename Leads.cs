
using System.Text.Json.Serialization;

public class Leads
{
    [JsonPropertyName("_id")]
    public required string Id { get; set; }

    [JsonPropertyName("email")]
    public required string Email { get; set; }

    [JsonPropertyName("firstName")]
    public required string FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public required string LastName { get; set; }

    [JsonPropertyName("address")]
    public required string Address { get; set; }

    [JsonPropertyName("entryDate")]
    public required string EntryDate { get; set; }
    private List<Leads> sourceLeads;

    public List<Leads> GetSourceLeads()
    {
        return sourceLeads;
    }

    public void SetSourceLeads(List<Leads> value)
    {
        sourceLeads = value;
    }
}