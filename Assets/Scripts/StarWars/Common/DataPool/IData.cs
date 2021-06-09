
/**
 * @file IData.cs
 * @brief 数据接口
 *
 * @author
 * @version
 * @date
 */

using System;
using System.Collections.Generic;
//using System.Diagnostics;

namespace StarWars
{

    /// <summary>
    /// 数据接口
    /// </summary>
    public interface IData
  {
        /// <summary>
        /// 提取数据
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        bool CollectDataFromDBC(DBC_Row node);

        /// <summary>
        /// 获取数据ID
        /// </summary>
        /// <returns></returns>
        int GetId();
  }
}
