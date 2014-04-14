using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;


namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Method Extension .NET 2.0 walkaround
    /// http://msdn.microsoft.com/en-us/library/system.runtime.compilerservices.extensionattribute(v=vs.110).aspx
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Assembly|AttributeTargets.Class|AttributeTargets.Method)]
    public sealed class ExtensionAttribute : Attribute { }
}

namespace mp4box
{
    namespace InvokeEx
    {
        /// <summary>
        /// A wrapper to make Invoke more easy by using Method Extension.
        /// </summary>
        static class Invoker
        {
            public static void InvokeIfRequired(this ISynchronizeInvoke control, MethodInvoker action)
            {
                if (control.InvokeRequired)
                    control.Invoke(action, null);
                else
                    action();
            }
        }
    }
}
