using NUnit.Framework;
using System;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using NSelene;
using static NSelene.Utils;
using NSelene.Conditions;

namespace NSeleneTests
{
    namespace NSeleneTests
    {
        public static class Tasks
        {
            public static SCollection List = SS("#todo-list>li");

            public static void Visit()
            {
                Open("https://todomvc4tasj.herokuapp.com/");
            }

            public static void FilterActive()
            {
                S(By.LinkText("Active")).Click();
            }

            public static void FilterCompleted()
            {
                S(By.LinkText("Completed")).Click();
            }

            public static void Add(params string[] taskTexts)
            {
                foreach (var text in taskTexts)
                {
                    S("#new-todo").SetValue(text).PressEnter();
                }
            }

            public static void Toggle(string taskText)
            {
                List.FindBy(Have.ExactText(taskText)).Find(".toggle").Click();
            }

            public static void ShouldBe(params string[] names)
            {
                List.FilterBy(Be.Visible).Should(Have.Texts(names));
            }
        }

    }

}
