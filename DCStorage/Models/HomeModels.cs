using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace DCStorage.Models
{
    public class HomeModels
    {
        [DisplayName("ISYSログインユーザーID")]
        public string p1 { get; set; }

        [DisplayName("I-SYSのユーザー所属")]
        public string p2 { get; set; }

        [DisplayName("オーダー情報")]
        public string p3 { get; set; }

        [DisplayName("取引先コード")]
        public string p4 { get; set; }

        [DisplayName("取引金額")]
        public double? p5 { get; set; }

        [DisplayName("取引先名刺")]
        public string p6 { get; set; }

        [DisplayName("取引日付")]
        public string p7 { get; set; }

        [DisplayName("発注残照会")]
        public string p8 { get; set; }

        [DisplayName("工番")]
        public string p9 { get; set; }

        [DisplayName("通貨")]
        public string p10 { get; set; }


        [DisplayName("検収日/完成日")]
        public string p11 { get; set; }

        [DisplayName("検収金額/完成金額")]
        public double? p12 { get; set; }

        [DisplayName("’登録’OR’検索'")]
        public string p13 { get; set; }

    }
}
