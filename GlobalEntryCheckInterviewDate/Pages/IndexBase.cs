using System.Xml.Linq;
using System;

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
    public DateTime? EndDate { get; set; } = DateTime.Today.AddMonths(1);
    public List<LocationDatesViewModel> LocationDates { get; set; } = new List<LocationDatesViewModel>();
    public bool ShowResults;
    public bool busy;
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
    public async Task ShowInfo(int id)
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
        busy = true;
        LocationDates = new List<LocationDatesViewModel>();
        StartDate ??= DateTime.Today;
        EndDate ??= DateTime.Today.AddYears(1);
        if (EndDate.Value.Date > StartDate.Value.AddYears(1))
        {
            EndDate = StartDate.Value.AddYears(1);
        }
        foreach (var item in SelectedLocations)
        {
            var apiData = await HttpClient.GetFromJsonAsync<List<InterviewDateModel>>
                ($"https://ttp.cbp.dhs.gov/schedulerapi/locations/{item.Id}/slots?startTimestamp={StartDate:yyyy-MM-ddTHH:mm}&endTimestamp={EndDate:yyyy-MM-ddTHH:mm}");
            if (apiData.Any(a => a.Active > 0))
            {
                var testData = apiData.Where(w => w.Active > 0)
                    .Select(s => new LocationDatesViewModel()
                    {
                        LocationName = Locations.First(f => f.Id == item.Id).Name,
                        Active = s.Active,
                        Total = s.Total,
                        Pending = s.Pending,
                        Conflicts = s.Conflicts,
                        Duration = s.Duration,
                        Timestamp = s.Timestamp,
                        Remote = s.Remote
                    }).ToList();
                LocationDates.AddRange(testData);
            }
        }
        StateHasChanged();
        busy = false;
        ShowResults = true;
    }
    public void OnFilter(DataGridColumnFilterEventArgs<LocationModel> args) => SelectedLocations = new List<LocationModel>();
    public void DateRenderNoWeekends(DateRenderEventArgs args)
    {
        args.Disabled = args.Disabled || args.Date.DayOfWeek == DayOfWeek.Sunday || args.Date.DayOfWeek == DayOfWeek.Saturday;
    }
    public void OnRender(DataGridRenderEventArgs<LocationDatesViewModel> args)
    {
        if (args.FirstRender)
        {
            args.Grid.Groups.Add(new GroupDescriptor() { Property = "LocationName", Title = "Location", SortOrder = SortOrder.Ascending });
            StateHasChanged();
        }
    }
}