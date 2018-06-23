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
            if (parameter == null || !(parameter is string)) text = "選択:正解:誤り:エラー";
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
}
