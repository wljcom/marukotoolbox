// ------------------------------------------------------------------
// Copyright (C) 2011-2015 Maruko Toolbox Project
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
// express or implied.
// See the License for the specific language governing permissions
// and limitations under the License.
// -------------------------------------------------------------------
//

using System;
using System.Collections.Generic;

using System.Text;
using System.Windows.Forms;

namespace ControlExs
{
    /// <summary>
    /// 消息框QQMessageBox 上按钮枚举
    /// </summary>
    public enum QQMessageBoxButtons
    {
        /// <summary>
        /// 消息框包含“确定”按钮
        /// </summary>
        OK,
        /// <summary>
        /// 消息框包含“确定”与“取消”按钮
        /// </summary>
        OKCancel,
        /// <summary>
        /// 消息框包含“重试”与“取消”按钮
        /// </summary>
        RetryCancel
    }
}
