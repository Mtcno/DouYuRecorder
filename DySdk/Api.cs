using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace DySdk
{

    public class UserFile
    {
        public static bool Exists(string Name)
        {
            if (File.Exists(Name))
            {
                return true;
            }

            string[] path = Environment.GetEnvironmentVariable("path").Split(';');
            foreach (var p in path)
            {
                bool bExists = File.Exists(p + "\\" + Name);
                
                if (bExists)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class CommonTime
    {
        public static long GetSeconds()
        {
            long timeStamp = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1).Ticks;
            return timeStamp / 10000000;
        }
        public static long GetTimeStamp()
        {
            long timeStamp = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1).Ticks;
            return timeStamp / 10000;
        }

        public static long GetTimeTick()
        {
            long timeStamp = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1).Ticks;
            return timeStamp;
        }
        public static string GetSaveTimeString()
        {                
            return DateTime.Now.ToString("yyyyMMdd_HHmmss");
        }
        public static string GetDayTimeString()
        {
            return DateTime.Now.ToString("yyyyMMdd");
        }

    }

    public class HttpReq
    {
        public static string ReqUrl(string url)
        {
            string resString = null;
            using (WebClient wc = new WebClient())
            {

                var data = wc.OpenRead(url);
                using (var reader = new StreamReader(data))
                {
                    resString = reader.ReadToEnd();
                }
            }
            return resString;
        }
        public static string OpenDyRoom(string url)
        {
            string resString = null;
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add(
                    "user-agent",
                    "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.62 Safari/537.36"
                 );
                var data = wc.OpenRead(url);
                using (var reader = new StreamReader(data))
                {
                    resString = reader.ReadToEnd();
                }
            }
            return resString;
        }
    }

    public static class DataPack
    {
        public static byte[] String2Bytes(string str)
        {
            return System.Text.UTF8Encoding.Default.GetBytes(str);
        }

        public static byte[] MakeMsg(short type, string msg)
        {
            uint len = (uint)msg.Length + 8;
            byte[] msgraw = new byte[12 + msg.Length];
            msgraw[3] = (byte)((len >> 24) & 0xFF);
            msgraw[2] = (byte)((len >> 16) & 0xFF);
            msgraw[1] = (byte)((len >> 8) & 0xFF);
            msgraw[0] = (byte)(len & 0xFF);
            msgraw[7] = msgraw[3];
            msgraw[6] = msgraw[2];
            msgraw[5] = msgraw[1];
            msgraw[4] = msgraw[0];
            msgraw[9] = (byte)((type >> 8) & 0xFF);
            msgraw[8] = (byte)(type & 0xFF);
            msgraw[10] = 0;
            msgraw[11] = 0;
            byte[] tmp = String2Bytes(msg);
            for (int i = 0; i < tmp.Length; i++)
            {
                msgraw[12 + i] = tmp[i];
            }
            return msgraw;
        }
    }

    public class JsonHandle
    {
        public static Dictionary<string, string> GetDict(string jsonstr)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            JsonTextReader reader = new JsonTextReader(new StringReader(jsonstr));
            while (reader.Read())
            {

                if (reader.TokenType == JsonToken.Integer
                     && reader.Path == "error")
                {
                    int error = int.Parse(reader.Value.ToString());
                    if (error != 0) return null;
                }

                if (reader.Value != null)
                {
                    if (reader.TokenType != JsonToken.PropertyName)
                        dict.Add(reader.Path, reader.Value.ToString());
                }

            }

            return dict;
        }

    }

    public class Function
    {
        public static void PrintDict(Dictionary<string, string> jsonDict)
        {
            foreach (KeyValuePair<string, string> kv in jsonDict)
                Console.Write("key: {0} , value: {1} \n", kv.Key, kv.Value);
        }

        public static ulong GetRidFromUrl(string douyuUrl)
        {
            string resRoomContent = HttpReq.OpenDyRoom(douyuUrl);
            var match = Regex.Match(resRoomContent, @"\$ROOM.room_id\D+(\d+);");
            if (match.Success)
            {
                ulong rid;
                if (ulong.TryParse(match.Groups[1].Value, out rid))
                {
                    return rid;
                }
            }
            return 0;
        }
    }
    public class DyMsg
    {
        public static Dictionary<string, string> GetDict(string str)
        {
            string[] msgarr = str.Split('/');
            Dictionary<string, string> kvdict = new Dictionary<string, string>();
            foreach (string kv in msgarr)
            {
                Regex regex = new Regex("@=");
                string[] kvpair = regex.Split(kv);
                if (kvpair.Length != 2) continue;
                kvdict.Add(kvpair[0], kvpair[1]);
            }
            return kvdict;
        }

        public static string[] GetMsgArray(string str)
        {
            string[] msgarr = str.Split('/');
            return msgarr;
        }
    }



    //https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets?view=netframework-3.5

    public class DySocket
    {
        static string DanmunServer = "openbarrage.douyutv.com";
        static int port = 8601;
        Socket ClientSocket = null;
        long LastTimeStamp = 0;
        public DySocket()
        {
            lock (this)
            {
                ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ClientSocket.Connect(DanmunServer, port);
            }
        }

        public void setTimeout(int second = 30)
        {
            ClientSocket.ReceiveTimeout = second * 1000;
        }

        public void login(ulong roomid)
        {
            string logintext = "type@=loginreq/roomid@=" + roomid.ToString() + "/\0";
            string jointext = "type@=joingroup/rid@=" + roomid.ToString() + "/gid@=-9999/\0";
            byte[] loginraw = DataPack.MakeMsg(689, logintext);
            byte[] joinraw = DataPack.MakeMsg(689, jointext);
            lock (this)
            {
                ClientSocket.Send(loginraw);
                ClientSocket.Send(joinraw);
            }
            LastTimeStamp = CommonTime.GetSeconds();
        }

        public void logout()
        {
            string logouttext = "type@=logout/\0";
            byte[] logoutraw = DataPack.MakeMsg(689, logouttext);
            lock (this)
            {
                ClientSocket.Send(logoutraw);
            }
        }

        public void keeplive(int s = 45)
        {
            long sec = CommonTime.GetSeconds() - LastTimeStamp;
            if (sec < s) return;
            LastTimeStamp = CommonTime.GetSeconds();
            string mrkltext = "type@=mrkl/\0";
            byte[] mrklraw = DataPack.MakeMsg(689, mrkltext);
            lock (this)
            {
                int snum = ClientSocket.Send(mrklraw);
            }
        }

        public byte[] recv(int size = 1024)
        {
            lock (this)
            {
                byte[] raw = new byte[size];
                try
                {
                    ClientSocket.Receive(raw);
                }
                catch (System.Net.Sockets.SocketException eSocket)
                {
                    return null;
                }
                return raw;
            }
        }

        public void release()
        {
            lock (this)
            {
                ClientSocket.Shutdown(SocketShutdown.Both);
                ClientSocket.Close();
            }
        }


    }

    public class DyOpenApiV2
    {
        public static string GetRoomInfoString(ulong rid)
        {
            string api = "http://open.douyucdn.cn/api/RoomApi/room/" + rid.ToString();
            return HttpReq.ReqUrl(api);
        }

        public static int GetRoomStatus(ulong rid)
        {            
            string jsonstr = GetRoomInfoString(rid);
            Dictionary<string, string> dict = JsonHandle.GetDict(jsonstr);
            if (dict == null) return 0;
            if (!dict.ContainsKey("data.room_status")) return 0;
            int status = 0;
            if (int.TryParse(dict["data.room_status"], out status))
            {
                return status;
            }
            return 0;
        }

    }

    public class DyVideo
    {
        public static string GetReqInfoLink(ulong roomid)
        {
            string api_url = "http://www.douyutv.com/api/v1/";
            string args = string.Format(
                "room/{0}?aid=wp&client_sys=wp&time={1}",
                roomid.ToString(),
                CommonTime.GetSeconds().ToString()
                );

            string auth_md5 = args + "zNzMV1y4EMxOHS6I5WKm";
            MD5 md5 = new MD5CryptoServiceProvider();
            string auth_str = BitConverter.ToString
                (md5.ComputeHash(Encoding.UTF8.GetBytes(auth_md5))).Replace("-", "");
            string json_request_url = string.Format(
                "{0}{1}&auth={2}", api_url, args, auth_str.ToLower()
                );
            return json_request_url;
        }

        public static string GetRoomJsonString(ulong roomid)
        {
            string url = GetReqInfoLink(roomid);
            string jsonstr = null;
            using (WebClient client = new WebClient()) {
                client.Headers.Add(
                    "user-agent",
                    "Mozilla/5.0 (iPad; CPU OS 8_1_3 like Mac OS X) AppleWebKit/600.1.4 (KHTML, like Gecko) Version/8.0 Mobile/12B466 Safari/600.1.4"
                    );
                Stream data = client.OpenRead(url);
                StreamReader reader = new StreamReader(data);
                jsonstr = reader.ReadToEnd();
            }
            return jsonstr;
        }

        public static string GetRealRtmp(ulong roomid)
        {
            string room_info = GetRoomJsonString(roomid);
            Dictionary<string, string> jsonDict = JsonHandle.GetDict(room_info);
            if (jsonDict == null) return null;
            string rtmp_url = jsonDict["data.rtmp_url"];
            string rtmp_live = jsonDict["data.rtmp_live"];
            string rtmputl = rtmp_url + "/" + rtmp_live;
            return rtmputl;
        }

        private static Dictionary<Process, FileStream> stDictFs = new Dictionary<Process, FileStream>();

        public static string VideoOperate(ulong roomid,string op,string savepath,out Process proc)
        {
            proc = null;
            string rtmputl = GetRealRtmp(roomid);           
            if (rtmputl == null) return "Rtmp Url Error";
            string proOutString = null;

            string logName = string.Format("FFmpeg.log");
            File.Create(logName).Close();
            

            using (Process cmd = new Process())
            {
                FileStream fs = new FileStream(logName, FileMode.Append);
                stDictFs.Add(cmd, fs);
                proc = cmd;
                string ffplayArgs = null;
                //string reqHeaders = "-headers \"user-agent:Mozilla/5.0 (iPad; CPU OS 8_1_3 like Mac OS X) AppleWebKit/600.1.4 (KHTML, like Gecko) Version/8.0 Mobile/12B466 Safari/600.1.4\r\n\" ";
                switch (op)
                {
                    case "play":
                        System.Environment.SetEnvironmentVariable("SDL_AUDIODRIVER", "directsound");
                        cmd.StartInfo.FileName = "ffplay.exe";
                        ffplayArgs = string.Format("-i {0}",
                            rtmputl);
                        break;
                    case "save":
                        cmd.StartInfo.FileName = "ffmpeg.exe";
                        ffplayArgs = string.Format("-threads 1 -y -re " +
                            "-i {0} " +
                            "-c copy -bsf:a aac_adtstoasc \"{1}\" ",
                             rtmputl, 
                             savepath);
                        break;
                    case null:
                        return "Not Found Command";
                }

                cmd.StartInfo.Arguments = ffplayArgs;
                cmd.StartInfo.UseShellExecute = false;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.RedirectStandardError = true;
                cmd.ErrorDataReceived += Proc_OutDataReceived;
                cmd.OutputDataReceived += Proc_OutDataReceived;
                cmd.StartInfo.CreateNoWindow = true;


                try
                {
                    cmd.Start();
                }
                catch (Exception e)
                {
                    proOutString = "RecordVideo: " + e.Message;
                    proc = null;
                }

                cmd.BeginOutputReadLine();
                cmd.BeginErrorReadLine();
                cmd.WaitForExit();

                fs.Close();
                stDictFs.Remove(cmd);
            }
            return proOutString;
        }

        static void Proc_OutDataReceived(object sender, DataReceivedEventArgs e)
        {
            Process proc = sender as Process;
            string strMessage = e.Data;
            if (!stDictFs.ContainsKey(proc))
            {
                return;
            }

            FileStream fs = stDictFs[proc];
   
            if (strMessage != null && strMessage.Length > 0)
            {
                var wbStr = System.Text.Encoding.Default.GetBytes(strMessage + Environment.NewLine);
                fs.Write(wbStr, 0, wbStr.Length);
            }
        }

    }
}
