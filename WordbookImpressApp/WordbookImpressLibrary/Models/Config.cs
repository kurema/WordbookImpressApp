using System;
using System.Collections.Generic;
using System.Text;

namespace WordbookImpressLibrary.Models
{
    public class Config
    {
        public bool SkipChecked = false;
        public double SkipMinRate = 1.0;
        public int SkipMinCorrect = int.MaxValue;
        public int ChoiceCount = 4;
    }
}
