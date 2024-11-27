namespace StockportContentApi.Factories;

public class EventRecurrenceFactory
{
    private readonly Dictionary<EventFrequency, Func<Event, int, Event>> _recurrenceDictionary =
        new()
        {
            { EventFrequency.None, (e, dayNum) => null },
            { EventFrequency.Daily, (e, dayNum) => GetRecurringEvent(e, e.EventDate.Date.AddDays(dayNum * 1)) },
            { EventFrequency.Weekly, (e, dayNum) => GetRecurringEvent(e, e.EventDate.Date.AddDays(dayNum * 7)) },
            { EventFrequency.Fortnightly, (e, dayNum) => GetRecurringEvent(e, e.EventDate.Date.AddDays(dayNum * 14)) },
            { EventFrequency.Monthly, (e, monthNum) => GetRecurringEvent(e, e.EventDate.Date.AddMonths(monthNum * 1)) },
            { EventFrequency.MonthlyDate, (e, monthNum) => GetRecurringEvent(e, e.EventDate.Date.AddMonths(monthNum * 1)) },
            { EventFrequency.MonthlyDay, (e, monthNum) => GetRecurringEvent(e, GetCorrespondingMonthsDay(e.EventDate, monthNum)) },
            { EventFrequency.Yearly, (e, monthNum) => GetRecurringEvent(e, e.EventDate.Date.AddMonths(monthNum * 12)) }
        };

    public List<Event> GetRecurringEventsOfEvent(Event eventItem)
    {
        List<Event> recurringEventsByFrequency = new();
        
        for (int i = 1; i < eventItem.Occurences; i++)
        {
            Event recurringEvent = _recurrenceDictionary[eventItem.EventFrequency].Invoke(eventItem, i);

            if (recurringEvent is not null)
                recurringEventsByFrequency.Add(recurringEvent);
        }

        return recurringEventsByFrequency;
    }

    public static Event GetRecurringEvent(Event entry, DateTime newDate)
    {
        return new Event(entry.Title,
                        entry.Slug,
                        entry.Teaser,
                        entry.ImageUrl,
                        entry.Description,
                        entry.Fee,
                        entry.Location,
                        entry.SubmittedBy,
                        newDate,
                        entry.StartTime,
                        entry.EndTime,
                        entry.Occurences,
                        entry.EventFrequency,
                        entry.Breadcrumbs,
                        entry.ThumbnailImageUrl,
                        entry.Documents,
                        entry.Categories,
                        entry.MapPosition,
                        entry.Featured,
                        entry.BookingInformation,
                        entry.UpdatedAt,
                        entry.Tags,
                        entry.Group,
                        entry.Alerts,
                        entry.EventCategories,
                        entry.Free,
                        entry.Paid,
                        entry.AccessibleTransportLink,
                        entry.EventBranding,
                        entry.PhoneNumber,
                        entry.Email,
                        entry.Website,
                        entry.Facebook,
                        entry.Instagram,
                        entry.LinkedIn,
                        entry.MetaDescription,
                        entry.Duration,
                        entry.Languages);
    }

    private static DateTime GetCorrespondingMonthsDay(DateTime date, int occurrence)
    {
        DateTime followingMonth = date.AddMonths(occurrence);
        DateTime newDate = GetFirstMatchingDayOfMonth(date, followingMonth);

        return GetWeeklyDayInMonth(date, newDate);
    }

    private static DateTime GetFirstMatchingDayOfMonth(DateTime date, DateTime nextMonth)
    {
        DateTime newDate = new(nextMonth.Year, nextMonth.Month, 1, date.Hour, date.Minute, date.Second, date.Kind);

        while (!newDate.DayOfWeek.Equals(date.DayOfWeek))
            newDate = newDate.AddDays(1);

        return newDate;
    }

    private static DateTime GetWeeklyDayInMonth(DateTime date, DateTime newDate)
    {
        int dayOccurrence = (date.Day - 1) / 7 + 1;

        for (int i = 1; i < dayOccurrence; i++)
        {
            newDate = newDate.AddDays(7);

            if (!newDate.AddDays(7).Month.Equals(newDate.Month))
                break;
        }

        return newDate;
    }
}
