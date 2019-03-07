using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JsEngineChakra
{
    public class JSRTString
    {
        public static string JsGetErrCodeString(JsErrorCode err)
        {
            if ((uint)err < JsErrorCodeString.Length)
                return JsErrorCodeString[(uint)err];
            return "Unknow error code.";
        }

        static string[] JsErrorCodeString = {
             "Success error code.",
             "Category of errors that relates to incorrect usage of the API itself.",
             "An argument to a hosting API was invalid.",
             "An argument to a hosting API was null in a context where null is not allowed.",
             "The hosting API requires that a context be current, but there is no current context.",
             "The engine is in an exception state and no APIs can be called until the exception is cleared.",
             "A hosting API is not yet implemented.",
             "A hosting API was called on the wrong thread.",
             "A runtime that is still in use cannot be disposed.",
             "A bad serialized script was used, or the serialized script was serialized by a different version of the Chakra engine.",
             "The runtime is in a disabled state.",
             "Runtime does not support reliable script interruption.",
             "A heap enumeration is currently underway in the script context.",
             "A hosting API that operates on object values was called with a non-object value.",
             "A script context is in the middle of a profile callback.",
             "A thread service callback is currently underway.",
             "Scripts cannot be serialized in debug contexts.",
             "The context cannot be put into a debug state because it is already in a debug state.",
             "The context cannot start profiling because it is already profiling.",
             "Idle notification given when the host did not enable idle processing.",
             "The context did not accept the enqueue callback.",
             "Failed to start projection.",
             "The operation is not supported in an object before collect callback.",
             "Object cannot be unwrapped to IInspectable pointer.",
             "A hosting API that operates on symbol property ids but was called with a non-symbol property id. The error code is returned by JsGetSymbolFromPropertyId if the function is called with non-symbol property id.",
             "A hosting API that operates on string property ids but was called with a non-string property id. The error code is returned by existing JsGetPropertyNamefromId if the function is called with non-string property id.",
             "Module evaluation is called in wrong context.",
             "The Module HostInfoKind provided was invalid.",
             "Module was parsed already when JsParseModuleSource is called.",
             "Argument passed to JsCreateWeakReference is a primitive that is not managed by the GC. No weak reference is required, the value will never be collected.",
             "The Promise object is still in the pending state.",
             "Category of errors that relates to errors occurring within the engine itself.",
             "The Chakra engine has run out of memory.",
             "The Chakra engine failed to set the Floating Point Unit state.",
             "Category of errors that relates to errors in a script.",
             "A JavaScript exception occurred while running a script.",
             "JavaScript failed to compile.",
             "A script was terminated due to a request to suspend a runtime.",
             "A script was terminated because it tried to use eval or function and eval was disabled.",
             "Category of errors that are fatal and signify failure of the engine.",
             "A fatal error in the engine has occurred.",
             "A hosting API was called with object created on different javascript runtime.",
             "Category of errors that are related to failures during diagnostic operations.",
             "The object for which the debugging API was called was not found.",
             "The debugging API can only be called when VM is in debug mode.",
             "The debugging API can only be called when VM is at a break.",
             "Debugging API was called with an invalid handle.",
             "The object for which the debugging API was called was not found.",
             "VM was unable to perform the request action.",
        };
    }
}
