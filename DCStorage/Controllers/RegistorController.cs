using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DCStorage.DataContext;
using DCStorage.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Transactions;
using Box.V2.Config;
using Box.V2.JWTAuth;
using Box.V2.Models;
using Box.V2;
using System.Net.Http;
using Box.V2.Auth;


namespace DCStorage.Controllers
{
    public class RegistorController : Controller
    {

        //private readonly ILogger<RegistorController> _logger;
        private ApplicationDbContext _context = new ApplicationDbContext();

        // BOX API Config
        private string RedirectUri;
        private string AuthenticationUrl;
        private string ClientId;
        private string ClientSecret;
        private string UploadUri;

        // BOX API Config
        private string BoxParentId;
        private List<string> UploadFolderName;
        private BoxAppSettings BoxAppSettingJwt;
        private string EnterpriseIdJwt;
        public List<string> orderReceived { get; set; }
        public List<string> placeOrder { get; set; }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public RegistorController()
        {
            getBoxconfig();
        }
        /*------------------------------------------------------------------------------
        機能名：初期化画面ーIndex
        引数：なし
        戻る値：URLからPara
        ------------------------------------------------------------------------------*/
        public ActionResult Index(Register data)
        {
            log.Info("Starting...");
            if (data.RegistorType == "発注" || data.RegistorType == "検収")
            {
                data.lstOrder = placeOrder;
            }
            else
            {
                data.lstOrder = orderReceived;
            }
            return View(data);
        }

        /*------------------------------------------------------------------------------
        機能名：AuthenticateWithBox
        引数：なし
        戻る値：証憑したURLを移動する
        ------------------------------------------------------------------------------*/
        [HttpGet]
        public ActionResult AuthenticateWithBox()
        {
            var authorizationUrl = $"https://account.box.com/api/oauth2/authorize?client_id={ClientId}&response_type=code&redirect_uri={RedirectUri}";
            return Redirect(authorizationUrl);
        }
        /*------------------------------------------------------------------------------
        機能名：OAuthCallback
        引数：証憑したURLからコードを取得
        戻る値：Boxに保有するファイルを移動する
        ------------------------------------------------------------------------------*/
        [HttpGet]
        public ActionResult OAuthCallback(string code)
        {
            // Call method to change code to AccessToken
            ExchangeCodeForToken(code);
            return Content("<script>window.opener.postMessage('success', '*'); window.close();</script>");
        }

        /*------------------------------------------------------------------------------
        機能名：ExchangeCodeForToken
        引数：証憑したコード
        戻る値：なし
        ------------------------------------------------------------------------------*/
        private void ExchangeCodeForToken(string code)
        {
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("client_secret", ClientSecret),
                new KeyValuePair<string, string>("redirect_uri", RedirectUri)
            });

            var response = client.PostAsync(AuthenticationUrl, content).Result;

            // Get Token from code
            var data = response.Content.ReadAsStringAsync().Result;
            var token = JsonConvert.DeserializeObject<TokenInfo>(data);
            var accessToken = token != null ? token.access_token : "";
            var refreshToken = token != null ? token.refresh_token : "";
            var expiresIn = token != null ? token.expires_in : 0;

            DateTime expiresAt = DateTime.Now.AddSeconds(expiresIn);
            string expiresAtString = expiresAt.ToString("yyyy-MM-ddTHH:mm:ss");

            Session["accessToken"] = accessToken;
            Session["refreshToken"] = refreshToken;
            Session["expiresAt"] = expiresAtString;
        }

        [HttpPost]
        public async Task<ActionResult> CheckTokenServices(Register model)
        {
            var checkToken = await CheckAndRefreshToken().ConfigureAwait(false);
            model.Token = checkToken;
            if (!checkToken)
            {
                return Json(new { error = true, message = "Token expried" });
            }
            else
            {
                return Json(new { success = true, message = "Token Valid" });
            }

        }

        /*------------------------------------------------------------------------------
       機能名：Save
       引数：画面から入力したデータ
       戻る値：登録した結果
       ------------------------------------------------------------------------------*/
        [HttpPost]
        public async Task<ActionResult> Save(Register model)
        {
            log.Info("Starting...");
            Console.WriteLine($"Thread ID trước khi tạo TransactionScope: {Thread.CurrentThread.ManagedThreadId}");

            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    string formattedAmount = model.Order_Amount_View.Replace(",", "");
                    double convertDouble = double.TryParse(formattedAmount, out double order_amount_double) ? order_amount_double : 0.0;
                    DateTime order_datetime = DateTime.ParseExact(model.Order_Date_View, "yyyyMMdd", CultureInfo.InvariantCulture);
                    string data_seq = GenerateUniqueRandomValue();

                    var data = new C_ER001
                    {
                        Data_Seq = data_seq,
                        Order_No = model.Order_No,
                        Work_No = model.Work_No,
                        Supplier_Cd = model.Supplier_Cd,
                        Supplier_Name = model.Supplier_Name,
                        Order_Date = order_datetime,
                        Order_Amount = order_amount_double,
                        Currency_Ut = model.Currency_Ut,
                        Detail_No = model.Detail_No,
                        Report_Typ = model.Report_Typ,
                        Report_Fmt = model.Report_Fmt,
                        Reg_User = model.UserId,
                        Reg_Date = DateTime.Now.Date,
                        Reg_Time = DateTime.Now.TimeOfDay,
                        Del_Flag = 0,
                        Personal_Cd = model.UserId,
                        Memo = model.Memo
                    };

                    // I-SYS情報を登録する
                    var isysCnt = await SavePhoneBookRegistor(data);

                    // 証憑情報を登録する
                    var currentDate = DateTime.Now;
                    var year_folder = currentDate.Year;
                    var month_folder = currentDate.Month.ToString().PadLeft(2, '0');
                    var day_folder = currentDate.Day.ToString().PadLeft(2, '0');

                    var year_file = order_datetime.Year;
                    var month_file = order_datetime.Month.ToString().PadLeft(2, '0');
                    var day_file = order_datetime.Day.ToString().PadLeft(2, '0');

                    string registorDateString = currentDate.ToString("yyyyMMddHHmmss");

                    // Kết hợp các giá trị vào biến string
                    var save_dir_Year = year_folder + "年";
                    var save_dir_Month = month_folder + "月";
                    var save_dir_Day = day_folder + "日";

                    var save_dir = "";
                    if (UploadFolderName.Count == 0)
                    {
                        save_dir = year_folder + "年" + "/" + month_folder + "月" + "/" + day_folder + "日";
                    }
                    else
                    {
                        string pathFolder = string.Join("/", UploadFolderName);
                        save_dir = pathFolder + "/" + year_folder + "年" + "/" + month_folder + "月" + "/" + day_folder + "日";
                    }

                    var filename = model.Report_Typ + "_" + model.Supplier_Cd + "_" + model.Supplier_Name + "_"
                        + order_amount_double.ToString() + "_" + model.Order_Date_View + "_" + model.UserId + "_" + registorDateString + "_";
                    for (var i = 0; i < model.FileList.Count; i++)
                    {
                        var file = model.FileList[i];
                        if (file != null && file.ContentLength > 0)
                        {
                            //Todo Xac nhan folder upload file
                            string extension = Path.GetExtension(file.FileName);
                            var saveName = filename + (i + 1).ToString() + extension;
                            var uploadedFile = await UploadFileAndGetId(file, saveName, save_dir_Year, save_dir_Month, save_dir_Day);
                            int revision = int.TryParse(uploadedFile.SequenceId, out int revisionCnt) ? revisionCnt : 0;
                            var attachedFile = new C_ER002
                            {
                                Data_Seq = data_seq,
                                Seq_No = uploadedFile.Id,
                                Revision_Cnt = revisionCnt,
                                Attach_File = file.FileName,
                                Save_Dir = save_dir,
                                Save_Filename = Path.GetFileNameWithoutExtension(uploadedFile.Name),
                                Save_Fileext = Path.GetExtension(uploadedFile.Name).TrimStart('.'),
                                Reg_User = model.UserId,
                                Reg_Date = DateTime.Now.Date,
                                Reg_Time = DateTime.Now.TimeOfDay,
                            };
                            var fileUpCnt = await SaveAttachFile(attachedFile);
                        }
                    }
                    // commit transaction
                    transactionScope.Complete();
                    log.Info("Ending...");
                    return Json(new { success = true, message = "証憑の登録が完了しました。", token = model.Token });

                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    //rollback
                    Console.WriteLine($"When Save has Error: {ex.Message}");
                    return Json(new { error = true, message = "証憑の登録が失敗しました。", token = model.Token });
                }
                finally
                {
                    try
                    {
                        Console.WriteLine($"Thread ID sau khi dispose TransactionScope: {Thread.CurrentThread.ManagedThreadId}");
                        transactionScope.Dispose();
                    }
                    catch (Exception disposeEx)
                    {
                        log.Error(disposeEx.Message);
                        Console.WriteLine($"When Save has Error: {disposeEx.Message}");
                    }
                }
            }
        }

        /*------------------------------------------------------------------------------
       機能名：SavePhoneBookRegistor
       引数：PhoneBookRegistrationMaster
       戻る値：なし
       ------------------------------------------------------------------------------*/
        private async Task<int> SavePhoneBookRegistor(C_ER001 data)
        {
            _context.C_ER001s.Add(data);
            var recordCnt = await _context.SaveChangesAsync();
            return recordCnt;
        }

        /*------------------------------------------------------------------------------
        機能名：SaveAttachFile
        引数：AttachmentFileMaster
        戻る値：なし
        ------------------------------------------------------------------------------*/
        private async Task<int> SaveAttachFile(C_ER002 file)
        {

            _context.C_ER002s.Add(file);
            int recordCnt = await _context.SaveChangesAsync();
            return recordCnt;
        }

        private string GenerateUniqueRandomValue()
        {
            // Guid to render ID
            return $"{Guid.NewGuid().ToString("N").Substring(0, 19)}";
        }
        /*------------------------------------------------------------------------------
        機能名：UploadFileAndGetId
        引数：ファイル、保管するフォルダ
        戻る値：保管したID
        ------------------------------------------------------------------------------*/
        private async Task<BoxFile> UploadFileAndGetId(HttpPostedFileBase file, string saveName, string folderY, string folderM, string folderD)
        {
            log.Info("Starting Upload File...");
            var checkToken = await CheckAndRefreshToken();
            string accessToken = Session["accessToken"] as string;
            string refreshToken = Session["refreshToken"] as string;

            var config = new BoxConfig(ClientId, ClientSecret, new Uri(UploadUri));
            var session = new OAuthSession(accessToken, refreshToken, 3600, "bearer");
            var client = new BoxClient(config, session);

            log.Info("Get token...");
            var uploadedFile = new BoxFile();

            byte[] fileContent = null;

            if (file != null && file.ContentLength > 0)
            {
                using (var stream = file.InputStream)
                {
                    // Read content from file                        
                    using (var memoryStream = new MemoryStream())
                    {
                        await stream.CopyToAsync(memoryStream);
                        fileContent = memoryStream.ToArray();
                    }
                }
            }
            //Foler Parent ID 
            string parentId = BoxParentId; // parentId ban đầu sẽ là ID của thư mục cha
            //for (int i = 0; i < UploadFolderName.Count; i++)
            //{
            //    log.Info("Get Folder Id ..." + i);
            //    var folderId = await CheckFolderSaveFile(client, parentId, UploadFolderName[i]);
            //    parentId = folderId;
            //}
            ////var folderParentID = await CheckFolderSaveFile(client, BoxParentId, UploadFolderName[0]);
            //var folderYearID = await CheckFolderSaveFile(client, parentId, folderY);
            //var folderMonthID = await CheckFolderSaveFile(client, folderYearID, folderM);
            //var folderDayID = await CheckFolderSaveFile(client, folderMonthID, folderD);

            //フォルダにファイルを存在しているかどうかをチェックする
            //フォルダの 100 ~1000 recordを取得できる
            var folderItems = await client.FoldersManager.GetFolderItemsAsync(parentId, 1000);


            // Find file by filename
            var fileId = folderItems.Entries
                .OfType<BoxFile>()
                .Where(f => f.Name == saveName)
                .Select(f => f.Id)
                .FirstOrDefault();

            if (fileId != null)
            {
                // API を呼び出して既存のファイルをアップロードして上書きする
                uploadedFile = await client.FilesManager.UploadNewVersionAsync(saveName, fileId, new MemoryStream(fileContent));
            }
            else
            {
                //ファイルが存在しない場合はアップロードします
                var uploadRequest = new BoxFileRequest()
                {
                    Name = saveName,
                    Parent = new BoxRequestEntity() { Id = parentId },
                };

                // API を呼び出して既存のファイルをアップロードする
                uploadedFile = await client.FilesManager.UploadAsync(uploadRequest, new MemoryStream(fileContent));
            }
            log.Info("Ending Upload File...");
            return uploadedFile;
        }

        private async Task<string> CheckFolderSaveFile(BoxClient client, string parentId, string folderName)
        {
            log.Info("Starting...");
            var folderId = "";
            BoxCollection<BoxItem> items = await client.FoldersManager.GetFolderItemsAsync(parentId, limit: 1000);
            // Find folder by Name
            BoxItem existingFolder = items.Entries.FirstOrDefault(item => item.Type == "folder" && item.Name == folderName);
            if (existingFolder != null)
            {
                folderId = existingFolder.Id;
            }
            else
            {
                BoxFolderRequest request = new BoxFolderRequest
                {
                    Name = folderName,
                    Parent = new BoxRequestEntity { Id = parentId }
                };

                BoxFolder folder = await client.FoldersManager.CreateAsync(request);
                folderId = folder.Id;
            }
            log.Info("Ending...");
            return folderId;
        }

        /*------------------------------------------------------------------------------
        機能名：CheckAndRefreshToken
        引数：なし
        戻る値：AccessTokenの有無
        ------------------------------------------------------------------------------*/
        private async Task<bool> CheckAndRefreshToken()
        {
            string refreshToken = Session["refreshToken"] as string;
            string expiresAtString = Session["expiresAt"] as string;
            DateTime.TryParseExact(expiresAtString, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime expiresAt);

            var client = new HttpClient();

            // Check Access Token
            if (DateTime.Now < expiresAt)
            {
                // Access Token valid
                return true;
            }
            else
            {
                // Token expried, refresh
                var refreshContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("refresh_token", refreshToken),
                    new KeyValuePair<string, string>("client_id", ClientId),
                    new KeyValuePair<string, string>("client_secret", ClientSecret)
                });

                var refreshResponse = await client.PostAsync(AuthenticationUrl, refreshContent);

                if (refreshResponse.IsSuccessStatusCode)
                {
                    // Get New AccessToken
                    var newTokenInfo = JsonConvert.DeserializeObject<TokenInfo>(await refreshResponse.Content.ReadAsStringAsync());
                    Session["accessToken"] = newTokenInfo != null ? newTokenInfo.access_token : "";
                    // Token refresh success
                    return true;
                }
                else
                {
                    // Refresh failed
                    return false;
                }
            }
        }

        private void getBoxconfig()
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
            RedirectUri = config.RedirectUri;
            AuthenticationUrl = config.AuthenticationUrl;
            ClientId = config.ClientId;
            ClientSecret = config.ClientSecret;
            UploadUri = config.UploadUri;
        }
    }
}