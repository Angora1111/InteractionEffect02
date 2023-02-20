using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikeNoteManager : MonoBehaviour
{
    const int LANE_NUM = 1;// レーンの数

    static bool[] isStruck;// 1フレーム内で、レーンごとにノーツ判定処理がなされたかどうか

    void Start()
    {
        isStruck = new bool[LANE_NUM];
        for (int i = 0; i < LANE_NUM; i++)
        {
            isStruck[i] = false;
        }
    }

    void Update()
    {
        // 毎フレームフラグをリセットする
        for (int i = 0; i < LANE_NUM; i++)
        {
            isStruck[i] = false;
        }
    }

    /// <summary>
    /// 1フレーム内で、レーンごとにノーツ判定処理がなされたかどうかを表すフラグを true にする
    /// </summary>
    /// <param name="argLaneNum">true にするレーン番号</param>
    public static void SetIsStruck(int argLaneNum)
    {
        int num = argLaneNum;
        // インデックスの範囲外なら終了する
        if (num < 0 || num > LANE_NUM) { return; }
        // 指定したレーンのisStruckを true にする
        isStruck[num] = true;
    }

    /// <summary>
    /// 1フレーム内で、レーンごとにノーツ判定処理がなされたかどうかを表すフラグを取得する
    /// </summary>
    /// <param name="argLaneNum">取得するレーン番号</param>
    /// <returns></returns>
    public static bool GetIsStruck(int argLaneNum)
    {
        int num = argLaneNum;
        // インデックスの範囲外なら終了する
        if (num < 0 || num > LANE_NUM) { return false; }
        // 指定したレーンのisStruckを true にする
        return isStruck[num];
    }
}
