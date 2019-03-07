using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace JsEngineChakra
{
    public delegate void JavaScriptBackgroundWorkItemCallback(IntPtr callbackData);
    public delegate bool JsThreadServiceCallback(JavaScriptBackgroundWorkItemCallback callbackFunction, IntPtr callbackData);

    public static class Native
    {
        const string DllName = "ChakraCore.dll";

        static bool bInit = false;
        // C# 设置环境变量，检查 32 、64 dll;
        public static bool InitChakra()
        {
            if (bInit) return bInit;
            string x86path = Directory.GetCurrentDirectory() + "\\x86" ;
            string x64path = Directory.GetCurrentDirectory() + "\\x64" ;
            if (!File.Exists(DllName))
            {
                if (File.Exists(x86path + "\\" + DllName) &&
                    IntPtr.Size == 4
                    )
                {
                    var syspath = Environment.GetEnvironmentVariable("path");
                    Environment.SetEnvironmentVariable("path", x86path + ";" + syspath);
                    bInit = true;
                }
                if (File.Exists(x64path + "\\" + DllName) &&
                    IntPtr.Size == 8
                    )
                {
                    var syspath = Environment.GetEnvironmentVariable("path");
                    Environment.SetEnvironmentVariable("path", x64path + ";" + syspath);
                    bInit = true;
                }
            }
            else
            {
                bInit = true;
            }
            return bInit;
        }

        [DllImport(DllName)]
        public static extern JsErrorCode JsCreateRuntime(JsRuntimeAttributes attributes, JsThreadServiceCallback threadService, out JsRuntimeHandle runtime);

        [DllImport(DllName)]
        public static extern JsErrorCode JsCreateContext(JsRuntimeHandle runtime, out JsContextRef context);

        [DllImport(DllName)]
        public static extern JsErrorCode JsSetCurrentContext(JsContextRef context);

        [DllImport(DllName, CharSet = CharSet.Unicode)]
        public static extern JsErrorCode JsRunScript(string script, JsSourceContext sourceContext, string sourceUrl, out JsValueRef result);

        [DllImport(DllName)]
        public static extern JsErrorCode JsConvertValueToString(JsValueRef value, out JsValueRef stringValue);

        [DllImport(DllName)]
        public static extern JsErrorCode JsConvertValueToString(IntPtr value, out IntPtr stringValue);

        [DllImport(DllName)]
        public static extern JsErrorCode JsCopyString(JsValueRef value, out byte[] stringValue, IntPtr bufferSize, out IntPtr bytesCount);

        [DllImport(DllName)]
        public static extern JsErrorCode JsCopyString(IntPtr value, out byte[] stringValue, IntPtr bufferSize, out IntPtr bytesCount);

        [DllImport(DllName)]
        public static extern JsErrorCode JsStringToPointer(JsValueRef value, out JsValueRef stringValue, out IntPtr stringLength);

        [DllImport(DllName)]
        public static extern JsErrorCode JsStringToPointer(IntPtr value, out IntPtr stringValue, out IntPtr stringLength);

        [DllImport(DllName)]
        public static extern JsErrorCode JsDisposeRuntime(JsRuntimeHandle runtime);

        [DllImport(DllName)]
        public static extern JsErrorCode JsGetAndClearException(out JsValueRef exception);

        [DllImport(DllName)]
        public static extern JsErrorCode JsGetAndClearException(out IntPtr exception);

        public static string JsValue2String(IntPtr JsValue)
        {
            IntPtr ResultString;
            IntPtr ResultStringPtr;
            if (JsErrorCode.JsNoError != JsConvertValueToString(JsValue, out ResultString)) return null;
            IntPtr bytesCount = IntPtr.Zero;
            if (JsErrorCode.JsNoError != JsStringToPointer(ResultString, out ResultStringPtr, out bytesCount)) return null;
            return Marshal.PtrToStringUni(ResultStringPtr);
        }

        public static string JsValue2String(JsValueRef Result)
        {
            JsValueRef ResultString;
            JsValueRef ResultStringPtr;
            if (JsErrorCode.JsNoError != JsConvertValueToString(Result, out ResultString)) return null;
            IntPtr bytesCount = IntPtr.Zero;
            if (JsErrorCode.JsNoError != JsStringToPointer(ResultString, out ResultStringPtr, out bytesCount)) return null;
            return Marshal.PtrToStringUni(ResultStringPtr.Value);
        }


    }

    public class ChakraHost
    {
        JsRuntimeHandle ChakraRuntime;
        JsContextRef ChakraContext;
        JsErrorCode ChakraErrorCode;
        JsSourceContext ChakraSourceContext = JsSourceContext.FromIntPtr(IntPtr.Zero);
        JsValueRef ChakraResult;
        JsValueRef ChakraException = JsValueRef.Invalid;

        public JsValueRef Result
        {
            get
            {
                return ChakraResult;
            }
        }

        public JsErrorCode ErrorValue
        {
            get
            {
                return ChakraErrorCode;
            }
        }

        public string ErrorString
        {
            get
            {
                return JSRTString.JsGetErrCodeString(ChakraErrorCode);
            }
        }

        public JsValueRef Exception
        {
            get
            {                
                return ChakraException;
            }
        }

        public ChakraHost()
        {
            if (!Native.InitChakra())
            {
                return;
            }

            ChakraErrorCode = Native.JsCreateRuntime(JsRuntimeAttributes.JsRuntimeAttributeNone, null, out ChakraRuntime);
            if (JsErrorCode.JsNoError != ChakraErrorCode)
            {
                JsGetException();
                return;
            }

            ChakraErrorCode = Native.JsCreateContext(ChakraRuntime, out ChakraContext);
            if (JsErrorCode.JsNoError != ChakraErrorCode)
            {
                JsGetException();
                return;
            }

            ChakraErrorCode = Native.JsSetCurrentContext(ChakraContext);
            if (JsErrorCode.JsNoError != ChakraErrorCode)
            {
                JsGetException();
                return;
            }

        }

        public bool RunScript(string script)
        {
            if (ChakraErrorCode != JsErrorCode.JsNoError) return false;
            ChakraErrorCode = Native.JsRunScript(
                script,
                ChakraSourceContext++,
                "", 
                out ChakraResult
                );
            if (JsErrorCode.JsNoError != ChakraErrorCode)
            {
                JsGetException();
                return false;
            }
            return true;
        }

        ~ChakraHost()
        {
            Native.JsSetCurrentContext(JsContextRef.Invalid);
            Native.JsDisposeRuntime(ChakraRuntime);
        }

        private void JsGetException() {
            Native.JsGetAndClearException(out ChakraException);
        }
    }


    

}
