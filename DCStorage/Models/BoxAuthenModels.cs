using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DCStorage.Models
{
    public class TokenInfo
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
    }

    public class Register
    {
        public string RegistorType { get; set; }
        public string Data_Seq { get; set; }
        public string Order_No { get; set; }
        public string Work_No { get; set; }
        public string Supplier_Cd { get; set; }
        public string Supplier_Name { get; set; }
        public string Order_Date { get; set; }
        public string Order_Amount { get; set; }
        public string Currency_Ut { get; set; }
        public string Detail_No { get; set; }
        public string Report_Typ { get; set; }
        public string Report_Fmt { get; set; }
        public List<HttpPostedFileBase> FileList { get; set; }
        public Boolean Token { get; set; }
        public string UserId { get; set; }

        public List<string> lstOrder { get; set; }

        public string Completion_Date { get; set; }

        public string Completion_Amount { get; set; }

        public string Order_Date_View { get; set; }
        public string Order_Amount_View { get; set; }

        public string Memo { get; set; }

    }
}