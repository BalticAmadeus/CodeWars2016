using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Game.AdminClient.ViewModels;

namespace Game.AdminClient.Converters
{
    public class CellStateConverter : IValueConverter
    {
        public const string Background = "#000000";
        public const string BackgroundWhite = "#595959";

        public const string WhiteOuterTop = "#fefefe";
        public const string WhiteOuterBottom = "#c3c3c3";
        public const string WhiteInnerTop = "#dcdcdc";
        public const string WhiteInnerBottom = "#f0f0f0";

        public const string BlackOuterTop = "#333333";
        public const string BlackOuterBottom = "#0a0a0a";
        public const string BlackInnerTop = "#1b1b1b";
        public const string BlackInnerBottom = "#2a2a2a";

        public const string RedOuterTop = "#f7415e";
        public const string RedOuterBottom = "#b00823";
        public const string RedInnerTop = "#d01836";
        public const string RedInnerBottom = "#df2745";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var model = value as CellViewModel;
            if (model != null)
            {
                string state = model.State;

                if (state.IndexOf('C') >= 0)
                {
                    if (state.Length > 2)
                        return ((SolidColorBrush)new BrushConverter().ConvertFromString("#ffffff"));
                    else
                        return ((SolidColorBrush)new BrushConverter().ConvertFromString("#ffff00"));
                }
                if (state.IndexOf('1') >= 0)
                    return ((SolidColorBrush)new BrushConverter().ConvertFromString("#ff0000"));
                if (state.IndexOf('2') >= 0)
                    return ((SolidColorBrush)new BrushConverter().ConvertFromString("#00ffff"));
                if (state.IndexOf('3') >= 0)
                    return ((SolidColorBrush)new BrushConverter().ConvertFromString("#ff69b4"));
                if (state.IndexOf('4') >= 0)
                    return ((SolidColorBrush)new BrushConverter().ConvertFromString("#ffa500"));

                switch (model.State[0])
                {
                    case '#':
                        return ((SolidColorBrush)new BrushConverter().ConvertFromString("#0000ff"));
                    case '.':
                        return ((SolidColorBrush)new BrushConverter().ConvertFromString("#303030"));
                    default:
                        return ((SolidColorBrush)new BrushConverter().ConvertFromString("#000000"));
                }

            }
            // Fallback
            return ((SolidColorBrush)new BrushConverter().ConvertFromString(Background));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static Color GetBackgroundColor(CellViewModel model)
        {
            if (model.Y % 2 == 0 && model.X % 2 != 0)
                return ((Color)ColorConverter.ConvertFromString(Background));

            if (model.Y % 2 == 0 && model.X % 2 == 0)
                return ((Color)ColorConverter.ConvertFromString(BackgroundWhite));

            if (model.Y % 2 != 0 && model.X % 2 != 0)
                return ((Color)ColorConverter.ConvertFromString(BackgroundWhite));

            if (model.Y % 2 != 0 && model.X % 2 == 0)
                return ((Color)ColorConverter.ConvertFromString(Background));

            return ((Color)ColorConverter.ConvertFromString(Background));
        }
    }

    public class ColorStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int code;
            if (value is int)
                code = (int)value;
            else
                code = -1;

            switch (code)
            {
                case 0:
                    return new SolidColorBrush(Colors.Yellow);
                case 1:
                    return new SolidColorBrush(Colors.Red);
                default:
                    return new SolidColorBrush(Colors.White);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CellStateOuterTopConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string code = null;
            if (value is int)
                code = value.ToString();

            var s = value as string;
            if (s != null)
                code = s;

            var model = value as CellViewModel;
            if (model != null)
                code = model.State;

            switch (code)
            {
                case ".":
                    return CellStateConverter.GetBackgroundColor(model);
                case "0":
                case "a":
                case "A":
                    return ((Color)ColorConverter.ConvertFromString(CellStateConverter.BlackOuterTop));
                case "1":
                case "b":
                case "B":
                    return ((Color)ColorConverter.ConvertFromString(CellStateConverter.WhiteOuterTop));
                case "!":
                    return ((Color)ColorConverter.ConvertFromString(CellStateConverter.RedOuterTop));
                default:
                    return CellStateConverter.GetBackgroundColor(model);
                    // FIXME throw new NotSupportedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CellStateOuterBottomConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string code = null;
            if (value is int)
                code = value.ToString();

            var s = value as string;
            if (s != null)
                code = s;

            var model = value as CellViewModel;
            if (model != null)
                code = model.State;

            switch (code)
            {
                case ".":
                    return CellStateConverter.GetBackgroundColor(model);
                case "0":
                case "a":
                case "A":
                    return ((Color)ColorConverter.ConvertFromString(CellStateConverter.BlackOuterBottom));
                case "1":
                case "b":
                case "B":
                    return ((Color)ColorConverter.ConvertFromString(CellStateConverter.WhiteOuterBottom));
                case "!":
                    return ((Color)ColorConverter.ConvertFromString(CellStateConverter.RedOuterBottom));
                default:
                    return CellStateConverter.GetBackgroundColor(model);
                    // FIXME throw new NotSupportedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CellStateInnerTopConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string code = null;
            if (value is int)
                code = value.ToString();

            var s = value as string;
            if (s != null)
                code = s;

            var model = value as CellViewModel;
            if (model != null)
                code = model.State;

            switch (code)
            {
                case ".":
                    return CellStateConverter.GetBackgroundColor(model);
                case "0":
                case "a":
                case "A":
                    return ((Color)ColorConverter.ConvertFromString(CellStateConverter.BlackInnerTop));
                case "1":
                case "b":
                case "B":
                    return ((Color)ColorConverter.ConvertFromString(CellStateConverter.WhiteInnerTop));
                case "!":
                    return ((Color)ColorConverter.ConvertFromString(CellStateConverter.RedInnerTop));
                default:
                    return CellStateConverter.GetBackgroundColor(model);
                    // FIXME throw new NotSupportedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CellStateInnerBottomConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string code = null;
            if (value is int)
                code = value.ToString();

            var s = value as string;
            if (s != null)
                code = s;

            var model = value as CellViewModel;
            if (model != null)
                code = model.State;

            switch (code)
            {
                case ".":
                    return CellStateConverter.GetBackgroundColor(model);
                case "0":
                case "a":
                case "A":
                    return ((Color)ColorConverter.ConvertFromString(CellStateConverter.BlackInnerBottom));
                case "1":
                case "b":
                case "B":
                    return ((Color)ColorConverter.ConvertFromString(CellStateConverter.WhiteInnerBottom));
                case "!":
                    return ((Color)ColorConverter.ConvertFromString(CellStateConverter.RedInnerBottom));
                default:
                    return CellStateConverter.GetBackgroundColor(model);
                    // FIXME throw new NotSupportedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}