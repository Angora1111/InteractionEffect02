using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeJudgementCriteria : MonoBehaviour
{
    [SerializeField] Dropdown dd_creteria;
    [SerializeField] GameObject[] distObjs;
    [SerializeField] GameObject[] frameObjs;

    // 判定の基準（距離 or フレーム）によって、表示を変更する
    public void ChangeCreteria()
    {
        switch(dd_creteria.value)
        {
            // 距離
            case 0:
                if(distObjs != null)
                {
                    foreach(var obj in distObjs)
                    {
                        obj.SetActive(true);
                    }
                }
                if (frameObjs != null)
                {
                    foreach (var obj in frameObjs)
                    {
                        obj.SetActive(false);
                    }
                }
                break;

            // フレーム
            case 1:
                if (distObjs != null)
                {
                    foreach (var obj in distObjs)
                    {
                        obj.SetActive(false);
                    }
                }
                if (frameObjs != null)
                {
                    foreach (var obj in frameObjs)
                    {
                        obj.SetActive(true);
                    }
                }
                break;
        }
    }
}
