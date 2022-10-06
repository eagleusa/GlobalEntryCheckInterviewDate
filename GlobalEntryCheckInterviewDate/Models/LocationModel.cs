namespace GlobalEntryCheckInterviewDate.Models;

public class LocationModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }
    public string LocationType { get; set; }
    public string LocationCode { get; set; }
    public string Address { get; set; }
    public string AddressAdditional { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string CountryCode { get; set; }
    public string TzData { get; set; }
    public string PhoneNumber { get; set; }
    public string PhoneAreaCode { get; set; }
    public string PhoneCountryCode { get; set; }
    public string PhoneExtension { get; set; }
    public string PhoneAltNumber { get; set; }
    public string PhoneAltAreaCode { get; set; }
    public string PhoneAltCountryCode { get; set; }
    public string PhoneAltExtension { get; set; }
    public string FaxNumber { get; set; }
    public string FaxAreaCode { get; set; }
    public string FaxCountryCode { get; set; }
    public string FaxExtension { get; set; }
    public string EffectiveDate { get; set; }
    public bool Temporary { get; set; } = false;
    public bool InviteOnly { get; set; } = false;
    public bool Operational { get; set; } = false;
    public string Directions { get; set; }
    public string Notes { get; set; }
    public string MapFileName { get; set; }
    public string AccessCode { get; set; }
    public string LastUpdatedBy { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
    public DateTime? CreatedDate { get; set; }
    public bool RemoteInd { get; set; } = false;
    public List<LocationServices> Services { get; set; } = new List<LocationServices>();
}
public class LocationServices
{
    public int Id { get; set; }
    public string Name { get; set; }
}