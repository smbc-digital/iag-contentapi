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
    }
}
