namespace StockportContentApiTests.Unit.Factories;

public class EventRecurrenceFactoryTests
{
    [Fact]
    public void ShouldCreateRecurringEventsAccordingToMonthlyDate()
    {
        Event eventItem = new EventBuilder().Occurrences(3).Frequency(EventFrequency.MonthlyDate).EventDate(new DateTime(2017, 01, 24)).Build();

        List<Event> events = new EventRecurrenceFactory().GetRecurringEventsOfEvent(eventItem);

        events.Count.Should().Be(2);
        events[0].EventDate.Should().Be(new DateTime(2017, 02, 24));
        events[1].EventDate.Should().Be(new DateTime(2017, 03, 24));
    }

    [Fact]
    public void ShouldCreateRecurringEventsAccordingToMonthlyDay()
    {
        Event eventItem = new EventBuilder().Occurrences(3).Frequency(EventFrequency.MonthlyDay).EventDate(new DateTime(2017, 01, 19)).Build();

        List<Event> events = new EventRecurrenceFactory().GetRecurringEventsOfEvent(eventItem);

        events.Count.Should().Be(2);
        events[0].EventDate.Should().Be(new DateTime(2017, 02, 16));
        events[0].EventDate.DayOfWeek.Should().Be(DayOfWeek.Thursday);
        events[1].EventDate.Should().Be(new DateTime(2017, 03, 16));
        events[1].EventDate.DayOfWeek.Should().Be(DayOfWeek.Thursday);
    }

    [Fact]
    public void ShouldCreateRecurringEventsAccordingToMonthlyDayOnLastOccurenceOfDayInMonth()
    {
        Event eventItem = new EventBuilder().Occurrences(3).Frequency(EventFrequency.MonthlyDay).EventDate(new DateTime(2017, 01, 29)).Build();

        List<Event> events = new EventRecurrenceFactory().GetRecurringEventsOfEvent(eventItem);

        events.Count.Should().Be(2);
        events[0].EventDate.Should().Be(new DateTime(2017, 02, 26));
        events[0].EventDate.DayOfWeek.Should().Be(DayOfWeek.Sunday);
        events[1].EventDate.Should().Be(new DateTime(2017, 03, 26));
        events[1].EventDate.DayOfWeek.Should().Be(DayOfWeek.Sunday);
    }
}
