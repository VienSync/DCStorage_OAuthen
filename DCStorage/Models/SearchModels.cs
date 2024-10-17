using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace DCStorage.Models
{
    public class SearchModels
    {
        [Key]
        [MaxLength(20)]
        [DisplayName("管理番号")]
        public string Data_Seq { get; set; }

        [MaxLength(20)]
        [DisplayName("オーダー情報")]
        public string Order_No { get; set; }

        [MaxLength(40)]
        [DisplayName("工番（部門コード）")]
        public string Work_No { get; set; }

        [MaxLength(128)]
        [DisplayName("取引先名称")]
        public string Supplier_Name { get; set; }

        [MaxLength(20)]
        [DisplayName("取引先コード")]
        public string Supplier_Cd { get; set; }

        [DisplayName("取引日付")]
        public DateTime? Order_Date { get; set; }

        public DateTime? Order_Date1 { get; set; }

        public DateTime? Order_Date2 { get; set; }

        [Required]
        [Range(0, 9999999999)]
        [DisplayName("取引金額")]
        public double? Order_Amount { get; set; }

        [Required]
        [Range(0, 999999)]
        public int? Order_Amount1 { get; set; }

        [Required]
        [Range(0, 999999)]
        public int? Order_Amount2 { get; set; }

        [MaxLength(10)]
        [DisplayName("通貨")]
        public string Currency_Ut { get; set; }

        [MaxLength(6)]
        [DisplayName("個人コード")]
        public string Personal_Cd { get; set; }

        [MaxLength(40)]
        [DisplayName("所属部門")]
        public string Department_Name { get; set; }

        [MaxLength(20)]
        [DisplayName("呼出画面")]
        public string Source_Scrn { get; set; }

        [MaxLength(10)]
        [DisplayName("呼出ボタン")]
        public string Source_Btn { get; set; }

        [MaxLength(20)]
        [DisplayName("対応明細行")]
        public string Detail_No { get; set; }

        [MaxLength(20)]
        [DisplayName("証憑区分")]
        public string Report_Typ { get; set; }

        [Range(0, 999)]
        [DisplayName("データの元形式")]
        public string Report_Fmt { get; set; }

        [Required]
        [Range(0, 999)]
        [DisplayName("改訂数")]
        public int? Revision_Cnt { get; set; }

        [MaxLength(20)]
        [DisplayName("システム登録者")]
        public string Reg_User { get; set; }

        [DisplayName("システム登録日付")]
        public DateTime? Reg_Date { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy年MM月dd日}")]
        public DateTime? Reg_Date_Start { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy年MM月dd日}")]
        public DateTime? Reg_Date_End { get; set; }


        [DisplayName("システム登録時間")]
        public TimeSpan? Reg_Time { get; set; }


        [DisplayName("除外フラグ")]
        public int? Del_Flag { get; set; }

        [MaxLength(20)]
        [DisplayName("検査除外者")]
        public string Del_User { get; set; }

        [DisplayName("除外日")]
        public DateTime? Del_Date { get; set; }


        [DisplayName("除外時間")]
        public TimeSpan? Del_Time { get; set; }

        [MaxLength(256)]
        [DisplayName("ファイル名")]
        public string FileName { get; set; }

        [MaxLength(50)]
        [DisplayName("検索理由")]
        public string Del_Memo { get; set; }

        public string Mess_err { get; set; }

        public List<ResultSearch> lstResultSearches { get; set; }

        public string UserId { get; set; }

        public string RegistorType { get; set; }

        public bool statusToggle { get; set; }

        public List<string> lstOrder { get; set; }
    }
}
