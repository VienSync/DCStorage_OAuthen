using DCStorage.DataContext;
using DCStorage.Models;
using HigLabo.Core;
using Newtonsoft.Json;
using System.Globalization;
using System.Data;
using Box.V2;
using Box.V2.Auth;
using Box.V2.Config;
using System.IO;
using Org.BouncyCastle.Utilities;
using System.Security.Policy;
using Box.V2.JWTAuth;
using System.Web.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Data.Entity;
using Mysqlx;

namespace DCStorage.Controllers
{
    public class SearchController : Controller
    {
        private ApplicationDbContext _phonebookContext = new ApplicationDbContext();
        private ApplicationDbContext _attachContext = new ApplicationDbContext();
        private string AuthenticationUrl;
        private string ClientId;
        private string ClientSecret;
        private string BoxParentId;
        private List<string> UploadFolderName;
        private BoxAppSettings BoxAppSettingJwt;
        private string EnterpriseIdJwt;

        public List<string> orderReceived { get; set; }
        public List<string> placeOrder { get; set; }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public SearchController()
        {
            getBoxconfig();
        }

        [HttpGet]
        public ActionResult Index(SearchModels model)
        {
            log.Info("Starting...");
            ViewData["Message"] = TempData["Message"];
            TempData["Message"] = "";
            //if (model.RegistorType == "発注")
            //{
            //    model.lstOrder = placeOrder;
            //    model.lstOrder.AddRange(orderReceived);
            //}
            //else
            //{
            //    model.lstOrder = placeOrder; 
            //    model.lstOrder.AddRange(orderReceived);
            //}
            if (model.Reg_Date_Start == null && model.Reg_Date_End == null)
            {
                model.Reg_Date_Start =  DateTime.Now.Date;
                model.Reg_Date_End =  DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999); ;
            }
            model.lstOrder = placeOrder;
            model.lstOrder.AddRange(orderReceived);
            model.Personal_Cd = model.UserId;
            log.Info("Ending...");
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> DownloadMultipleFiles(string dataseq)
        {
            log.Info("Starting Download...");
            try
            {
                //_logger.LogInformation("SearchController Index Start.");
                List<C_ER002> fileID_lst = new List<C_ER002>();
                fileID_lst = await _attachContext.C_ER002s
                                                         .Where(file => file.Data_Seq == dataseq)
                                                         .ToListAsync();
                // JWT Authen Config
                var boxConfig = new BoxConfigBuilder(BoxAppSettingJwt.ClientID, BoxAppSettingJwt.ClientSecret,
                    EnterpriseIdJwt, BoxAppSettingJwt.AppAuth.PrivateKey, BoxAppSettingJwt.AppAuth.Passphrase, BoxAppSettingJwt.AppAuth.PublicKeyID)
                    .Build();
                var boxJWT = new BoxJWTAuth(boxConfig);
                log.Info("Starting GetToken...");
                var adminToken = await boxJWT.AdminTokenAsync();
                var client = boxJWT.AdminClient(adminToken);

                List<String> fileResults = new List<String>();
                foreach (var attachFile in fileID_lst)
                {
                    try
                    {
                        Uri downloadUri = await client.FilesManager.GetDownloadUriAsync(id: attachFile.Seq_No);
                        fileResults.Add(downloadUri.ToString());
                    }
                    catch (Box.V2.Exceptions.BoxException ex)
                    {
                        log.Error(ex.Message);
                    }
                }
                log.Info("Ending Download...");
                return Json(new { fileUris = fileResults });
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                Console.WriteLine($"Error retrieving information for file ID {dataseq}: {ex.Message}");
                return Json(new { error = true, message = "ダウンロード失敗" });
            }
        }

        [HttpPost]
        public ActionResult Index(SearchModels model, string userId)
        {
            log.Info("Starting...");
            try
            {
                ViewData["Message"] = "";
                model.lstOrder = placeOrder;
                model.lstOrder.AddRange(orderReceived);
                //if (model.RegistorType == "発注")
                //{
                //    model.lstOrder = placeOrder;
                //    model.lstOrder.AddRange(orderReceived);
                //}
                //else
                //{
                //    model.lstOrder = placeOrder;
                //    model.lstOrder.AddRange(orderReceived);
                //}
                List<ResultSearch> resultSearchList = new List<ResultSearch>();

                //LINQ を適用する。
                var query = from pb in _phonebookContext.C_ER001s
                            select pb;

                // 値がある場合,条件を適用する。
                //オーダー情報
                if (model.Order_No != null)
                {
                    query = query.Where(s => s.Order_No.Contains(model.Order_No.Trim()));
                }

                //工番
                if (model.Work_No != null)
                {
                    query = query.Where(s => s.Work_No.Contains(model.Work_No.Trim()));
                }

                //通貨
                if (model.Currency_Ut != null)
                {
                    query = query.Where(s => s.Currency_Ut.Contains(model.Currency_Ut.Trim()));
                }
                //取引先コード
                if (model.Supplier_Cd != null)
                {
                    query = query.Where(s => s.Supplier_Cd.Contains(model.Supplier_Cd.Trim()));
                }
                //取引先名称
                if (model.Supplier_Name != null)
                {
                    query = query.Where(s => s.Supplier_Name.Contains(model.Supplier_Name.Trim()));
                }
                //取引日付
                if (model.Order_Date2 != null)
                {
                    model.Order_Date2 = model.Order_Date2.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
                if (model.Order_Date1 != null && model.Order_Date2 != null)
                {
                    query = query.Where(s => (s.Order_Date >= model.Order_Date1) && (s.Order_Date <= model.Order_Date2));
                }

                if (model.Order_Date1 != null && model.Order_Date2 == null)
                {
                    query = query.Where(s => (s.Order_Date >= model.Order_Date1));
                }

                if (model.Order_Date1 == null && model.Order_Date2 != null)
                {
                    query = query.Where(s => s.Order_Date <= model.Order_Date2);
                }


                //取引金額
                if (model.Order_Amount1 != null && model.Order_Amount2 != null)
                {
                    query = query.Where(s => (s.Order_Amount > model.Order_Amount1) && (s.Order_Amount < model.Order_Amount2));
                }

                if (model.Order_Amount1 == null && model.Order_Amount2 != null)
                {
                    query = query.Where(s => (s.Order_Amount < model.Order_Amount2));
                }

                if (model.Order_Amount1 != null && model.Order_Amount2 == null)
                {
                    query = query.Where(s => s.Order_Amount > model.Order_Amount1);
                }

                //登録者
                if (model.Personal_Cd != null)
                {
                    query = query.Where(s => s.Personal_Cd.Contains(model.Personal_Cd));
                }

                //帳票タイプ
                if (model.Report_Typ != null && model.Report_Typ != "全て")
                {
                    query = query.Where(s => s.Report_Typ.Contains(model.Report_Typ));
                }
                //システム登録日付
                if (model.Reg_Date_End != null)
                {
                    model.Reg_Date_End = model.Reg_Date_End.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
                if (model.Reg_Date_Start != null && model.Reg_Date_End != null)
                {
                    query = query.Where(s => (s.Reg_Date >= model.Reg_Date_Start) && (s.Reg_Date <= model.Reg_Date_End));
                }
                if (model.Reg_Date_Start != null && model.Reg_Date_End == null)
                {
                    query = query.Where(s => (s.Reg_Date >= model.Reg_Date_Start));
                }

                if (model.Reg_Date_Start == null && model.Reg_Date_End != null)
                {
                    query = query.Where(s => (s.Reg_Date <= model.Reg_Date_End));
                }

                //データの元形式

                if (model.Report_Fmt != null && model.Report_Fmt != "両方")
                {
                    query = query.Where(s => s.Report_Fmt == model.Report_Fmt);
                }

                //データの状態
                if (model.Del_Flag != null && model.Del_Flag != 2)
                {
                    query = query.Where(s => s.Del_Flag == model.Del_Flag);
                }

                // 最後の結果を取得する。
                List<C_ER001> lstPhoneBook = query.OrderByDescending(item => item.Reg_Date).ThenByDescending(e => e.Reg_Time).Take(100).ToList();
                resultSearchList.AddRange(lstPhoneBook.Select(item => item.Map(new ResultSearch())));
                model.lstResultSearches = resultSearchList;
                log.Info("Ending...");
                return View(model);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return View("Error", ex.Message);
            }

        }


        /*------------------------------------------------------------------------------
        機能名：CheckAndRefreshToken
        引数：なし
        戻る値：AccessTokenの有無
        ------------------------------------------------------------------------------*/
        [HttpPost]
        //public async Task<ActionResult> CheckAndRefreshToken()
        //{
        //    string refreshToken = HttpContext.Session.GetString("refreshToken");
        //    string expiresAtString = HttpContext.Session.GetString("expiresAt");
        //    DateTime.TryParseExact(expiresAtString, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime expiresAt);

        //    var client = new HttpClient();

        //    // Check Access Token
        //    if (DateTime.UtcNow < expiresAt)
        //    {
        //        // Access Token valid
        //        return Json(new { success = true, message = "Token Valid!" });
        //    }
        //    else
        //    {
        //        // Token expried, refresh
        //        var refreshContent = new FormUrlEncodedContent(new[]
        //        {
        //            new KeyValuePair<string, string>("grant_type", "refresh_token"),
        //            new KeyValuePair<string, string>("refresh_token", refreshToken),
        //            new KeyValuePair<string, string>("client_id", ClientId),
        //            new KeyValuePair<string, string>("client_secret", ClientSecret)
        //        });

        //        var refreshResponse = await client.PostAsync(AuthenticationUrl, refreshContent);

        //        if (refreshResponse.IsSuccessStatusCode)
        //        {
        //            // Get New AccessToken
        //            var newTokenInfo = JsonConvert.DeserializeObject<TokenInfo>(await refreshResponse.Content.ReadAsStringAsync());
        //            HttpContext.Session.SetString("accessToken", newTokenInfo != null ? newTokenInfo.access_token : "");

        //            // Token refresh success
        //            return Json(new { success = true, message = "Token Valid!" });
        //        }
        //        else
        //        {
        //            // Refresh failed
        //            return Json(new { success = false, message = "Token expried!" });
        //        }
        //    }
        //}

        public void getBoxconfig()
        {
            string filePath = "C:\\DCStorage\\config.json";
            string jsonContent = System.IO.File.ReadAllText(filePath);
            ConfigModels config = JsonConvert.DeserializeObject<ConfigModels>(jsonContent);
            //config for JWT
            placeOrder = config.placeOrder;
            orderReceived = config.orderReceived;            
            BoxParentId = config.BoxParentId;
            UploadFolderName = config.UploadFolderName;            
            BoxAppSettingJwt = config.BoxAppSettings;
            EnterpriseIdJwt = config.EnterpriseID;
        }
    }
}
