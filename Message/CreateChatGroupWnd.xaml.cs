﻿using Message.AdditionalItems;
using Message.Interfaces;
using Message.ViewModel;
using System;
using System.Windows;

namespace Message
{
    /// <summary>
    /// Interaction logic for CreateChatGroupWnd.xaml
    /// </summary>
    public partial class CreateChatGroupWnd : Window, IView
    {
        public CreateChatGroupWnd()
        {
            InitializeComponent();

            DwmDropShadow.DropShadowToWindow(this);

            DataContext = new CreateChatGroupWndVM(this);
        }

        public void AnimatedResize(int h, int w)
        {
            throw new NotImplementedException();
        }

        public void CloseWindow()
        {
            Close();
        }

        public void Hide(bool isVisible)
        {
            if (isVisible)
            {
                Visibility = Visibility.Visible;
            }
            else
            {
                Visibility = Visibility.Hidden;
            }
        }

        public void SetOpacity(double opasity)
        {
            throw new NotImplementedException();
        }

        private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }
    }
}