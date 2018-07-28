using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WordbookImpressApp.ValueConverters
{
    public class QuizWordChoiceViewModelQuizStatusToCorrectVisibilityValueConverter : Xamarin.Forms.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result= (value is WordbookImpressLibrary.ViewModels.QuizWordChoiceViewModel.QuizStatus && (WordbookImpressLibrary.ViewModels.QuizWordChoiceViewModel.QuizStatus)value == WordbookImpressLibrary.ViewModels.QuizWordChoiceViewModel.QuizStatus.Correct);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class QuizWordChoiceViewModelQuizStatusToWrongVisibilityValueConverter : Xamarin.Forms.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is WordbookImpressLibrary.ViewModels.QuizWordChoiceViewModel.QuizStatus && (WordbookImpressLibrary.ViewModels.QuizWordChoiceViewModel.QuizStatus)value == WordbookImpressLibrary.ViewModels.QuizWordChoiceViewModel.QuizStatus.Wrong);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class QuizWordChoiceViewModelQuizStatusToChoiceVisibilityValueConverter : Xamarin.Forms.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is WordbookImpressLibrary.ViewModels.QuizWordChoiceViewModel.QuizStatus && (WordbookImpressLibrary.ViewModels.QuizWordChoiceViewModel.QuizStatus)value == WordbookImpressLibrary.ViewModels.QuizWordChoiceViewModel.QuizStatus.Choice);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NullToTextValueConverter : Xamarin.Forms.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text;
            if (parameter == null || !(parameter is string)) text = "Null:Not Null";
            else text = (string)parameter;
            var texts = text.Split(':', (char)2);
            return value==null?texts[0]:texts[1];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NullOrEmptyStringToTextValueConverter : Xamarin.Forms.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text;
            if (parameter == null || !(parameter is string)) text = "Null:Not Null";
            else text = (string)parameter;
            var texts = text.Split(':', (char)2);
            return value == null ? texts[0] : (value is string && string.IsNullOrEmpty((string)value) ? texts[0]: texts[1]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class QuizWordChoiceViewModelTestResultToStringValueConverter : Xamarin.Forms.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text;
            if (parameter == null || !(parameter is string)) text = "::::";
            else text = (string)parameter;
            var texts = text.Split(':');

            if (value == null || !(value is WordbookImpressLibrary.ViewModels.QuizWordChoiceViewModel.TestResult))
            {
                return texts[4];
            }
            switch ((WordbookImpressLibrary.ViewModels.QuizWordChoiceViewModel.TestResult)value)
            {
                case WordbookImpressLibrary.ViewModels.QuizWordChoiceViewModel.TestResult.Correct:return texts[0];
                case WordbookImpressLibrary.ViewModels.QuizWordChoiceViewModel.TestResult.Wrong:return texts[1];
                case WordbookImpressLibrary.ViewModels.QuizWordChoiceViewModel.TestResult.Pass:return texts[2];
                case WordbookImpressLibrary.ViewModels.QuizWordChoiceViewModel.TestResult.Yet:return texts[3];
                default:return texts[4];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TimeSpanFormatValueConverter: Xamarin.Forms.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is TimeSpan))
            {
                return "";
            }
            if (parameter == null || !(parameter is string)) return ((TimeSpan)value).ToString();
            else return FormatTimeSpan((TimeSpan)value, (string)parameter);
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static string FormatTimeSpan (TimeSpan ts, string tx)
        {
            var dic = new Dictionary<string, double>()
            {
                { nameof(ts.Days),ts.Days },
                { nameof(ts.Hours),ts.Hours },
                { nameof(ts.Milliseconds),ts.Milliseconds },
                { nameof(ts.Minutes),ts.Minutes },
                { nameof(ts.Seconds),ts.Seconds },
                { nameof(ts.Ticks),ts.Ticks },
                { nameof(ts.TotalDays),ts.TotalDays },
                { nameof(ts.TotalHours),ts.TotalHours },
                { nameof(ts.TotalMilliseconds),ts.TotalMilliseconds },
                { nameof(ts.TotalMinutes),ts.TotalMinutes },
                { nameof(ts.TotalSeconds),ts.TotalSeconds },
                { nameof(ts.TotalDays)+nameof(Math.Floor), Math.Floor(ts.TotalDays) },
                { nameof(ts.TotalHours)+nameof(Math.Floor),Math.Floor(ts.TotalHours) },
                { nameof(ts.TotalMilliseconds)+nameof(Math.Floor),Math.Floor(ts.TotalMilliseconds) },
                { nameof(ts.TotalMinutes)+nameof(Math.Floor),Math.Floor(ts.TotalMinutes) },
                { nameof(ts.TotalSeconds)+nameof(Math.Floor),Math.Floor(ts.TotalSeconds) },
            };
            {
                var reg = new System.Text.RegularExpressions.Regex(@"\[(\w+)\]");
                tx = reg.Replace(tx, new System.Text.RegularExpressions.MatchEvaluator((m) => { if (dic.ContainsKey(m.Groups[1].Value)) return dic[m.Groups[1].Value].ToString(); else return m.Value; }));
            }
            {
                var reg = new System.Text.RegularExpressions.Regex(@"\[:(\w+):\]");
                tx = reg.Replace(tx, new System.Text.RegularExpressions.MatchEvaluator((m) => { return ts.ToString(m.Groups[1].Value); }));
            }
            {
                var reg = new System.Text.RegularExpressions.Regex(@"\[(\w+):([^\[\]]+)\]");
                tx = reg.Replace(tx, new System.Text.RegularExpressions.MatchEvaluator((m) => { if (dic.ContainsKey(m.Groups[1].Value)) return String.Format(m.Groups[2].Value, dic[m.Groups[1].Value]); else return m.Value; }));
            }
            {
                var reg = new System.Text.RegularExpressions.Regex(@"\[if:(\w+):([^\[\]]+)\]");
                tx = reg.Replace(tx, new System.Text.RegularExpressions.MatchEvaluator((m) => { if (dic.ContainsKey(m.Groups[1].Value) && dic[m.Groups[1].Value]>0) return m.Groups[2].Value; else return ""; }));
            }
            return tx;
        }
    }

    public class BooleanToColorValueConverter : Xamarin.Forms.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text;
            if (parameter == null || !(parameter is string)) text = "#00FFFFFF:#00FFFFFF:#00FFFFFF";
            else text = (string)parameter;
            var texts = text.Split(':');

            if (value == null || !(value is bool))
            {
                return Xamarin.Forms.Color.FromHex(texts[2]);
            }
            if ((bool)value == true)
            {
                return Xamarin.Forms.Color.FromHex(texts[0]);
            }
            else
            {
                return Xamarin.Forms.Color.FromHex(texts[1]);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringReplaceValueConverter : Xamarin.Forms.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value.ToString();
            if (parameter == null) { return value; }
            var dicts = parameter.ToString().Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
            var dict = new Dictionary<string, string>();
            foreach(var item in dicts)
            {
                var kvp = item.Split(':', (char)2);
                if (kvp.Length != 2) continue;
                dict.Add(kvp[0], kvp[1]);
            }
            foreach(var item in dict)
            {
                text = text.Replace(item.Key, item.Value);
            }
            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ToStringSwitchValueConverter : Xamarin.Forms.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value.ToString();
            if (parameter == null) { return value; }
            var dicts = parameter.ToString().Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
            var dict = new Dictionary<string, string>();
            foreach (var item in dicts)
            {
                var kvp = item.Split(':', (char)2);
                if (kvp.Length != 2) continue;
                dict.Add(kvp[0], kvp[1]);
            }
            foreach (var item in dict)
            {
                if (text == item.Key) return item.Value;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BooleanNotValueConverter : Xamarin.Forms.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is bool))
            {
                return null;
            }
            return !((bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is bool))
            {
                return null;
            }
            return !((bool)value);
        }
    }

    public class BooleanToStringValueConverter : Xamarin.Forms.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text;
            if (parameter == null || !(parameter is string)) text = "::";
            else text = (string)parameter;
            var texts = text.Split(':');

            if (value == null || !(value is bool))
            {
                return texts[2];
            }
            if((bool)value == true)
            {
                return texts[0];
            }
            else
            {
                return texts[1];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class QuizWordChoiceViewModelQuizStatusToTextValueConverter : Xamarin.Forms.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text;
            if (parameter == null || !(parameter is string)) text = "Select:Correct:Wrong:Error";
            else text = (string)parameter;
            var texts = text.Split(':');

            if (value == null || !( value is WordbookImpressLibrary.ViewModels.QuizWordChoiceViewModel.QuizStatus))
            {
                return texts[3];
            }
            switch ((WordbookImpressLibrary.ViewModels.QuizWordChoiceViewModel.QuizStatus)value)
            {
                case WordbookImpressLibrary.ViewModels.QuizWordChoiceViewModel.QuizStatus.Choice:return texts[0];
                case WordbookImpressLibrary.ViewModels.QuizWordChoiceViewModel.QuizStatus.Correct: return texts[1];
                case WordbookImpressLibrary.ViewModels.QuizWordChoiceViewModel.QuizStatus.Wrong: return texts[2];
                default:return texts[3];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SpecialToTextValueConverter : Xamarin.Forms.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var t = (value as WordbookImpressLibrary.Schemas.WordbookSuggestion.infoBooksBookSpecial);
            if (t == null) return "";
            var texts = parameter.ToString().Split('/');
            if (texts.Length < 5) return "";
            List<string> result = new List<string>();
            if (t.ebook?.Length > 0) result.Add(texts[1]);
            if (t.wordbook?.Length > 0) result.Add(texts[2]);
            if (t.voice?.Length > 0) result.Add(texts[3]);
            if (result.Count == 0) return texts[4];
            return texts[0] + " " + string.Join(" ", result);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class QuizResultViewModelTextValueConverter : Xamarin.Forms.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is WordbookImpressLibrary.ViewModels.QuizResultViewModel o)) return "";
            var p = parameter?.ToString() ?? "";
            return string.Format(p,o.AnswerCorrectPercentage,o.AnswerCountCorrect,o.AnswerCountTotal);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is WordbookImpressLibrary.ViewModels.QuizResultViewModel.TestResultItemViewModel o)) return "";
            var p = parameter?.ToString() ?? "";
            return string.Format(p, o.Word?.AnswerCountCorrectPercentage,o.Word?.AnswerCountCorrect,o.Word?.AnswerCountTotal);
        }
    }

    public class TestResultItemViewModelTextValueConverter : Xamarin.Forms.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is WordbookImpressLibrary.ViewModels.QuizResultViewModel.TestResultItemViewModel o)) return "";
            var p = parameter?.ToString() ?? "";
            return string.Format(p, o.Word.AnswerCountCorrectPercentage, o.Word.AnswerCountCorrect, o.Word.AnswerCountTotal);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TestStatusViewModelTextValueConverter : Xamarin.Forms.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is WordbookImpressLibrary.ViewModels.TestStatusViewModel o)) return "";
            var p = parameter?.ToString() ?? "";
            return string.Format(p, o.AnswerCountCorrectPercentage, o.AnswerCountCorrect, o.AnswerCountTotal);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IWordViewModelTextValueConverter : Xamarin.Forms.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is WordbookImpressLibrary.ViewModels.IWordViewModel o)) return "";
            var p = parameter?.ToString() ?? "";
            return string.Format(p, o.AnswerCountCorrectPercentage, o.AnswerCountCorrect, o.AnswerCountTotal);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
