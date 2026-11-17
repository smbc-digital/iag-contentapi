namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class DirectoryEntry
{
    public string Slug { get; set; }
    public string Name { get; set; }
    public string Provider { get; set; }
    public string Description { get; set; }
    public string Teaser { get; set; }
    public string MetaDescription { get; set; }
    public IEnumerable<FilterTheme> Themes { get; set; } = new List<FilterTheme>();
    public IEnumerable<MinimalDirectory> Directories { get; set; }
    public IEnumerable<Alert> Alerts { get; set; }
    public IEnumerable<Alert> AlertsInline { get; set; }
    public IEnumerable<TrustedLogo> TrustedLogos { get; set; } = new List<TrustedLogo>();
    public IEnumerable<string> Tags { get; set; } = new List<string>();
    public string Image { get; set; }
    public MapPosition MapPosition { get; set; } = new();
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string Twitter { get; set; } = string.Empty;
    public string Facebook { get; set; } = string.Empty;
    public string Youtube { get; set; } = string.Empty;
    public string Instagram { get; set; } = string.Empty;
    public string LinkedIn { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsOpenPlus { get; set; }
    public string MondayOpen { get; set; }
    public string MondayClose { get; set; }
    public string TuesdayOpen { get; set; }
    public string TuesdayClose { get; set; }
    public string WednesdayOpen { get; set; }
    public string WednesdayClose { get; set; }
    public string ThursdayOpen { get; set; }
    public string ThursdayClose { get; set; }
    public string FridayOpen { get; set; }
    public string FridayClose { get; set; }
    public string SaturdayOpen { get; set; }
    public string SaturdayClose { get; set; }
    public string SundayOpen { get; set; }
    public string SundayClose { get; set; }
    public string MondayStaffedOpen { get; set; }
    public string MondayStaffedClose { get; set; }
    public string TuesdayStaffedOpen { get; set; }
    public string TuesdayStaffedClose { get; set; }
    public string WednesdayStaffedOpen { get; set; }
    public string WednesdayStaffedClose { get; set; }
    public string ThursdayStaffedOpen { get; set; }
    public string ThursdayStaffedClose { get; set; }
    public string FridayStaffedOpen { get; set; }
    public string FridayStaffedClose { get; set; }
    public string SaturdayStaffedOpen { get; set; }
    public string SaturdayStaffedClose { get; set; }
    public string SundayStaffedOpen { get; set; }
    public string SundayStaffedClose { get; set; }
    public string ClosuresText { get; set; }

    internal DirectoryEntry WithLibrary(ConverseLibrary library)
    {
        DirectoryEntry directoryEntry = (DirectoryEntry)MemberwiseClone();
        directoryEntry.Name = library.LibraryName;
        directoryEntry.Address = library.LibraryAddress;
        directoryEntry.IsOpenPlus = library.IsOpenPlus;
        directoryEntry.MondayOpen = library.MondayOpen;
        directoryEntry.MondayClose = library.MondayClose;
        directoryEntry.TuesdayOpen = library.TuesdayOpen;
        directoryEntry.TuesdayClose = library.TuesdayClose;
        directoryEntry.WednesdayOpen = library.WednesdayOpen;
        directoryEntry.WednesdayClose = library.WednesdayClose;
        directoryEntry.ThursdayOpen = library.ThursdayOpen;
        directoryEntry.ThursdayClose = library.ThursdayClose;
        directoryEntry.FridayOpen = library.FridayOpen;
        directoryEntry.FridayClose = library.FridayClose;
        directoryEntry.SaturdayOpen = library.SaturdayOpen;
        directoryEntry.SaturdayClose = library.SaturdayClose;
        directoryEntry.SundayOpen = library.SundayOpen;
        directoryEntry.SundayClose = library.SundayClose;
        directoryEntry.MondayStaffedOpen = library.MondayStaffedOpen;
        directoryEntry.MondayStaffedClose = library.MondayStaffedClose;
        directoryEntry.TuesdayStaffedOpen = library.TuesdayStaffedOpen;
        directoryEntry.TuesdayStaffedClose = library.TuesdayStaffedClose;
        directoryEntry.WednesdayStaffedOpen = library.WednesdayStaffedOpen;
        directoryEntry.WednesdayStaffedClose = library.WednesdayStaffedClose;
        directoryEntry.ThursdayStaffedOpen = library.ThursdayStaffedOpen;
        directoryEntry.ThursdayStaffedClose = library.ThursdayStaffedClose;
        directoryEntry.FridayStaffedOpen = library.FridayStaffedOpen;
        directoryEntry.FridayStaffedClose = library.FridayStaffedClose;
        directoryEntry.SaturdayStaffedOpen = library.SaturdayStaffedOpen;
        directoryEntry.SaturdayStaffedClose = library.SaturdayStaffedClose;
        directoryEntry.SundayStaffedOpen = library.SundayStaffedOpen;
        directoryEntry.SundayStaffedClose = library.SundayStaffedClose;
        directoryEntry.ClosuresText = library.ClosuresText;
        return directoryEntry;
    }
}