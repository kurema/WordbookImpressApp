using System;
using System.Collections.Generic;
using System.Text;

namespace WordbookImpressLibrary.Models
{
    public class WordbookImpressInfo
    {
        public string Url { get; set; } = DefaultUrl;
        public string ID { get; set; } = "";
        public string Password { get; set; } = "";
        public string Format { get; set; } = "";

        public static class Formats
        {
            public static string DataJs => "quizgenerator.data.js";
            public static string ConfigJs => "quizgenerator.config.js";
            public static string QuizGenerator => "quizgenerator";
            public static string Csv => "csv";
        }

        public static string DefaultUrl=> @"https://impress2.quizgenerator.net/";
    }
}
