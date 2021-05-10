using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace DisneyAvailibilityChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> days = new List<string>
            {
                "2021-05-10",
                "2021-05-11"
            };
            List<string> availible = new List<string>();
            new DriverManager().SetUpDriver(new ChromeConfig());
            ChromeDriver cd = new ChromeDriver();
            cd.Url = @"https://disneyworld.disney.go.com/availability-calendar/";
            cd.Navigate();
            cd.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            while (true)
            {
                try
                {
                    availible = Availible(days, cd);
                    if (availible != null)
                    {
                        foreach (string date in availible)
                        {
                            Console.WriteLine(date);
                        }
                    }
                }
                catch { }
            }
            cd.Close();
        }
        public static List<string> Availible(List<string> days, ChromeDriver cd)
        {
            cd.Navigate().Refresh();
            IWebElement s = (IWebElement)((IJavaScriptExecutor)cd).ExecuteScript("return document.querySelector(\"body\").querySelector(\"app-root\").querySelector(\"div\").querySelector(\"app-availability-calendar\").querySelector(\"awakening-calendar\").shadowRoot.querySelector(\"div\").querySelector(\"wdat-calendar\")");
            int elements = 0;
            List<string> availDates = new List<string>();
            while (true)
            {
                try
                {
                    elements++;
                    IWebElement day = s.FindElement(By.XPath($"wdat-date[{elements}]"));
                    if (day != null)
                    {
                        foreach (string date in days)
                        {
                            if (day.GetAttribute("date") == date)
                            {
                                if (day.GetAttribute("class").Contains("has-availability"))
                                {
                                    availDates.Add(day.GetAttribute("Date"));
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    break;
                }
            }
            return availDates;
        }
        public static IWebElement FindShadowRootElement(IWebDriver Driver, string[] Selectors)
        {
            IWebElement root = null;
            foreach (var selector in Selectors)
            {
                root = (IWebElement)((IJavaScriptExecutor)Driver).ExecuteScript("return arguments[0].querySelector(arguments[1]).shadowRoot", root, selector);
            }
            return root;
        }
    }
}
