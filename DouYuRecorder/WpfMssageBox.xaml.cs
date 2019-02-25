using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DouYuRecorder
{
    /// <summary>
    /// WpfMessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class WpfMessageBox : Window
    {
        public WpfMessageBox(Window parent , string msg = null,int width=400,int height = 180)
        {
            InitializeComponent();
            Width = width;
            Height = height;
            if (msg != null) msgtext.Text = msg;
            Owner = parent;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Show();
        }

        private void Btn_Clicked(object sender, RoutedEventArgs e)
        {
            var con = e.Source as FrameworkElement;
            if(con != null)
            {
                switch (con.Name)
                {
                    case "btnMinimized":
                        this.WindowState = WindowState.Minimized;
                        break;
                    case "btnClose":
                        this.Close();
                        break;
                }
            }
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
