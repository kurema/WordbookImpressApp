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
        }

        public struct TestStatus
        {
            public ViewModels.QuizWordChoiceViewModel.ChoiceKind ChoiceKind;
            public string Key;
            public int AnswerCountTotal;
            public int AnswerCountCorrect;
            public int AnswerCountPass;
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
        }
    }
}
