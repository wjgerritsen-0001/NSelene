namespace NSelene.Tests.Integration.Condition
{
    public class Should_HaveUrl_Specs : BaseTest
    {
        [Test]
        public void Should_HaveUrlContaining_OfInitialOtherText()
        {
            Given.OpenedPageWithBody("""
                <a href='#first' style='display:none'>go to Heading 1</a>
                <a href='#second' style='display:none'>go to Heading 1</a>
                """);
            Given.ExecuteScriptWithTimeout("""
                document.getElementsByTagName('a')[1].click();
                """,
                PollingPeriod.TotalMilliseconds
            );

            var act = () =>
            {
                Selene.Should(Have.UrlContaining("second"));
            };

            Assert.That(act, Does.NotTimeout(PollingPeriod));
        }
        [Test]
        public void Should_HaveUrlContaining_OtherText()
        {
            Given.OpenedEmptyPage();

            var act = () =>
            {
                Selene.Should(Have.UrlContaining("XXX"));
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Should(Have.UrlContaining(«XXX»))
                Reason:
                    Actual url: «{{EmptyHtmlUri}}»
                """));
        }
        [Test]
        public void Should_HaveNoUrlContaining_OfInitialOtherText()
        {
            Given.OpenedPageWithBody("""
                <a href='#first' style='display:none'>go to Heading 1</a>
                <a href='#second' style='display:none'>go to Heading 1</a>
                """);
            Given.ExecuteScriptWithTimeout("""
                document.getElementsByTagName('a')[0].click();
                """,
                0
            );
            Selene.Should(Have.UrlContaining("first"));
            Given.ExecuteScriptWithTimeout("""
                document.getElementsByTagName('a')[1].click();
                """,
                PollingPeriod.TotalMilliseconds
            );

            var act = () =>
            {
                Selene.Should(Have.No.UrlContaining("first"));
            };

            Assert.That(act, Does.NotTimeout(PollingPeriod));
        }
        [Test]
        public void Should_HaveUrl_OfInitialOtherText()
        {
            Given.OpenedPageWithBody("""
                <a href='#first' style='display:none'>go to Heading 1</a>
                <a href='#second' style='display:none'>go to Heading 1</a>
                """);
            Given.ExecuteScriptWithTimeout("""
                document.getElementsByTagName('a')[0].click();
                """,
                PollingPeriod.TotalMilliseconds
            );

            var act = () =>
            {
                Selene.Should(Have.UrlContaining("first"));
            };

            Assert.That(act, Does.NotTimeout(PollingPeriod));
        }
        [Test]
        public void Should_HaveUrl_OtherText()
        {
            var UrlValue = nameof(Should_HaveUrl_OtherText);
            Given.OpenedEmptyPage();

            var act = () =>
            {
                Selene.Should(Have.Url(UrlValue.Substring(5, 5)));
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Should(Have.Url(«d_Hav»))
                Reason:
                    Actual url: «{{EmptyHtmlUri}}»
                """));
        }
        [Test]
        public void Should_HaveNoUrl_OfInitialOtherText()
        {
            Given.OpenedPageWithBody("""
                <a href='#first' style='display:none'>go to Heading 1</a>
                <a href='#second' style='display:none'>go to Heading 1</a>
                """);
            Given.ExecuteScriptWithTimeout("""
                document.getElementsByTagName('a')[0].click();
                """,
                0
            );
            Selene.Should(Have.UrlContaining("first"));
            Given.ExecuteScriptWithTimeout("""
                document.getElementsByTagName('a')[1].click();
                """,
                PollingPeriod.TotalMilliseconds
            );

            var act = () =>
            {
                Selene.Should(Have.No.Url($"{EmptyHtmlUri}#first"));
            };

            Assert.That(act, Does.NotTimeout(PollingPeriod));
        }
    }
}
