using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OandC : MonoBehaviour
{
    [SerializeField] GameObject[] openObjs;
    [SerializeField] GameObject[] closeObjs;

    public void OpenAndClose()
    {
        //�\������I�u�W�F�N�g
        if (openObjs != null)
        {
            foreach (GameObject openObj in openObjs)
            {
                openObj.SetActive(true);
            }
        }

        //��\���ɂ���I�W�F�N�g
        if (closeObjs != null)
        {
            foreach (GameObject closeObj in closeObjs)
            {
                closeObj.SetActive(false);
            }
        }
    }
}
