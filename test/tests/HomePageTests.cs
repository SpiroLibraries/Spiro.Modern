﻿//Copyright 2014 Stef Cascarini, Dan Haywood, Richard Pawson
//Licensed under the Apache License, Version 2.0(the
//"License"); you may not use this file except in compliance
//with the License.You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//Unless required by applicable law or agreed to in writing,
//software distributed under the License is distributed on an
//"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
//KIND, either express or implied.See the License for the
//specific language governing permissions and limitations
//under the License.

using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Web.UnitTests.Selenium {
    [TestClass]
    public abstract class HomePageTests : SpiroTest {
        [TestMethod]
        public virtual void HomePage() {
            bool found = wait.Until(d => d.FindElements(By.ClassName("service")).Count == ServicesCount);
            Assert.IsTrue(found, "Services not found on home page");
        }

        [TestMethod]
        public virtual void Services() {
            wait.Until(d => d.FindElements(By.ClassName("service")).Count == ServicesCount);

            ReadOnlyCollection<IWebElement> services = br.FindElements(By.ClassName("service"));

            Assert.AreEqual("Customers", services[0].Text);
            Assert.AreEqual("Orders", services[1].Text);
            Assert.AreEqual("Products", services[2].Text);
            Assert.AreEqual("Employees", services[3].Text);
            Assert.AreEqual("Sales", services[4].Text);
            Assert.AreEqual("Special Offers", services[5].Text);
            Assert.AreEqual("Contacts", services[6].Text);
            Assert.AreEqual("Vendors", services[7].Text);
            Assert.AreEqual("Purchase Orders", services[8].Text);
            Assert.AreEqual("Work Orders", services[9].Text);
        }

        [TestMethod]
        public virtual void MenuBar() {
            wait.Until(d => d.FindElements(By.ClassName("app-bar")).Count == 1);

            Assert.IsTrue(br.FindElement(By.ClassName("home")).Displayed);
            Assert.IsTrue(br.FindElement(By.ClassName("back")).Displayed);
            Assert.IsTrue(br.FindElement(By.ClassName("forward")).Displayed);
            Assert.IsFalse(br.FindElement(By.ClassName("refresh")).Displayed);
            Assert.IsFalse(br.FindElement(By.ClassName("edit")).Displayed);
            Assert.IsFalse(br.FindElement(By.ClassName("help")).Displayed);
        }

        [TestMethod]
        public virtual void GoToService() {
            GoToServiceFromHomePage("Customers");
            Assert.AreEqual("Customers", br.FindElement(By.CssSelector("div.object-view > div > div.header > div.title")).Text);
        }
    }

    #region browsers specific subclasses 

    [TestClass, Ignore]
    public class HomePageTestsIe : HomePageTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.IEDriverServer.exe");
            SpiroTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitIeDriver();
            br.Navigate().GoToUrl(Url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

    [TestClass]
    public class HomePageTestsFirefox : HomePageTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            SpiroTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitFirefoxDriver();
            br.Navigate().GoToUrl(Url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

    [TestClass, Ignore]
    public class HomePageTestsChrome : HomePageTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.chromedriver.exe");
            SpiroTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitChromeDriver();
            br.Navigate().GoToUrl(Url);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }

        protected override void ScrollTo(IWebElement element) {
            string script = string.Format("window.scrollTo(0, {0})", element.Location.Y);
            ((IJavaScriptExecutor) br).ExecuteScript(script);
        }
    }

    #endregion
}