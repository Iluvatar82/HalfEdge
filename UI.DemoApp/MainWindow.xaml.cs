using OpenTK.Windowing.Common;
using OpenTK.Wpf;
using System;
using System.Windows;
using UI.DemoApp.Models;

namespace UI.DemoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ExampleScene _scene;
        private Size _size;


        public MainWindow()
        {
            InitializeComponent();
            InitializeOpenGL();

            _scene = new ExampleScene();
        }


        private void InitializeOpenGL()
        {
            var mainSettings = new GLWpfControlSettings { MajorVersion = 4, MinorVersion = 6,
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
    }
}
