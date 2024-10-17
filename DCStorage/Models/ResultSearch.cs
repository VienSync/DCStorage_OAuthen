using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace DCStorage.Models
{
    public class ResultSearch
    {
        [MaxLength(20)]
        [DisplayName("管理番号")]
        public string Data_Seq { get; set; }

        [MaxLength(20)]
        [DisplayName("証憑区分")]
        public string Report_Typ { get; set; }

        [MaxLength(20)]
        [DisplayName("登録者")]
        public string Reg_User { get; set; }

        [MaxLength(40)]
        [DisplayName("工番")]
        public string Work_No { get; set; }

        [MaxLength(20)]
        [DisplayName("オーダー")]
        public string Order_No { get; set; }

        [MaxLength(128)]
        [DisplayName("取引先名称")]
        public string Supplier_Name { get; set; }

        [MaxLength(20)]
        [DisplayName("取引先コード")]
        public string Supplier_Cd { get; set; }

        [DisplayName("取引日付")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? Order_Date { get; set; }

        [Range(0, 9999999999)]
        [DisplayName("金額")]
        public double? Order_Amount { get; set; }

        [MaxLength(10)]
        [DisplayName("通貨")]
        public string Currency_Ut { get; set; }

        [DisplayName("システム登録日付")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? Reg_Date { get; set; }

        [DisplayName("システム登録時間")]
        public TimeSpan? Reg_Time { get; set; }

        [MaxLength(20)]
        [DisplayName("データの元形式")]
        public string Report_Fmt { get; set; }

        [MaxLength(1)]
        [DisplayName("除外フラグ")]
        public int? Del_Flag { get; set; }

        [MaxLength(200)]
        [DisplayName("メモー")]
        public string Memo { get; set; }

    }
}
