using System;
using System.Collections.Generic;
using System.Text;

namespace WordbookImpressLibrary.Models
{
    public class PurchaseHistory
    {
        public List<string> ClickedASIN { get; set; } = new List<string>();
        public List<string> SpecialObtainedUrl { get; set; } = new List<string>();
    }
}
