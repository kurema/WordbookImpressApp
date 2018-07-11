using System;
using System.Collections.Generic;
using System.Text;

using System.Collections.ObjectModel;
using WordbookImpressLibrary.Models;

using System.Linq;

namespace WordbookImpressLibrary.ViewModels
{
    public class TestStatusesViewModel:BaseViewModel
    {
        private ObservableCollection<TestStatusViewModel> items;
        public ObservableCollection<TestStatusViewModel> Items
        {
            get => items; set { SetProperty(ref items, value);
                OnPropertyChanged(nameof(DailyStatuses));
                OnPropertyChanged(nameof(Total));
            }
        }
        private Dictionary<DateTime, Record.TestStatus> dailyStatuses;
        public Dictionary<DateTime, Record.TestStatus> DailyStatuses { get => dailyStatuses = dailyStatuses ?? GetDailyStatuses(); }
        private WordbookImpressViewModel target;
        public WordbookImpressViewModel Target { get => target; set
            {
                SetProperty(ref target, value);
                Items = TestStatusViewModel.GetTestStatusesGeneral((w) => value.Wordbooks.Count((a) => a.Uri == w.Key) > 0);
            }
        }

        public TestStatusViewModel Total => new TestStatusViewModel(GetTotalTestStatus());

        private Record.TestStatus GetTotalTestStatus()
        {
            var result = Record.TestStatus.GetEmpty();
            foreach(var item in items)
            {
                result = Record.TestStatus.GetSum(result, item.Content);
            }
            return result;
        }

        public Dictionary<DateTime, Record.TestStatus> GetDailyStatuses()
        {
            var result = new Dictionary<DateTime, Record.TestStatus>();
            foreach (var itemVm in items)
            {
                var item = itemVm.Content;
                if(result.ContainsKey(item.DateTimeNative.Date))
                {
                    result[item.DateTimeNative.Date] = Record.TestStatus.GetSum(result[item.DateTimeNative.Date], item);
                }
                else
                {
                    result[item.DateTimeNative.Date] = item;
                }
            }

            //ToDictionary()が便利なのでメモ。
            //result.ToDictionary((a) => a.Key, (b) => Record.TestStatus.GetCorrectRate(b.Value.AnswerCountCorrect, b.Value.AnswerCountTotal));
            //var b = result.Select((a) => (a.Key.ToString(), a.Value.AnswerCountCorrect / a.Value.AnswerCountTotal)).ToDictionary((a) => a.Item1, (a) => a.Item2);

            return result;
        }

    }
}
