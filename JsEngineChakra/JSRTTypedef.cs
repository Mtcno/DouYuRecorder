using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JsEngineChakra
{
    public struct JsRef
    {
        private readonly IntPtr Handle;

        public JsRef(IntPtr reference)
        {
            Handle = reference;
        }

        public static JsRef Invalid
        {
            get { return new JsRef(IntPtr.Zero); }
        }

        public IntPtr Value
        {
            get { return Handle; }
        }
    }

    public struct JsValueRef
    {
        private readonly IntPtr Handle;

        public JsValueRef(IntPtr reference)
        {
            Handle = reference;
        }

        public static JsValueRef Invalid
        {
            get { return new JsValueRef(IntPtr.Zero); }
        }

        public IntPtr Value
        {
            get { return Handle; }
        }

        public string Text
        {
            get
            {
                return Native.JsValue2String(this);
            }
        }

    }

    public struct JsRuntimeHandle
    {
        private readonly IntPtr Handle;

        public JsRuntimeHandle(IntPtr reference)
        {
            Handle = reference;
        }

        public static JsRuntimeHandle Invalid
        {
            get { return new JsRuntimeHandle(IntPtr.Zero); }
        }

        public IntPtr Value
        {
            get { return Handle; }
        }
    }

    public struct JsContextRef
    {
        private readonly IntPtr Handle;

        public JsContextRef(IntPtr reference)
        {
            Handle = reference;
        }

        public static JsContextRef Invalid
        {
            get { return new JsContextRef(IntPtr.Zero); }
        }

        public IntPtr Value
        {
            get { return Handle; }
        }
    }

    public struct JsSourceContext 
    {
        private readonly IntPtr context;

        public static IntPtr Invalid = IntPtr.Zero;


        private JsSourceContext(IntPtr context)
        {
            this.context = context;
        }

        public static JsSourceContext FromIntPtr(IntPtr cookie)
        {
            return new JsSourceContext(cookie);
        }

        public static JsSourceContext FromInt(int cookie)
        {
            return new JsSourceContext(new IntPtr(cookie));
        }

        public static JsSourceContext FromInt(long cookie)
        {
            return new JsSourceContext(new IntPtr(cookie));
        }

        public static JsSourceContext operator ++(JsSourceContext context)
        {
            if (IntPtr.Size == 8)
            {
                Int64 addnum = 1;
                return FromInt(context.context.ToInt64() + addnum);
            }
            if (IntPtr.Size == 4)
            {
                Int32 addnum = 1;
                return FromInt(context.context.ToInt32() + addnum);
            }
            return FromInt(0);
        }


    }
    
}
