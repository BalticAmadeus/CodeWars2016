using Game.DebugClient.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Game.DebugClient.Converters
{
    public class PlayerIndexColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var move = (PlayerMoveModel)value;
            if (move.IsTecman)
                return new SolidColorBrush(Colors.Yellow);
            else
            {
                Color ghost1 = Color.FromRgb(0xff, 0x00, 0x00);
                Color ghost2 = Color.FromRgb(0x00, 0xff, 0xff);
                Color ghost3 = Color.FromRgb(0xff, 0x69, 0xb4);
                Color ghost4 = Color.FromRgb(0xff, 0xa5, 0x00);
                switch (move.Index)
                {
                    case 0:
                        return new SolidColorBrush(ghost1);
                    case 1:
                        return new SolidColorBrush(ghost2);
                    case 2:
                        return new SolidColorBrush(ghost3);
                    case 3:
                        return new SolidColorBrush(ghost4);
                    default:
                        return new SolidColorBrush(Colors.White);
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
