using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTests
{
    public class TimeSensitiveTests
{
    [Fact]
    public void FailsBetween7And715PDT()
    {
        // This unit test is used to test if CICD is evaluating unit tests
        // This test will fail by design between 7:00pm pst and 7:15pm pst
        // Subsequently a Github commit triggered production deployment will fail between 7:00pm pst and 7:15pm pst

        var pacificNow = DateTime.UtcNow.AddHours(-7).TimeOfDay;

        if (pacificNow >= new TimeSpan(19, 0, 0) && pacificNow <= new TimeSpan(19, 15, 0))
        {
            Assert.True(false, $"Failing test: It's between 7:00 PM and 7:15 PM PDT. Time now: {pacificNow}");
        }

        Assert.True(true);
    }
}

}