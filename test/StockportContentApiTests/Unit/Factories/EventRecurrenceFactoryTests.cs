namespace StockportContentApiTests.Unit.Factories;

public class EventRecurrenceFactoryTests
{
    [Fact]
    public void ShouldCreateRecurringEventsAccordingToMonthlyDate()
    {
        // Arrange
        Event eventItem = new EventBuilder()
            .Occurrences(3)
            .Frequency(EventFrequency.MonthlyDate)
            .EventDate(new DateTime(2017, 01, 24))
            .Build();

        // Act
        List<Event> events = new EventRecurrenceFactory().GetRecurringEventsOfEvent(eventItem);

        // Assert
        Assert.Equal(2, events.Count);
        Assert.Equal(new DateTime(2017, 02, 24), events[0].EventDate);
        Assert.Equal(new DateTime(2017, 03, 24), events[1].EventDate);
    }

    [Fact]
    public void ShouldCreateRecurringEventsAccordingToMonthlyDay()
    {
        // Arrange
        Event eventItem = new EventBuilder()
            .Occurrences(3)
            .Frequency(EventFrequency.MonthlyDay)
            .EventDate(new DateTime(2017, 01, 19))
            .Build();

        // Act
        List<Event> events = new EventRecurrenceFactory().GetRecurringEventsOfEvent(eventItem);

        // Assert
        Assert.Equal(2, events.Count);
        Assert.Equal(new DateTime(2017, 02, 16), events[0].EventDate);
        Assert.Equal(new DateTime(2017, 03, 16), events[1].EventDate);
        Assert.Equal(DayOfWeek.Thursday, events[0].EventDate.DayOfWeek);
        Assert.Equal(DayOfWeek.Thursday, events[1].EventDate.DayOfWeek);
    }

    [Fact]
    public void ShouldCreateRecurringEventsAccordingToMonthlyDayOnLastOccurenceOfDayInMonth()
    {
        // Arrange
        Event eventItem = new EventBuilder()
            .Occurrences(3)
            .Frequency(EventFrequency.MonthlyDay)
            .EventDate(new DateTime(2017, 01, 29))
            .Build();

        // Act
        List<Event> events = new EventRecurrenceFactory().GetRecurringEventsOfEvent(eventItem);

        // Assert
        Assert.Equal(2, events.Count);
        Assert.Equal(new DateTime(2017, 02, 26), events[0].EventDate);
        Assert.Equal(DayOfWeek.Sunday, events[0].EventDate.DayOfWeek);
        Assert.Equal(new DateTime(2017, 03, 26), events[1].EventDate);
        Assert.Equal(DayOfWeek.Sunday, events[1].EventDate.DayOfWeek);
    }
}