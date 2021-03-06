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

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NakedObjects.Web.UnitTests.Selenium {
    [TestClass]
    public abstract class DialogTests : SpiroTest
    {

        private const int CustomersFindCustomerByAccountNumber = 0;

        private const int OrdersOrdersByValue = 3;

        private const int StoresSearchForOrders = 4;

        private const int SalesListAccountsForSalesPerson = 2;

        private const int ProductsFindProductByName = 0;
        private const int ProductsFindProductByNumber = 1;
        private const int ProductsListProductsBySubcategory = 2;
        private const int ProductsListProductsBySubcategories = 3;
        private const int ProductsFindByProductLineAndClass = 4;
        private const int ProductsFindByProductLinesAndClasses = 5;
        private const int ProductsFindProduct = 6;
        private const int ProductsFindProductsByCategory = 7;
       

        [TestMethod]
        public virtual void ChoicesParm() {
            br.Navigate().GoToUrl(OrderServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == OrderServiceActions);
            ReadOnlyCollection<IWebElement> actions = br.FindElements(By.ClassName("action"));

            var showList = new Action<string, string>((type, test) => {
                // click on action to open dialog 
                Click(br.FindElements(By.ClassName("action"))[OrdersOrdersByValue]); // orders by value

                wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
                string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

                Assert.AreEqual("Orders By Value", title);

                br.FindElement(By.CssSelector(".parameter-value  select")).SendKeys(type);

                Click(br.FindElement(By.ClassName("show")));

                wait.Until(d => d.FindElement(By.ClassName("list-view")));

                string topItem = br.FindElement(By.CssSelector("div.list-item > a")).Text;

                Assert.AreEqual(test, topItem);
            });

            var cancelList = new Action(() => {
                // cancel object 
                Click(br.FindElement(By.CssSelector("div.list-view .cancel")));

                wait.Until(d => {
                    try {
                        br.FindElement(By.ClassName("list-view"));
                        return false;
                    }
                    catch (NoSuchElementException) {
                        return true;
                    }
                });
            });

            var cancelDialog = new Action(() => {
                Click(br.FindElement(By.CssSelector("div.action-dialog  .cancel")));

                wait.Until(d => {
                    try {
                        br.FindElement(By.ClassName("action-dialog"));
                        return false;
                    }
                    catch (NoSuchElementException) {
                        return true;
                    }
                });
            });

            showList("Ascending", "SO51782");
            cancelList();
            cancelDialog();

            showList("Descending", "SO51131");
            cancelDialog();
            cancelList();
        }

        [TestMethod]
        public virtual void ScalarChoicesParmKeepsValue() {
            br.Navigate().GoToUrl(OrderServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == OrderServiceActions);

            // click on action to open dialog 
            Click(br.FindElements(By.ClassName("action"))[OrdersOrdersByValue]); // orders by value

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("Orders By Value", title);

            br.FindElement(By.CssSelector(".parameter-value  select")).SendKeys("Ascending");

            Click(br.FindElement(By.ClassName("show")));

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            string topItem = br.FindElement(By.CssSelector("div.list-item > a")).Text;

            Assert.AreEqual("SO51782", topItem);

            var selected = new SelectElement(br.FindElement(By.CssSelector(".parameter-value  select")));

            Assert.AreEqual("Ascending", selected.SelectedOption.Text);
        }

        [TestMethod]
        public virtual void ScalarParmKeepsValue() {
            br.Navigate().GoToUrl(CustomerServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == CustomerServiceActions);

            // click on action to open dialog 
            Click(br.FindElements(By.ClassName("action"))[CustomersFindCustomerByAccountNumber]); // find customer by account number

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("Find Customer By Account Number", title);

            br.FindElement(By.CssSelector("div.parameter-value input")).SendKeys("00000042");

            Click(br.FindElement(By.ClassName("show")));

            wait.Until(d => d.FindElement(By.ClassName("nested-object")));

            Assert.AreEqual("AW00000042", br.FindElement(By.CssSelector("div.parameter-value input")).GetAttribute("value"));
        }

        [TestMethod]
        public virtual void DateTimeParmKeepsValue() {
            br.Navigate().GoToUrl(Store555Url);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == StoreActions);


            // click on action to open dialog 
            Click(br.FindElements(By.CssSelector("div.action-button a"))[StoresSearchForOrders]); // Search for orders

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("Search For Orders", title);

            br.FindElements(By.CssSelector(".parameter-value input"))[0].SendKeys("1 Jan 2003");
            br.FindElements(By.CssSelector(".parameter-value input"))[1].SendKeys("1 Dec 2003" + Keys.Escape);

            Thread.Sleep(2000); // need to wait for datepicker :-(

            wait.Until(d => br.FindElement(By.ClassName("show")));

            Click(br.FindElement(By.ClassName("show")));

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            Assert.AreEqual("1 Jan 2003", br.FindElements(By.CssSelector(".parameter-value input"))[0].GetAttribute("value"));
            Assert.AreEqual("1 Dec 2003", br.FindElements(By.CssSelector(".parameter-value input"))[1].GetAttribute("value"));
        }

        [TestMethod]
        public virtual void RefChoicesParmKeepsValue() {
            br.Navigate().GoToUrl(ProductServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == ProductServiceActions);

            // click on action to open dialog 
            Click(br.FindElements(By.ClassName("action"))[ProductsListProductsBySubcategory]); // list products by sub cat

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("List Products By Sub Category", title);

            br.FindElement(By.CssSelector(".parameter-value  select")).SendKeys("Forks");

            Click(br.FindElement(By.ClassName("show")));

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            string topItem = br.FindElement(By.CssSelector("div.list-item > a")).Text;

            Assert.AreEqual("HL Fork", topItem);

            var selected = new SelectElement(br.FindElement(By.CssSelector("select")));

            Assert.AreEqual("Forks", selected.SelectedOption.Text);
        }

       
        [TestMethod]
        public virtual void MultipleRefChoicesDefaults()
        {
            br.Navigate().GoToUrl(ProductServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == ProductServiceActions);

            // click on action to open dialog 
            Click(br.FindElements(By.ClassName("action"))[ProductsListProductsBySubcategories]); // list products by sub cat

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("List Products By Sub Categories", title);

            var selected = new SelectElement(br.FindElement(By.CssSelector("div#subcategories select")));

            Assert.AreEqual(2, selected.AllSelectedOptions.Count);
            Assert.AreEqual("Mountain Bikes", selected.AllSelectedOptions.First().Text);
            Assert.AreEqual("Touring Bikes", selected.AllSelectedOptions.Last().Text);

            Click(br.FindElement(By.ClassName("show")));

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            string topItem = br.FindElement(By.CssSelector("div.list-item > a")).Text;

            Assert.AreEqual("Mountain-100 Black, 38", topItem);

            selected = new SelectElement(br.FindElement(By.CssSelector("div#subcategories select")));

            Assert.AreEqual(2, selected.AllSelectedOptions.Count);
            Assert.AreEqual("Mountain Bikes", selected.AllSelectedOptions.First().Text);
            Assert.AreEqual("Touring Bikes", selected.AllSelectedOptions.Last().Text);
        }

        [TestMethod]
        public virtual void MultipleRefChoicesChangeDefaults()
        {
            br.Navigate().GoToUrl(ProductServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == ProductServiceActions);

            // click on action to open dialog 
            Click(br.FindElements(By.ClassName("action"))[ProductsListProductsBySubcategories]); // list products by sub cat

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("List Products By Sub Categories", title);


            br.FindElement(By.CssSelector(".parameter-value  select")).SendKeys("HandleBars");
            IKeyboard kb = ((IHasInputDevices)br).Keyboard;

            kb.PressKey(Keys.Control);
            br.FindElement(By.CssSelector(".parameter-value  select option[value='5']")).Click();
            kb.ReleaseKey(Keys.Control);


            Click(br.FindElement(By.ClassName("show")));

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            string topItem = br.FindElement(By.CssSelector("div.list-item > a")).Text;

            Assert.AreEqual("Front Brakes", topItem);

            var selected = new SelectElement(br.FindElement(By.CssSelector("div#subcategories select")));

            Assert.AreEqual(2, selected.AllSelectedOptions.Count);
            Assert.AreEqual("Handlebars", selected.AllSelectedOptions.First().Text);
            Assert.AreEqual("Brakes", selected.AllSelectedOptions.Last().Text);
        }




        [TestMethod]
        public virtual void ChoicesDefaults()
        {
            br.Navigate().GoToUrl(ProductServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == ProductServiceActions);

            // click on action to open dialog 
            Click(br.FindElements(By.ClassName("action"))[ProductsFindByProductLineAndClass]); // find by product line and class

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("Find By Product Line And Class", title);

            var slctPl = new SelectElement(br.FindElement(By.CssSelector("div#productline select")));
            var slctPc = new SelectElement(br.FindElement(By.CssSelector("div#productclass select")));

            Assert.AreEqual("M", slctPl.SelectedOption.Text);
            Assert.AreEqual("H", slctPc.SelectedOption.Text);

            Click(br.FindElement(By.ClassName("show")));

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            string topItem = br.FindElement(By.CssSelector("div.list-item > a")).Text;

            Assert.AreEqual("Mountain-300 Black, 38", topItem);

            slctPl = new SelectElement(br.FindElement(By.CssSelector("div#productline select")));
            slctPc = new SelectElement(br.FindElement(By.CssSelector("div#productclass select")));

            Assert.AreEqual("M", slctPl.SelectedOption.Text);
            Assert.AreEqual("H", slctPc.SelectedOption.Text);

        }

        [TestMethod]
        public virtual void ChoicesChangeDefaults()
        {
            br.Navigate().GoToUrl(ProductServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == ProductServiceActions);

            // click on action to open dialog 
            Click(br.FindElements(By.ClassName("action"))[ProductsFindByProductLineAndClass]); // find by product line and class

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("Find By Product Line And Class", title);

            br.FindElement(By.CssSelector("div#productline .parameter-value  select")).SendKeys("R");
            br.FindElement(By.CssSelector("div#productclass .parameter-value  select")).SendKeys("L");

            Click(br.FindElement(By.ClassName("show")));

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            string topItem = br.FindElement(By.CssSelector("div.list-item > a")).Text;

            Assert.AreEqual("HL Road Frame - Black, 58", topItem);

            var slctPl = new SelectElement(br.FindElement(By.CssSelector("div#productline select")));
            var slctPc = new SelectElement(br.FindElement(By.CssSelector("div#productclass select")));

            Assert.AreEqual("R", slctPl.SelectedOption.Text);
            Assert.AreEqual("L", slctPc.SelectedOption.Text);

        }

        [TestMethod]
        public virtual void ConditionalChoicesDefaults()
        {
            br.Navigate().GoToUrl(ProductServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == ProductServiceActions);

            // click on action to open dialog 
            Click(br.FindElements(By.ClassName("action"))[ProductsFindProductsByCategory]); 

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("Find Products By Category", title);

            var slctCs = new SelectElement(br.FindElement(By.CssSelector("div#categories select")));

            Assert.AreEqual("Bikes", slctCs.SelectedOption.Text);


            var slct = new SelectElement(br.FindElement(By.CssSelector("div#subcategories select")));

            Assert.AreEqual(2, slct.AllSelectedOptions.Count);
            Assert.AreEqual("Mountain Bikes", slct.AllSelectedOptions.First().Text);
            Assert.AreEqual("Road Bikes", slct.AllSelectedOptions.Last().Text);

            Click(br.FindElement(By.ClassName("show")));

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            string topItem = br.FindElement(By.CssSelector("div.list-item > a")).Text;

            Assert.AreEqual("Road-150 Red, 62", topItem);

            slctCs = new SelectElement(br.FindElement(By.CssSelector("div#categories select")));

            Assert.AreEqual("Bikes", slctCs.SelectedOption.Text);

            slct = new SelectElement(br.FindElement(By.CssSelector("div#subcategories select")));

            Assert.AreEqual(2, slct.AllSelectedOptions.Count);
            Assert.AreEqual("Mountain Bikes", slct.AllSelectedOptions.First().Text);
            Assert.AreEqual("Road Bikes", slct.AllSelectedOptions.Last().Text);
        }

        [TestMethod]
        public virtual void ConditionalChoicesChangeDefaults()
        {
            br.Navigate().GoToUrl(ProductServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == ProductServiceActions);

            // click on action to open dialog 
            Click(br.FindElements(By.ClassName("action"))[ProductsFindProductsByCategory]);

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("Find Products By Category", title);


            var slctCs = new SelectElement(br.FindElement(By.CssSelector("div#categories select")));

            Assert.AreEqual("Bikes", slctCs.SelectedOption.Text);

            var slct = new SelectElement(br.FindElement(By.CssSelector("div#subcategories select")));

            Assert.AreEqual(2, slct.AllSelectedOptions.Count);
            Assert.AreEqual("Mountain Bikes", slct.AllSelectedOptions.First().Text);
            Assert.AreEqual("Road Bikes", slct.AllSelectedOptions.Last().Text);

            Click(br.FindElement(By.ClassName("show")));

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            string topItem = br.FindElement(By.CssSelector("div.list-item > a")).Text;

            Assert.AreEqual("Road-150 Red, 62", topItem);

            slctCs = new SelectElement(br.FindElement(By.CssSelector("div#categories select")));

            Assert.AreEqual("Bikes", slctCs.SelectedOption.Text);

            slct = new SelectElement(br.FindElement(By.CssSelector("div#subcategories select")));

            Assert.AreEqual(2, slct.AllSelectedOptions.Count);
            Assert.AreEqual("Mountain Bikes", slct.AllSelectedOptions.First().Text);
            Assert.AreEqual("Road Bikes", slct.AllSelectedOptions.Last().Text);
        }


        [TestMethod]
        public virtual void AutoCompleteParmShow() {
            br.Navigate().GoToUrl(SalesServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == SalesServiceActions);

            // click on action to open dialog 
            Click(br.FindElements(By.ClassName("action"))[SalesListAccountsForSalesPerson]); // list accounts for sales person 

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("List Accounts For Sales Person", title);

            br.FindElement(By.CssSelector(".parameter-value input[type='text']")).SendKeys("Valdez");

            wait.Until(d => d.FindElement(By.ClassName("ui-menu-item")));

            Click(br.FindElement(By.CssSelector(".ui-menu-item a")));

            Click(br.FindElement(By.ClassName("show")));

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            Assert.AreEqual("Rachel Valdez", br.FindElement(By.CssSelector(".parameter-value input[type='text']")).GetAttribute("value"));
        }

        [TestMethod]
        public virtual void AutoCompleteParmGo()
        {
            br.Navigate().GoToUrl(SalesServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == SalesServiceActions);

            // click on action to open dialog 
            Click(br.FindElements(By.ClassName("action"))[SalesListAccountsForSalesPerson]); // list accounts for sales person 

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("List Accounts For Sales Person", title);

            br.FindElement(By.CssSelector(".parameter-value input[type='text']")).SendKeys("Valdez");

            wait.Until(d => d.FindElement(By.ClassName("ui-menu-item")));

            Click(br.FindElement(By.CssSelector(".ui-menu-item a")));

            Click(br.FindElement(By.ClassName("go")));

            wait.Until(d => d.FindElement(By.ClassName("list-view")));

            try
            {
                br.FindElement(By.CssSelector(".parameter-value input[type='text']"));
                // found so it fails
                Assert.Fail();
            }
            catch 
            {
                // all OK 
            }

            
        }


        [TestMethod]
        public virtual void AutoCompleteParmDefault()
        {
            br.Navigate().GoToUrl(ProductServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == ProductServiceActions);

            // click on action to open dialog 
            Click(br.FindElements(By.ClassName("action"))[ProductsFindProduct]); // list accounts for sales person 

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("Find Product", title);

            Assert.AreEqual("Adjustable Race", br.FindElement(By.CssSelector(".parameter-value input[type='text']")).GetAttribute("value"));

            Click(br.FindElement(By.ClassName("show")));

            wait.Until(d => d.FindElement(By.ClassName("nested-object")));

            Assert.AreEqual("Adjustable Race", br.FindElement(By.CssSelector(".nested-object .title")).Text);

            Assert.AreEqual("Adjustable Race", br.FindElement(By.CssSelector(".parameter-value input[type='text']")).GetAttribute("value"));
        }

        [TestMethod]
        public virtual void AutoCompleteParmShowSingleItem()
        {
            br.Navigate().GoToUrl(ProductServiceUrl);

            wait.Until(d => d.FindElements(By.ClassName("action")).Count == ProductServiceActions);

            // click on action to open dialog 
            Click(br.FindElements(By.ClassName("action"))[ProductsFindProduct]); // list accounts for sales person 

            wait.Until(d => d.FindElement(By.ClassName("action-dialog")));
            string title = br.FindElement(By.CssSelector("div.action-dialog > div.title")).Text;

            Assert.AreEqual("Find Product", title);

            var acElem = br.FindElement(By.CssSelector(".parameter-value input[type='text']"));

           

            for (int i = 0; i < 15; i++) {
                acElem.SendKeys(Keys.Backspace);
            }

            acElem.SendKeys("BB");

            wait.Until(d => d.FindElement(By.ClassName("ui-menu-item")));

            Click(br.FindElement(By.CssSelector(".ui-menu-item a")));

            Click(br.FindElement(By.ClassName("show")));

            wait.Until(d => d.FindElement(By.ClassName("nested-object")));

            Assert.AreEqual("BB Ball Bearing", br.FindElement(By.CssSelector(".nested-object .title")).Text);

            Assert.AreEqual("BB Ball Bearing", br.FindElement(By.CssSelector(".parameter-value input[type='text']")).GetAttribute("value"));
        }


    }

    #region browsers specific subclasses

    [TestClass, Ignore]
    public class DialogTestsIe : DialogTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.IEDriverServer.exe");
            SpiroTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitIeDriver();
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

    [TestClass]
    public class DialogTestsFirefox : DialogTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            SpiroTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitFirefoxDriver();
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

    [TestClass, Ignore]
    public class DialogTestsChrome : DialogTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.chromedriver.exe");
            SpiroTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitChromeDriver();
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }

        protected override void ScrollTo(IWebElement element) {
            string script = string.Format("window.scrollTo({0}, {1});return true;", element.Location.X, element.Location.Y);
            ((IJavaScriptExecutor) br).ExecuteScript(script);
        }
    }

    #endregion
}