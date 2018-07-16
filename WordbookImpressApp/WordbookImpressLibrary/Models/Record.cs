using System;
using System.Collections.Generic;
using System.Text;

namespace WordbookImpressLibrary.Models
{
    public class Record
    {
        [System.Xml.Serialization.XmlElement(ElementName = "Words")]
        public WordStatus[] WordsXml
        {
            get
            {
                var result = new List<WordStatus>();
                foreach (var item in Words)
                {
                    var temp = item.Value;
                    temp.Hash = item.Key;
                    result.Add(temp);
                }
                return result.ToArray();
            }
            set
            {
                Words = new Dictionary<string, WordStatus>();
                if (value == null) return;
                foreach(var item in value)
                {
                    Words[item.Hash] = item;
                }
            }
        }

        [System.Xml.Serialization.XmlIgnore()]
        public Dictionary<string, WordStatus> Words { get; set; } = new Dictionary<string, WordStatus>();
        public List<TestStatus> TestStatuses { get; set; } = new List<TestStatus>();

        public WordStatus GetWordStatusByHash(string Hash)
        {
            var result= Words.ContainsKey(Hash) ? Words[Hash] : new Record.WordStatus();
            result.Hash = Hash;
            return result;
        }

        public void SetWordStatusByHash(string Hash,WordStatus status)
        {
            status.Hash = Hash;
            Words[Hash] = status;
        }

        public struct WordStatus
        {
            public string Hash;
            public int AnswerCountTotal;
            public int AnswerCountCorrect;
            public int AnswerCountPass;
            public bool ExcludeRemembered;
            public long LastAnswerTicks;
            [System.Xml.Serialization.XmlIgnore()]
            public DateTime LastAnswerDateTime
            {
                get => new DateTime(LastAnswerTicks);
                set => LastAnswerTicks = value.Ticks;
            }
            public long LastCorrectTicks;
            [System.Xml.Serialization.XmlIgnore()]
            public DateTime LastCorrectDateTime
            {
                get => new DateTime(LastCorrectTicks);
                set => LastCorrectTicks = value.Ticks;
            }
        }

        public struct TestStatus
        {
            public ViewModels.QuizWordChoiceViewModel.ChoiceKind ChoiceKind;
            public ViewModels.QuizWordChoiceViewModel.RetryStatusEnum RetryStatus;
            public string Key;
            public static string KeyCombined => "[combined]";
            public static string KeyAll => "[all]";
            public static string KeyDaily => "[daily]";
            public int AnswerCountTotal;
            public int AnswerCountCorrect;
            public int AnswerCountPass;

            public static bool KeyEqual(string a,string b)
            {
                if (a == b) return true;
                if (a == KeyAll || b == KeyAll) return true;
                if (a == KeyCombined || b == KeyCombined) return true;
                return false;
            }

            public static bool KeyContains(string a, string b)
            {
                if (a == b) return true;
                if (a == KeyAll) return true;
                if (a == KeyCombined) return true;
                return false;
            }

            public static double GetCorrectRate(double Correct,double Total)
            {
                return Correct / Math.Max(Total, 1);
            }

            public long ElapsedTicks
            {
                get => ElapsedTime.Ticks;
                set => ElapsedTime = new TimeSpan(value);
            }
            [System.Xml.Serialization.XmlIgnore()]
            public TimeSpan ElapsedTime;
            public int Seed;
            public string DateTime
            {
                get => DateTimeNative.ToString();
                set => DateTimeNative = System.DateTime.Parse(value);
            }
            [System.Xml.Serialization.XmlIgnore()]
            public DateTime DateTimeNative;

            public static TestStatus GetEmpty()
            {
                var result = new TestStatus();
                result.DateTimeNative = new DateTime();
                result.Seed = 0;
                result.AnswerCountCorrect = 0;
                result.AnswerCountPass = 0;
                result.AnswerCountTotal = 0;
                result.ChoiceKind = ViewModels.QuizWordChoiceViewModel.ChoiceKind.Combined;
                result.RetryStatus = ViewModels.QuizWordChoiceViewModel.RetryStatusEnum.First;
                result.Key = "";
                return result;
            }

            public static TestStatus GetSum(TestStatus status,TestStatus item)
            {
                //public static implicit operator +() maybe good.
                //but because Seed and Key data is lost GetSum.
                status.DateTimeNative = item.DateTimeNative.Date;
                status.Key = KeyDaily;
                status.AnswerCountCorrect += item.AnswerCountCorrect;
                status.AnswerCountPass += item.AnswerCountPass;
                status.AnswerCountTotal += item.AnswerCountTotal;
                status.ChoiceKind = ViewModels.QuizWordChoiceViewModel.ChoiceKind.Combined;
                status.ElapsedTime += item.ElapsedTime;
                status.Seed = 0;
                return status;
            }
        }
    }
}
