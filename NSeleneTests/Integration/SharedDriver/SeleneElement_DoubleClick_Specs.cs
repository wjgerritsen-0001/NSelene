namespace NSelene.Tests.Integration.SharedDriver.SeleneSpec
{
    [TestFixture]
    public class SeleneElement_DoubleClick_Specs : BaseTest
    {
        // TODO: move here error messages tests, and at least some ClickByJs tests...
        
        [Test]
        public void DoubleClick_WaitsForVisibility_OfInitialyAbsent()
        {
            Given.OpenedEmptyPage();
            Given.OpenedPageWithBodyTimedOut(
                @"
                <span ondblclick='window.location=this.href + ""#second""'>to h2</span>
                <h2 id='second'>Heading 2</h2>
                ",
                PollingPeriod.TotalMilliseconds
            );

            var act = () =>
            {
                S("span").DoubleClick();
            };

            Assert.That(act, Does.NotTimeout(PollingPeriod));
            Assert.That(Configuration.Driver.Url, Does.Contain("second"));
        }

        [Test]
        public void DoubleClick_WaitsForVisibility_OfInitialyHidden()
        {
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
            Given.ExecuteScriptWithTimeout(
                @"
                document.getElementById('link').style.display = 'block';
                ",
                PollingPeriod.TotalMilliseconds);

            var act = () =>
            {
                S("span").DoubleClick();
            };
           
            Assert.That(act, Does.NotTimeout(PollingPeriod));
            Assert.That(Configuration.Driver.Url, Does.Contain("second"));
        }

        [Test]
        public void DoubleClick_IsRenderedInError_OnHiddenFailure()
        {
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

            var act = () => {
                S("span").DoubleClick();
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(span).Actions.DoubleClick(self.ActualWebElement).Perform()
                Reason:
                    javascript error: {"status":60,"value":"[object HTMLSpanElement] has no size and location"}
                """
                ));
            Assert.That(Configuration.Driver.Url, Does.Not.Contain("second"));
        }

        [Test]
        public void DoubleClick_IsRenderedInError_OnHiddenFailure_WhenCustomizedToWaitForNoOverlap()
        {
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

            var act = () =>
            {
                S("span").With(waitForNoOverlapFoundByJs: true).DoubleClick();
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(span).Actions.DoubleClick(self.ActualNotOverlappedWebElement).Perform()
                Reason:
                    javascript error: element is not visible
                """));
            Assert.That(Configuration.Driver.Url, Does.Not.Contain("second"));
        }

        [Test]
        public void DoubleClick_PassesWithoutEffect_UnderOverlay()
        {
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

            var act = () =>
            {
                S("span").DoubleClick();
            };

            Assert.That(act, Does.NotTimeout());
            Assert.That(Configuration.Driver.Url, Does.Not.Contain("second"));
        }

        [Test]
        public void DoubleClick_Waits_For_NoOverlay_IfCustomized()
        {
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
            Given.ExecuteScriptWithTimeout(
                @"
                document.getElementById('overlay').style.display = 'none';
                ",
                PollingPeriod.TotalMilliseconds
            );

            var act = () =>
            {
                S("span").With(waitForNoOverlapFoundByJs: true).DoubleClick();
            };

            Assert.That(act, Does.NotTimeout(PollingPeriod));
            Assert.That(Configuration.Driver.Url, Does.Contain("second"));
        }

        [Test]
        public void DoubleClick_IsRenderedInError_OnOverlappedWithOverlayFailure_IfCustomizedToWaitForNoOverlayByJs()
        {
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

            var act = () =>
            {
                S("span").With(waitForNoOverlapFoundByJs: true).DoubleClick();
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(span).Actions.DoubleClick(self.ActualNotOverlappedWebElement).Perform()
                Reason:
                    Element: <span id="link" ondblclick="window.location=this.href + &quot;#second&quot;">to h2</span>
                    is overlapped by: <div id="overlay" style=
                """));
            Assert.That(Configuration.Driver.Url, Does.Not.Contain("second"));
        }
    }
}

