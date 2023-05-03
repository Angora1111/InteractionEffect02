using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CommonData
{
    // *** 判定に関するデータ *** ------------------------------------------------
    // 判定の距離
    /// <summary>
    /// PERFECTの距離
    /// </summary>
    public static float perfectDist = 2f;
    /// <summary>
    /// GOODの距離
    /// </summary>
    public static float goodDist = 6f;
    /// <summary>
    /// MISSの距離（判定される最大距離）
    /// </summary>
    public static float missDist = 8f;
    /// <summary>
    /// ノーツが消滅する距離
    /// </summary>
    public static float neglectDist = 10f;
}
