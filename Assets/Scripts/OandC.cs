using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OandC : MonoBehaviour
{
    [SerializeField] GameObject[] openObjs;
    [SerializeField] GameObject[] closeObjs;

    public void OpenAndClose()
    {
        //表示するオブジェクト
        if (openObjs != null)
        {
            foreach (GameObject openObj in openObjs)
            {
                openObj.SetActive(true);
            }
        }

        //非表示にするオジェクト
        if (closeObjs != null)
        {
            foreach (GameObject closeObj in closeObjs)
            {
                closeObj.SetActive(false);
            }
        }
    }
}
