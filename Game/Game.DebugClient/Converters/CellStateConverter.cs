using Game.DebugClient.Infrastructure;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Game.DebugClient.Converters
{
    public class CellStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CellState cs = (CellState)value;

            Color ghost1 = Color.FromRgb(0xff, 0x00, 0x00);
            Color ghost2 = Color.FromRgb(0x00, 0xff, 0xff);
            Color ghost3 = Color.FromRgb(0xff, 0x69, 0xb4);
            Color ghost4 = Color.FromRgb(0xff, 0xa5, 0x00);

            // Check for simple case of unoccupied cell
            if (cs < CellState.Ghost1)
            {
                switch (cs)
                {
                    case CellState.Empty:
                        return new SolidColorBrush(Colors.Black);
                    case CellState.Cookie:
                        return new SolidColorBrush(Color.FromRgb(0x30, 0x30, 0x30));
                    case CellState.Wall:
                        return new SolidColorBrush(Colors.Blue);
                    default:
                        throw new NotSupportedException();
                }
            }

            // Handle tecman case
            if ((cs & CellState.Tecman) != 0)
            {
                return new SolidColorBrush(Colors.Yellow);
            }

            // Handle current ghosts
            if ((cs & CellState.AllGhosts) != 0)
            {
                // Just show by priority
                if ((cs & CellState.Ghost1) != 0)
                    return new SolidColorBrush(ghost1);
                if ((cs & CellState.Ghost2) != 0)
                    return new SolidColorBrush(ghost2);
                if ((cs & CellState.Ghost3) != 0)
                    return new SolidColorBrush(ghost3);
                return new SolidColorBrush(ghost4);
            }

            // Anything else - just don't handle right now
            switch (cs & CellState.SceneryMask)
            {
                case CellState.Empty:
                    return new SolidColorBrush(Colors.Black);
                case CellState.Cookie:
                    return new SolidColorBrush(Color.FromRgb(0x30, 0x30, 0x30));
                case CellState.Wall:
                    return new SolidColorBrush(Colors.Blue);
                default:
                    throw new NotSupportedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}