namespace NSelene.Tests.Integration.Condition
{
    public class Should_HaveTitle_Specs : BaseTest
    {
        [Test]
        public void Should_HaveTitleContaining_OfInitialOtherText()
        {
            var titleValue = nameof(Should_HaveTitleContaining_OfInitialOtherText);
            Given.OpenedEmptyPage();
            Given.WithPageTimedOut($$"""
                <html>
                    <head>
                      <title>{{titleValue}}</title>
                    </head>
                </html>
                """,
                PollingPeriod.TotalMilliseconds
            );

            var act = () =>
            {
                Selene.Should(Have.TitleContaining(titleValue.Substring(5, 5)));
            };

            Assert.That(act, Does.NotTimeout(PollingPeriod));
        }
        [Test]
        public void Should_HaveTitleContaining_OtherText()
        {
            var titleValue = nameof(Should_HaveTitleContaining_OfInitialOtherText);
            Given.OpenedEmptyPage();

            var act = () =>
            {
                Selene.Should(Have.TitleContaining(titleValue.Substring(5, 5)));
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Should(Have.TitleContaining(«d_Hav»))
                Reason:
                    Actual title: «start page»
                """));
        }
        [Test]
        public void Should_HaveNoTitleContaining_OfInitialOtherText()
        {
            var titleValue = nameof(Should_HaveNoTitleContaining_OfInitialOtherText);
            Given.WithPage($$"""
                <html>
                    <head>
                      <title>{{titleValue}}</title>
                    </head>
                </html>
                """
            );
            Given.WithPageTimedOut(
                "<html/>",
                PollingPeriod.TotalMilliseconds);

            var act = () =>
            {
                Selene.Should(Have.No.TitleContaining(titleValue.Substring(5, 5)));
            };

            Assert.That(act, Does.NotTimeout(PollingPeriod));
        }
        [Test]
        public void Should_HaveTitle_OfInitialOtherText()
        {
            var titleValue = nameof(Should_HaveTitle_OfInitialOtherText);
            Given.OpenedEmptyPage();
            Given.WithPageTimedOut($$"""
                <html>
                    <head>
                      <title>{{titleValue}}</title>
                    </head>
                </html>
                """,
                PollingPeriod.TotalMilliseconds
            );

            var act = () =>
            {
                Selene.Should(Have.Title(titleValue));
            };

            Assert.That(act, Does.NotTimeout(PollingPeriod));
        }
        [Test]
        public void Should_HaveTitle_OtherText()
        {
            var titleValue = nameof(Should_HaveTitle_OtherText);
            Given.OpenedEmptyPage();

            var act = () =>
            {
                Selene.Should(Have.Title(titleValue.Substring(5, 5)));
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Should(Have.Title(«d_Hav»))
                Reason:
                    Actual title: «start page»
                """));
        }
        [Test]
        public void Should_HaveNoTitle_OfInitialOtherText()
        {
            var titleValue = nameof(Should_HaveNoTitle_OfInitialOtherText);
            Given.WithPage($$"""
                <html>
                    <head>
                      <title>{{titleValue}}</title>
                    </head>
                </html>
                """
            );
            Given.WithPageTimedOut(
                "<html/>",
                PollingPeriod.TotalMilliseconds);

            var act = () =>
            {
                Selene.Should(Have.No.Title(titleValue));
            };

            Assert.That(act, Does.NotTimeout(PollingPeriod));
        }

    }
}
