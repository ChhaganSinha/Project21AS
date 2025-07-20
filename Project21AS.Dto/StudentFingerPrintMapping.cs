using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project21AS.Dto
{
    public class StudentFingerPrintMapping
    {
        public int Id { get; set; }
        public string Admin { get; set; } = string.Empty;
        public string FingerPrint1 { get; set; } = string.Empty;
        public string FingerPrint2 { get; set; } = string.Empty;
        public string FingerPrint3 { get; set; } = string.Empty;
        public string FingerPrint4 { get; set; } = string.Empty;
        public string FingerPrint5 { get; set; } = string.Empty;
        public string FingerPrint6 { get; set; } = string.Empty;
        public string FingerPrint7 { get; set; } = string.Empty;
        public string FingerPrint8 { get; set; } = string.Empty;
        public string FingerPrint9 { get; set; } = string.Empty;
        public string FingerPrint10 { get; set; } = string.Empty;

        [ForeignKey("Id")]
        public virtual Student ? student { get; set;}
    }
}
