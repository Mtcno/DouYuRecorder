using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Threading;
using System.Diagnostics;
using DySdk;

namespace DouYuRecorder
{

    class Operator
    {
        private MainWindow win = null;
        private ulong RoomID = 0;
        private delegate void ThreadOperator(string msg);
        private string CurrentDir = null;
        private string RecordSavePath = null;
        private string RecordFilePath = null;

        public bool ShowDanmu = false;


        public Operator(Window win)
        {
            this.win = (MainWindow)win;
            CurrentDir = Directory.GetCurrentDirectory();
        }

        private string MakeSaveDir(){
            string roomdir = string.Format("[{0}] {1}", RoomID, CommonTime.GetDayTimeString());
            return roomdir;        
        }

        private string MakeRecordFilePath(string extName = "")
        {
            return string.Format("[{0}] {1}", RoomID, CommonTime.GetSaveTimeString() + extName);
        }

        private void SetAutoRecordEnd()
        {
            StartDanmu = false;
            win.Dispatcher.Invoke(
                new MethodInvoker(
                delegate () {
                    win.btn_auto_record_clicked = false;
                    win.btn_auto_record.Content = "自动录制";
                }
                )
            );
        }

        private string GetDouYuUrl()
        {
            string url = null;
            win.Dispatcher.Invoke(
                new MethodInvoker(
                delegate () { url = win.DouYuUrl.Text; }
                )
            );
            return url;
        }

        private string GetSavePath(bool Thread = false)
        {
            string path = null;
            if (Thread)
            {
                win.Dispatcher.Invoke(
                    new MethodInvoker(
                    delegate () { path = win.FileSavePathTextBox.Text; }
                    )
                );
            }
            else
            {
                path = win.FileSavePathTextBox.Text;
            }
            return path;
        }

        private void LogError(string msgError,bool thread = false)
        {
            string timeLog = DateTime.Now.ToString();
            string msg = timeLog + " " + msgError;
            if (thread)
            {
                win.Dispatcher.Invoke(new ThreadOperator(win.LogMsgLine), msg);
            }
            else
            {
                win.LogMsgLine(msg);
            }
        }

        private void RecordDanmu(string msgDanmu)
        {
            var dict = DyMsg.GetDict(msgDanmu);
            string msg = string.Format("{0}:{1}", dict["nn"], dict["txt"]);
            if (ShowDanmu)
            {
                DanmuPrint(msg + Environment.NewLine, true);
            }
            
        }

        private void RecordDanmuClosing()
        {            
            DanmuPrint("录制弹幕结束"+ Environment.NewLine, true);
        }

        public void DanmuPrint(string msgDanmu,bool thread = false)
        {
            if (thread)
            {
                win.DanmuTextBox.Dispatcher.Invoke(new ThreadOperator(win.DanmuTextBox.AppendText), msgDanmu);
            }
            else
            {
                win.DanmuTextBox.AppendText(msgDanmu);
            }
        }


        public bool StartDanmu = false;
        private static void StartDanmuRecord(object obj)
        {
            Operator op = (Operator)obj;
            DySocket s = new DySocket();

            s.login(op.RoomID);
            op.StartDanmu = true;

            //打开弹幕文件
            FileStream fs = new FileStream(op.RecordFilePath + ".danmu",
                FileMode.OpenOrCreate
                );

            long LastTime = CommonTime.GetTimeStamp();

            while (true)
            {
                s.keeplive();
                if (op.StartDanmu == false)break;
                
                byte[] resraw = s.recv(4);
                if (resraw == null)
                {
                    op.LogError("Recv Timeout");
                    break;
                }

                int len = BitConverter.ToInt32(resraw, 0);
                if (len <= 0)
                    continue;

                resraw = s.recv(8);
                short type = BitConverter.ToInt16(resraw, 4);

                if (type != 690)
                    continue;

                // 正式获取消息
                resraw = s.recv(len - 8);

                string resstr = System.Text.Encoding.UTF8.GetString(resraw);

                // 弹幕
                if (resstr.Contains("type@=chatmsg")
                && resstr.Contains("txt@=")
                && resstr.Contains("nn@=")
                )
                {
                    long timeCount = CommonTime.GetTimeStamp() - LastTime;
                    var dict = DyMsg.GetDict(resstr);
                    string outDanmu = null;
                    if (dict.ContainsKey("col"))
                    {
                         outDanmu = string.Format("TimeCount={0},user={1},text={2},col={3}",
                            timeCount, dict["nn"], dict["txt"], dict["col"]) + Environment.NewLine;
                    }
                    else
                    {
                         outDanmu = string.Format("TimeCount={0},user={1},text={2}",
                            timeCount, dict["nn"], dict["txt"]) + Environment.NewLine;
                    }
                    byte[] bDanmu = Encoding.UTF8.GetBytes(outDanmu);
                    try
                    {
                        fs.Write(bDanmu, 0, bDanmu.Length);
                    }
                    catch(Exception e)
                    {
                        op.LogError("Danmu:"+ e.Message, true);
                    }              
                    op.RecordDanmu(resstr);
                }
            }
            fs.Close();
            s.logout();
            s.release();
            op.RecordDanmuClosing();
        }

        private static void StartVideoRecord(object obj)
        {
            Operator op = (Operator)obj;
            string procmsg = DyVideo.VideoOperate(op.RoomID, 
                "save",op.RecordFilePath + ".flv" , 
                out op.VideoProc);
            op.LogError(procmsg,true);
            op.LogError("Video Record 结束", true);
        }

        public bool GetShowStatus()
        {
            int status = DySdk.DyOpenApiV2.GetRoomStatus(RoomID);
            if(status == 1)
            {
                return true;
            }
            return false;
        }

        // 录制开始、停止
        private Thread ThreadDanmu = null;
        private Thread ThreadVideo = null;
        private Process VideoProc = null;

        public bool RecordStart()
        {
            string url = GetDouYuUrl();
            try {
                RoomID = Function.GetRidFromUrl(url);
            }
            catch (Exception e)
            {
                LogError(e.Message, true);
                RoomID = RoomID = 0;
            }

            if (RoomID == 0)
            {
                win.MsgBox("未能获取房间号");
                return false;
            }

            var save_path = this.GetSavePath(true);
            if (!Directory.Exists(save_path))
            {
                win.MsgBox("储存路径不正确");
                return false;
            }

            // 处理保存目录
            try
            {
                RecordSavePath = save_path + "\\" + MakeSaveDir();
                if (Directory.Exists(RecordSavePath + MakeSaveDir()) == false)
                {
                    Directory.CreateDirectory(RecordSavePath);
                }
                RecordFilePath = RecordSavePath + "\\" + MakeRecordFilePath();
            } catch (Exception e)
            {
                LogError("RecordStart:" + e.Message,true);
                return false;
            }

            ThreadDanmu = null;
            ThreadDanmu = new Thread(new ParameterizedThreadStart(StartDanmuRecord));
            ThreadDanmu.Start(this);

            VideoProc = null;
            ThreadVideo = null;
            ThreadVideo = new Thread(new ParameterizedThreadStart(StartVideoRecord));
            ThreadVideo.Start(this);

            return true;
        }

        public void RecordStop()
        {
            if(VideoProc!=null && !VideoProc.HasExited) VideoProc.Kill();
            VideoProc = null;
            StartDanmu = false; 
            RoomID = 0;    
            
        }


        public bool bAutoRecord = false;
        public static void AutoRecordThread(object obj)
        {
            Operator op = (Operator)obj;

            string url = op.GetDouYuUrl();
            op.RoomID = Function.GetRidFromUrl(url);
            if (op.RoomID == 0) return;

            while (op.bAutoRecord)
            {
                if (!op.GetShowStatus())
                {
                    Thread.Sleep(3000);
                    continue;
                }
                else
                {
                    
                    if (op.VideoProc == null)
                    {
                        bool bStart = op.RecordStart();
                        if (bStart == false) op.SetAutoRecordEnd();             
                    }
                    else
                    {
                        //开播期间断开
                        if (!op.ThreadVideo.IsAlive)
                        {
                            op.RecordStop();
                            int i = 0;
                            while (op.ThreadDanmu.IsAlive) { //等待弹幕线程结束
                                if (i>10)
                                {
                                    op.ThreadDanmu.Abort();
                                    break;
                                }
                                i++;
                                Thread.Sleep(1000);

                            }
                        }
                    }

                    Thread.Sleep(3000);
                }
            }
            op.RecordStop();
            op.SetAutoRecordEnd();
        }

        public Thread ThreadAutoRecord = null;
        public void AutoRecord()
        {
            string url = GetDouYuUrl();
            RoomID = Function.GetRidFromUrl(url);
            if (RoomID == 0)
            {
                System.Windows.MessageBox.Show("未能找到房间号，可能网址有误。", "消息：");
                return;
            }
            
            ThreadAutoRecord = new Thread(new ParameterizedThreadStart(AutoRecordThread));
            ThreadAutoRecord.Start(this);
        }

        public bool GetRVStatus()
        {
            if(ThreadVideo!=null && ThreadVideo.IsAlive)
            {
                return true;
            }
            return false;
        }

        public bool GetRDStatus()
        {
            if (ThreadDanmu != null && ThreadDanmu.IsAlive)
            {
                return true;
            }
            return false;
        }


    }
}
