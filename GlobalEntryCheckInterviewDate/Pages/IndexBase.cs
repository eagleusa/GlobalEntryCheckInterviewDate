namespace GlobalEntryCheckInterviewDate.Pages;

public class IndexBase : ComponentBase
{
    [Inject]
    public HttpClient HttpClient { get; set; }
    [Inject]
    public DialogService DialogService { get; set; }
    public RadzenDataGrid<LocationModel> LocationsGrid;
    public RadzenDataGrid<LocationDatesViewModel> InterviewDatesGrid;
    public IEnumerable<LocationModel> Locations { get; set; } = Enumerable.Empty<LocationModel>();
    public IList<LocationModel> SelectedLocations { get; set; } = new List<LocationModel>();
    public DateTime? StartDate { get; set; } = DateTime.Today;
    public DateTime? EndDate { get; set; } = DateTime.Today.AddYears(1);
    public IEnumerable<LocationDatesViewModel> LocationDates { get; set; } = new List<LocationDatesViewModel>();
    protected override async Task OnInitializedAsync()
    {
        try
        {
            Locations = await HttpClient.GetFromJsonAsync<List<LocationModel>>
                ("https://ttp.cbp.dhs.gov/schedulerapi/locations/?temporary=false&inviteOnly=false&operational=true&serviceName=Global%20Entry");
            Locations = Locations.OrderByDescending(o => o.CountryCode).ThenBy(t => t.State).ThenBy(t => t.City).ThenBy(t => t.Name);
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
    public async Task OnClick(int id)
    {
        var model = Locations.First(f => f.Id == id);
        // Show info dialog
        await DialogService.OpenAsync<LocationInformationDialog>(
            $"Location Information",
            new Dictionary<string, object>() { { "Model", model } },
            new DialogOptions()
            {
                Width = "60vw",
                Height = "auto",
                CloseDialogOnEsc = true,
                CloseDialogOnOverlayClick = true,
                Resizable = true
            });
    }
    public async Task GetInterviews()
    {
        LocationDates = new List<LocationDatesViewModel>();
        StartDate ??= DateTime.Today;
        EndDate ??= DateTime.Today.AddYears(1);
        foreach (var item in SelectedLocations)
        {
            var apiData = await HttpClient.GetFromJsonAsync<List<InterviewDateModel>>
                ($"https://ttp.cbp.dhs.gov/schedulerapi/locations/{item.Id}/slots?startTimestamp={StartDate:yyyy-MM-ddTHH:mm}&endTimestamp={EndDate:yyyy-MM-ddTHH:mm}");
            if (apiData.Any(a => a.Active > 0))
            {
                ((List<LocationDatesViewModel>)LocationDates).Add(new LocationDatesViewModel()
                {
                    Location = Locations.First(f => f.Id == item.Id),
                    InterviewDates = apiData.Where(w => w.Active > 0).ToList()
                });
            }
        }
        StateHasChanged();
    }
    public void DateRenderNoWeekends(DateRenderEventArgs args)
    {
        args.Disabled = args.Disabled || args.Date.DayOfWeek == DayOfWeek.Sunday || args.Date.DayOfWeek == DayOfWeek.Saturday;
    }
}