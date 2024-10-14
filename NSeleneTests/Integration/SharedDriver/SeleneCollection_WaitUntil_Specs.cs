namespace NSelene.Tests.Integration.SharedDriver.SeleneElementSpec
{
    [TestFixture]
    public class SeleneCollection_WaitUntil_Specs : BaseTest
    {
        [Test]
        public void ReturnsTrue_AfterWaiting_OnMatched_AfterDelayLessThanTimeout()
        {
            Configuration.Timeout = 2.000;
            var hiddenDelay = 500;
            var visibleDelay = hiddenDelay + 250;
            Given.OpenedEmptyPage();
            var beforeCall = DateTime.Now;
            Given.WithBodyTimedOut(
                "<p id='will-appear' style='display:none'>Hello!</p>", 
                hiddenDelay
            );
            Given.ExecuteScriptWithTimeout(
                "document.getElementsByTagName('p')[0].style = 'display:block';", 
                visibleDelay
            );

            var result = SS("#will-appear").WaitUntil(Have.Texts("Hello!"));
            var afterCall = DateTime.Now;

            Assert.That(result, Is.True);
            Assert.That(afterCall, Is.GreaterThan(beforeCall.AddMilliseconds(visibleDelay)));
            Assert.That(afterCall, Is.LessThan(beforeCall.AddSeconds(Configuration.Timeout)));
        }
        
        [Test]
        public void ReturnsFalse_AfterWaitingTimeout_OnNotMatched_WithExceptionReason()
        {
            Configuration.Timeout = 0.75;
            var beforeCall = DateTime.Now;

            var result = SS("#absent").WaitUntil(Have.Count(2));
            var afterCall = DateTime.Now;

            Assert.That(result, Is.False);
            Assert.That(afterCall, Is.GreaterThanOrEqualTo(beforeCall.AddSeconds(Configuration.Timeout)));
        }
        
        [Test]
        public void ReturnsFalse_AfterWaitingTimeout_OnNotMatched()
        {
            Configuration.Timeout = 0.75;
            var beforeCall = DateTime.Now;
            Given.OpenedPageWithBody(
                "<p id='hidden' style='display:none'>Hello!</p>"
            );

            var result = SS("#hidden").WaitUntil(Have.Texts("Hello!"));
            var afterCall = DateTime.Now;

            Assert.That(result, Is.False);
            Assert.That(afterCall, Is.GreaterThanOrEqualTo(beforeCall.AddSeconds(Configuration.Timeout)));
        }
    }
}

