using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using WebAPP_LIULI.Models;

namespace WebAPP_LIULI.Controllers
{
    public class HomeController : Controller
    {
        MainModel model = new MainModel();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var actionName = filterContext.ActionDescriptor.ActionName;

            Session["User"] = model.User.Where(p => p.UserName == "何磊").First();

            var user = Session["User"] as User;
            if (controllerName != "Home" && actionName != "Index" && user == null)
            {
                //重定向至登录页面
                filterContext.Result = RedirectToAction("Index");
                return;
            }

        }

        public ActionResult Index()
        {            
            return View();
        }


        public string GetUserInfo(string code)
        {
            HttpClient _httpClient = new HttpClient();

            HttpResponseMessage ret = _httpClient.GetAsync("https://oapi.dingtalk.com/gettoken?appkey=dingbf00i6u9uslk4vso&appsecret=FMYLKxLu17x1qJVFD15oPKK_aSk0E6WG7SGnFKGZn1_-qaW94yeAasG-ntagK1vO").Result;

            string retstring = ret.Content.ReadAsStringAsync().Result;
            JObject o = JObject.Parse(retstring);

            _httpClient = new HttpClient();

            ret = _httpClient.GetAsync("https://oapi.dingtalk.com/user/getuserinfo?access_token=" + o["access_token"] + "&code=" + code).Result;

            retstring = ret.Content.ReadAsStringAsync().Result;

            JObject o2 = JObject.Parse(retstring);

            string userId = o2["userid"].ToString();
            string userName = o2["name"].ToString();

            User user = model.User.Where(p => p.DingUserId == userId).FirstOrDefault();

            if(user == null)
            {
                user = new User();
                user.UserName = userName;
                user.DingUserId = userId;
                user.CreateTime = DateTime.Now;

                model.User.Add(user);
                model.SaveChanges();
            }

            Session["User"] = user;

            return user.UserTeam;

        }

        public string SendDDMessage(string message)
        {

            string msg = "{\"msgtype\":\"text\",\"text\":{\"content\":\"" + message + "\"}}";

            HttpClient _httpClient = new HttpClient();

            HttpResponseMessage ret = _httpClient.GetAsync("https://oapi.dingtalk.com/gettoken?appkey=dingbf00i6u9uslk4vso&appsecret=FMYLKxLu17x1qJVFD15oPKK_aSk0E6WG7SGnFKGZn1_-qaW94yeAasG-ntagK1vO").Result;

            string retstring = ret.Content.ReadAsStringAsync().Result;
            JObject o = JObject.Parse(retstring);

            _httpClient = new HttpClient();

            List<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>();

            KeyValuePair<string, string> keyvaluep = new KeyValuePair<string, string>("agent_id", "326443844");
            KeyValuePair<string, string> keyvaluep1 = new KeyValuePair<string, string>("to_all_user", "true");
            KeyValuePair<string, string> keyvaluep2 = new KeyValuePair<string, string>("msg", msg);

            keyValuePairs.Add(keyvaluep);
            keyValuePairs.Add(keyvaluep1);
            keyValuePairs.Add(keyvaluep2);

            HttpContent httpContent = new FormUrlEncodedContent(keyValuePairs);

            ret = _httpClient.PostAsync("https://oapi.dingtalk.com/topapi/message/corpconversation/asyncsend_v2?access_token=" + o["access_token"], httpContent).Result;

            retstring = ret.Content.ReadAsStringAsync().Result;

            JObject o2 = JObject.Parse(retstring);
            //o2["errcode"].ToString();
            return "";
        }

        public ActionResult SendMaterial()
        {
            List<Driver> drivers = model.MaterialDriver.Where(p => p.Status == 1).ToList();
            ViewBag.drivers = drivers;

            BaseData b = model.BaseData.First();

            ViewBag.OneOfTonPrice = b.OneOfTonPrice;
            return View();
        }

        [HttpPost]
        public ActionResult SendMaterial(MaterialOrder m)
        {

            m.CreateTime = DateTime.Now;
            Driver driver = model.MaterialDriver.Where(p => p.Id == m.MaterialDriver.Id).FirstOrDefault();
            m.SendTime = DateTime.Now;
            m.TaskStatus = "已发货";
            m.MaterialDriver = driver;
            model.MaterialOrder.Add(m);

            model.SaveChanges();
            return RedirectToAction("DefalutReport");
        }


        public ActionResult ReceiveMaterialList()
        {
            List<MaterialOrder> mList = model.MaterialOrder.Where(p => p.TaskStatus == "已发货").ToList();
            return View(mList);
        }

        public ActionResult ReceiveMaterial(int id)
        {
            MaterialOrder m = model.MaterialOrder.Where(p => p.Id == id).FirstOrDefault();
            return View(m);
        }

        [HttpPost]
        public ActionResult ReceiveMaterial(MaterialOrder m)
        {
            MaterialOrder _m = model.MaterialOrder.Where(p => p.Id == m.Id).FirstOrDefault();

            _m.ReceiveCount = m.ReceiveCount;
            _m.TaskStatus = "已收货";
            _m.ReceiveDeterminePerson = m.ReceiveDeterminePerson;
            _m.ReceiveTime = DateTime.Now;

            model.SaveChanges();
            return RedirectToAction("ReceiveMaterialList");
        }

        public ActionResult WarehousingInspectionList()
        {
            List<List<Warehousing>> models = new List<List<Warehousing>>();
            List<DateTime?> times = model.Warehousing.Where(p => p.WarehousingTime == null && p.IsConfirm == 1).Select(p => p.InspectionTime).Distinct().ToList();
            foreach (var t in times)
            {
                var m = model.Warehousing.Where(p =>
                p.InspectionTime.Value.Year == t.Value.Year &&
                p.InspectionTime.Value.Month == t.Value.Month &&
                p.InspectionTime.Value.Day == t.Value.Day &&
                p.InspectionTime.Value.Hour == t.Value.Hour &&
                p.InspectionTime.Value.Minute == t.Value.Minute &&
                p.InspectionTime.Value.Second == t.Value.Second //&&
                //p.InspectionTime.Value.Millisecond == t.Value.Millisecond
                ).ToList();
                models.Add(m);
            }
            return View(models);
        }

        public ActionResult WarehousingInspection()
        {
            List<Customer> customers = model.Customer.ToList();

            ViewBag.customers = customers;

            List<Product> products = model.Product.ToList();
            ViewBag.products = products;
            return View();
        }


        [HttpPost]
        public string WarehousingInspection(List<Warehousing> warehousings)
        {

            var CreateTime = DateTime.Now;
            foreach (var w in warehousings)
            {
                w.CreateTime = CreateTime;
                w.InspectionTime = CreateTime;
                string team = model.User.Where(p => p.UserName == w.InspectionUserName).First().UserTeam;
                w.WarehousingTeam = team;//w.WarehousingTeam;
                if (w.InspectionCount != 0)
                    model.Warehousing.Add(w);
            }
            model.SaveChanges();
            return "{\"success\":true}";
        }

        public ActionResult Warehousing(int id)
        {

            List<string> teams = model.User.Select(p => p.UserTeam).Distinct().ToList();
            ViewBag.teams = teams;

            DateTime? t = model.Warehousing.Where(p => p.Id == id).First().InspectionTime;

            List<Warehousing> modesls = model.Warehousing.Where(p =>
                p.InspectionTime.Value.Year == t.Value.Year &&
                p.InspectionTime.Value.Month == t.Value.Month &&
                p.InspectionTime.Value.Day == t.Value.Day &&
                p.InspectionTime.Value.Hour == t.Value.Hour &&
                p.InspectionTime.Value.Minute == t.Value.Minute &&
                p.InspectionTime.Value.Second == t.Value.Second
                ).ToList();

            return View(modesls);
        }

        [HttpPost]
        public string Warehousing(List<Warehousing> warehousings)
        {
            var WarehousingTime = DateTime.Now;
            foreach (var w in warehousings)
            {
                var _w = model.Warehousing.Where(p => p.Id == w.Id).First();
                _w.WarehousingCount = w.WarehousingCount;
                _w.WarehousingName = w.WarehousingName;
                //string team = model.User.Where(p => p.UserName == _w.InspectionUserName).First().UserTeam;
                //_w.WarehousingTeam = team;//w.WarehousingTeam;
                _w.WarehousingTime = WarehousingTime;
                //_w.CustomerShortName = w.CustomerShortName;
                _w.Remarks = w.Remarks;
            }

            model.SaveChanges();

            return "{\"success\":true}";
        }

        public ActionResult WarehousingConfirmList()
        {
            List<List<Warehousing>> models = new List<List<Warehousing>>();
            List<DateTime?> times = model.Warehousing.Where(p => p.WarehousingTime == null && p.IsConfirm == 0).Select(p => p.InspectionTime).Distinct().ToList();
            foreach (var t in times)
            {
                var m = model.Warehousing.Where(p =>
                p.InspectionTime.Value.Year == t.Value.Year &&
                p.InspectionTime.Value.Month == t.Value.Month &&
                p.InspectionTime.Value.Day == t.Value.Day &&
                p.InspectionTime.Value.Hour == t.Value.Hour &&
                p.InspectionTime.Value.Minute == t.Value.Minute &&
                p.InspectionTime.Value.Second == t.Value.Second //&&
                //p.InspectionTime.Value.Millisecond == t.Value.Millisecond
                ).ToList();
                models.Add(m);
            }
            return View(models);
        }
        public ActionResult WarehousingConfirm(int id)
        {
            List<string> teams = model.User.Select(p => p.UserTeam).Distinct().ToList();
            ViewBag.teams = teams;

            DateTime? t = model.Warehousing.Where(p => p.Id == id).First().InspectionTime;

            List<Warehousing> modesls = model.Warehousing.Where(p =>
                p.InspectionTime.Value.Year == t.Value.Year &&
                p.InspectionTime.Value.Month == t.Value.Month &&
                p.InspectionTime.Value.Day == t.Value.Day &&
                p.InspectionTime.Value.Hour == t.Value.Hour &&
                p.InspectionTime.Value.Minute == t.Value.Minute &&
                p.InspectionTime.Value.Second == t.Value.Second
                ).ToList();

            return View(modesls);
        }

        [HttpPost]
        public string WarehousingConfirm(List<Warehousing> warehousings)
        {
            var WarehousingTime = DateTime.Now;
            foreach (var w in warehousings)
            {
                var _w = model.Warehousing.Where(p => p.Id == w.Id).First();
                _w.InspectionCount = w.InspectionCount;
                _w.ConfirmName = w.ConfirmName;
                _w.Remarks = w.Remarks;
                _w.IsConfirm = 1;
            }

            model.SaveChanges();

            return "{\"success\":true}";
        }

        public ActionResult HalfWarehousingInspection()
        {
            List<Product> products = model.Product.ToList();
            ViewBag.products = products;
            return View();
        }


        [HttpPost]
        public string HalfWarehousingInspection(List<HalfWarehousing> warehousings)
        {
            var CreateTime = DateTime.Now;
            foreach (var w in warehousings)
            {
                w.CreateTime = CreateTime;
                w.InspectionTime = CreateTime;
                string username = w.InspectionUserName;
                User user = model.User.Where(p => p.UserName == username).First();
                w.HalfWarehousingTeam = user.UserTeam;
                if (w.InspectionCount != 0)
                    model.HalfWarehousing.Add(w);
            }
            model.SaveChanges();
            return "{\"success\":true}";
        }

        public ActionResult HalfWarehousingConfirmList()
        {
            List<List<HalfWarehousing>> models = new List<List<HalfWarehousing>>();
            List<DateTime?> times = model.HalfWarehousing.Where(p => p.HalfWarehousingTime == null && p.IsConfirm == 0).Select(p => p.InspectionTime).Distinct().ToList();
            foreach (var t in times)
            {
                var m = model.HalfWarehousing.Where(p =>
                p.InspectionTime.Value.Year == t.Value.Year &&
                p.InspectionTime.Value.Month == t.Value.Month &&
                p.InspectionTime.Value.Day == t.Value.Day &&
                p.InspectionTime.Value.Hour == t.Value.Hour &&
                p.InspectionTime.Value.Minute == t.Value.Minute &&
                p.InspectionTime.Value.Second == t.Value.Second //&&
                //p.InspectionTime.Value.Millisecond == t.Value.Millisecond
                ).ToList();
                models.Add(m);
            }
            return View(models);
        }
        public ActionResult HalfWarehousingConfirm(int id)
        {
            List<string> teams = model.User.Select(p => p.UserTeam).Distinct().ToList();
            ViewBag.teams = teams;

            DateTime? t = model.HalfWarehousing.Where(p => p.Id == id).First().InspectionTime;

            List<HalfWarehousing> modesls = model.HalfWarehousing.Where(p =>
                p.InspectionTime.Value.Year == t.Value.Year &&
                p.InspectionTime.Value.Month == t.Value.Month &&
                p.InspectionTime.Value.Day == t.Value.Day &&
                p.InspectionTime.Value.Hour == t.Value.Hour &&
                p.InspectionTime.Value.Minute == t.Value.Minute &&
                p.InspectionTime.Value.Second == t.Value.Second
                ).ToList();

            return View(modesls);
        }

        [HttpPost]
        public string HalfWarehousingConfirm(List<HalfWarehousing> warehousings)
        {
            var WarehousingTime = DateTime.Now;
            foreach (var w in warehousings)
            {
                var _w = model.HalfWarehousing.Where(p => p.Id == w.Id).First();
                _w.InspectionCount = w.InspectionCount;
                _w.ConfirmName = w.ConfirmName;
                _w.Remarks = w.Remarks;
                _w.IsConfirm = 1;
            }

            model.SaveChanges();

            return "{\"success\":true}";
        }

        public ActionResult QualityInspection()
        {
            List<string> teams = model.User.Select(p => p.UserTeam).Distinct().ToList();
            ViewBag.teams = teams;

            List<Product> products = model.Product.ToList();
            ViewBag.products = products;

            return View();
        }

        [HttpPost]
        public ActionResult QualityInspection(QualityInspection m)
        {
            m.CreateTime = DateTime.Now;
            //m.ScrapCount = 1;
            if (m.CheckResult == "合格")
                m.UnqualifiedReson = null;
            else
            {
                string message = "成品检验信息\n";
                message += "日期:" + m.CreateTime.ToString("yyyy年MM月dd日") + "\n";
                message += "工序:" + m.ProcessName + "\n";
                message += "产品:" + m.ProductName + "\n";
                message += "数量:" + m.ScrapCount + "\n";
                message += "班组:" + m.CheckTeam + "\n";
                message += "原因:" + m.UnqualifiedReson + "\n";
                message += "备注:" + m.Remarks + "\n";
                SendDDMessage(message);
            }

            model.QualityInspection.Add(m);
            model.SaveChanges();

            List<string> teams = model.User.Select(p => p.UserTeam).Distinct().ToList();
            ViewBag.teams = teams;
            List<Product> products = model.Product.ToList();
            ViewBag.products = products;

            return View();
        }

        public ActionResult HalfQualityInspection()
        {
            return View();
        }

        [HttpPost]
        public ActionResult HalfQualityInspection(HalfQualityInspection m)
        {
            m.CreateTime = DateTime.Now;
            m.CheckResult = "不合格";

            model.HalfQualityInspection.Add(m);
            model.SaveChanges();

            return View();
        }

        public ActionResult OrderList(DateTime? starttime ,DateTime? endtime, string customer)
        {
            User user = (User)Session["User"];

            List<Order> mList = new List<Order>();

            if (starttime == null && endtime == null)
                mList = model.Order.Where(p => p.OrderStatus != "已完成" || p.OrderStatus != "已关闭").OrderByDescending(p => p.OrderStatus).ToList();
            else
                mList = model.Order.Where(p =>
                (p.DeliveryTime >= starttime.Value && p.DeliveryTime <= endtime.Value) &&
                (p.OrderStatus != "已完成" ||
                p.OrderStatus != "已关闭")
                ).OrderByDescending(p => p.OrderStatus).ToList();

            if (!string.IsNullOrEmpty(customer))
            {
                mList = mList.Where(p => p.Customer.CustomerName.Contains(customer) || p.Customer.CustomerShortName.Contains(customer)).ToList();
            }

            if (user.UserPermission != "管理员")
            {
                mList = mList.Where(p => p.Salesman == user.UserName).ToList();
            }
            
            return View(mList);
        }

        public ActionResult MidSendOrderList()
        {
            List<MidSendOrder> mList = new List<MidSendOrder>();

            mList = model.MidSendOrder.Where(p => p.TaskStatus != "全部发货").ToList();

            return View(mList);
        }
        public ActionResult OrderQueryList(DateTime? starttime, DateTime? endtime,string customer)
        {
            List<Customer> customers = model.Customer.ToList();

            ViewBag.customers = customers;

            List<Order> mList = new List<Order>();
            if (starttime == null && endtime == null)
                mList = model.Order.Where(p => p.OrderStatus == "已完成").OrderByDescending(p => p.OrderStatus).ToList();
            else
                mList = model.Order.Where(p =>
                (p.DeliveryTime >= starttime.Value && p.DeliveryTime <= endtime.Value) &&
                (p.OrderStatus == "已完成")
                ).OrderByDescending(p => p.OrderStatus).ToList();

            if(!string.IsNullOrEmpty(customer))
            {
                mList = mList.Where(p => p.Customer.CustomerName.Contains(customer) || p.Customer.CustomerShortName.Contains(customer)).ToList();
            }

            return View(mList);
        }


        public ActionResult OrderInput(int? id)
        {
            List<Customer> customers = model.Customer.ToList();
            List<Product> products = model.Product.ToList();

            ViewBag.customers = customers;
            ViewBag.products = products;

            Order m = model.Order.Where(p => p.Id == id).FirstOrDefault();
            if (m == null)
                return View();
            else
                return View(m);
        }

        [HttpPost]
        public ActionResult OrderInput(Order m)
        {
            if(m.Id == 0)
            {
                m.CreateTime = DateTime.Now;
                Customer customer = model.Customer.Where(p => p.Id == m.Customer.Id).FirstOrDefault();
                m.Customer = customer;
                model.Order.Add(m);
            }
            else
            {
                Order _m = model.Order.Where(p => p.Id == m.Id).FirstOrDefault();
                Customer customer = model.Customer.Where(p => p.Id == m.Customer.Id).FirstOrDefault();
                _m.Customer = customer;
                _m.DeliveryTime = m.DeliveryTime;
                _m.OrderStatus = m.OrderStatus;
                _m.ProductCount = m.ProductCount;
                _m.ProductName = m.ProductName;
                _m.Remarks = m.Remarks;
                _m.Salesman = m.Salesman;
                _m.OrderStatus = m.OrderStatus;
                _m.OrderPrice = m.OrderPrice;
            }
            
            model.SaveChanges();
            return RedirectToAction("OrderList");
        }

        public ActionResult DriverList()
        {
            List<Driver> driveList = model.MaterialDriver.ToList();
            return View(driveList);
        }

        public ActionResult DriverDetail(int? id)
        {
            Driver driver = model.MaterialDriver.Where(p => p.Id == id).FirstOrDefault();
            if (driver == null)
                return View();
            else
                return View(driver);
        }

        public string DriverDetailJson(int? id)
        {
            Driver m = model.MaterialDriver.Where(p => p.Id == id).FirstOrDefault();

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            string json = JsonConvert.SerializeObject(m,settings);
            return json;
        }

        [HttpPost]
        public ActionResult DriverDetail(Driver m)
        {
            if (m.Id == 0)
            {
                m.CreateTime = DateTime.Now;
                model.MaterialDriver.Add(m);
            }
            else
            {
                Driver _m = model.MaterialDriver.Where(p => p.Id == m.Id).FirstOrDefault();
                _m.BankName = m.BankName;
                _m.BankNumer = m.BankNumer;
                _m.CarNumber = m.CarNumber;
                _m.CarType = m.CarType;
                _m.MaxLoad = m.MaxLoad;
                _m.Name = m.Name;
                _m.PersonId = m.PersonId;
                _m.PhoneNumber = m.PhoneNumber;
                _m.Remarks = m.Remarks;
                _m.Status = m.Status;
            }
            
            model.SaveChanges();
            return RedirectToAction("DriverList");
        }

        public ActionResult CustomerList()
        {
            List<Customer> mList = model.Customer.ToList();
            return View(mList);
        }

        public ActionResult CustomerDetail(int? id)
        {
            Customer m = model.Customer.Where(p => p.Id == id).FirstOrDefault();
            if (m == null)
                return View();
            else
                return View(m);
        }

        public string CustomerDetailJson(int id)
        {
            Customer m = model.Customer.Where(p => p.Id == id).FirstOrDefault();
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            string json = JsonConvert.SerializeObject(m, settings);
            return json;
        }

        [HttpPost]
        public ActionResult CustomerDetail(Customer m)
        {
            if (m.Id == 0)
            {
                m.CreateTime = DateTime.Now;
                model.Customer.Add(m);
            }
            else
            {
                Customer _m = model.Customer.Where(p => p.Id == m.Id).FirstOrDefault();
                _m.CustomerName = m.CustomerName;
                _m.CustomerCompanyNumber = m.CustomerCompanyNumber;
                _m.CustomerShortName = m.CustomerShortName;
                _m.CustomerAddress = m.CustomerAddress;
                _m.CustomerPhone = m.CustomerPhone;
                _m.Contact = m.Contact;
                _m.ContactPhone = m.ContactPhone;
                _m.Remarks = m.Remarks;
            }
           
            model.SaveChanges();
            return RedirectToAction("CustomerList");
        }

        public ActionResult UserList()
        {
            List<User> mList = model.User.ToList();
            return View(mList);
        }

        public ActionResult UserDetail(int? id)
        {
            User m = model.User.Where(p => p.Id == id).FirstOrDefault();
            if (m == null)
                return View();
            else
                return View(m);
        }

        [HttpPost]
        public ActionResult UserDetail(User m)
        {
            if (m.Id == 0)
            {
                m.CreateTime = DateTime.Now;
                model.User.Add(m);
            }
            else
            {
                User _m = model.User.Where(p => p.Id == m.Id).FirstOrDefault();
                _m.UserName = m.UserName;
                _m.UserPermission = m.UserPermission;
                _m.UserPhone = m.UserPhone;
                _m.UserTeam = m.UserTeam;
                _m.UserType = m.UserType;
                _m.Remarks = m.Remarks;
            }
            
            model.SaveChanges();
            return RedirectToAction("UserList");
        }

        public ActionResult BaseData()
        {
            BaseData m = model.BaseData.FirstOrDefault();
            if (m == null)
                return View();
            else
                return View(m);
        }

        [HttpPost]
        public ActionResult BaseData(BaseData m)
        {
            BaseData _m = model.BaseData.Where(p => p.Id == m.Id).FirstOrDefault();
            _m.OneOfTonPrice = m.OneOfTonPrice;
            _m.AllocationRate = m.AllocationRate;
            _m.MonitorMoney = m.MonitorMoney;
            _m.Name = m.Name;
            _m.HalfMoney = m.HalfMoney;
            model.SaveChanges();
            return RedirectToAction("DefalutReport");
        }

        public ActionResult ProductList()
        {
            List<Product> mList = model.Product.ToList();
            return View(mList);
        }

        public ActionResult ProductDetail(int? id)
        {
            Product m = model.Product.Where(p => p.Id == id).FirstOrDefault();
            if (m == null)
                return View();
            else
                return View(m);
        }

        [HttpPost]
        public ActionResult ProductDetail(Product m)
        {
            if (m.Id == 0)
            {
                model.Product.Add(m);
            }
            else
            {
                Product _m = model.Product.Where(p => p.Id == m.Id).FirstOrDefault();
                _m.ProductName = m.ProductName;
                _m.OneOfWeight = m.OneOfWeight;
                _m.OneOfPrice = m.OneOfPrice;
            }
            
            model.SaveChanges();
            return RedirectToAction("ProductList");
        }

        public ActionResult MidSendOrder(int? id)
        {
            BaseData basedata = model.BaseData.First();

            List<Warehousing> allws = model.Warehousing.ToList();
            double WarehousingCount = 0;
            if (allws.Any())
            {
                WarehousingCount = allws.Sum(p => p.WarehousingCount) * basedata.Name;
            }

            List<MidSendOrder> sendorders = model.MidSendOrder.ToList();
            double sendCount = 0;
            if (sendorders.Any())
            {
                sendCount = sendorders.Sum(p => p.SendCount);
            }

            double Respository = WarehousingCount - sendCount;

            ViewBag.Respository = Respository;

            MidSendOrder m = null;//model.SendOrder.Where(p => p.Order.Id == id && p.TaskStatus != "已收货").FirstOrDefault();
            if (m == null)
            {
                m = new MidSendOrder();
                m.CreateTime = DateTime.Now;
                Order o = model.Order.Where(p => p.Id == id).First();
                m.Order = o;
                m.TaskStatus = "待发货";
                m.TaskId = "SendOrder_" + m.CreateTime.ToString("yyyyMMddHHmmss");
                model.MidSendOrder.Add(m);
                model.SaveChanges();
            }
            return View(m);
        }

        [HttpPost]
        public ActionResult MidSendOrder(SendOrder m)
        {
            MidSendOrder _m = model.MidSendOrder.Where(p => p.Id == m.Id).FirstOrDefault();

            _m.TaskStatus = "准备发货";
            _m.SendCount = m.SendCount;
            _m.SendTime = m.SendTime; ;
            _m.SendDeterminePerson = m.SendDeterminePerson;
            _m.CustomerAddress = m.CustomerAddress;
            _m.Contact = m.Contact;
            _m.ContactPhone = m.ContactPhone;
            _m.Remarks = m.Remarks;

            model.SaveChanges();

            Order order = _m.Order;
            List<MidSendOrder> beforeSendOrders = model.MidSendOrder.Where(p => p.Order.Id == order.Id).ToList();
            double count = 0;
            if (beforeSendOrders.Any())
            {
                count = beforeSendOrders.Sum(p => p.SendCount);
            }

            if (count < _m.Order.ProductCount)
                _m.Order.OrderStatus = "部分发货";
            else
                _m.Order.OrderStatus = "全部发货";

            model.SaveChanges();
            return RedirectToAction("OrderList");
        }
        public ActionResult SendOrder(int? id)
        {
            BaseData basedata = model.BaseData.First();

            List<Warehousing> allws = model.Warehousing.ToList();
            double WarehousingCount = 0;
            if (allws.Any())
            {
                WarehousingCount = allws.Sum(p => p.WarehousingCount) * basedata.Name;
            }

            List<SendOrder> sendorders = model.SendOrder.ToList();
            double sendCount = 0;
            if (sendorders.Any())
            {
                sendCount = sendorders.Sum(p => p.SendCount);
            }

            double Respository = WarehousingCount - sendCount;

            ViewBag.Respository = Respository;

            List<Driver> drivers = model.MaterialDriver.Where(p => p.Status == 1).ToList();
            ViewBag.drivers = drivers;

            SendOrder m = null;//model.SendOrder.Where(p => p.Order.Id == id && p.TaskStatus != "已收货").FirstOrDefault();
            if (m == null)
            {
                m = new SendOrder();
                m.CreateTime = DateTime.Now;
                MidSendOrder o = model.MidSendOrder.Where(p => p.Id == id).First();
                m.MidSendOrder = o;
                m.TaskStatus = "待发货";
                m.TaskId = "SendOrder_" + m.CreateTime.ToString("yyyyMMddHHmmss");
                model.SendOrder.Add(m);
                model.SaveChanges();
            }
            return View(m);
        }

        [HttpPost]
        public ActionResult SendOrder(SendOrder m)
        {
            SendOrder _m = model.SendOrder.Where(p => p.Id == m.Id).FirstOrDefault();

            //_m.TaskStatus = "准备发货";
            _m.SendCount = m.SendCount;
            _m.SendTime = m.SendTime; ;
            _m.SendDeterminePerson = m.SendDeterminePerson;
            _m.CustomerAddress = m.CustomerAddress;
            _m.Contact = m.Contact;
            _m.ContactPhone = m.ContactPhone;
            _m.Remarks = m.Remarks;

            _m.TaskStatus = "已发货";

            _m.OneOfTonPrice = m.OneOfTonPrice;

            Driver driver = model.MaterialDriver.Where(p => p.Id == m.MaterialDriver.Id).First();
            _m.MaterialDriver = driver;

            model.SaveChanges();

            MidSendOrder order = _m.MidSendOrder;
            List<SendOrder> beforeSendOrders = model.SendOrder.Where(p => p.MidSendOrder.Id == order.Id).ToList();
            double count = 0;
            if (beforeSendOrders.Any())
            {
                count = beforeSendOrders.Sum(p => p.SendCount);
            }

            order.ReceiveCount = count;

            if (count < _m.MidSendOrder.SendCount)
                _m.MidSendOrder.TaskStatus = "部分发货";
            else
                _m.MidSendOrder.TaskStatus = "全部发货";

            model.SaveChanges();
            return RedirectToAction("MidSendOrderList");
        }

        public ActionResult SendOrderDriverList()
        {
            List<SendOrder> mList = model.SendOrder.Where(p => p.TaskStatus == "准备发货").ToList();
            return View(mList);
        }

        public ActionResult SendOrderDriver(int id)
        {
            List<Driver> drivers = model.MaterialDriver.Where(p => p.Status == 1).ToList();
            ViewBag.drivers = drivers;

            SendOrder m = model.SendOrder.Where(p => p.Id == id).FirstOrDefault();

            return View(m);
        }

        [HttpPost]
        public ActionResult SendOrderDriver(SendOrder m)
        {
            SendOrder _m = model.SendOrder.Where(p => p.Id == m.Id).FirstOrDefault();

            _m.TaskStatus = "已发货";
            
            _m.Remarks = m.Remarks;

            _m.OneOfTonPrice = m.OneOfTonPrice;

            Driver driver = model.MaterialDriver.Where(p => p.Id == m.MaterialDriver.Id).First();
            _m.MaterialDriver = driver;

            model.SaveChanges();
            return RedirectToAction("SendOrderDriverList");
        }

        public ActionResult ReceiveSendOrderList()
        {
            List<SendOrder> mList = model.SendOrder.Where(p => p.TaskStatus == "已发货").ToList();
            return View(mList);
        }


        public ActionResult ReceiveSendOrder(int id)
        {
            SendOrder m = model.SendOrder.Where(p => p.Id == id).FirstOrDefault();
            return View(m);
        }

        [HttpPost]
        public ActionResult ReceiveSendOrder(SendOrder m)
        {
            SendOrder _m = model.SendOrder.Where(p => p.Id == m.Id).FirstOrDefault();

            _m.ReceiveCount = m.ReceiveCount;
            _m.TaskStatus = "已收货";
            _m.ReceiveDeterminePerson = m.ReceiveDeterminePerson;
            _m.ReceiveTime = DateTime.Now;
            _m.Remarks = m.Remarks;

            model.SaveChanges();

            MidSendOrder order = _m.MidSendOrder;
            List<SendOrder> beforeSendOrders = model.SendOrder.Where(p => p.MidSendOrder.Id == order.Id).ToList();
            double count = 0;
            if(beforeSendOrders.Any())
            {
                count = beforeSendOrders.Sum(p => p.ReceiveCount);
            }
            _m.MidSendOrder.Order.DeliveryCount = count;
            _m.MidSendOrder.ReceiveCount = count;

            if (count > _m.MidSendOrder.SendCount)
                _m.MidSendOrder.TaskStatus = "全部发货";

            if (count >= order.Order.ProductCount)
                _m.MidSendOrder.Order.OrderStatus = "已完成";
            

            model.SaveChanges();
            return RedirectToAction("ReceiveSendOrderList");
        }

        public ActionResult RewardPunishmentList()
        {
            List<RewardPunishment> mList = model.RewardPunishment.ToList();
            return View(mList);
        }

        public ActionResult RewardPunishment(int? id)
        {
            List<string> teams = model.User.Select(p => p.UserTeam).Distinct().ToList();
            ViewBag.teams = teams;

            RewardPunishment m = model.RewardPunishment.Where(p => p.Id == id).FirstOrDefault();
            if (m == null)
                return View();
            else
                return View(m);
        }

        [HttpPost]
        public ActionResult RewardPunishment(RewardPunishment m)
        {
            if (m.Id == 0)
            {
                m.CreateTime = DateTime.Now;
                model.RewardPunishment.Add(m);

                string message = "";
                message += "日期:" + m.CreateTime.ToString("yyyy年MM月dd日") + "\n";
                message += "奖罚类型:" + m.Type + "\n";
                message += "奖罚金额:" + m.Count + "\n";
                message += "奖罚班组:" + m.Team + "\n";
                message += "提出人员:" + m.UserName + "\n";
                SendDDMessage(message);
            }
            else
            {
                RewardPunishment _m = model.RewardPunishment.Where(p => p.Id == m.Id).FirstOrDefault();
                _m.Date = m.Date;
                _m.Type = m.Type;
                _m.Team = m.Team;
                _m.Count = m.Count;
                _m.Remarks = m.Remarks;
                _m.UserName = m.UserName;
            }

            model.SaveChanges();        

            return RedirectToAction("RewardPunishmentList");
        }

        public ActionResult TeamAllocation(string team)
        {
            BaseData basedata = model.BaseData.First();

            if(string.IsNullOrEmpty(team))
            {
                return RedirectToAction("DefalutReport");
            }

            List<User> users = model.User.Where(p => p.UserTeam == team).ToList();          

            Product product = model.Product.First();

            double LastProductions = 0;
            double QualityRate = 0;
            double Income = 0;
            double Punishment = 0;
            double FixedAllocation = 0;
            double AllocationCount = 0;

            DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
            DateTime end = start.AddMonths(1);

            List<Warehousing> ws = model.Warehousing.Where(p => p.WarehousingTime >= start && p.WarehousingTime <= end).ToList();

            double AllLastProductions = 0;
            double AllLastProductions_ = 0;
            if (ws.Any())
            {
                AllLastProductions_ = ws.Sum(p => p.WarehousingCount * model.Product.Where(a => a.ProductName == p.ProductName).FirstOrDefault().OneOfPrice * basedata.Name);
                AllLastProductions = ws.Sum(p => p.WarehousingCount) * basedata.Name;
            }

            List<QualityInspection> qs = model.QualityInspection.Where(p => p.CreateTime >= start && p.CreateTime <= end && p.CheckTeam == team).ToList();

            double Scrap = 0;
            double Scrap_ = 0;
            if (qs.Any())
            {
                Scrap_ = qs.Sum(p => p.ScrapCount * model.Product.Where(a => a.ProductName == p.ProductName).FirstOrDefault().OneOfPrice * basedata.Name);
                Scrap = qs.Sum(p => p.ScrapCount) * basedata.Name;
            }

            LastProductions = AllLastProductions;

            QualityRate = (1 - (Scrap / AllLastProductions)) * 100;


            List<HalfWarehousing> hws = model.HalfWarehousing.Where(p => p.InspectionTime >= start && p.InspectionTime <= end && p.HalfWarehousingTeam == team).ToList();

            double Half = 0;
            if (hws.Any())
            {
                Half = hws.Sum(p => p.InspectionCount) * basedata.HalfMoney;
            }


            string dateString = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");
            List<RewardPunishment> rps = model.RewardPunishment.Where(p => p.Date == dateString).ToList();
            if(rps.Any())
            {
                Punishment = rps.Sum(p => p.Count);
            }

            Income = AllLastProductions_ + Half + Punishment;

            //Income =  LastProductions * product.OneOfPrice + Half * product.OneOfPrice + Punishment;


            FixedAllocation = Income * (1- basedata.AllocationRate);

            AllocationCount = Income * basedata.AllocationRate - basedata.MonitorMoney;

            List<TeamAllocation> tas = new List<TeamAllocation>();

            foreach (var u in users)
            {
                TeamAllocation ta = new TeamAllocation();
                ta.UserName = u.UserName;
                if(u.UserPermission == "班长")
                {
                    ta.FixedAllocation = (FixedAllocation / users.Count) + basedata.MonitorMoney;
                }
                else
                {
                    ta.FixedAllocation = FixedAllocation / users.Count;
                }

                tas.Add(ta);
            }

            ViewBag.LastProductions = LastProductions;
            ViewBag.QualityRate = QualityRate;
            ViewBag.Income = Income;
            ViewBag.Punishment = Punishment;
            ViewBag.FixedAllocation = FixedAllocation;
            ViewBag.AllocationCount = AllocationCount;


            return View(tas);
        }

        public ActionResult TeamAllocationQuery(string date)
        {            
            List<TeamAllocation> tas = model.TeamAllocation.Where(p => p.Date == date).ToList();

            return View(tas);
        }

        public ActionResult WaterTeamAllocation()
        {

            DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
            DateTime end = start.AddMonths(1);



            List<HalfWarehousing> hws = model.HalfWarehousing.Where(p => p.InspectionTime >= start && p.InspectionTime <= end && p.HalfWarehousingTeam == "水洗组" && p.IsConfirm == 1).ToList();

            List<string> userNames = hws.Select(p => p.InspectionUserName).Distinct().ToList();


            List<WaterTeamAllocation> tas = new List<WaterTeamAllocation>();

            foreach(var name in userNames)
            {
                List<HalfWarehousing> hw = hws.Where(p => p.InspectionUserName == name).ToList();

                WaterTeamAllocation ta = new WaterTeamAllocation();

                ta.Date = DateTime.Now.ToString("yyyy-MM");
                ta.FixedAllocation = hw.Sum(p => p.HalfWarehousingCount);

                double count = 0;

                foreach(var h in hw)
                {
                    double a = 0;
                    if (h.HalfWarehousingCount >= 2800)
                    {
                        double last = h.HalfWarehousingCount - 2800;

                        int n = Convert.ToInt32(last / 50);

                        a = 250 + n * 50;

                    }
                    else
                    {
                        a = -150;
                    }

                    count += a;
                }

                ta.Allocation = count;
                User u = model.User.Where(p => p.UserName == name).FirstOrDefault();
                ta.User = u;
                ta.CreateTime = DateTime.Now;
                ta.UserName = name;

                tas.Add(ta);
            }


            return View(tas);
        }

        public ActionResult WaterTeamAllocationQuery(string date)
        {
            List<WaterTeamAllocation> tas = model.WaterTeamAllocation.Where(p => p.Date == date).ToList();

            return View(tas);
        }


        [HttpPost]
        public string TeamAllocation(List<TeamAllocation> teamAllocations)
        {
            var CreateTime = DateTime.Now;
            foreach (var w in teamAllocations)
            {
                w.Date = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");
                w.CreateTime = CreateTime;
                if (!model.TeamAllocation.Where(p => p.Date == w.Date).Any())               
                model.TeamAllocation.Add(w);
                
            }
            model.SaveChanges();
            return "{\"success\":true}";
        }

        [HttpPost]
        public string WaterTeamAllocation(List<WaterTeamAllocation> teamAllocations)
        {
            var CreateTime = DateTime.Now;
            foreach (var w in teamAllocations)
            {
                w.Date = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");
                w.CreateTime = CreateTime;
                if (!model.WaterTeamAllocation.Where(p => p.Date == w.Date).Any())
                    model.WaterTeamAllocation.Add(w);
            }
            model.SaveChanges();
            return "{\"success\":true}";
        }

        public ActionResult DefalutReport(string team)
        {
            BaseData basedata = model.BaseData.First();

            double LastProductions = 0;
            double QualityRate = 0;
            double Income = 0;
            double Punishment = 0;

            Product product = model.Product.First();

            DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime end = start.AddMonths(1);

            List<Warehousing> ws = model.Warehousing.Where(p => p.WarehousingTime >= start && p.WarehousingTime <= end && p.WarehousingTeam == team).ToList();

            double AllLastProductions = 0;
            double AllLastProductions_ = 0;
            if (ws.Any())
            {
                AllLastProductions_ = ws.Sum(p => p.WarehousingCount * model.Product.Where(a => a.ProductName == p.ProductName).FirstOrDefault().OneOfPrice * basedata.Name);
                AllLastProductions = ws.Sum(p => p.WarehousingCount) * basedata.Name;
            }

            List<QualityInspection> qs = model.QualityInspection.Where(p => p.CreateTime >= start && p.CreateTime <= end && p.CheckTeam == team && p.CheckResult == "不合格").ToList();

            double Scrap = 0;
            double Scrap_ = 0;
            if (qs.Any())
            {
                Scrap_ = qs.Sum(p => p.ScrapCount * model.Product.Where(a => a.ProductName == p.ProductName).FirstOrDefault().OneOfPrice * basedata.Name);
                Scrap = qs.Sum(p => p.ScrapCount) * basedata.Name;
            }

            LastProductions = AllLastProductions;

            QualityRate = (1- (Scrap / AllLastProductions)) * 100;


            List<HalfWarehousing> hws = model.HalfWarehousing.Where(p => p.InspectionTime >= start && p.InspectionTime <= end && p.HalfWarehousingTeam == team).ToList();

            double Half = 0;
            if (hws.Any())
            {
                Half = hws.Sum(p => p.InspectionCount) * basedata.HalfMoney;
            }

            //LastProductions = LastProductions + Half;
             

            string dateString = DateTime.Now.ToString("yyyy-MM");
            List<RewardPunishment> rps = model.RewardPunishment.Where(p => p.Date == dateString).ToList();
            if (rps.Any())
            {
                Punishment = rps.Sum(p => p.Count);
            }

            //Income =  LastProductions * product.OneOfPrice + Punishment;
            Income = AllLastProductions_ + Half + Punishment;

            //
            List<TodayProductRate> tprList = new List<TodayProductRate>();

            DateTime todaystart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime todayend = todaystart.AddDays(1);

            List<Warehousing> todayws = model.Warehousing.Where(p => p.WarehousingTime >= todaystart && p.WarehousingTime <= todayend && p.WarehousingTeam == team).ToList();

            List<string> ProductNameList = todayws.Select(p => p.ProductName).Distinct().ToList();

            

            double todayProduction = 0;
            if(todayws.Any())
            {
                todayProduction = todayws.Sum(p => p.WarehousingCount) * basedata.Name;
            }

            foreach (var productname in ProductNameList)
            {
                TodayProductRate tpr = new TodayProductRate();
                tpr.ProductName = productname;
                tpr.ProductCount = 0;
                if (todayws.Where(p => p.ProductName == productname).Any())
                {
                    tpr.ProductCount = todayws.Where(p => p.ProductName == productname).Sum(p => p.WarehousingCount) * basedata.Name;
                }
                tpr.ProductRate = 0;
                if(todayProduction != 0)
                {
                    tpr.ProductRate = tpr.ProductCount / todayProduction *100;
                }

                tprList.Add(tpr);
            }

            //
            List<EveryDayProducttion> edpList = new List<EveryDayProducttion>();

            TimeSpan ts = todaystart - start;

            DateTime lastDay = todaystart.AddDays( -ts.Days);

            for(int i = 0; i < ts.Days +1 ; i++)
            {
                DateTime temptime = lastDay.AddDays(i);
                DateTime tempendtime = lastDay.AddDays(i + 1);

                EveryDayProducttion edp = new EveryDayProducttion();
                edp.Name = temptime.ToString("MM/dd");
                edp.Value = 0;
                List<Warehousing> edpws = model.Warehousing.Where(p => p.WarehousingTime >= temptime && p.WarehousingTime <= tempendtime).ToList();
                if(edpws.Any())
                {
                    edp.Value = edpws.Sum(p => p.WarehousingCount) * basedata.Name;
                }

                edpList.Add(edp);
            }

            ViewBag.LastProductions = LastProductions;
            ViewBag.QualityRate = QualityRate;
            ViewBag.Income = Income;
            ViewBag.Punishment = Punishment;
            ViewBag.tprList = tprList;
            ViewBag.edpList = edpList;

            return View();
        }

        public ActionResult ProductReport()
        {
            BaseData basedata = model.BaseData.First();

            Product product = model.Product.First();

            List<Order> orders = model.Order.ToList();
            double AllorderCount = 0;
            if (orders.Any())
            {
                AllorderCount = orders.Sum(p => p.ProductCount);
            }

            

            List<Warehousing> allws = model.Warehousing.ToList();
            double WarehousingCount = 0;
            if(allws.Any())
            {
                WarehousingCount = allws.Sum(p => p.WarehousingCount) * basedata.Name;
            }

            

            List<SendOrder> sendorders = model.SendOrder.ToList();
            double sendCount = 0;
            if(sendorders.Any())
            {
                sendCount = sendorders.Sum(p => p.SendCount);
            }

            double Respository = WarehousingCount - sendCount;

            List<OrderRepository> rsList = new List<OrderRepository>();
            List<string> productNameList = model.Product.Select(p => p.ProductName).Distinct().ToList();

            foreach(var productName in productNameList)
            {
                OrderRepository rs = new OrderRepository();
                List<Order> temporders = orders.Where(p => p.ProductName == productName).ToList();

                rs.ProductName = productName;
                rs.OrderCount = temporders.Sum(p => p.ProductCount - p.DeliveryCount);
                rs.RepositoryCount = allws.Where(p => p.ProductName == productName).Sum(p => p.WarehousingCount) - sendorders.Where(p => p.MidSendOrder.Order.ProductName == productName).Sum(p => p.SendCount);

                rsList.Add(rs);
            }

            double MonthProductions = 0;

            DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime end = start.AddMonths(1);

            List<Warehousing> monthws = model.Warehousing.Where(p => p.WarehousingTime >= start && p.WarehousingTime <= end).ToList();

            double AllMonthProductions = 0;
            if (monthws.Any())
            {
                AllMonthProductions = monthws.Sum(p => p.WarehousingCount) * basedata.Name;
            }

            List<QualityInspection> monthqs = model.QualityInspection.Where(p => p.CreateTime >= start && p.CreateTime <= end).ToList();

            double Scrap = 0;
            if (monthqs.Any())
            {
                Scrap = monthqs.Sum(p => p.ScrapCount) * basedata.Name;
            }

            MonthProductions = AllMonthProductions;

            //
            List<TodayProductRate> tprList = new List<TodayProductRate>();
            DateTime todaystart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime todayend = todaystart.AddDays(1);

            List<Warehousing> todayws = model.Warehousing.Where(p => p.WarehousingTime >= todaystart && p.WarehousingTime <= todayend).ToList();

            List<string> ProductNameList = todayws.Select(p => p.ProductName).Distinct().ToList();

            double todayProduction = 0;
            if (todayws.Any())
            {
                todayProduction = todayws.Sum(p => p.WarehousingCount) * basedata.Name;
            }

            foreach (var productname in ProductNameList)
            {
                TodayProductRate tpr = new TodayProductRate();
                tpr.ProductName = productname;
                tpr.ProductCount = 0;
                if (todayws.Where(p => p.ProductName == productname).Any())
                {
                    tpr.ProductCount = todayws.Where(p => p.ProductName == productname).Sum(p => p.WarehousingCount) * basedata.Name;
                }
                tpr.ProductRate = 0;
                if (todayProduction != 0)
                {
                    tpr.ProductRate = tpr.ProductCount / todayProduction * 100;
                }

                tprList.Add(tpr);
            }
            //
            List<TeamProduction> tpList = new List<TeamProduction>();            

            List<string> TeamList = monthws.Select(p => p.WarehousingTeam).Distinct().ToList();

            foreach (var team in TeamList)
            {
                TeamProduction tp = new TeamProduction();
                tp.Team = team;
                double ProductCount = 0;
                if (monthws.Where(p => p.WarehousingTeam == team).Any())
                {
                    ProductCount = monthws.Where(p => p.WarehousingTeam == team).Sum(p => p.WarehousingCount) * basedata.Name;
                }

                tp.ProductCount = ProductCount;

                tp.ProductRate = 0;
                if (AllMonthProductions != 0)
                {
                    tp.ProductRate = (ProductCount / AllMonthProductions) * 100;
                }

                double teamScrap = 0;
                if (monthqs.Where(p => p.CheckTeam == team).Any())
                {
                    teamScrap = monthqs.Where(p => p.CheckTeam == team && p.CheckResult == "不合格").Sum(p => p.ScrapCount) * basedata.Name;
                }
                tp.ScrapRate = 0;
                if(ProductCount != 0)
                {
                    tp.ScrapRate = (teamScrap / ProductCount) * 100;
                }


                tpList.Add(tp);
            }

            ViewBag.AllorderCount = AllorderCount;
            ViewBag.Respository = Respository;
            ViewBag.MonthProductions = MonthProductions;
            ViewBag.todayProduction = todayProduction;

            ViewBag.tprList = tprList;
            ViewBag.tpList = tpList;
            ViewBag.rsList = rsList;

            return View();
        }

        public ActionResult SaleReport()
        {
            BaseData basedata = model.BaseData.First();

            Product product = model.Product.First();

            List<Order> orders = model.Order.ToList();
            double AllorderCount = 0;
            double OrderPriceCount = 0;
            if (orders.Any())
            {
                AllorderCount = orders.Sum(p => p.ProductCount);
                OrderPriceCount = orders.Sum(p => p.OrderPrice);
            }

            double DoneOrderCount = 0;
            List<SendOrder> sendOrders = model.SendOrder.ToList();
            if (sendOrders.Any())
            {
                DoneOrderCount = sendOrders.Sum(p => p.ReceiveCount);
            }

            double ContinueOrderCount = AllorderCount - DoneOrderCount;

            List<Warehousing> allws = model.Warehousing.ToList();
            double WarehousingCount = 0;
            if (allws.Any())
            {
                WarehousingCount = allws.Sum(p => p.WarehousingCount) * basedata.Name;
            }



            List<SendOrder> sendorders = model.SendOrder.ToList();
            double sendCount = 0;
            if (sendorders.Any())
            {
                sendCount = sendorders.Sum(p => p.SendCount);
            }

            double Respository = WarehousingCount - sendCount;

            List<OrderRepository> rsList = new List<OrderRepository>();
            List<string> productNameList = model.Product.Select(p => p.ProductName).Distinct().ToList();

            foreach (var productName in productNameList)
            {
                OrderRepository rs = new OrderRepository();
                List<Order> temporders = orders.Where(p => p.ProductName == productName).ToList();

                rs.ProductName = productName;
                rs.OrderCount = temporders.Sum(p => p.ProductCount - p.DeliveryCount);
                rs.RepositoryCount = allws.Where(p => p.ProductName == productName).Sum(p => p.WarehousingCount) - sendorders.Where(p => p.MidSendOrder.Order.ProductName == productName).Sum(p => p.SendCount);

                rsList.Add(rs);
            }

            //
            List<CustomerSpread> csList = new List<CustomerSpread>();
            List<CustomerPriceRate> cprList = new List<CustomerPriceRate>();
            List<Customer> customers = orders.Select(p => p.Customer).Distinct().ToList();

            foreach(var custom in customers)
            {
                
                List<Order> temporder = orders.Where(p => p.Customer.Id == custom.Id).ToList();

                List<int> orderids = orders.Select(p => p.Id).ToList();

                List<SendOrder> tempsend = sendOrders.Where(p => orderids.Contains(p.MidSendOrder.Order.Id)).ToList();

                CustomerSpread cs = new CustomerSpread();

                double a = temporder.Sum(p => p.ProductCount);

                double b = tempsend.Sum(p => p.ReceiveCount);

                double c = (a - b) / a * 100;

                double d = 0;

                cs.CustomerName = custom.CustomerName;
                cs.CustomerCount = a;
                cs.Rate = c;
                cs.ScrapRate = d;

                csList.Add(cs);

                CustomerPriceRate cpr = new CustomerPriceRate();
                cpr.CustomerName = custom.CustomerName;
                cpr.CustomerPriceCount = temporder.Sum(p => p.OrderPrice);
                if(OrderPriceCount != 0)
                {
                    cpr.Rate = cpr.CustomerPriceCount / OrderPriceCount * 100;
                }

                cprList.Add(cpr);
            }
            //
            List<FurtureXisMonth> fxmList = new List<FurtureXisMonth>();
            DateTime monthstart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            for(int i = 0; i < 6; i++)
            {
                FurtureXisMonth fxm = new FurtureXisMonth();
                DateTime tempstart = monthstart.AddMonths(i);
                DateTime tempend = tempstart.AddMonths(1);

                List<Order> temporder = orders.Where(p => p.DeliveryTime >= tempstart && p.DeliveryTime <= tempend).ToList();

                fxm.Name = tempstart.ToString("yy/MM");
                if(temporder.Any())
                {
                    fxm.Value = temporder.Sum(p => p.ProductCount);
                }

                fxmList.Add(fxm);
            }

            ViewBag.AllorderCount = AllorderCount;
            ViewBag.DoneOrderCount = DoneOrderCount;
            ViewBag.ContinueOrderCount = ContinueOrderCount;
            ViewBag.OrderPriceCount = OrderPriceCount;
            ViewBag.csList = csList;
            ViewBag.fxmList = fxmList;
            ViewBag.cprList = cprList;
            ViewBag.rsList = rsList;


            return View();
        }

        public ActionResult MainReport()
        {
            BaseData basedata = model.BaseData.First();

            Product product = model.Product.First();

            List<Order> orders = model.Order.ToList();
            double AllorderCount = 0;
            double OrderPriceCount = 0;
            if (orders.Any())
            {
                AllorderCount = orders.Sum(p => p.ProductCount);
                OrderPriceCount = orders.Sum(p => p.OrderPrice);
            }

            double MonthOrderPriceCount = 0;
            DateTime monthstart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime monthend = monthstart.AddMonths(1);

            List<Order> monthOrders = orders.Where(p => p.CreateTime >= monthstart && p.CreateTime <= monthend).ToList();
            if(monthOrders.Any())
            {
                MonthOrderPriceCount = monthOrders.Sum(p => p.OrderPrice);
            }

            double DoneOrderCount = 0;
            List<SendOrder> sendOrders = model.SendOrder.ToList();
            if (sendOrders.Any())
            {
                DoneOrderCount = sendOrders.Sum(p => p.ReceiveCount);
            }

            double DoneOrderPriceCount = DoneOrderCount * product.OneOfPrice;

            List<QualityInspection> qs = model.QualityInspection.ToList();

            double Scrap = 0;
            if (qs.Any())
            {
                Scrap = qs.Where(p => p.ProcessName == "客户" && p.CheckResult == "不合格").Sum(p => p.ScrapCount);
            }

            double ScrapRate = 0;
            if(AllorderCount !=0)
            {
                ScrapRate = Scrap * basedata.Name / AllorderCount * 100;
            }

            List<Warehousing> allws = model.Warehousing.ToList();
            double WarehousingCount = 0;
            if (allws.Any())
            {
                WarehousingCount = allws.Sum(p => p.WarehousingCount) * basedata.Name;
            }

            List<SendOrder> sendorders = model.SendOrder.ToList();
            double sendCount = 0;
            if (sendorders.Any())
            {
                sendCount = sendorders.Sum(p => p.SendCount);
            }

            double Respository = WarehousingCount - sendCount;

            double MonthScrapRate = 0;

            List<Warehousing> monthws = model.Warehousing.Where(p => p.WarehousingTime >= monthstart && p.WarehousingTime <= monthend).ToList();

            double AllMonthProductions = 0;
            if (monthws.Any())
            {
                AllMonthProductions = monthws.Sum(p => p.WarehousingCount) *basedata.Name;
            }

            List<QualityInspection> monthqs = model.QualityInspection.Where(p => p.CreateTime >= monthstart && p.CreateTime <= monthend && p.CheckResult == "不合格").ToList();

            double MonthScrap = 0;
            if (monthqs.Any())
            {
                MonthScrap = monthqs.Sum(p => p.ScrapCount) * basedata.Name;
            }

            if(AllMonthProductions != 0)
            {
                MonthScrapRate = MonthScrap / AllMonthProductions *100;
            }

            List<MonthProduction> mpList = new List<MonthProduction>();
            DateTime lastmonthstart = monthstart.AddMonths(-5);

            for(int i=0;i<6;i++)
            {
                MonthProduction mp = new MonthProduction();

                DateTime tempstart = lastmonthstart.AddMonths(i);
                DateTime tempend = tempstart.AddMonths(1);

                List<Warehousing> tempmonthws = model.Warehousing.Where(p => p.WarehousingTime >= tempstart && p.WarehousingTime <= tempend).ToList();

                mp.Name = tempstart.ToString("yy/MM");

                if (tempmonthws.Any())
                {
                    mp.Value = tempmonthws.Sum(p => p.WarehousingCount);
                }

                mpList.Add(mp);
            }

            List<MonthSand> msList = new List<MonthSand>();

            for (int i = 0; i < 6; i++)
            {
                MonthSand ms = new MonthSand();

                DateTime tempstart = lastmonthstart.AddMonths(i);
                DateTime tempend = tempstart.AddMonths(1);

                List<MaterialOrder> tempmonthmo = model.MaterialOrder.Where(p => p.ReceiveTime >= tempstart && p.ReceiveTime <= tempend).ToList();

                ms.Name = tempstart.ToString("yy/MM");

                if (tempmonthmo.Any())
                {
                    ms.Value = tempmonthmo.Sum(p => p.ReceiveCount);
                }

                msList.Add(ms);
            }

            ViewBag.OrderPriceCount = OrderPriceCount;
            ViewBag.MonthOrderPriceCount = MonthOrderPriceCount;
            ViewBag.AllorderCount = AllorderCount;
            ViewBag.DoneOrderCount = DoneOrderCount;
            ViewBag.DoneOrderPriceCount = DoneOrderPriceCount;
            ViewBag.ScrapRate = ScrapRate;
            ViewBag.WarehousingCount = WarehousingCount;
            ViewBag.Respository = Respository;
            ViewBag.MonthScrapRate = MonthScrapRate;
            ViewBag.mpList = mpList;
            ViewBag.msList = msList;

            return View();
        }

        public ActionResult MaterialDriverQuery(int? id)
        {
            List<Driver> drivers = model.MaterialDriver.ToList();
            ViewBag.drivers = drivers;

            List<MaterialOrder> mList = new List<MaterialOrder>();
            if (id != null)
                mList = model.MaterialOrder.Where(p => p.MaterialDriver.Id == id).OrderByDescending(p => p.IsComfirm).ToList();

            ViewBag.DriverID = id;

            return View(mList);
        }

        public string MaterialConfirm(int id)
        {
            MaterialOrder order = model.MaterialOrder.Where(p => p.Id == id).First();
            order.IsComfirm = 1;
            model.SaveChanges();
            return "{\"data\": true }";
        }

        public ActionResult SendOrderDriverQuery(int? id)
        {
            List<Driver> drivers = model.MaterialDriver.ToList();
            ViewBag.drivers = drivers;

            List<SendOrder> mList = new List<SendOrder>();
            if (id != null)
                mList = model.SendOrder.Where(p => p.MaterialDriver.Id == id).OrderByDescending(p => p.IsComfirm).ToList();

            ViewBag.DriverID = id;

            return View(mList);
        }

        public string SendOrderConfirm(int id)
        {
            SendOrder order = model.SendOrder.Where(p => p.Id == id).First();
            order.IsComfirm = 1;
            model.SaveChanges();
            return "{\"data\": true }";
        }

    }
}