using System;
using System.Collections.Generic;
using System.Text;

namespace WordbookImpressLibrary.Models
{
    public class Config
    {
        public bool SkipChecked = false;
        public double SkipMinRate = 2.0;
        public int SkipMinCorrect = int.MaxValue;
        public int ChoiceCount = 4;
        public ViewModels.WordbookImpressViewModel.SortKindInfo SortKind = ViewModels.WordbookImpressViewModel.SortKindInfo.GetDefault();
    }
}
