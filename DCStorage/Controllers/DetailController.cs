using DCStorage.DataContext;
using DCStorage.Models;
using System.Web.Mvc;
using System.Diagnostics;
using System.Linq;
using HigLabo.Core;
using Box.V2.Config;
using Box.V2.JWTAuth;
using Box.V2.Models;
using System.Transactions;
using Newtonsoft.Json;
using Box.V2.Auth;
using Box.V2;
using Box.V2.Exceptions;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.Entity;
using System;
using System.Web.UI.WebControls;
using System.Globalization;

namespace DCStorage.Controllers
{
    public class DetailController : Controller
    {
        private ApplicationDbContext _phonebookContext = new ApplicationDbContext();
        private ApplicationDbContext _attachContext = new ApplicationDbContext();
        private string BoxParentId;
        private List<string> UploadFolderName;
        private BoxAppSettings BoxAppSettingJwt;
        private string EnterpriseIdJwt;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DetailController()
        {
            getBoxconfig();
        }

        [HttpGet]
        public async Task<ActionResult> Index(string data_seq, string userId,string getValueFrom)
        {
            try
            {
                //_logger.LogInformation("DetailController Index Start.");
                log.Info("Starting...");
                var model = await _phonebookContext.
                    C_ER001s.Where(record => record.Data_Seq == data_seq).FirstOrDefaultAsync();

                List<C_ER002> attachFile = new List<C_ER002>();
                attachFile = await _attachContext.C_ER002s
                                                         .Where(file => file.Data_Seq == data_seq)
                                                         .ToListAsync();
                // JWT Authen Config
                var boxConfig = new BoxConfigBuilder(BoxAppSettingJwt.ClientID, BoxAppSettingJwt.ClientSecret,
                    EnterpriseIdJwt, BoxAppSettingJwt.AppAuth.PrivateKey, BoxAppSettingJwt.AppAuth.Passphrase, BoxAppSettingJwt.AppAuth.PublicKeyID)
                    .Build();
                var boxJWT = new BoxJWTAuth(boxConfig);

                var adminToken = await boxJWT.AdminTokenAsync();
                var client = boxJWT.AdminClient(adminToken);

                DetailModels detailModel = new DetailModels();
                var listFileUri = new List<FileBox>();
                foreach (var file in attachFile)
                {
                    try
                    {
                        var fileFromBox = await client.FilesManager.GetInformationAsync(file.Seq_No);                        
                        if (fileFromBox.ItemStatus != "trashed")
                        {
                            Uri embedUri = await client.FilesManager.GetPreviewLinkAsync(id: file.Seq_No);
                            FileBox fileInfo = new FileBox();
                            fileInfo.IdFile = file.Seq_No;
                            fileInfo.FileName = fileFromBox.Name;
                            fileInfo.FileUri = embedUri.ToString();
                            listFileUri.Add(fileInfo);
                        }
                        else
                        {
                            //なし
                        }
                    }
                    catch (Box.V2.Exceptions.BoxException ex)
                    {
                        //なし
                    }
                }

                detailModel = model.Map(new DetailModels());
                detailModel.FileUriList = listFileUri;
                detailModel.UserId = userId;
                detailModel.Value_Search = getValueFrom;
                //_logger.LogInformation("DetailController Index End.");
                log.Info("Ending...");
                return View(detailModel);
            }
            catch (Exception ex)
            {
                //_logger.LogInformation("DetailController Index End.");
                log.Error(ex.Message);
                return View("Error", ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Index(DetailModels modelDetails)
        {
            log.Info("Starting...");
            try
            {
                //_logger.LogInformation("DetailController Index Start.");
                C_ER001 model = MappingModelDetail(modelDetails);
                var result = Delete(model, modelDetails.UserId);
                TempData["Message"] = result;
                log.Info("Ending...");
                return RedirectToAction("Index", "Search", new { UserId = modelDetails.UserId });
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                //_logger.LogInformation("DetailController Index End.");
                return View("Error", ex.Message);
            }
        }

        public async Task<ActionResult> LoadFile(string idfile)
        {
            log.Info("Starting...");
            try
            {
                // JWT Authen Config
                var boxConfig = new BoxConfigBuilder(BoxAppSettingJwt.ClientID, BoxAppSettingJwt.ClientSecret,
                    EnterpriseIdJwt, BoxAppSettingJwt.AppAuth.PrivateKey, BoxAppSettingJwt.AppAuth.Passphrase, BoxAppSettingJwt.AppAuth.PublicKeyID)
                    .Build();
                var boxJWT = new BoxJWTAuth(boxConfig);
                var adminToken = await boxJWT.AdminTokenAsync();
                var client = boxJWT.AdminClient(adminToken);

                Uri embedUri = await client.FilesManager.GetPreviewLinkAsync(id: idfile);
                log.Info("Ending...");
                return Json(new { success = true, srcfile = embedUri.ToString()});
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return Json(new { success = false, message = "Boxからのファイル読み込みでエラーが発生しています。" });
            }
        }

        public string Delete(C_ER001 model, string userId)
        {
            log.Info("Starting...");
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    //_logger.LogInformation("DetailController Delete Start.");

                    var recordToUpdate = _phonebookContext.C_ER001s
                        .Where(record => record.Data_Seq == model.Data_Seq)
                        .FirstOrDefault();
                    if (recordToUpdate != null)
                    {
                        recordToUpdate.Del_Flag = 1;
                        recordToUpdate.Del_User = userId != null? userId : null;
                        recordToUpdate.Del_Date = DateTime.Now.Date;
                        recordToUpdate.Del_Time = DateTime.Now.TimeOfDay;
                        recordToUpdate.Del_Memo = model.Del_Memo != null? model.Del_Memo.Length <= 50 ? model.Del_Memo : model.Del_Memo.Substring(0, 50) : null;
                        _phonebookContext.SaveChanges();
                    }
                    //model.Del_Flag = 1;
                    //model.Del_Date = DateTime.Now.Date;
                    //model.Del_Time = DateTime.Now.TimeOfDay;
                    //_phonebookContext.PhoneBookRegistrationMasters.Update(model);
                    //_phonebookContext.SaveChangesAsync();
                    scope.Complete();
                    var message = "削除が完了しました。!";
                    log.Info("Ending...");
                    return message;
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    scope.Dispose();
                    var message = "削除が失敗しました。!";
                    return message;
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> DownloadSingleFile(string fileID)
        {
            log.Info("Starting DownloadFile...");
            try
            {
                // JWT Authen Config
                var boxConfig = new BoxConfigBuilder(BoxAppSettingJwt.ClientID, BoxAppSettingJwt.ClientSecret,
                    EnterpriseIdJwt, BoxAppSettingJwt.AppAuth.PrivateKey, BoxAppSettingJwt.AppAuth.Passphrase, BoxAppSettingJwt.AppAuth.PublicKeyID)
                    .Build();
                var boxJWT = new BoxJWTAuth(boxConfig);
                var adminToken = await boxJWT.AdminTokenAsync();
                var client = boxJWT.AdminClient(adminToken);
                Uri downloadUri = await client.FilesManager.GetDownloadUriAsync(id: fileID);

                log.Info("Ending DownloadFile...");
                return Json(new { fileUri = downloadUri.ToString() });
            }
            catch (Box.V2.Exceptions.BoxException ex)
            {
                log.Error(ex.Message);
                Console.WriteLine($"Error retrieving information for file ID {fileID}: {ex.Message}");
                return Json(new { error = true, message = "ダウンロード失敗" });
            }
        }

        public void getBoxconfig()
        {
            string filePath = "C:\\DCStorage\\config.json";
            string jsonContent = System.IO.File.ReadAllText(filePath);
            ConfigModels config = JsonConvert.DeserializeObject<ConfigModels>(jsonContent);
            //config for JWT
            BoxParentId = config.BoxParentId;
            UploadFolderName = config.UploadFolderName;
            BoxAppSettingJwt = config.BoxAppSettings;
            EnterpriseIdJwt = config.EnterpriseID;
        }

        private C_ER001 MappingModelDetail(DetailModels model)
        {
            C_ER001 result = new C_ER001();
            result.Data_Seq = model.Data_Seq;
            result.Order_No = model.Order_No;
            result.Work_No = model.Work_No;
            result.Supplier_Name = model.Supplier_Name;
            result.Supplier_Cd = model.Supplier_Cd;
            result.Order_Date = model.Order_Date;
            result.Order_Amount = int.Parse(model.Order_Amount.Replace(",", ""));
            result.Currency_Ut = model.Currency_Ut;
            result.Personal_Cd = model.Personal_Cd;
            result.Department_Name = model.Department_Name;
            result.Source_Scrn = model.Source_Scrn;
            result.Source_Btn = model.Source_Btn;
            result.Detail_No = model.Detail_No;
            result.Report_Typ = model.Report_Typ;
            result.Report_Fmt = model.Report_Fmt;
            result.Revision_Cnt = model.Revision_Cnt;
            result.Reg_User = model.Reg_User;
            result.Reg_Date = model.Reg_Date;
            result.Reg_Time = model.Reg_Time;
            result.Del_Flag = model.Del_Flag != null ? int.Parse(model.Del_Flag) : (int?)null;
            result.Del_User = model.Del_User;
            result.Del_Date = model.Del_Date;
            result.Del_Time = model.Del_Time;
            result.Del_Memo = model.Del_Memo;

            return result;
        }
    }
}