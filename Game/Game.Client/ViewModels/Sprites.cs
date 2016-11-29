using Game.AdminClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Game.AdminClient.ViewModels
{
    public class ActorSprite
    {
        protected Point _position;
        protected GeometryGroup _geometry;
        protected Drawing _drawing;
        protected TranslateTransform _offset;

        public Drawing SpriteDrawing { get { return _drawing; } }

        protected ActorSprite(Point position)
        {
            _position = position;
            _offset = new TranslateTransform(_position.X, _position.Y);
            _geometry = new GeometryGroup();
            _geometry.Transform = _offset;
        }

        public void Move(Point position)
        {
            // Movement direction
            do
            {
                int angle;
                if (position.X > _position.X)
                    angle = 0; // right
                else if (position.X < _position.X)
                    angle = 180; // left
                else if (position.Y > _position.Y)
                    angle = 90; // down
                else if (position.Y < _position.Y)
                    angle = 270; // up
                else
                    break; // stay
                SetAngle(angle);
            } while (false);
            // Actual movement
            var duration = new System.Windows.Duration(TimeSpan.FromSeconds(0.5));
            var xanim = new DoubleAnimation(_position.X, position.X, duration);
            var yanim = new DoubleAnimation(_position.Y, position.Y, duration);
            _position = position;
            _offset.BeginAnimation(TranslateTransform.XProperty, xanim);
            _offset.BeginAnimation(TranslateTransform.YProperty, yanim);
        }

        protected virtual void SetAngle(int angle)
        {
            // custom action when overriden
        }
    }

    public class TecmanSprite : ActorSprite
    {
        private RotateTransform _mouthRotate;

        public TecmanSprite(Point position) : base(position)
        {
            // Body
            var body = new EllipseGeometry(new System.Windows.Point(0.5, 0.5), 0.4, 0.4);
            body.Transform = _offset;
            var bodyDrawing = new GeometryDrawing(Brushes.Yellow, null, body);

            // Mouth
            var mouth = new PathGeometry();
            var fig = new PathFigure();
            fig.StartPoint = new System.Windows.Point(0.5, 0.5);
            fig.Segments.Add(new LineSegment(new System.Windows.Point(0.9, 0.4), true));
            fig.Segments.Add(new LineSegment(new System.Windows.Point(0.9, 0.6), true));
            fig.IsClosed = true;
            mouth.Figures.Add(fig);
            var mouthTransform = new TransformGroup();
            _mouthRotate = new RotateTransform(0, 0.5, 0.5);
            mouthTransform.Children.Add(_mouthRotate);
            mouthTransform.Children.Add(_offset);
            mouth.Transform = mouthTransform;
            var mouthDrawing = new GeometryDrawing(Brushes.Red, null, mouth);

            // Combine
            var drawing = new DrawingGroup();
            drawing.Children.Add(bodyDrawing);
            drawing.Children.Add(mouthDrawing);
            _drawing = drawing;
        }

        protected override void SetAngle(int angle)
        {
            _mouthRotate.Angle = angle;
        }
    }

    public class GhostSprite : ActorSprite
    {
        private RotateTransform _dirRotate;

        public GhostSprite(Point position, int index) : base(position)
        {
            Color color;
            System.Windows.Point pop;
            double box = 0.2;
            double margin = 0.02;
            switch (index)
            {
                case 0:
                    color = Color.FromRgb(0xff, 0x00, 0x00);
                    pop = new System.Windows.Point(margin, margin);
                    break;
                case 1:
                    color = Color.FromRgb(0x00, 0xff, 0xff);
                    pop = new System.Windows.Point(1 - margin - box, margin);
                    break;
                case 2:
                    color = Color.FromRgb(0xff, 0x69, 0xb4);
                    pop = new System.Windows.Point(1 - margin - box, 1 - margin - box);
                    break;
                case 3:
                    color = Color.FromRgb(0xff, 0xa5, 0x00);
                    pop = new System.Windows.Point(margin, 1 - margin - box);
                    break;
                default:
                    color = Colors.Green;
                    pop = new System.Windows.Point(0, 0);
                    break;
            }
            // body
            var body = new PathGeometry();
            var fig = new PathFigure();
            fig.StartPoint = new System.Windows.Point(0.5, 0.1);
            fig.Segments.Add(new LineSegment(new System.Windows.Point(0.9, 0.4), true));
            fig.Segments.Add(new LineSegment(new System.Windows.Point(0.7, 0.9), true));
            fig.Segments.Add(new LineSegment(new System.Windows.Point(0.5, 0.7), true));
            fig.Segments.Add(new LineSegment(new System.Windows.Point(0.3, 0.9), true));
            fig.Segments.Add(new LineSegment(new System.Windows.Point(0.1, 0.4), true));
            fig.IsClosed = true;
            body.Figures.Add(fig);
            // marker popup
            var rect = new System.Windows.Rect(pop.X, pop.Y, box, box);
            var marker = new EllipseGeometry(rect);
            // combine body & marker
            var geometry = new GeometryGroup();
            geometry.Children.Add(body);
            geometry.Children.Add(marker);
            var bodyDrawing = new GeometryDrawing(new SolidColorBrush(color), null, geometry);
            // direction
            var dir = new PathGeometry();
            fig = new PathFigure();
            fig.StartPoint = new System.Windows.Point(0.8, 0.3);
            fig.Segments.Add(new LineSegment(new System.Windows.Point(1.0, 0.5), true));
            fig.Segments.Add(new LineSegment(new System.Windows.Point(0.8, 0.7), true));
            fig.IsClosed = true;
            dir.Figures.Add(fig);
            _dirRotate = new RotateTransform(0, 0.5, 0.5);
            dir.Transform = _dirRotate;
            var dirDrawing = new GeometryDrawing(Brushes.Yellow, new Pen(Brushes.Black, 0.05), dir);
            // combine all
            var drawing = new DrawingGroup();
            drawing.Children.Add(bodyDrawing);
            drawing.Children.Add(dirDrawing);
            drawing.Transform = _offset;
            _drawing = drawing;
        }

        protected override void SetAngle(int angle)
        {
            _dirRotate.Angle = angle;
        }
    }

    public class CookieSprite
    {
        private Geometry _geometry;
        private Drawing _drawing;

        public Drawing SpriteDrawing { get { return _drawing; } }

        public CookieSprite(int x, int y)
        {
            _geometry = new EllipseGeometry(
                new System.Windows.Point(x + 0.5, y + 0.5),
                1.0 / 4, 1.0 / 6);
            _drawing = new GeometryDrawing(Brushes.White, null, _geometry);
        }

        public void Eat()
        {
            var duration = new System.Windows.Duration(TimeSpan.FromSeconds(0.5));
            var ranim = new DoubleAnimation(0, duration);
            _geometry.BeginAnimation(EllipseGeometry.RadiusXProperty, ranim);
            _geometry.BeginAnimation(EllipseGeometry.RadiusYProperty, ranim);
        }
    }
}
