namespace NSelene.Tests.Integration.SharedDriver.SeleneSpec
{
    [TestFixture]
    public class SeleneElement_Should_Specs : BaseTest
    {
        // TODO: improve coverage and consider breaking down into separate test classes

        [Test]
        public void Should_HaveValue_WaitsForPresenceInDom_OfInitialyAbsent()
        {
            Given.OpenedEmptyPage();
            Given.OpenedPageWithBodyTimedOut(
                @"
                <input value='initial' style='display:none'></input>
                ",
                PollingPeriod.TotalMilliseconds
            );

            var act = () =>
            {
                S("input").Should(Have.Value("initial"));
            };

            Assert.That(act, Does.NotTimeout(PollingPeriod));
        }

        [Test]
        public void Should_HaveNoValue_WaitsForAbsenceInDom_OfInitialyPresent()
        {
            Given.OpenedPageWithBody(
                @"
                <input value='initial' style='display:none'></input>
                "
            );
            Given.WithBodyTimedOut(
                @"
                ",
                PollingPeriod.TotalMilliseconds
            );

            var act = () =>
            {
                S("input").Should(Have.No.Value("initial"));
            };

            Assert.That(act, Does.NotTimeout(PollingPeriod));
            try
            {
                Configuration.Driver.FindElement(By.TagName("input")).GetAttribute("value");
                Assert.Fail("should fail because the element should be already absent");
            }
            catch (WebDriverException error)
            {
                Assert.That(
                    error.Message, Does.Contain("no such element: Unable to locate element: ")
                );
            }
        }
        
        [Test]
        public void Should_HaveValue_IsRenderedInError_OnAbsentElementTimeoutFailure()
        {
            Given.OpenedEmptyPage();

            var act = () =>
            {
                S("input").Should(Have.Value("initial"));
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(input).Should(Have.Attribute(value = «initial»))
                Reason:
                    no such element: Unable to locate element: {"method":"css selector","selector":"input"}
                """));
        }

        [Test]
        public void Should_HaveValue_IsRenderedInError_OnAbsentValueAttributeTimeoutFailure()
        {
            Given.OpenedPageWithBody(
                @"
                <label style='display:none'></label>
                "
            );

            var act = () => {
                S("label").Should(Have.Value("initial"));
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(label).Should(Have.Attribute(value = «initial»))
                Reason:
                    Actual value: Null (attribute is absent)
                Actual webelement: <label style="display:none"></label>
                """));
        }

        [Test]
        public void Should_HaveValue_IsRenderedInError_OnDifferentActualAsEmptyFailure()
        {
            Given.OpenedPageWithBody(
                @"
                <input value='' style='display:none'></input>
                "
            );

            var act = () => {
                S("input").Should(Have.Value("some"));
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(input).Should(Have.Attribute(value = «some»))
                Reason:
                    Actual value: «»
                Actual webelement: <input value="" style="display:none">
                """));
        }

        [Test]
        public void Should_HaveValue_Or_HaveText_IsRenderedInError_OnDifferentActualAsEmptyFailure()
        {
            Given.OpenedPageWithBody(
                @"
                <label value='some'>thing</label>
                "
            );

            var act = () => {
                S("label").Should(Have.Value("").Or(Have.ExactText("")));
            };

        Assert.That(act, Does.Timeout($$"""
                Browser.Element(label).Should(Have.Attribute(value = «») OR Have.ExactText(«»))
                Reason:
                    Actual value: «some»
                Actual webelement: <label value="some">thing</label>
                Actual text: «thing»
                Actual webelement: <label value="some">thing</label>
                """)); // TODO: fix final extra webelement html
        }
        [Test]
        public void Should_HaveValue_And_HaveText_IsRenderedInError_OnDifferentActualAsEmptyFailure()
        {
            Given.OpenedPageWithBody(
                @"
                <label value='some'></label>
                "
            );

            var act = () => {
                S("label").Should(Have.Value("some").And(Have.ExactText("thing")));
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(label).Should(Have.Attribute(value = «some») AND Have.ExactText(«thing»))
                Reason:
                    Actual text: «»
                Actual webelement: <label value="some"></label>
                """));
        }
        
        [Test]
        public void Should_HaveNoValue_IsRenderedInError_OnInDomElementTimeoutFailure()
        {
            Given.OpenedPageWithBody(
                @"
                <input value='initial' style='display:none'></input>
                "
            );

            var act = () => {
                S("input").Should(Have.No.Value("initial"));
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(input).Should(Not.Have.Attribute(value = «initial»))
                Reason:
                    condition not matched
                """));
        }
        
        [Test]
        public void Should_HaveText_WaitsForVisibility_OfInitialyHidden()
        {
            Given.OpenedPageWithBody(
                @"
                <label style='display:none'>initial</label>
                "
            );
            Given.ExecuteScriptWithTimeout(
                @"
                document.getElementsByTagName('label')[0].style.display = 'block';
                ",
                PollingPeriod.TotalMilliseconds
            );

            var act = () =>
            {
                S("label").Should(Have.Text("initial"));
            };

            Assert.That(act, Does.NotTimeout(PollingPeriod));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("label")).Text,
                Is.EqualTo("initial")
            );
        }

        [Test]
        public void Should_HaveText_WaitsForAskedText_OfInitialyOtherText()
        {
            Given.OpenedPageWithBody(
                @"
                <label>initial</label>
                "
            );
            Given.OpenedPageWithBodyTimedOut(
                @"
                <label>new</label>
                ",
                PollingPeriod.TotalMilliseconds
            );

            var act = () =>
            {
                S("label").Should(Have.Text("new"));
            };

            Assert.That(act, Does.NotTimeout(PollingPeriod));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("label")).Text,
                Is.EqualTo("new")
            );
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("label")).Text,
                Is.EqualTo("new")
            );
        }
        
        [Test]
        public void Should_HaveText_IsRenderedInError_OnHiddenElementFailure()
        {
            Given.OpenedPageWithBody(
                @"
                <label style='display:none'>initial</label>
                "
            );

            var act = () => {
                S("label").Should(Have.Text("initial"));
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(label).Should(Have.TextContaining(«initial»))
                Reason:
                    Actual text: «»
                Actual webelement: <label style="display:none">initial</label>
                """));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("label")).Text, Is.EqualTo("")
            );
        }

        [Test]
        public void Should_BeVisible_WaitsForVisibility_OfInitialyHiddenInDom()
        {
            Given.OpenedPageWithBody(
                @"
                <label style='display:none'>initial</label>
                "
            );
            Given.ExecuteScriptWithTimeout(
                @"
                document.getElementsByTagName('label')[0].style.display = 'block';
                ",
                PollingPeriod.TotalMilliseconds
            );

            var act = () =>
            {
                S("label").Should(Be.Visible);
            };

            Assert.That(act, Does.NotTimeout(PollingPeriod));
        }


        [Test]
        public void Should_BeNotVisible_WaitsForHiddenInDom_FromInitialyVisible()
        {
            Given.OpenedPageWithBody(
                @"
                <label style='display:block'>initial</label>
                "
            );
            Given.ExecuteScriptWithTimeout(
                @"
                document.getElementsByTagName('label')[0].style.display = 'none';
                ",
                PollingPeriod.TotalMilliseconds
            );

            var act = () =>
            {
                S("label").Should(Be.Not.Visible);
            };

            Assert.That(act, Does.NotTimeout(PollingPeriod));
        }
        
        [Test]
        public void Should_BeVisible_WaitsForVisibility_OfInitialyAbsentInDom()
        {
            Given.OpenedEmptyPage();
            Given.WithBodyTimedOut(
                @"
                <label>initial</label>
                ",
                PollingPeriod.TotalMilliseconds
            );

            var act = () =>
            {
                S("label").Should(Be.Visible);
            };

            Assert.That(act, Does.NotTimeout(PollingPeriod));
        }

        [Test]
        public void Should_BeNotVisible_WaitsForAbsentInDom_FromInitiallyVisibile()
        {
            Given.OpenedPageWithBody(
                @"
                <label>initial</label>
                "
            );
            Given.WithBodyTimedOut(
                @"
                "
                ,
                PollingPeriod.TotalMilliseconds
            );

            var act = () =>
            {
                S("label").Should(Be.Not.Visible);
            };

            Assert.That(act, Does.NotTimeout(PollingPeriod));
        }

        [Test]
        public void Should_BeVisible_IsRenderedInError_OnHiddenInDomElementFailure()
        {
            Given.OpenedPageWithBody(
                @"
                <label style='display:none'>initial</label>
                "
            );

            var act = () => {
                S("label").Should(Be.Visible);
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(label).Should(Be.Visible)
                Reason:
                    Found element is not visible: <label style="display:none">initial</label>
                """));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("label")).Displayed, Is.EqualTo(false)
            );
        }

        [Test]
        public void Should_BeNotVisible_IsRenderedInError_OnVisibleElementFailure()
        {
            Given.OpenedPageWithBody(
                @"
                <label>initial</label>
                "
            );

            var act = () => {
                S("label").Should(Be.Not.Visible);
            };


            Assert.That(act, Does.Timeout($$"""
                Browser.Element(label).Should(Not.Be.Visible)
                Reason:
                    condition not matched
                """));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("label")).Displayed, Is.EqualTo(true)
            );
        }

        [Test]
        public void Should_BeVisible_IsRenderedInError_OnAbsentInDomElementFailure()
        {
            Given.OpenedEmptyPage();

            var act = () => {
                S("label").Should(Be.Visible);
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(label).Should(Be.Visible)
                Reason:
                    no such element: Unable to locate element: {"method":"css selector","selector":"label"}
                """));
        }

        [Test]
        public void Should_HaveText_IsRenderedInError_OnElementDifferentTextFailure()
        {
            Given.OpenedPageWithBody(
                @"
                <label>initial</label>
                "
            );

            var act = () => {
                S("label").Should(Have.Text("new"));
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(label).Should(Have.TextContaining(«new»))
                Reason:
                    Actual text: «initial»
                Actual webelement: <label>initial</label>
                """));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("label")).Text, Is.EqualTo("initial")
            );
        }

        [Test]
        [Ignore("Overlay does not fail Have.Text()")]

        public void Should_HaveText_DoesNotWaitForNoOverlay() // TODO: but should it?
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

                <label>initial</label>
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
                S("label").Should(Have.Text("initial"));
            };

            Assert.That(act, Does.NotTimeout(PollingPeriod));
            Assert.That(
                Configuration.Driver.FindElement(By.TagName("label")).Text,
                Is.EqualTo("initial")
            );
        }

        [Test]
        [Ignore("Overlay does not fail Have.Text()")]

        public void Should_HaveText_IsRenderedInError_OnOverlappedWithOverlayFailure() // IS NOT RELEVANT
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
                 <label>initial</label>
                 "
            );

            var act = () =>
            {
                S("label").Should(Have.Text("initial"));
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.Element(label).contains initial
                Reason:
                    Element is overlapped by: <div id=\"overlay\" 
                """));
        }
    }
}

