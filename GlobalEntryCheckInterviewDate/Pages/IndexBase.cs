
namespace GlobalEntryCheckInterviewDate.Pages;

public class IndexBase : ComponentBase
{
    [Inject]
    public HttpClient HttpClient { get; set; }
    public IEnumerable<LocationsModel> Locations { get; set; } = Enumerable.Empty<LocationsModel>();
    public List<StatesModel> States { get; set; } = new List<StatesModel>();
    protected override async Task OnInitializedAsync()
    {
        try
        {
            Locations = await HttpClient.GetFromJsonAsync<List<LocationsModel>>
                ("https://ttp.cbp.dhs.gov/schedulerapi/locations/?temporary=false&inviteOnly=false&operational=true&serviceName=Global%20Entry");
            States = Locations.Select(s => new StatesModel() { City = s.City, State = s.State, CountryCode = s.CountryCode })
                .Distinct()
                .OrderByDescending(o => o.CountryCode).ThenBy(t => t.State).ThenBy(t => t.City)
                .ToList();
        }
        catch (HttpRequestException) // Non success
        {
            Console.WriteLine("An error occurred.");
        }
        catch (NotSupportedException) // When content type is not valid
        {
            Console.WriteLine("The content type is not supported.");
        }
        catch (JsonException) // Invalid JSON
        {
            Console.WriteLine("Invalid JSON.");
        }
    }
}