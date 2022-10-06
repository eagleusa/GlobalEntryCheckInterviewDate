namespace GlobalEntryCheckInterviewDate.Models;

public class LocationDatesViewModel
{
    public LocationModel Location { get; set; }
    public List<InterviewDateModel> InterviewDates { get; set; } = new List<InterviewDateModel>();
}
