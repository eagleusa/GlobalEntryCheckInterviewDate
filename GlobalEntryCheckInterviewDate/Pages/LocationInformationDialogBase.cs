namespace GlobalEntryCheckInterviewDate.Pages;

public class LocationInformationDialogBase : ComponentBase
{
    [Parameter]
    public LocationModel Model { get; set; }
    [Inject]
    public DialogService DialogService { get; set; }
}
