using DCStorage.Models;
using HigLabo.Core;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Web.Mvc;
using System;
using DCStorage.DataContext;
using System.Linq;

namespace DCStorage.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _phonebookContext = new ApplicationDbContext();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public HomeController()
        {

        }

        //[HttpPost]
        public ActionResult Index(HomeModels home)
        {
            log.Info("Starting...");
            
            MainModel model = new MainModel();
            if (home != null)
            {
                model = MappingModel(home);
            }
            
            if (model.SelectFunc == "登録")
            {
                Register registerModel = model.Map(new Register());
                if (model.ReferType == "工番検索")
                {

                    if (model.Completion_Date != null && model.Completion_Date != "" && model.Completion_Date != "0")
                    {
                        registerModel.RegistorType = "完成";
                        registerModel.Order_Date_View = model.Completion_Date;
                        registerModel.Order_Amount_View = model.Completion_Amount.ToString();
                    }
                    else
                    {
                        registerModel.RegistorType = "受注";
                        registerModel.Completion_Date = "0";
                        registerModel.Order_Date_View = model.Order_Date;
                        registerModel.Order_Amount_View = model.Order_Amount.ToString();
                    }
                }
                else if(model.ReferType == "発注残照会")
                {

                    if (model.Completion_Date != null && model.Completion_Date != "" && model.Completion_Date != "0")
                    {
                        registerModel.RegistorType = "検収";
                        registerModel.Order_Date_View = model.Completion_Date;
                        registerModel.Order_Amount_View = model.Completion_Amount.ToString();
                    }
                    else
                    {
                        registerModel.RegistorType = "発注";
                        registerModel.Completion_Date = "0";
                        registerModel.Order_Date_View = model.Order_Date;
                        registerModel.Order_Amount_View = model.Order_Amount.ToString();
                    }
                }
                else
                {
                    //nothing
                }
                //_logger.LogInformation("HomeController Index End.");
                log.Info("RedirectToAction 登録.");
                return RedirectToAction("Index", "Registor", registerModel);
            }
            else
            {

                SearchModels searchModel = MappingModelSearch(model);//.Map(new SearchModels());
                if (model.ReferType == "工番検索")
                {

                    if (model.Completion_Date != null && model.Completion_Date != "" && model.Completion_Date != "0")
                    {
                        searchModel.RegistorType = "完成";
                        searchModel.Order_Amount = model.Completion_Amount;
                    }
                    else
                    {
                        searchModel.RegistorType = "受注";
                    }
                }
                else if (model.ReferType == "発注残照会")
                {

                    if (model.Completion_Date != null && model.Completion_Date != "" && model.Completion_Date != "0")
                    {
                        searchModel.RegistorType = "検収";
                        searchModel.Order_Amount = model.Completion_Amount;
                    }
                    else
                    {
                        searchModel.RegistorType = "発注";
                    }
                }
                else
                {
                    //nothing
                }
                //_logger.LogInformation("HomeController Index End.");
                log.Info("RedirectToAction 登録.");
                return RedirectToAction("Index", "Search", searchModel);
            }
            

        }

        private MainModel MappingModel(HomeModels model)
        {
            MainModel result = new MainModel();
            result.UserId = model.p1;
            result.Department_Name = model.p2;
            result.Order_No = model.p3;
            result.Supplier_Cd = model.p4;
            result.Order_Amount = model.p5;
            result.Supplier_Name = model.p6;
            result.Order_Date = model.p7;
            result.ReferType = model.p8;
            result.Work_No = model.p9;
            result.Currency_Ut = model.p10;
            result.Completion_Date = model.p11;
            result.Completion_Amount = model.p12;
            result.SelectFunc = model.p13;

            return result;
        }

        private SearchModels MappingModelSearch(MainModel main)
        {
            SearchModels result = new SearchModels();
            result.UserId = main.UserId;
            result.Department_Name = main.Department_Name;
            result.Order_No = main.Order_No;
            result.Supplier_Cd = main.Supplier_Cd;
            result.Order_Amount = main.Order_Amount;
            result.Supplier_Name = main.Supplier_Name;
            result.Order_Date = main.Order_Date != null ? DateTime.ParseExact(main.Order_Date, "yyyyMMdd", CultureInfo.InvariantCulture) : (DateTime?)null;
            //result.ReferType = main.ReferType;
            result.Work_No = main.Work_No;
            result.Currency_Ut = main.Currency_Ut;
            //result.Completion_Date = DateTime.Parse(main.Completion_Date);
            //result.Completion_Amount = main.Completion_Amount;
            //result.SelectFunc = main.SelectFunc;

            return result;
        }

    }
}