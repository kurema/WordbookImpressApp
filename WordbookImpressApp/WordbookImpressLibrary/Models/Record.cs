using System;
using System.Collections.Generic;
using System.Text;

namespace WordbookImpressLibrary.Models
{
    public class Record
    {
        public Dictionary<string, WordStatus> Words { get; set; } = new Dictionary<string, WordStatus>();
        public List<TestStatus> TestStatuses { get; set; } = new List<TestStatus>();

        public WordStatus GetWordStatusByHash(string Hash)
        {
            return Words.ContainsKey(Hash) ? Words[Hash] : new Record.WordStatus();
        }

        public void SetWordStatusByHash(string Hash,WordStatus status)
        {
            Words[Hash] = status;
        }

        public struct WordStatus
        {
            public uint AnswerCountTotal;
            public uint AnswerCountCorrect;
            public bool ExcludeRemembered;
        }

        public struct TestStatus
        {
            public string Key;
            public uint AnswerCountTotal;
            public uint AnswerCountCorrect;
            public TimeSpan ElapsedTime;
        }
    }
}
