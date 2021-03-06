using NSelene;
using static NSelene.Selene;
using NUnit.Framework;
using OpenQA.Selenium;

namespace NSelene.Tests.Integration.SharedDriver
{
    using Harness;

    [TestFixture]
    public class ErrorMessagesSpecs : BaseTest
    {
        // todo: make it "condition tests"

        [TearDown]
        public void TeardownTest()
        {
            Configuration.Timeout = 4;
        }

        [Test]
        public void SeleneElement_ShouldNot_FailsWhenAssertingNotVisibleForVisibleElement()
        {
            Configuration.Timeout = 0.2;
            Given.OpenedPageWithBody("<input id='new-text' type='text' value='ku ku'/>");
            Assert.Throws(Is.TypeOf(typeof(WebDriverTimeoutException))
                          .And.Message.Contains("not " + Be.Visible.GetType().Name), () => {
                S("#new-text").ShouldNot(Be.Visible);
            });
        }

        [Test]
        public void SeleneElement_Should_FailsWhenAssertingVisibleForHiddenElement()
        {
            Configuration.Timeout = 0.2;
            Given.OpenedPageWithBody("<input id='new-text' type='text' value='ku ku' style='display:none'/>");
            Assert.Throws(Is.TypeOf(typeof(WebDriverTimeoutException))
                          .And.Message.Not.Contains("not " + Be.Visible.GetType().Name)
                          .And.Message.Contains(Be.Visible.GetType().Name), () => {
                S("#new-text").Should(Be.Visible);
            });
        }
    }
}

