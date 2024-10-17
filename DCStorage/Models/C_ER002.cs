using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DCStorage.Models
{
    public class C_ER002
    {
        [Key, Column(Order = 0)]
        [MaxLength(20)]
        public string Data_Seq { get; set; }

        [Key, Column(Order = 1)]
        [MaxLength(20)]
        public string Seq_No { get; set; }

        [MaxLength(256)]
        public string Attach_File { get; set; }

        [Required]
        [Range(0, 999)]
        public int? Revision_Cnt { get; set; }

        [MaxLength(256)]
        public string Save_Dir { get; set; }

        [MaxLength(256)]
        public string Save_Filename { get; set; }

        [MaxLength(10)]
        public string Save_Fileext { get; set; }

        [MaxLength(20)]
        public string Reg_User { get; set; }

        public DateTime? Reg_Date { get; set; }

        public TimeSpan? Reg_Time { get; set; }
    }
}