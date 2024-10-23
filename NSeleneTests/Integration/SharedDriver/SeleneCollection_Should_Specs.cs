namespace NSelene.Tests.Integration.SharedDriver.SeleneSpec
{
    [TestFixture]
    public class SeleneCollection_Should_Specs : BaseTest
    {
        // TODO: improve coverage and consider breaking down into separate test classes

        [Test]
        public void Should_HaveCount_WaitsForPresenceInDom_OfInitialyAbsent()
        {
            Given.OpenedEmptyPage();
            Given.OpenedPageWithBodyTimedOut(
                @"
                <p style='display:none'>a</p>
                <p style='display:none'>b</p>
                ",
                PollingPeriod.TotalMilliseconds
            );

            var act = () =>
            {
                SS("p").Should(Have.Count(2));
            };

            Assert.That(act, Does.NotTimeout(PollingPeriod));
        }

        [Test]
        public void Should_HaveNoCount_WaitsForAbsenceInDom_OfInitialyPresent()
        {
            Given.OpenedPageWithBody(
                @"
                <p style='display:none'>a</p>
                <p style='display:none'>b</p>
                "
            );
            Given.WithBodyTimedOut(
                @"
                ",
                PollingPeriod.TotalMilliseconds
            );

            var act = () =>
            {
                SS("p").Should(Have.No.Count(2));
            };

            Assert.That(act, Does.NotTimeout(PollingPeriod));
        }
        
        [Test]
        public void Should_HaveCount_IsRenderedInError_OnAbsentElementTimeoutFailure()
        {
            Given.OpenedEmptyPage();

            var act = () => {
                SS("p").Should(Have.Count(2));
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.All(p).Should(Have.Count = 2)
                Reason:
                    actual: Have.Count = 0
                """));
        }
        
        [Test]
        public void Should_HaveNoCount_IsRenderedInError_OnInDomElementsTimeoutFailure()
        {
            Given.OpenedPageWithBody(
                @"
                <p style='display:none'>a</p>
                <p style='display:none'>b</p>
                "
            );

            var act = () => {
                SS("p").Should(Have.No.Count(2));
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.All(p).Should(Not.Have.Count = 2)
                Reason:
                    condition not matched
                """
                ));
        }

        [Test]
        public void Should_HaveCount_WaitsForAsked_OfInitialyOtherVisibleCount()
        {
            Given.OpenedPageWithBody(
                @"
                <p>a</p>
                "
            );
            Given.OpenedPageWithBodyTimedOut(
                @"
                <p>a</p>
                <p>b</p>
                ",
                PollingPeriod.TotalMilliseconds
            );

            var act = () =>
            {
                SS("p").Should(Have.Count(2));
            };

            Assert.That(act, Does.NotTimeout(PollingPeriod));
        }
    }
}

