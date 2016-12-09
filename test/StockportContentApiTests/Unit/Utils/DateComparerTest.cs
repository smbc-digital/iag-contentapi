using System;
using FluentAssertions;
using Moq;
using StockportContentApi.Utils;
using Xunit;

namespace StockportContentApiTests.Unit.Utils
{
    public class DateComparerTest
    {
        private readonly DateComparer _comparer;
        private readonly Mock<ITimeProvider> _dateNow;

        public DateComparerTest()
        {
            _dateNow = new Mock<ITimeProvider>();
            _comparer = new DateComparer(_dateNow.Object);
        }

        [Fact]
        public void ShouldReturnTrueIfSunriseDateIsWithinTheToAndFromDates()
        {
            _dateNow.Setup(o => o.Now()).Returns(new DateTime(2016, 12, 10));

            var isWithin = _comparer.SunriseDateIsBetweenStartAndEndDates(new DateTime(2016, 8, 5), new DateTime(2016, 8, 1), new DateTime(2016, 8, 31));

            isWithin.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnFalseIfSunriseDateIsOutsideTheToAndFromDates()
        {
            _dateNow.Setup(o => o.Now()).Returns(new DateTime(2016, 12, 10));

            var isWithin = _comparer.SunriseDateIsBetweenStartAndEndDates(new DateTime(2016, 7, 5), new DateTime(2016, 8, 1), new DateTime(2016, 8, 31));

            isWithin.Should().BeFalse();
        }

        [Fact]
        public void ShouldReturnFalseIfSunriseDateIsWithinTheToAndFromDatesButIsAfterTodaysDate()
        {
            _dateNow.Setup(o => o.Now()).Returns(new DateTime(2016, 8, 5));

            var isWithin = _comparer.SunriseDateIsBetweenStartAndEndDates(new DateTime(2016, 8, 10), new DateTime(2016, 8, 1), new DateTime(2016, 8, 31));

            isWithin.Should().BeFalse();
        }

        [Fact]
        public void ShouldReturnADateTimeFromAOffsetDateTime()
        {
            var offsetDateTime = new DateTimeOffset(2016, 09, 21, 0, 0, 0, new TimeSpan(+1, 0, 0));

            var date = DateComparer.DateFieldToDate(offsetDateTime);

            date.Should().Be(new DateTime(2016, 09, 20, 23, 0, 0));
        }

        [Fact]
        public void ShouldReturnADateTimeFromANormalDateTime()
        {
            var inputDate = new DateTime(2016, 01, 20);

            var date = DateComparer.DateFieldToDate(inputDate);

            date.Should().Be(inputDate);
        }

        [Fact]
        public void ShouldReturnMinimumDateTimePossibleFromAInvalidDateString()
        {
            var date = DateComparer.DateFieldToDate("not-valid");

            date.Should().Be(DateTime.MinValue);
        }

        [Fact]
        public void ShouldReturnTrueForSunriseDateIsWithinAndNoSunsetDate()
        {
            _dateNow.Setup(o => o.Now()).Returns(new DateTime(2016, 8, 5));

            var isWithin = _comparer.DateNowIsWithinSunriseAndSunsetDates(new DateTime(2016, 8, 4), DateTime.MinValue);

            isWithin.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnTrueForNoSunriseDateIsWithinAndASunsetDateWhenSunsetDate()
        {
            _dateNow.Setup(o => o.Now()).Returns(new DateTime(2016, 8, 5));

            var isWithin = _comparer.DateNowIsWithinSunriseAndSunsetDates(DateTime.MinValue, new DateTime(2016, 8, 6));

            isWithin.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnTrueForSunriseDateIsWithinAndSunsetDateWithin()
        {
            _dateNow.Setup(o => o.Now()).Returns(new DateTime(2016, 8, 5));

            var isWithin = _comparer.DateNowIsWithinSunriseAndSunsetDates(new DateTime(2016, 8, 4), new DateTime(2016, 8, 6));

            isWithin.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnFalseForSunriseDateIsWithinAndSunsetDateOutside()
        {
            _dateNow.Setup(o => o.Now()).Returns(new DateTime(2016, 8, 5));

            var isWithin = _comparer.DateNowIsWithinSunriseAndSunsetDates(new DateTime(2016, 8, 1), new DateTime(2016, 8, 4));

            isWithin.Should().BeFalse();
        }

        [Fact]
        public void ShouldReturnFalseForSunriseDateIsOutsideAndSunsetDateWithin()
        {
            _dateNow.Setup(o => o.Now()).Returns(new DateTime(2016, 8, 5));

            var isWithin = _comparer.DateNowIsWithinSunriseAndSunsetDates(new DateTime(2016, 8, 6), new DateTime(2016, 8, 10));

            isWithin.Should().BeFalse();
        }
    }
}
