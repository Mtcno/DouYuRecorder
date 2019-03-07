using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JsEngineChakra
{
    public enum JsErrorCode : uint
    {
        JsNoError,
        JsErrorCategoryUsage,
        JsErrorInvalidArgument,
        JsErrorNullArgument,
        JsErrorNoCurrentContext,
        JsErrorInExceptionState,
        JsErrorNotImplemented,
        JsErrorWrongThread,
        JsErrorRuntimeInUse,
        JsErrorBadSerializedScript,
        JsErrorInDisabledState,
        JsErrorCannotDisableExecution,
        JsErrorHeapEnumInProgress,
        JsErrorArgumentNotObject,
        JsErrorInProfileCallback,
        JsErrorInThreadServiceCallback,
        JsErrorCannotSerializeDebugScript,
        JsErrorAlreadyDebuggingContext,
        JsErrorAlreadyProfilingContext,
        JsErrorIdleNotEnabled,
        JsCannotSetProjectionEnqueueCallback,
        JsErrorCannotStartProjection,
        JsErrorInObjectBeforeCollectCallback,
        JsErrorObjectNotInspectable,
        JsErrorPropertyNotSymbol,
        JsErrorPropertyNotString,
        JsErrorInvalidContext,
        JsInvalidModuleHostInfoKind,
        JsErrorModuleParsed,
        JsNoWeakRefRequired,
        JsErrorPromisePending,
        JsErrorCategoryEngine,
        JsErrorOutOfMemory,
        JsErrorBadFPUState,
        JsErrorCategoryScript,
        JsErrorScriptException,
        JsErrorScriptCompile,
        JsErrorScriptTerminated,
        JsErrorScriptEvalDisabled,
        JsErrorCategoryFatal,
        JsErrorFatal,
        JsErrorWrongRuntime,
        JsErrorCategoryDiagError,
        JsErrorDiagAlreadyInDebugMode,
        JsErrorDiagNotInDebugMode,
        JsErrorDiagNotAtBreak,
        JsErrorDiagInvalidHandle,
        JsErrorDiagObjectNotFound,
        JsErrorDiagUnableToPerformAction,
    }

    public enum JsRuntimeAttributes
    {
        JsRuntimeAttributeNone,
        JsRuntimeAttributeDisableBackgroundWork,
        JsRuntimeAttributeAllowScriptInterrupt,
        JsRuntimeAttributeEnableIdleProcessing,
        JsRuntimeAttributeDisableNativeCodeGeneration,
        JsRuntimeAttributeDisableEval,
        JsRuntimeAttributeEnableExperimentalFeatures,
        JsRuntimeAttributeDispatchSetExceptionsToDebugger,
    }
}
