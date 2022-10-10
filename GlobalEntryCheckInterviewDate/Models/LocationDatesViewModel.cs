namespace GlobalEntryCheckInterviewDate.Models;

public class LocationDatesViewModel
{
    public string LocationName { get; set; }
    public int Active { get; set; }
    public int Total { get; set; }
    public int Pending { get; set; }
    public int Conflicts { get; set; }
    public int Duration { get; set; }
    public DateTime Timestamp { get; set; }
    public bool Remote { get; set; }
}