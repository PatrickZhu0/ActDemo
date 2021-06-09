/**
 * @file MathUtil.cs
 * @brief 数学工具
 *
 * @author lixiaojiang
 * @version 0.0.1
 * @date 2012-12-12
 */

using System;
using System.Collections.Generic;

using UnityEngine;

namespace StarWars
{
    public class Geometry3D
    {
        public static Vector3 GetCenter(Vector3 fvPos1, Vector3 fvPos2)
        {
            Vector3 fvRet = new Vector3();

            fvRet.x = (fvPos1.x + fvPos2.x) / 2.0f;
            fvRet.y = (fvPos1.y + fvPos2.y) / 2.0f;
            fvRet.z = (fvPos1.z + fvPos2.z) / 2.0f;

            return fvRet;
        }
    }
}

