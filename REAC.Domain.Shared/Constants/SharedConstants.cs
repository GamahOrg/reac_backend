using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Domain.Shared.Constants
{
    public static class SharedConstants
    {
        public const string ArabicLanguage = "ar";
        public const string RootFolder = "reac";
        public const string EnglishLanguage = "en";
        public static readonly List<string> Languages = new()
    {
        EnglishLanguage, ArabicLanguage
    };
    }
}
