using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebTruyenHay.Models
{
    public class ChuongAndImage
    {
        public Chuong chuong { get; set; }
        public IEnumerable<imagechuong> Chapters { get; set; }
        public bool IsLastChapter { get; set; }
    }
}