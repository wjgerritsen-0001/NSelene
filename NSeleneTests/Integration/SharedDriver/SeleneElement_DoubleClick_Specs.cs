using NUnit.Framework;
using static NSelene.Selene;
using System;
using NSelene.Tests.Integration.SharedDriver.Harness;
using Microsoft.Extensions.Time.Testing;
using System.Threading;
using System.Threading.Tasks;

namespace NSelene.Tests.Integration.SharedDriver.SeleneSpec
{
    [TestFixture]
    public class SeleneElement_DoubleClick_Specs : BaseTest
    {
        // TODO: move here error messages tests, and at least some ClickByJs tests...
        
        [Test]
        public void DoubleClick_WaitsForVisibility_OfInitiialyAbsent()
        {
            Configuration.Timeout = 1.0;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedEmptyPage();
            var beforeCall = _timeProvider.GetTimestamp();
            int delay = 200;

            Given.OpenedPageWithBodyTimedOut(
                @"
                <span ondblclick='window.location=this.href + ""#second""'>to h2</span>
                <h2 id='second'>Heading 2</h2>
                ",
                delay
            );
            // increment the FakeTimeProvider slowly until 3 seconds are passed with increments of 100ms
            using (var advancer = FakeTimeProviderExtensions.FakeTimeProviderExtensions.TimeAdvancer(_timeProvider, advance: TimeSpan.FromSeconds(3), increment: TimeSpan.FromMilliseconds(100)))
            {
                S("span").DoubleClick();
            }
            Assert.That(_timeProvider.GetElapsedTime(beforeCall), Is.GreaterThan(TimeSpan.FromMilliseconds(delay)));
            Assert.That(_timeProvider.GetElapsedTime(beforeCall), Is.LessThan(TimeSpan.FromSeconds(Configuration.Timeout)));
            Assert.IsTrue(Configuration.Driver.Url.Contains("second"));
        }

        [Test]
        public void DoubleClick_WaitsForVisibility_OfInitiialyHidden()
        {
            Configuration.Timeout = 1.0;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <span 
                  id='link' 
                  ondblclick='window.location=this.href + ""#second""' 
                  style='display:none'
                >to h2</span>
                <h2 id='second'>Heading 2</h2>
                "
            );
            var beforeCall = DateTime.Now;
            Given.ExecuteScriptWithTimeout(
                @"
                document.getElementById('link').style.display = 'block';
                ",
                300
            );

            S("span").DoubleClick();

            var afterCall = DateTime.Now;
            Assert.Greater(afterCall, beforeCall.AddSeconds(0.3));
            Assert.Less(afterCall, beforeCall.AddSeconds(1.0));
            StringAssert.Contains("second", Configuration.Driver.Url);
        }

        [Test]
        public void DoubleClick_IsRenderedInError_OnHiddenFailure()
        {
            Configuration.Timeout = 0.25;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <span 
                  id='link' 
                  ondblclick='window.location=this.href + ""#second""' 
                  style='display:none'
                >to h2</span>
                <h2 id='second'>Heading 2</h2>
                "
            );
            var beforeCall = DateTime.Now;

            try 
            {
                S("span").DoubleClick();
                Assert.Fail("should fail with exception");
            }

            catch (TimeoutException error)
            {
                var afterCall = DateTime.Now;
                Assert.Greater(afterCall, beforeCall.AddSeconds(0.25));
                Assert.Less(afterCall, beforeCall.AddSeconds(1.0));

                Assert.That(error.Message.Trim(), Does.Contain($$"""
                Timed out after {{0.25}}s, while waiting for:
                    Browser.Element(span).Actions.DoubleClick(self.ActualWebElement).Perform()
                Reason:
                    javascript error: {"status":60,"value":"[object HTMLSpanElement] has no size and location"}
                """.Trim()
                ));

                StringAssert.DoesNotContain("second", Configuration.Driver.Url);
            }
        }

        [Test]
        public void DoubleClick_IsRenderedInError_OnHiddenFailure_WhenCustomizedToWaitForNoOverlap()
        {
            Configuration.Timeout = 0.25;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <span 
                  id='link' 
                  ondbclick='window.location=this.href + ""#second""' 
                  style='display:none'
                >to h2</span>
                <h2 id='second'>Heading 2</h2>
                "
            );
            var beforeCall = DateTime.Now;

            try 
            {
                S("span").With(waitForNoOverlapFoundByJs: true).DoubleClick();
                Assert.Fail("should fail with exception");
            }

            catch (TimeoutException error)
            {
                var afterCall = DateTime.Now;
                Assert.Greater(afterCall, beforeCall.AddSeconds(0.25));
                Assert.Less(afterCall, beforeCall.AddSeconds(1.0));

                Assert.That(error.Message.Trim(), Does.Contain($$"""
                Timed out after {{0.25}}s, while waiting for:
                    Browser.Element(span).Actions.DoubleClick(self.ActualNotOverlappedWebElement).Perform()
                Reason:
                    javascript error: element is not visible
                """.Trim()
                ));

                StringAssert.DoesNotContain("second", Configuration.Driver.Url);
            }
        }

        [Test]
        public void DoubleClick_PassesWithoutEffect_UnderOverlay()
        {
            Configuration.Timeout = 1.0;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <div 
                    id='overlay' 
                    style='
                        display:block;
                        position: fixed;
                        display: block;
                        width: 100%;
                        height: 100%;
                        top: 0;
                        left: 0;
                        right: 0;
                        bottom: 0;
                        background-color: rgba(0,0,0,0.1);
                        z-index: 2;
                        cursor: pointer;
                    '
                >
                </div>

                <span 
                  id='link' 
                  ondblclick='window.location=this.href + ""#second""' 
                >to h2</span>
                <h2 id='second'>Heading 2</h2>
                "
            );
            var beforeCall = DateTime.Now;

            S("span").DoubleClick();

            var afterCall = DateTime.Now;
            Assert.Less(afterCall, beforeCall.AddSeconds(1.0));
            StringAssert.DoesNotContain("second", Configuration.Driver.Url);
        }

        [Test]
        public void DoubleClick_Waits_For_NoOverlay_IfCustomized()
        {
            Configuration.Timeout = 1.0;
            Configuration.PollDuringWaits = 0.05;

            Given.OpenedPageWithBody(
                @"
                <div 
                    id='overlay' 
                    style='
                        display:block;
                        position: fixed;
                        display: block;
                        width: 100%;
                        height: 100%;
                        top: 0;
                        left: 0;
                        right: 0;
                        bottom: 0;
                        background-color: rgba(0,0,0,0.1);
                        z-index: 2;
                        cursor: pointer;
                    '
                >
                </div>

                <span 
                  id='link' 
                  ondblclick='window.location=this.href + ""#second""' 
                >to h2</span>
                <h2 id='second'>Heading 2</h2>
                "
            );
            var beforeCall = DateTime.Now;
            Given.ExecuteScriptWithTimeout(
                @"
                document.getElementById('overlay').style.display = 'none';
                ",
                300
            );

            S("span").With(waitForNoOverlapFoundByJs: true).DoubleClick();

            var afterCall = DateTime.Now;
            Assert.Greater(afterCall, beforeCall.AddSeconds(0.3));
            Assert.Less(afterCall, beforeCall.AddSeconds(1.0));
            StringAssert.Contains("second", Configuration.Driver.Url);
        }

        [Test]
        public void DoubleClick_IsRenderedInError_OnOverlappedWithOverlayFailure_IfCustomizedToWaitForNoOverlayByJs()
        {
            Configuration.Timeout = 0.25;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <div 
                    id='overlay' 
                    style='
                        display: block;
                        position: fixed;
                        display: block;
                        width: 100%;
                        height: 100%;
                        top: 0;
                        left: 0;
                        right: 0;
                        bottom: 0;
                        background-color: rgba(0,0,0,0.1);
                        z-index: 2;
                        cursor: pointer;
                    '
                >
                </div>

                <span 
                  id='link' 
                  ondblclick='window.location=this.href + ""#second""' 
                >to h2</span>
                <h2 id='second'>Heading 2</h2>
                "
            );
            var beforeCall = DateTime.Now;

            try 
            {
                S("span").With(waitForNoOverlapFoundByJs: true).DoubleClick();
                
                Assert.Fail("should fail with exception");
            }

            catch (TimeoutException error)
            {
                var afterCall = DateTime.Now;
                Assert.Greater(afterCall, beforeCall.AddSeconds(0.25));
                Assert.Less(afterCall, beforeCall.AddSeconds(1.25));
                
                StringAssert.DoesNotContain("second", Configuration.Driver.Url);

                Assert.That(error.Message.Trim(), Does.Contain($$"""
                Timed out after {{0.25}}s, while waiting for:
                    Browser.Element(span).Actions.DoubleClick(self.ActualNotOverlappedWebElement).Perform()
                Reason:
                    Element: <span id="link" ondblclick="window.location=this.href + &quot;#second&quot;">to h2</span>
                    is overlapped by: <div id="overlay" style=
                """.Trim()
                ));
            }
        }
    }
}

