namespace StockportContentApiTests.Unit.Utils;

public class DateComparerTests
{
    private readonly DateComparer _comparer;
    private readonly Mock<ITimeProvider> _dateNow = new();

    public DateComparerTests() =>
        _comparer = new(_dateNow.Object);

    [Fact]
    public void SunriseDateIsBetweenStartAndEndDates_ShouldReturnTrueIfSunriseDateIsWithinTheToAndFromDates()
    {
        // Arrange
        _dateNow
            .Setup(date => date.Now())
            .Returns(new DateTime(2016, 12, 10));

        // Act
        bool isWithin = _comparer.SunriseDateIsBetweenStartAndEndDates(new DateTime(2016, 8, 5), new DateTime(2016, 8, 1), new DateTime(2016, 8, 31));

        // Assert
        Assert.True(isWithin);
    }

    [Fact]
    public void SunriseDateIsBetweenStartAndEndDates_ShouldReturnTrueIfSunriseDateIsWithinTheToAndFromDatesWithTimes()
    {
        // Arrange
        _dateNow
            .Setup(date => date.Now())
            .Returns(new DateTime(2016, 12, 10, 9, 35, 07));

        // Act
        bool isWithin = _comparer.SunriseDateIsBetweenStartAndEndDates(new DateTime(2016, 8, 5, 15, 15, 0), new DateTime(2016, 8, 1, 0, 0, 0), new DateTime(2016, 8, 31, 0, 0, 0));

        // Assert
        Assert.True(isWithin);
    }

    [Fact]
    public void SunriseDateIsBetweenStartAndEndDates_ShouldReturnFalseIfSunriseDateIsOutsideTheToAndFromDates()
    {
        // Arrange
        _dateNow
            .Setup(date => date.Now())
            .Returns(new DateTime(2016, 12, 10));

        // Act
        bool isWithin = _comparer.SunriseDateIsBetweenStartAndEndDates(new DateTime(2016, 7, 5), new DateTime(2016, 8, 1), new DateTime(2016, 8, 31));

        // Assert
        Assert.False(isWithin);
    }

    [Fact]
    public void SunriseDateIsBetweenStartAndEndDates_ShouldReturnFalseIfSunriseDateIsWithinTheToAndFromDatesButIsAfterTodaysDate()
    {
        // Arrange
        _dateNow
            .Setup(date => date.Now())
            .Returns(new DateTime(2016, 8, 5));

        // Act
        bool isWithin = _comparer.SunriseDateIsBetweenStartAndEndDates(new DateTime(2016, 8, 10), new DateTime(2016, 8, 1), new DateTime(2016, 8, 31));

        // Assert
        Assert.False(isWithin);
    }

    [Fact]
    public void DateFieldToDate_ShouldReturnADateTimeFromAOffsetDateTime()
    {
        // Arrange
        DateTimeOffset offsetDateTime = new(2016, 09, 21, 0, 0, 0, new TimeSpan(+1, 0, 0));

        // Act
        DateTime date = DateComparer.DateFieldToDate(offsetDateTime);

        // Assert
        Assert.Equal(new DateTime(2016, 09, 20, 23, 0, 0), date);
    }

    [Fact]
    public void DateFieldToDate_ShouldReturnADateTimeFromANormalDateTime()
    {
        // Arrange
        DateTime inputDate = new(2016, 01, 20);

        // Act
        DateTime date = DateComparer.DateFieldToDate(inputDate);

        // Assert
        Assert.Equal(inputDate, date);
    }

    [Fact]
    public void DateFieldToDate_ShouldReturnMinimumDateTimePossibleFromAInvalidDateString()
    {
        // Act
        DateTime date = DateComparer.DateFieldToDate("not-valid");

        // Assert
        Assert.Equal(DateTime.MinValue, date);
    }

    [Fact]
    public void DateNowIsWithinSunriseAndSunsetDates_ShouldReturnTrueForSunriseDateIsWithinAndNoSunsetDate()
    {
        // Arrange
        _dateNow
            .Setup(date => date.Now())
            .Returns(new DateTime(2016, 8, 5));

        // Act
        bool isWithin = _comparer.DateNowIsWithinSunriseAndSunsetDates(new DateTime(2016, 8, 4), DateTime.MinValue);

        // Assert
        Assert.True(isWithin);
    }

    [Fact]
    public void DateNowIsWithinSunriseAndSunsetDates_ShouldReturnTrueForNoSunriseDateIsWithinAndASunsetDateWhenSunsetDate()
    {
        // Arrange
        _dateNow
            .Setup(date => date.Now())
            .Returns(new DateTime(2016, 8, 5));

        // Act
        bool isWithin = _comparer.DateNowIsWithinSunriseAndSunsetDates(DateTime.MinValue, new DateTime(2016, 8, 6));

        // Assert
        Assert.True(isWithin);
    }

    [Fact]
    public void DateNowIsWithinSunriseAndSunsetDates_ShouldReturnTrueForSunriseDateIsWithinAndSunsetDateWithin()
    {
        // Arrange
        _dateNow
            .Setup(date => date.Now())
            .Returns(new DateTime(2016, 8, 5));

        // Act
        bool isWithin = _comparer.DateNowIsWithinSunriseAndSunsetDates(new DateTime(2016, 8, 4), new DateTime(2016, 8, 6));

        // Assert
        Assert.True(isWithin);
    }

    [Fact]
    public void DateNowIsWithinSunriseAndSunsetDates_ShouldReturnFalseForSunriseDateIsWithinAndSunsetDateOutside()
    {
        // Arrange
        _dateNow
            .Setup(date => date.Now())
            .Returns(new DateTime(2016, 8, 5));

        // Act
        bool isWithin = _comparer.DateNowIsWithinSunriseAndSunsetDates(new DateTime(2016, 8, 4), new DateTime(2016, 8, 1));

        // Assert
        Assert.False(isWithin);
    }

    [Fact]
    public void DateNowIsWithinSunriseAndSunsetDates_ShouldReturnFalseForSunriseDateIsOutsideAndSunsetDateWithin()
    {
        // Arrange
        _dateNow
            .Setup(date => date.Now())
            .Returns(new DateTime(2016, 8, 5));

        // Act
        bool isWithin = _comparer.DateNowIsWithinSunriseAndSunsetDates(new DateTime(2016, 8, 6), new DateTime(2016, 8, 10));

        // Assert
        Assert.False(isWithin);
    }

    [Fact]
    public void EventIsInTheFuture_ShouldReturnTrue_WhenEventDateIsInTheFuture()
    {
        // Act
        bool result = _comparer.EventIsInTheFuture(DateTime.Now.AddDays(1), "10:00", "11:00");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void EventIsInTheFuture_ShouldReturnFalse_WhenEventDateIsInThePast()
    {
        // Arrange
        _dateNow
            .Setup(date => date.Now())
            .Returns(DateTime.Now);

        // Act
        bool result = _comparer.EventIsInTheFuture(DateTime.Now.AddDays(-1), "10:00", "11:00");
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void EventIsInTheFuture_ShouldReturnTrue_WhenEventIsToday_AndEndTimeIsInTheFuture()
    {
        // Arrange
        DateTime eventDate = DateTime.Now.Date;
        string endTime = DateTime.Now.AddHours(2).ToString("HH:mm");
        string startTime = DateTime.Now.AddHours(-2).ToString("HH:mm");

        // Act
        bool result = _comparer.EventIsInTheFuture(eventDate, startTime, endTime);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void EventIsInTheFuture_ShouldReturnFalse_WhenEventIsToday_AndEndTimeIsInThePast()
    {
        // Arrange
        _dateNow
            .Setup(date => date.Now())
            .Returns(DateTime.Now);

        // Act
        bool result = _comparer.EventIsInTheFuture(DateTime.Now, DateTime.Now.AddHours(-3).ToString("HH:mm"), DateTime.Now.AddHours(-1).ToString("HH:mm"));
       
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void EventIsInTheFuture_ShouldReturnFalse_WhenEventIsToday_AndStartTimeIsInTheFuture_ButEndTimeIsInvalid()
    {
        // Arrange
        _dateNow
            .Setup(date => date.Now())
            .Returns(DateTime.Now);

        // Act
        bool result = _comparer.EventIsInTheFuture(DateTime.Now, "13:00", "invalid");
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ReturnsFalse_WhenEventIsToday_AndStartTimeIsInThePast_AndEndTimeInvalid()
    {
        // Arrange
        _dateNow
            .Setup(date => date.Now())
            .Returns(DateTime.Now);
        
        // Act
        bool result = _comparer.EventIsInTheFuture(DateTime.Now, "10:00", "invalid");
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ReturnsFalse_WhenEventIsToday_AndBothTimesAreInvalid()
    {
        // Arrange
        _dateNow
            .Setup(date => date.Now())
            .Returns(DateTime.Now);
        
        // Act
        bool result = _comparer.EventIsInTheFuture(DateTime.Now, "notatime", "alsoBad");
        
        // Assert
        Assert.False(result);
    }
}