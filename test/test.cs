using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.RegularExpressions;
using DySdk;

//https://docs.microsoft.com/zh-cn/dotnet/api/?view=netframework-3.5

namespace test
{

    class test
    {



        static void Main(string[] args)
        {
            ulong rid = 70231;
            Process proc;
            string msg = DyVideo.VideoOperate(rid, "play", @"C:\Users\Lucky\Videos\out.flv", out proc);
            Console.Write(msg);
            return;

        }
    }
}
