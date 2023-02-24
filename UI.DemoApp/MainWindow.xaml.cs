using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Wpf;
using System;
using System.Windows;
using UI.DemoApp.Models;

namespace UI.DemoApp
{
    public partial class MainWindow : Window
    {
        private ExampleScene _scene;
        private Size _size;
        private Vector2 _mousePosition;


        public MainWindow()
        {
            InitializeComponent();
            InitializeOpenGL();

            _scene = new ExampleScene();
            Title = $"HalfEdge Demo - displaying {_scene.TriangleCount} Triangles...";
        }


        private void InitializeOpenGL()
        {
            var mainSettings = new GLWpfControlSettings
            {
                MajorVersion = 4,
                MinorVersion = 6,
                GraphicsProfile = ContextProfile.Compatability,
                GraphicsContextFlags = ContextFlags.Debug,
                RenderContinuously = false
            };
            _size = ViewControl.RenderSize;
            ViewControl.Start(mainSettings);
        }

        private void ViewControl_OnRender(TimeSpan delta)
        {
            _scene.Render((int)_size.Width, (int)_size.Height);
        }

        private void ViewControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _size = e.NewSize;
            ViewControl.InvalidateVisual();
        }

        private void ViewControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var currentPosition = new Vector2((float)e.GetPosition(this).X, (float)e.GetPosition(this).Y);
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                var delta = currentPosition - _mousePosition;
                delta.Y *= -1;
                _scene.HandleViewChange(delta * .01f / MathHelper.Pi, 1f);
                ViewControl.InvalidateVisual();
            }

            _mousePosition = currentPosition;
        }

        private void ViewControl_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            _scene.HandleViewChange(Vector2.Zero, (.25f * e.Delta) / 120f);
            ViewControl.InvalidateVisual();
        }
    }
}
