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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using DySdk;


namespace DouYuRecorder
{
    [Serializable]
    class ConfigData
    {
        public string SavePath = null;
    }

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private Operator opdata;
        private string system_user_video = null;
        private string log_file = null;
        //private string config_file = null;
        private NotifyIcon nficon = null;

        private void Window_Notify(object sender, EventArgs args)
        {
            WindowState = WindowState.Normal;
            ShowInTaskbar = true;
        }

        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            nficon = new NotifyIcon();
            nficon.Click += new EventHandler(Window_Notify);
            nficon.Icon = Resource.Hani9_Icon;
            nficon.Visible = true;

            // 默认路径
            system_user_video = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)) + "\\Videos";

            if (!Directory.Exists(system_user_video))
            {
                system_user_video = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            // 日志
            log_file = Directory.GetCurrentDirectory() + "\\" + "logfile.log";
            FileInfo fi = new FileInfo(log_file);
            long LogFileMaxSize = 0x100000; //大于1m，覆盖
            if (fi.Exists && fi.Length > LogFileMaxSize)
            {
                File.Create(log_file).Close();
            }


            LogMsgLine(string.Format("启动时间：{0}", DateTime.Now));
            // 配置文件位置
            string config_file = Directory.GetCurrentDirectory() + "\\" + "config.conf";

            if (!File.Exists(config_file))
            {
                File.Create(config_file).Close();
                FileSavePathTextBox.Text = system_user_video;
                SaveConfig();
            }
            else {

                BinaryFormatter formatter = new BinaryFormatter();
                ConfigData config = OpenConfig();

                if (config != null && Directory.Exists(config.SavePath))
                {
                    FileSavePathTextBox.Text = config.SavePath;
                }
                else
                {
                    FileSavePathTextBox.Text = system_user_video;
                }

            }

            string ffmpeg_path = Directory.GetCurrentDirectory() + "\\" +
                "FFmpeg";

            if (Directory.Exists(ffmpeg_path))
            {
                var syspath = Environment.GetEnvironmentVariable("path");
                Environment.SetEnvironmentVariable("path", ffmpeg_path + ";" + syspath);
            }
            else
            {
                Directory.CreateDirectory(ffmpeg_path);
            }

            opdata = new Operator(this);

        }


        private void SetAppSysPath(string addpath)
        {
            if (Directory.Exists(addpath))
            {
                var syspath = Environment.GetEnvironmentVariable("path");
                Environment.SetEnvironmentVariable("path", addpath + ";" + syspath);
            }
        }


        // 配置
        private ConfigData OpenConfig()
        {
            string config_file = Directory.GetCurrentDirectory() + "\\" + "config.conf";
            FileStream fs = new FileStream(config_file, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            ConfigData config = null;
            try
            {
                config = (ConfigData)formatter.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                config = null;
                FileSavePathTextBox.Text = system_user_video;
                LogMsgLine("OpenConfig：" + e.Message);
            }
            finally
            {
                fs.Close();
            }
            return config;
        }

        private void SaveConfig()
        {
            string config_file = Directory.GetCurrentDirectory() + "\\" + "config.conf";
            FileStream fs = null;
            try
            {
                fs = new FileStream(config_file, FileMode.Open, FileAccess.ReadWrite);
            }
            catch (Exception e1)
            {
                LogMsgLine("SaveConfig：" + e1.Message + "\n");
            }

            BinaryFormatter formatter = new BinaryFormatter();
            ConfigData config = new ConfigData();
            config.SavePath = FileSavePathTextBox.Text;
            try
            {
                formatter.Serialize(fs, config);
            }
            catch (SerializationException e2)
            {
                LogMsgLine("SaveConfig：" + e2.Message + "\n");
            }
            finally
            {
                if (fs != null) fs.Close();
            }
        }

        public void LogMsg(string msg)
        {
            FileStream fs = new FileStream(log_file,
                FileMode.OpenOrCreate | FileMode.Append);
            byte[] msgbytes = System.Text.UnicodeEncoding.Default.GetBytes(msg);
            fs.Write(msgbytes, 0, msgbytes.Length);
            fs.Close();
        }

        public void LogMsgLine(string msg)
        {
            FileStream fs = new FileStream(log_file,
                FileMode.OpenOrCreate | FileMode.Append);
            byte[] msgbytes = System.Text.UnicodeEncoding.Default.GetBytes(msg + Environment.NewLine);
            fs.Write(msgbytes, 0, msgbytes.Length);
            fs.Close();
        }

        private void RootWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        public string GetSavePath()
        {
            return FileSavePathTextBox.Text;
        }

        public void MsgBox(string msg)
        {
            new WpfMessageBox(this, msg);
            //System.Windows.MessageBox.Show(msg);
        }

        // text color
        void setColor()
        {
            
        }

        /*
        private static void CheckRecordStatus(object obj)
        {
            MainWindow win = (MainWindow)obj;
            while (true)
            {
                if (!win.btn_record_clicked) return;
                if (!win.opdata.GetRVStatus())
                {
                    //Thread operator
                    win.Dispatcher.Invoke(new MethodInvoker(
                    delegate ()
                    {
                        win.opdata.ShowDanmu = false;
                        win.DanmuTextBox.AppendText("录制弹幕结束" + Environment.NewLine);
                        win.btn_record.Content = "手动录制";
                        win.btn_record_clicked = false;
                    }
                    ));
                    break;
                }
                Thread.Sleep(3000);
            }
        }
        */



        // 录制按钮
        public bool btn_record_clicked = false;
        public bool btn_auto_record_clicked = false;
        //private Thread ThreadRecordCheck;
        private void BtnRecord_Clicked(object sender, RoutedEventArgs e)
        {
            FrameworkElement src = e.Source as FrameworkElement;
            if (src != null)
            {
                switch (src.Name) {
                    case "btn_record":

                        if (!UserFile.Exists("ffmpeg.exe"))
                        {
                            MsgBox("没能找到ffmpeg.exe，录制停止");
                            return;
                        }

                        if (btn_auto_record_clicked)
                        {
                            MsgBox("自动录制中...");
                            return;
                        }

                        if (!btn_record_clicked)
                        {
                            if (DouYuUrl.Text.Length == 0)
                            {
                                MsgBox("空网址");
                                return;
                            }

                            btn_record.Content = "停止录制";
                            opdata.RecordStart();
                            btn_record_clicked = true;
                        }
                        else
                        {
                            btn_record.Content = "手动录制";
                            opdata.RecordStop();
                            btn_record_clicked = false;
                        }
                        break;

                    case "btn_auto_record":

                        if (!UserFile.Exists("ffmpeg.exe"))
                        {
                            MsgBox( "没能找到ffmpeg.exe，录制停止");
                            return;
                        }

                        if (btn_record_clicked)
                        {
                            MsgBox("手动录制中...");
                            return;
                        }

                        if (DouYuUrl.Text.Length == 0)
                        {
                            MsgBox( "空网址");
                            return;
                        }

                        if (btn_auto_record_clicked)
                        {
                            btn_auto_record_clicked = false;
                            btn_auto_record.Content = "自动录制";
                            opdata.bAutoRecord = false;
                        }
                        else
                        {
                            btn_auto_record_clicked = true;
                            btn_auto_record.Content = "停止录制";
                            opdata.bAutoRecord = true;
                            opdata.AutoRecord();
                        }

                        break;


                    case "SelectPath":
                        var fbd = new System.Windows.Forms.FolderBrowserDialog();
                        if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            FileSavePathTextBox.Text = fbd.SelectedPath;
                            SaveConfig();
                        }

                        break;

                    case "WindowClose":
                        opdata.bAutoRecord = false;
                        opdata.RecordStop();
                        //opdata.StartDanmu = false;
                        nficon.Visible = false;
                        Close();
                        break;
                    case "WindowMinimized":
                        WindowState = WindowState.Minimized;
                        ShowInTaskbar = false;
                        break;

                    case "ShowDanmu":
                        System.Windows.Controls.CheckBox c = sender as System.Windows.Controls.CheckBox;
                        opdata.ShowDanmu = c.IsChecked.Value;
                        break;

                    //case "btn_test":
                       // WpfMessageBox child = new WpfMessageBox(this,"test");
                        //child.Show();
                        //break;

                }
            }
        }


        private void Title_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Control src = e.Source as System.Windows.Controls.Control;
            if (src != null)
            {
                switch (e.ChangedButton)
                {
                    case MouseButton.Left:
                        DragMove();
                        e.Handled = true;
                        break;

                    default:
                        break;
                }
            }

        }

        private void DanmuTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (KeepScroll.IsChecked == true)
            {
                DanmuTextBox.ScrollToEnd();
            }
            
        }
    }


}
