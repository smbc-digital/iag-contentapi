namespace StockportContentApiTests.Unit.Factories;

public class EventReccurenceFactoryTest
{
    [Fact]
    public void ShouldCreateReccuringEventsAccordingToMonthlyDate()
    {
        var eventItem = new EventBuilder().Occurrences(3).Frequency(EventFrequency.MonthlyDate).EventDate(new DateTime(2017, 01, 24)).Build();

        var events = new EventReccurenceFactory().GetReccuringEventsOfEvent(eventItem);

        events.Count.Should().Be(2);
        events[0].EventDate.Should().Be(new DateTime(2017, 02, 24));
        events[1].EventDate.Should().Be(new DateTime(2017, 03, 24));
    }

    [Fact]
    public void ShouldCreateReccuringEventsaccordingToMonthlyDay()
    {
        var eventItem = new EventBuilder().Occurrences(3).Frequency(EventFrequency.MonthlyDay).EventDate(new DateTime(2017, 01, 19)).Build();

        var events = new EventReccurenceFactory().GetReccuringEventsOfEvent(eventItem);

        events.Count.Should().Be(2);
        events[0].EventDate.Should().Be(new DateTime(2017, 02, 16));
        events[0].EventDate.DayOfWeek.Should().Be(DayOfWeek.Thursday);
        events[1].EventDate.Should().Be(new DateTime(2017, 03, 16));
        events[1].EventDate.DayOfWeek.Should().Be(DayOfWeek.Thursday);
    }

    [Fact]
    public void ShouldCreateReccuringEventsaccordingToMonthlyDayOnLastOccurenceOfDayInMonth()
    {
        var eventItem = new EventBuilder().Occurrences(3).Frequency(EventFrequency.MonthlyDay).EventDate(new DateTime(2017, 01, 29)).Build();

        var events = new EventReccurenceFactory().GetReccuringEventsOfEvent(eventItem);

        events.Count.Should().Be(2);
        events[0].EventDate.Should().Be(new DateTime(2017, 02, 26));
        events[0].EventDate.DayOfWeek.Should().Be(DayOfWeek.Sunday);
        events[1].EventDate.Should().Be(new DateTime(2017, 03, 26));
        events[1].EventDate.DayOfWeek.Should().Be(DayOfWeek.Sunday);
    }
}
