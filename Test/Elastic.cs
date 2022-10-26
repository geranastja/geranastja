using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System.Text;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Remote;

namespace BoxTests
{
    
    public class Elastic: Base
    {

        [Test]
        [TestCaseSource(nameof(GetSites))]
        public void BuyElastic(string site, string login)
        {

            ElasticTemplate(site, login);
            driver.Navigate().GoToUrl(baseURL);
            AuthorizationElastic("rnaonoebsezl@dropmail.me");

            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            RemoveFromCartElastic();

            //ОФОРМЛЕНИЕ ПОКУПКИ
            Message = "Ordering";
            driver.Navigate().GoToUrl(baseURL + "?p=item&id=" + IDproduct4);
            WaitUntilVisible(By.XPath("(.//*[@class='item-product__price']//span)[1]"));

            ChooseConfig(); //выбор доступной конфигурации

            WaitUntilClickable(By.XPath(".//*[@class='js-panel-buttons panel-buttons']//button[2]"));//кнопка купить
            if (driver.FindElement(By.XPath(".//*[contains(text(),'Способы доставки:')]")).Displayed == true) //тут условие если есть доставка
            {
                WaitUntilEnabled(By.XPath(".//*[contains(text(),'Выбрать позже')]"));
                js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath(".//*[@class='choose-delivery']/a")));
                // driver.FindElement(By.XPath(".//*[@class='choose-delivery']/a")).Click();
                WaitUntilVisible(By.XPath(
                    ".//*[contains(text(),'Выбрать позже')]"));
                js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath(".//*[contains(text(),'Выбрать позже')]")));
                //  driver.FindElement(By.XPath(".//*[contains(text(),'Выбрать позже')]")).Click();
                driver.FindElement(By.XPath(".//h5[contains(text(),'Способы доставки')]//ancestor::div[@class='modal-content']//button[contains(text(),'Ok')]")).Click();
            }
            //кнопка купить            
            WaitUntilVisible(By.XPath(".//*[@class='js-panel-buttons panel-buttons']//button[2]"));
            js.ExecuteScript("arguments[0].scrollIntoView();",
                driver.FindElement(By.XPath(".//*[@class='js-panel-buttons panel-buttons']//button[2]")));
            WaitUntilInVisible(By.XPath(".//*[@class='js-panel-buttons panel-buttons']//button[2][@disabled='disabled']"));
            js.ExecuteScript("arguments[0].click()", 
                driver.FindElement(By.XPath(".//*[@class='js-panel-buttons panel-buttons']//button[2]")));
            //driver.FindElement(By.XPath(".//*[@class='js-panel-buttons panel-buttons']//button[2]")).Click();
            try
            {
                Console.WriteLine("Waiting for the text 'Item added to cart!' after pressing the button");
                WaitUntilVisible(By.XPath(
                ".//*[contains(text(),'Товар добавлен в корзину!')]"));

            }
            catch
            {
                Console.WriteLine("Didn't wait for the text 'Item added to cart!' after pressing the button, press again");
                driver.FindElement(By.XPath(".//*[@class='js-panel-buttons panel-buttons']//button[2]")).Click();
                WaitUntilVisible(By.XPath(
              ".//*[contains(text(),'Товар добавлен в корзину!')]"));
            }

            //}
            //переход в корзину и оформление заказа
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath(
                ".//*[contains(text(),'Товар добавлен в корзину!')]//ancestor::div[@class='modal-content']" +
                "//*[contains(text(),'Оформить заказ')]")));
            WaitUntilVisible(By.XPath(
                "(.//*[@class='dashed js-basket-config'])[1]"));
            string Config = driver.FindElement(By.XPath("(.//*[@class='dashed js-basket-config'])[1]")).Text;

             MinimumCost0();  //проверка мин заказа

            WaitUntilVisible(By.XPath(".//*[contains(text(),'Оформить заказ')]"));
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath(".//*[contains(text(),'Оформить заказ')]")));
            WaitUntilVisible(By.XPath(".//*[@class='progressbar-inner-wrapper']"));
            //WaitUntilInVisible(By.XPath(".//*[@class='progressbar-inner-wrapper']"));
            //тут нужно добавить код если появилось предупреждение об изменении цены  if (IsElementPresent(By.XPath(".//*[@class='clrfix mb10 alert alert-danger']")) == true)
            //{
            //    js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath(".//*[contains(text(), 'Оформить заказ')]")));
            //}
            WaitUntilVisible(By.XPath("(.//div[contains(text(),'Цвет')])"));
            // проверка отображения конфигурации
            string Config2 = driver.FindElement(By.XPath("(.//div[contains(text(),'Цвет')]//parent::div)[1]/div[2]")).Text;
            Config2.Contains(Config);
            js.ExecuteScript("arguments[0].scrollIntoView();", driver.FindElement(By.XPath("(.//*[contains(text(),'Вес единицы товара')]//parent::div)[1]//input")));

            driver.FindElement(By.XPath("(.//*[contains(text(),'Вес единицы товара')]//parent::div)[1]//input")).Clear();
            WaitUntilVisible(By.XPath("(.//*[contains(text(),'Вес единицы товара')]//parent::div)[1]//input"));
            WaitUntilClickable(By.CssSelector(".comment-area.item-comment"));
            driver.FindElement(By.CssSelector(".comment-area.item-comment")).Clear();
            driver.FindElement(By.CssSelector(".comment-area.item-comment")).SendKeys("5");
            driver.FindElement(By.CssSelector(".comment-area.item-comment")).SendKeys(Keys.Tab);
            WaitUntilEnabled(By.XPath(".//*[@id='overlay'][contains(@style,'display: none;')]"));
            driver.FindElement(By.XPath("(.//*[contains(text(),'Вес единицы товара')]//parent::div)[1]//input")).SendKeys("5");
            driver.FindElement(By.XPath("(.//*[contains(text(),'Вес единицы товара')]//parent::div)[1]//input")).SendKeys(Keys.Enter);
            WaitUntilInVisible(By.XPath(
                "(.//div[contains(text(), 'Способ доставки:')]/parent::div//*[@itemprop='price'])[1]"));
            WaitUntilVisible(By.XPath(
             "(.//div[contains(text(), 'Способ доставки:')]/parent::div//*[@itemprop='price'])[1]"));
            // тут нужно разобраться нужна ли функция OrderSteps();
            WaitUntilVisibleText(By.XPath(
                "(.//*[@class='list-parcels__col-right total-weight'])[1]"), "5");

            //js.ExecuteScript("arguments[0].scrollIntoView();", driver.FindElement(By.XPath(".//*[@id='editProfile']")));
            //WaitUntilVisible(By.XPath(
            //    "(.//div[contains(text(), 'Способ доставки:')]/parent::div//*[@itemprop='price'])[1]"));
            //WaitUntilClickable(By.XPath("(.//*[contains(text(), 'Подтвердить заказ')])[1]"));
            driver.FindElement(By.XPath("(.//*[contains(text(), 'Подтвердить заказ')])[1]")).Click();
            Console.WriteLine(Message);
            Message = "Payment and get order number - ";
            WaitUntilClickable(By.XPath(".//*[@class='status']"));
            //try
            //{
            //driver.FindElement(By.XPath(".//*[@class='status']"));
            //}
            //catch 
            //{
            //    driver.FindElement(By.XPath("(.//*[contains(text(), 'Подтвердить заказ')])[1]")).Click();
            //    driver.FindElement(By.XPath(".//*[@class='status']"));
            //} 
            
            //получение номера заказа
            string Order = driver.FindElement(By.XPath(".//*[@class='main main-order-block']//h1")).Text;
            Order = Order.Replace("Заказ ", "");
            Order = Order.Replace(" (Ожидает оплаты)", "");

            //кнопка оплатить
            WaitUntilClickable(By.XPath("(.//*[contains(text(), 'Оплатить заказ')])[1]"));
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath("(.//*[contains(text(), 'Оплатить заказ')])[1]")));
            WaitUntilClickable(By.XPath("(.//*[@class='modal-content']//*[contains(text(), 'Оплатить')])[1]"));
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath("(.//*[@class='modal-content']//*[contains(text(), 'Оплатить')])[1]")));
            WaitUntilEnabled(By.XPath("//*[contains(text(),' Оплачено')]"));

            Console.WriteLine(Message + Order);

            //ПРОВЕРКА В АДМИНКЕ ОПЛАТЫ ЗАКАЗА
            Message = "Checking the order payment in the admin panel";

            driver.Navigate().GoToUrl(baseURL + "admin/?cmd=orders&do=view&id=" + Order);            
            WaitUntilVisibleText(By.CssSelector(".label.weight-normal.orderStatus"),
                    "Оплачено");
            Console.WriteLine("Checking the order payment in the admin panel");

            Message = "Change the status of the order to receive and check in the personal account. ";
            //СТАТУС МЕНЯЕТСЯ НА ПОЛУЧЕНО ПОКУПАТЕЛЕМ
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath("//div[@id='ot_order_goods_tab']/div[3]/div/div[2]/div/button")));
            driver.FindElement(By.XPath("(//a[contains(text(),'Получено покупателем')])[2]")).Click();
            WaitUntilVisibleText(By.CssSelector(".label.weight-normal.orderStatus"),
                   "Завершено");

            // ПРОВЕРКА СТАТУСА ЗАКАЗА "Получено покупателем" В ЛИЧНОМ КАБИНЕТЕ
            driver.Navigate().GoToUrl(baseURL + "?p=privateoffice&orderstate=1");
            WaitUntilVisible(By.XPath(".//*[@id='myTab']"));
            js.ExecuteScript("arguments[0].scrollIntoView();", driver.FindElement(By.XPath(".//*[@id='myTab']")));
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath(".//*[@id='closed-tab']")));
            //Console.WriteLine("Get the current time " + DateTime.Now);

            WaitUntilVisible(By.LinkText(Order));
            Console.WriteLine(Message + "the current time " + DateTime.Now);

            //ОТМЕНА ЗАКАЗА

            Message = "Change the status cancel the order and check in the personal account";
            driver.Navigate().GoToUrl(baseURL + "admin/?cmd=orders&do=view&id=" + Order);
            WaitUntilVisibleText(By.CssSelector(".label.weight-normal.orderStatus"),
                    "Завершено");

            //СТАТУС МЕНЯЕТСЯ НА отмена заказа
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath("//div[@id='ot_order_goods_tab']/div[3]/div/div[2]/div/button")));
            driver.FindElement(By.XPath("(//a[contains(text(),' Отменено')])[2]")).Click();
            WaitUntilVisibleText(By.CssSelector(".label.weight-normal.font-12.offset-left1.itemStatus"),
                  "Отменено");

            // ПРОВЕРКА СТАТУСА ЗАКАЗА "отменено" В ЛИЧНОМ КАБИНЕТЕ
            driver.Navigate().GoToUrl(baseURL + "?p=privateoffice&orderstate=1");
            WaitUntilEnabled(By.XPath(".//*[@id='myTab']"));
            js.ExecuteScript("arguments[0].scrollIntoView();", driver.FindElement(By.XPath(".//*[@id='myTab']")));
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath(".//*[@id='canceled_orders-tab']")));
           // Console.WriteLine("Get the current time " + DateTime.Now);
            //Console.WriteLine(driver.FindElement(By.XPath("(.//*[@id='closed_orders']//*[@class='orderNumder'])[1]")).Text);
            WaitUntilVisible(By.LinkText(Order));
            driver.FindElement(By.LinkText(Order));
            Console.WriteLine(Message);
            //  Message = "Not detected, the test was successful";
        }


        [Test]
        [TestCaseSource(nameof(GetSites))]
        public void CardAndFavoritesElastic(string site, string login)
        {
            ElasticTemplate(site, login);
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            driver.Navigate().GoToUrl(baseURL + "/admin/?cmd=SiteConfiguration&do=orders");
            AdminAuthorization();
            WaitUntilVisible(By.XPath(".//*[@class='ot_main_nav']"));
            driver.Navigate().GoToUrl(baseURL + "/admin/?cmd=SiteConfiguration&do=orders");
            WaitUntilVisible(By.XPath(".//h1[contains(text(), 'Общие')]"));
            WaitUntilVisible(By.XPath("//*[@data-name='IsFilteredItemsSellAllowed']"));
            if (driver.FindElement(By.XPath("//*[@data-name='IsFilteredItemsSellAllowed']")).Text == "Запретить")
            {
                driver.FindElement(By.XPath("//*[@data-name='IsFilteredItemsSellAllowed']")).Click();
                new SelectElement(driver.FindElement(By.CssSelector("select.input-medium"))).SelectByText("Разрешить");
                driver.FindElement(By.XPath("//button[@type='submit']")).Click();
            }
            if (driver.FindElement(By.XPath("//*[@data-name='hide_item_for_restrictions']")).Text == "Скрыть")
            {
                driver.FindElement(By.XPath("//*[@data-name='hide_item_for_restrictions']")).Click();
                new SelectElement(driver.FindElement(By.CssSelector("select.input-medium"))).SelectByText("Отобразить");
                driver.FindElement(By.XPath("//button[@type='submit']")).Click();
            }

            driver.Navigate().GoToUrl(baseURL);
            //ВХОД ПОД ИМЕЮЩИМСЯ ЛОГИНОМ
            AuthorizationElastic("tt650116@mail.ru");
            RemoveFromCartElastic();

            //проверка избранного на наличие товаров
            Message = "Removing existing items from your favorites";
            string NoteItemsCount = driver.FindElement(By.XPath(".//*[@class='favorite-amount js-favorite-amount']")).Text;
            Console.WriteLine(NoteItemsCount);
            int NoteItemsCountInt = Convert.ToInt32(NoteItemsCount);
            Console.WriteLine(NoteItemsCount);
            if (NoteItemsCountInt > 0) //если избранное не пусто, то удаление из избранного всех товаров
            { 
                driver.FindElement(By.XPath(".//*[@class='favorite-icon']")).Click();
                driver.FindElement(By.XPath(".//*[contains(text(), 'Очистить избранное')]")).Click();
                WaitUntilClickable(By.XPath("(.//button[contains(text(), 'Да')])[2]"));
                driver.FindElement(By.XPath("(.//button[contains(text(), 'Да')])[2]")).Click();
                WaitUntilVisible(By.XPath(".//*[@class='alert alert-empty-basket'][contains(text(),'Список пуст')]"));
                Console.WriteLine(Message);
            }
            //добавление в корзину и в избранное товаров с Id = IDproduct5, IDproduct6, IDproduct7
            string[] IDproducts = { IDproduct5, IDproduct6, IDproduct7, IDproduct8 };
            for (int i = 0; i < IDproducts.Length; i++)
            {
                Message = "Adding to shopping cart and favorites with ID " + IDproducts[i];
                driver.Navigate().GoToUrl(baseURL + "?p=item&id=" + IDproducts[i]);
                WaitUntilVisible(By.XPath("(.//*[@class='item-product__price']//span)[1]"));
                ChooseConfig(); //выбор доступной конфигурации


                //ДОБАВЛЕНИЕ  ТОВАРА В ИЗБРАННОЕ
                WaitUntilClickable(By.XPath(
                    ".//*[@class='add-favorite js-product-btn-in-favorite desktop-favourite js-tooltip']"));//выбор конфигурации
                js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath(".//*[@class='add-favorite js-product-btn-in-favorite desktop-favourite js-tooltip']")));
                WaitUntilVisible(By.XPath(".//*[contains(text(),'Товар добавлен в избранное')]"));
                Assert.AreEqual((i + 1).ToString(), driver.FindElement(By.XPath(".//*[@class='favorite-amount js-favorite-amount']")).Text);
                //добавление  в корзину
                WaitUntilClickable(By.XPath(".//*[@class='js-panel-buttons panel-buttons']//button[2]"));
                js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath(".//*[@class='js-panel-buttons panel-buttons']//button[2]")));
                WaitUntilVisible(By.XPath(".//*[contains(text(),'Товар добавлен в корзину!')]"));
                // driver.FindElement(By.XPath(".//*[contains(text(),'Товар добавлен в корзину!')]/parent::div/button/span")).Click();
                driver.Navigate().Refresh();
                WaitUntilVisibleText(By.XPath(".//*[@class='cart-amount js-cart-amount']"),
                    (i + 1).ToString());

                //  Console.WriteLine(driver.FindElement(By.XPath(".//*[@class='cart-amount js-cart-amount']")).Text + " - товаров в значке корзина, а i" + i);
              //  Assert.AreEqual((i + 1).ToString(), driver.FindElement(By.XPath(".//*[@class='cart-amount js-cart-amount']")).Text); //проверка отображения количества товара в значке корзина
                Console.WriteLine(Message);
            }

            //переход в избранное
            Message = "Add / remove / move goods from favorites";
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath(".//*[@class='favorite-icon']")));
            WaitUntilClickable(By.XPath("(.//*[@class='delete-item delete'])[1]"));
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath("(.//*[@class='delete-item delete'])[1]")));
            WaitUntilClickable(By.XPath("(.//button[contains(text(), 'Да')])[2]"));
            driver.FindElement(By.XPath("(.//button[contains(text(), 'Да')])[2]")).Click(); //вплывающее окно удаления - да
            WaitUntilInVisible(By.XPath("(.//button[contains(text(), 'Да')])[2]"));
            WaitUntilClickable(By.XPath("(.//*[@class='fa fa-shopping-basket'])[1]"));
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath("(.//*[@class='fa fa-shopping-basket'])[1]")));
            WaitUntilClickable(By.XPath("(.//button[contains(text(), 'Да')])[2]"));
            driver.FindElement(By.XPath("(.//button[contains(text(), 'Да')])[2]")).Click(); //вплывающее окно удаления - да
            WaitUntilInVisible(By.XPath("(.//button[contains(text(), 'Да')])[2]"));
                     driver.FindElement(By.XPath(".//*[contains(text(), 'Выбрать все товары')]")).Click(); // галочка выбрать все
            WaitUntilClickable(By.XPath(".//*[contains(text(), 'Удалить выбранные')]"));
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath(".//*[contains(text(), 'Удалить выбранные')]")));// удалить 
            WaitUntilClickable(By.XPath("(.//button[contains(text(), 'Да')])[2]"));
            driver.FindElement(By.XPath("(.//button[contains(text(), 'Да')])[2]")).Click(); //вплывающее окно удаления - да
            WaitUntilInVisible(By.XPath("(.//button[contains(text(), 'Да')])[2]"));
            WaitUntilVisibleText(By.XPath(".//*[@class='favorite-amount js-favorite-amount']"), "0");
            Console.WriteLine(Message);

            //// переход в корзину
            Message = "Add / remove / move goods from the shopping cart";
            WaitUntilClickable(By.ClassName("cart-icon"));
            js.ExecuteScript("arguments[0].click();", driver.FindElement(By.ClassName("cart-icon")));
            WaitUntilClickable(By.XPath("(.//*[@title='Удалить из корзины'])[1]"));
            js.ExecuteScript("arguments[0].click();", driver.FindElement(By.XPath("(.//*[@title='Удалить из корзины'])[1]")));//удалить первый товар
            WaitUntilClickable(By.XPath("(.//button[contains(text(), 'Да')])[2]"));
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath("(.//button[contains(text(), 'Да')])[2]")));
            WaitUntilVisible(By.Id("overlay-no-preloader"));
            WaitUntilEnabled(By.XPath(".//*[@id='overlay-no-preloader'][@style='display: none;']"));
            driver.Navigate().Refresh();
            WaitUntilVisibleText(By.XPath(".//*[@class='cart-amount js-cart-amount']"), "3");
            WaitUntilClickable(By.XPath("(.//*[@title='Переместить в избранное'])[1]"));
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath("(.//*[@title='Переместить в избранное'])[1]")));//перенести в избранное
            WaitUntilClickable(By.XPath("(.//button[contains(text(), 'Да')])[2]"));
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath("(.//button[contains(text(), 'Да')])[2]")));
            WaitUntilVisible(By.Id("overlay-no-preloader"));
            WaitUntilEnabled(By.XPath(".//*[@id='overlay-no-preloader'][@style='display: none;']"));

            driver.Navigate().Refresh();
            WaitUntilVisibleText(By.XPath(".//*[@class='cart-amount js-cart-amount']"), "2");
            WaitUntilClickable(By.XPath(
                "(.//*[@class='button js-basket-panel-button js-basket-btn-clear'])[1]"));
            driver.FindElement(By.XPath("(.//*[@class='button js-basket-panel-button js-basket-btn-clear'])[1]")).Click(); // удалить все
            WaitUntilClickable(By.XPath("(.//button[contains(text(), 'Да')])[2]"));
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath("(.//button[contains(text(), 'Да')])[2]")));
            WaitUntilVisibleText(By.XPath(".//*[@class='cart-amount js-cart-amount']"), "0");
            Console.WriteLine(Message);
            Message = "Not detected, the test was successful";
        }



        [Test]
        [TestCaseSource(nameof(GetSites))]
        public void RegisterElastic(string site, string login)
        {
            //Получаем почту         
            Message = "Obtaining a temporary mailbox on the site https://tempmail.io/ru/ ";
            driver.Navigate().GoToUrl("https://tempmail.io/ru/");
            WaitUntilVisible(By.CssSelector("#email"));
            string Mail = driver.FindElement(By.CssSelector("#email")).GetAttribute("value");
            Console.WriteLine(Message + Mail);

            // проверка что новый шаблон включен и если включен его отключить
            ElasticTemplate(site, login);
            // Отключение дополнительной активации
            Message = "Disable advanced activation";
            driver.Navigate().GoToUrl(baseURL + "/admin/?cmd=siteconfiguration&do=users");
            AdminAuthorization();
            driver.FindElement(By.XPath(".//*[@data-name='IsEmailConfirmationUsed']")).Click();
            new SelectElement(driver.FindElement(By.CssSelector("select.input-medium"))).SelectByText("Выключена");
            driver.FindElement(By.XPath("//button[@type='submit']")).Click();
            Console.WriteLine(Message);


            //РЕГИСТРАЦИЯ       
            driver.Navigate().GoToUrl(baseURL);
            Message = "Registration on the site with the help of received mail";
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.LinkText("Регистрация")));
            WaitUntilVisible(By.XPath("(.//*[@name='username'])[3]"));
            if (IsElementPresent(By.XPath(".//*[contains(text(), 'Зарегистрироваться по Email')]")) == true)           //если есть регистрация по телефону, переключится по емайлу
            {
                driver.FindElement(By.XPath(".//*[contains(text(), 'Зарегистрироваться по Email')]")).Click();
            }
            WaitUntilVisible(By.XPath("(.//*[@class='modal-row'])//*[@name='email']"));
            driver.FindElement(By.XPath("(.//*[@name='username'])[3]")).Clear();
            driver.FindElement(By.XPath("(.//*[@name='username'])[3]")).SendKeys(Mail);
            driver.FindElement(By.XPath(".//*[@name='email']")).Clear();
            driver.FindElement(By.XPath(".//*[@name='email']")).SendKeys(Mail);
            driver.FindElement(By.XPath("(.//*[@name='password'])[2]")).Clear();
            driver.FindElement(By.XPath("(.//*[@name='password'])[2]")).SendKeys(Mail);
            js.ExecuteScript("arguments[0].scrollIntoView();", driver.FindElement(By.XPath(".//*[contains(text(), 'Я принимаю условия')]")));
            driver.FindElement(By.XPath(".//*[contains(text(), 'Я принимаю условия')]")).Click();
            WaitUntilClickable(By.XPath(".//*[contains(text(), 'Зарегистрироваться')]"));
            driver.FindElement(By.XPath(".//button[contains(text(), 'Зарегистрироваться')]")).Click();



            if (IsElementPresent(By.XPath(".//title[contains(text(), 'Необходима активация')]")) == true)
            {
                // Отключение дополнительной активации
                Message = "Disable advanced activation";
                driver.Navigate().GoToUrl(baseURL + "/admin/?cmd=siteconfiguration&do=users");
                AdminAuthorization();
                driver.FindElement(By.XPath(".//*[@data-name='IsEmailConfirmationUsed']")).Click();
                new SelectElement(driver.FindElement(By.CssSelector("select.input-medium"))).SelectByText("Выключена");
                driver.FindElement(By.XPath("//button[@type='submit']")).Click();
                Console.WriteLine(Message);

                //РЕГИСТРАЦИЯ       
                driver.Navigate().GoToUrl(baseURL);
                Message = "Registration on the site with the help of received mail";
                js.ExecuteScript("arguments[0].click()", driver.FindElement(By.LinkText("Регистрация")));
                WaitUntilVisible(By.XPath("(.//*[@name='username'])[2]"));
                driver.FindElement(By.XPath("(.//*[@name='username'])[2]")).Clear();
                driver.FindElement(By.XPath("(.//*[@name='username'])[2]")).SendKeys(Mail);
                driver.FindElement(By.XPath(".//*[@name='email']")).Clear();
                driver.FindElement(By.XPath(".//*[@name='email']")).SendKeys(Mail);
                driver.FindElement(By.XPath("(.//*[@name='password'])[2]")).Clear();
                driver.FindElement(By.XPath("(.//*[@name='password'])[2]")).SendKeys(Mail);
                js.ExecuteScript("arguments[0].scrollIntoView();", driver.FindElement(By.XPath(".//*[contains(text(), 'Я принимаю условия')]")));
                driver.FindElement(By.XPath(".//*[contains(text(), 'Я принимаю условия')]")).Click();
                WaitUntilClickable(By.XPath(".//*[contains(text(), 'Зарегистрироваться')]"));
                driver.FindElement(By.XPath(".//button[contains(text(), 'Зарегистрироваться')]")).Click();
            }

            WaitUntilVisibleText(By.XPath(".//*[@class='box-up-menu_username_login']"),Mail); //проверка наличия логина на сайте
            Console.WriteLine(Message);

            //ВХОД В ЛИЧНЫЙ КАБИНЕТ проверка тайтла страницы и т.д
            Message = "Opening of a private office";
            driver.FindElement(By.XPath(".//*[@class='box-up-menu_username_login']")).Click();
            WaitUntilClickable(By.XPath("(.//*[@class='dropdown-menu show']/a)[1]"));
            driver.FindElement(By.XPath("(.//*[@class='dropdown-menu show']/a)[1]")).Click();
            //driver.FindElement(By.CssSelector(".flr.col690"));
            Console.WriteLine(Message);

            //Релогин 
            Message = "Logout and authorization on the site";
            driver.FindElement(By.XPath(".//*[@class='box-up-menu_username_login']")).Click();
            WaitUntilClickable(By.XPath(".//*[@class='dropdown-menu show']/a[contains(text(), 'Выход')]"));
            driver.FindElement(By.XPath(".//*[@class='dropdown-menu show']/a[contains(text(), 'Выход')]")).Click();
            WaitUntilClickable(By.XPath(".//a/*[contains(text(), 'Вход')]"));
            Console.WriteLine(Message);
        }


        [Test]
        [TestCaseSource(nameof(GetSites))]
        public void AdminEditCategoriesElastic(string site, string login)
        {

            ElasticTemplate(site, login);
            driver.Navigate().GoToUrl(baseURL + "admin/?cmd=categories");
            AdminAuthorization();
            // Message = "Переход на страницу Категории";
            string NovayaCategory = "NewElastic";
            //     bool present;
            //Actions actions = new Actions(driver);
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            // удаление категории NovayaCategory

            if (IsElementPresent(By.XPath(".//*[@name='" + NovayaCategory + "']/a")) == true)
            {
                Message = "Deleting category " + NovayaCategory;
                js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath(".//*[@name='" + NovayaCategory + "']//*[@title='Удалить категорию']")));
                WaitUntilClickable(By.XPath(
                    ".//*[@class='modal hide fade confirmDialog 1-confirmDialog level-1 in']//a[@id='confirm']"));
                driver.FindElement(By.XPath(".//*[@class='modal hide fade confirmDialog 1-confirmDialog level-1 in']//a[@id='confirm']")).Click();
                Console.WriteLine(Message);
                WaitUntilInVisible(By.XPath(
                    "(.//*[@class='modal hide fade confirmDialog 1-confirmDialog level-1 in'])[2]"));
                driver.Navigate().GoToUrl(baseURL + "/admin/?cmd=categories");
            }

            // удаление категории Дизайнер интерьера
            if (IsElementPresent(By.XPath(".//*[@name='Дизайнер интерьера']/a")) == true)
            {
                Message = "Deleting category 'Дизайнер интерьера'";
                js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath(".//*[@name='Дизайнер интерьера']//*[@title='Удалить категорию']")));
                WaitUntilClickable(By.XPath(
                    ".//*[@class='modal hide fade confirmDialog 1-confirmDialog level-1 in']//a[@id='confirm']"));
                driver.FindElement(By.XPath(".//*[@class='modal hide fade confirmDialog 1-confirmDialog level-1 in']//a[@id='confirm']")).Click();
                Console.WriteLine(Message);
                WaitUntilInVisible(By.CssSelector(
                    "(.//*[@class='modal hide fade confirmDialog 1-confirmDialog level-1 in'])[2]"));
                driver.Navigate().GoToUrl(baseURL + "/admin/?cmd=categories");
            }

            //создание категории
            Message = "Сreate a category '" + NovayaCategory + "'";
            if (IsElementPresent(By.CssSelector(".alert.ui-pnotify-container.alert-error.ui-pnotify-shadow")) == true)
            {
                js.ExecuteScript("arguments[0].click();", driver.FindElement(By.CssSelector(".alert.ui-pnotify-container.alert-error.ui-pnotify-shadow .icon-remove")));
            }
            WaitUntilVisibleText(By.CssSelector(".blink.blink-iconed.ot_show_crud_cat_item_window"),
                "Добавить корневую категорию");
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.CssSelector(".blink.blink-iconed.ot_show_crud_cat_item_window")));
            WaitUntilVisible(By.CssSelector(
                "#ot_cat_data1 > div.control-group > div.controls > #categoryName"));
            driver.FindElement(By.CssSelector("#ot_cat_data1 > div.control-group > div.controls > #categoryName")).SendKeys("" + NovayaCategory + "");
            new SelectElement(driver.FindElement(By.CssSelector("#ot_cat_data1 > div.control-group > div.controls >#preDefineMode"))).SelectByText("виртуальная");
            driver.FindElement(By.LinkText("Сохранить")).Click();
            Console.WriteLine(Message);

            // редактирование
            Message = "Editing category '" + NovayaCategory + "'.Change the name to the Дизайнер интерьера, the type of binding from the virtual to the category Taobao";
            WaitUntilEnabled(By.XPath(".//*[@name='" + NovayaCategory + "']/a"));
            driver.Navigate().Refresh();
            WaitUntilEnabled(By.XPath(".//*[@name='" + NovayaCategory + "']/span"));
            //  Actions actions8 = new Actions(driver);
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath(".//*[@name='" + NovayaCategory + "']//*[@title='Редактировать категорию']")));
            WaitUntilClickable(By.XPath("(//input[@id='categoryName'])[2]"));
            driver.FindElement(By.XPath("(//input[@id='categoryName'])[2]")).Clear();
            driver.FindElement(By.XPath("(//input[@id='categoryName'])[2]")).SendKeys("Дизайнер интерьера");
            WaitUntilVisibleAndClick(By.XPath("(//select[@id='preDefineMode'])[2]"));
            WaitUntilVisibleAndClick(By.XPath("(//select[@id='preDefineMode'])[2]/option[@value='category']"));
           // new SelectElement(driver.FindElement(By.XPath("(//select[@id='preDefineMode'])[2]"))).SelectByText("к категории");
            WaitUntilVisible(By.XPath(".//*[@id='ot_cat_data1']//input[@value='Taobao']"));
            driver.FindElement(By.XPath(".//*[@id='ot_cat_data1']//input[@value='Taobao']")).Click();
            WaitUntilVisible(By.XPath(".//*[@id='jstree-categoryRoot']/ul//ul/li[1]/a"));
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath(".//*[@id='jstree-categoryRoot']/ul//ul/li[1]/a")));
            WaitUntilClickable(By.XPath(
                ".//*[@class='modal hide fade confirmDialog 2-confirmDialog level-2 in']" +
                "//a[@class='btn btn-primary pull-left']"));
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath(".//*[@class='modal hide fade confirmDialog 2-confirmDialog level-2 in']//a[@class='btn btn-primary pull-left']")));
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath(".//*[@class='modal hide fade confirmDialog 1-confirmDialog level-1 in']//a[@class='btn btn-primary pull-left']")));
            WaitUntilVisible(By.XPath(".//*[@name='Дизайнер интерьера']/a[contains(text(),'Дизайнер интерьера  [Taobao]')]"));
            Console.WriteLine(Message);

            //надо добавить условияе вкл ли сео2 и есть ли вкладка
            // добавление, удаление картинки в содержании категории
            //добавляем сссылку
            Message = "Adding, deleting pictures in the category content";
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath(".//*[@name='Дизайнер интерьера']//*[@title='Редактировать категорию']")));
            WaitUntilClickable(By.XPath("(//input[@id='categoryName'])[2]"));
            driver.FindElement(By.XPath(".//*[@class='ot_cat_content1_toogle']")).Click();
            if (IsElementPresent(By.XPath(".//*[@id='ot_cat_content1']//*[@class='editableform-loading']")) == true)
            {
                js.ExecuteScript("arguments[0].scrollIntoView();",
                driver.FindElement(By.XPath(".//*[@id='ot_cat_content1']//*[@class='editableform-loading']")));
            }
            WaitUntilVisible(By.CssSelector(".mce-ico.mce-i-bold"));

            WaitUntilClickable(By.XPath(".//*[@id='ot_cat_content1']//span[contains(text(),'Указать')]"));
            driver.FindElement(By.XPath(".//*[@id='ot_cat_content1']//span[contains(text(),'Указать')]")).Click();
            string Link = "https://img.alicdn.com/imgextra/i3/TB1O.xPHVXXXXXlXXXXXXXXXXXX_!!0-item_pic.jpg_600x600.jpg";
            WaitUntilVisible(By.XPath("(.//*[@id='dataId'])[3]"));
            driver.FindElement(By.XPath("(.//*[@id='dataId'])[3]")).Clear();
            driver.FindElement(By.XPath("(.//*[@id='dataId'])[3]")).SendKeys(Link);
            driver.FindElement(By.XPath(".//*[@id='confirm'][contains(text(),'Добавить')]")).Click();
            //ожидание картинки????
            WaitUntilVisible(By.XPath("(.//*[@src='" + Link + "'])"));
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath(
                ".//*[@class='modal hide fade confirmDialog 1-confirmDialog level-1 in']" +
                "//*[contains(text(),'Сохранить')]")));
            //удаление картинки
            driver.Navigate().Refresh();
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath(
                ".//*[@name='Дизайнер интерьера']//*[@title='Редактировать категорию']")));
            WaitUntilClickable(By.XPath("(//input[@id='categoryName'])[2]"));
            driver.FindElement(By.XPath(".//*[@class='ot_cat_content1_toogle']")).Click();

            WaitUntilVisible(By.XPath("(.//*[@src='" + Link + "'])[2]"));
            WaitUntilVisible(By.CssSelector("#mceu_2-button"));
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath(
                ".//*[@id='ot_cat_content1']//span[contains(text(),'Удалить')]")));
            //ожидание удаления картинки????

            //  WaitUntilVisible(By.XPath(".//span[contains(text(),'loading...')]"));
            WaitUntilVisible(By.XPath(".//*[contains(text(),'Данные успешно обновлены')]"));
            WaitUntilEnabled(By.XPath(
                ".//*[@id='ot_cat_content1']//span[contains(text(),'Удалить')][@style='display: none;']"));
            driver.FindElement(By.XPath("(.//*[contains(text(),'Данные')])[2]")).Click();
            WaitUntilVisible(By.XPath("(.//*[@id='categoryName'])[2]"));
            driver.FindElement(By.XPath(".//*[@class='ot_cat_content1_toogle']")).Click();
            WaitUntilVisible(By.CssSelector("#mceu_2-button"));
            WaitUntilVisible(By.XPath("(.//*[@id='confirm'])[5]"));
            //  driver.FindElement(By.XPath("(.//*[@id='confirm'])[5]")).Click();
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath("(.//*[@id='confirm'])[5]")));

            //проверка удаления картинки
            driver.Navigate().Refresh();
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath(".//*[@name='Дизайнер интерьера']//*[@title='Редактировать категорию']")));
            WaitUntilClickable(By.XPath("(//input[@id='categoryName'])[2]"));
            driver.FindElement(By.XPath(".//*[@class='ot_cat_content1_toogle']")).Click();
            WaitUntilVisible(By.CssSelector("#mceu_2-button"));
            WaitUntilInVisible(By.XPath("(.//*[@src='" + Link + "'])[2]"));
            Console.WriteLine(Message);

            //удаление категории
            Message = "Deleting category 'Дизайнер интерьера'";
            driver.Navigate().Refresh();
            WaitUntilVisible(By.XPath(".//*[@name='Дизайнер интерьера']/a[contains(text(),'Дизайнер интерьера  [Taobao]')]"));
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath(".//*[@name='Дизайнер интерьера']//*[@title='Удалить категорию']")));
            WaitUntilClickable(By.XPath(
                ".//*[@class='modal hide fade confirmDialog 1-confirmDialog level-1 in']//a[@id='confirm']"));
            driver.FindElement(By.XPath(".//*[@class='modal hide fade confirmDialog 1-confirmDialog level-1 in']//a[@id='confirm']")).Click();
            Console.WriteLine(Message);

        }


        [Test]
        [TestCaseSource(nameof(GetSites))]
        public void RestorePasswordElastic(string site, string login)
        {
            ElasticTemplate(site, login);
            driver.Navigate().GoToUrl(baseURL);
            //ВОССТАНОВЛЕНИЕ ПАРОЛЯ
            Message =
                "Go to the password recovery page, enter data: tt650116@mail.ru, click on Restore";
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            WaitUntilClickable(
                By.XPath(".//*[@class='header-authorization']//*[contains(text(), 'Вход')]"));
            js.ExecuteScript("arguments[0].click()",driver.FindElement(
                By.XPath(".//*[@class='header-authorization']//*[contains(text(), 'Вход')]")));
            driver.FindElement(By.XPath(".//*[contains(text(), 'Забыли пароль')]")).Click();
            string Username = ".//*[contains(text(), 'Восстановление пароля')]/parent::div/parent::div//*[@name='username']";
            WaitUntilVisible(By.XPath(Username));
            driver.FindElement(By.XPath(Username)).Clear();
            driver.FindElement(By.XPath(Username)).SendKeys("tt650116@mail.ru");
            WaitUntilClickable(By.XPath(".//button[contains(text(),'Восстановить')]"));
            driver.FindElement(By.XPath(".//button[contains(text(),'Восстановить')]")).Click();
            WaitUntilVisible(By.XPath(".//*[@class='alert alert-success']"));
            Console.WriteLine(Message);

            //получение ссылки с почты
            Message =
                "Go to the mailbox tt650116@mail.ru/123456Tt and receive the password recovery link from the mail ";
            driver.Navigate().GoToUrl("https://mail.ru/");
            string LinkPost = driver.FindElement(By.XPath(".//a[contains(text(), 'Почта')]")).
                GetAttribute("href");
            driver.Navigate().GoToUrl(LinkPost);
            WaitUntilVisible(By.XPath("//*[@name='username']"));
            driver.FindElement(By.XPath("//*[@name='username']")).Clear();
            driver.FindElement(By.XPath("//*[@name='username']")).SendKeys("tt650116");
            driver.FindElement(By.XPath("//span[contains(text(), 'Ввести пароль')]")).Click();
            WaitUntilVisible(By.XPath("//*[@name='password']"));
            driver.FindElement(By.XPath("//*[@name='password']")).Clear();
            driver.FindElement(By.XPath("//*[@name='password']")).SendKeys("123456Tt");
            driver.FindElement(By.XPath("(//span[contains(text(), 'Войти')])[1]")).Click();
            WaitUntilVisibleText(
                By.XPath("(.//*[@class='ll-sj__normal'])[1]"),
                "Запрос");
            WaitUntilClickable(By.XPath("(.//*[@class='ll-sj__normal'])[1]"));
            js.ExecuteScript("arguments[0].click()", driver.FindElement(
              By.XPath("(.//*[@class='ll-sj__normal'])[1]")));
            WaitUntilVisible(By.XPath("(.//*[@class='letter__body']//a)[1]"));
            string Link;
            Link = driver.FindElement(By.XPath("(.//*[@class='letter__body']//a)[1]")).GetAttribute("href");
            string LinkOld = "http://dev.box.otdev.net/";
            if (Link.StartsWith(LinkOld) == true)
            {
                Link = Link.Replace(LinkOld, "");
                Link = baseURL + Link;
            }
            Console.WriteLine(Link);

            // Ввод нового пароля
            driver.Navigate().GoToUrl(Link);
            Console.WriteLine(Message);
            Message = "Entering a new password and opening a personal cabinet";
            WaitUntilVisible(By.XPath(".//*[@name='newPassword']"));
            driver.FindElement(By.XPath(".//*[@name='newPassword']")).Clear();
            driver.FindElement(By.XPath(".//*[@name='newPassword']")).SendKeys("tt650116@mail.ru");
            driver.FindElement(By.XPath(".//*[@name='newPasswordConfirm']")).Clear();
            driver.FindElement(By.XPath(".//*[@name='newPasswordConfirm']")).SendKeys("tt650116@mail.ru");
            driver.FindElement(By.XPath(".//button[contains(text(), 'Сохранить')]")).Click();
            Assert.AreEqual("Общая информация", driver.FindElement(By.CssSelector("h1")).Text);
            Console.WriteLine(Message);
        }

        [Test]
        [TestCaseSource(nameof(GetSites))]
        public void TestElastic(string site, string login)
        {
            double a;
            a = 5 / 3;
            Console.WriteLine(a);
        }

    }
}