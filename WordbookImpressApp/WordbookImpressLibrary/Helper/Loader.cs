using System;
using System.Collections.Generic;
using System.Text;

using WordbookImpressLibrary.Models;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using System.Linq;

namespace WordbookImpressLibrary.Helper
{
    public static class Loader
    {
        public static Word[] GetWords(string result)
        {
            var match = System.Text.RegularExpressions.Regex.Match(result, @"(\[.+\])", System.Text.RegularExpressions.RegexOptions.Multiline);
            var model = JsonConvert.DeserializeObject<string[][]>(match.Groups[1].Value);
            return model
                ?.Where(a => a.Count() >= 2)
                ?.Select(a => new Word() { Title = Word.GetAsText(a[0]), Description = Word.GetAsText(a[1]) })
                ?.ToArray() ?? new Word[0];
        }


        public static QuizChoice[] GetWordsConfig(string text)
        {
            var match = Regex.Match(text, @"(\{.+\})", RegexOptions.Multiline);
            var model = JsonConvert.DeserializeObject<Config>(match.Groups[1].Value);
            return model.GetQuizChoices();
        }

        public class Config
        {
            public Setting settings { get; set; }
            public List<Question> questions { get; set; }

            public QuizChoice[] GetQuizChoices()
            {
                return questions.Select(a => (QuizChoice)a).ToArray();
            }

            public class Setting
            {
                public string title { get; set; }
            }

            public class Question
            {
                public string question { get; set; }
                public string[] choice { get; set; }
                public string answer { get; set; }
                public string type { get; set; }
                public string[] feedback { get; set; }
                public string[] feedback_tf { get; set; }

                public static explicit operator QuizChoice(Question arg)
                {
                    return new QuizChoice()
                    {
                        Answer = Word.GetAsText(arg.answer),
                        Choices = arg.type == "true-false" ? new[] { "true", "false" } : arg.choice.Select(a => Word.GetAsText(a)).ToArray(),
                        Description = ((arg?.feedback?.Count() > 0) ? Word.GetAsText(String.Join("\n\n", arg.feedback)).Trim() : "") +
                        ((arg?.feedback_tf?.Count() > 0) ? Word.GetAsText(String.Join("\n\n", arg.feedback_tf)).Trim() : "")
                        ,
                        Title = Word.GetAsText(arg.question)
                    };
                }
            }
        }

        [Obsolete]
        public static QuizChoice[] GetWordsConfigClassic(string text)
        {
            //JSONのライブラリを使った方が手っ取り早いだろう。
            //"\[\s*\"([^\"]*)\"\s*,\s*\"([^\"]*)\"\s*\]"
            var reg = new Regex("\\{\\s*\\\"question\\\"\\s*:\\s*\\\"([^\\\"]*)\\\"\\s*,\\s*\\\"choice\\\":\\[([^\\[\\]]*)\\]\\s*,\\s*\\\"feedback\\\"\\s*:\\s*\\[\\\"([^\\\"]*)\\\"\\]\\s*,\\s*\\\"answer\\\":\\\"([^\\\"]*)\\\"\\}");
            var matches = reg.Matches(text);
            var words = new List<QuizChoice>();
            foreach (Match match in matches)
            {
                var choices = new Regex("\\\"([^\\\"]*)\\\"").Matches(match.Groups[2].Value);
                var choiceList = new List<string>();
                foreach (Match choice in choices)
                {
                    choiceList.Add(choice.Groups[1].Value);
                }
                words.Add(QuizChoice.GetQuizChoiceUnescape(match.Groups[1].Value, match.Groups[3].Value, choiceList.ToArray(), match.Groups[4].Value));
            }
            return words.ToArray();
        }

        [Obsolete]
        public static Word[] GetWordsClassic(string text)
        {
            //"\[\s*\"([^\"]*)\"\s*,\s*\"([^\"]*)\"\s*\]"
            var reg = new Regex("\\[\\s*\\\"([^\\\"]*)\\\"\\s*,\\s*\\\"([^\\\"]*)\\\"\\s*\\]");
            var matches = reg.Matches(text);
            var words = new List<Word>();
            foreach (Match match in matches)
            {
                words.Add(Word.GetWordUnescape(match.Groups[1].Value, match.Groups[2].Value));
            }
            return words.ToArray();
        }

    }
}
