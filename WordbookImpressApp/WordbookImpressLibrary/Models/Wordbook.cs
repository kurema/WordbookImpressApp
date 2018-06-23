using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace WordbookImpressLibrary.Models
{
    public interface IWordbook
    {
        bool IsValid { get; }
        string Title { get; }
        Word[] Words { get; }
    }
}
