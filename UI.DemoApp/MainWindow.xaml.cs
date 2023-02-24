﻿using OpenTK.Mathematics;
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
    public partial class MainWindow : System.Windows.Window
    {
        private ExampleScene _scene;
        private Size _size;
        private Vector2 _mousePosition;


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

        private void ViewControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var currentPosition = new Vector2((float)e.GetPosition(this).X, (float)e.GetPosition(this).Y);
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                var delta = currentPosition - _mousePosition;
                _scene.HandleViewChange(delta * .01f / MathHelper.Pi);
                ViewControl.InvalidateVisual();
            }

            _mousePosition = currentPosition;
        }
    }
}
