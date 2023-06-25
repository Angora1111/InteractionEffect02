using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeJudgementCriteria : MonoBehaviour
{
    [SerializeField] Dropdown dd_creteria;
    [SerializeField] GameObject[] distObjs;
    [SerializeField] GameObject[] frame60Objs;
    [SerializeField] GameObject[] frame120Objs;

    // ����̊�i���� or �t���[���j�ɂ���āA�\����ύX����
    public void ChangeCreteria()
    {
        switch(dd_creteria.value)
        {
            // ����
            case 0:
                if(distObjs != null)
                {
                    foreach(var obj in distObjs)
                    {
                        obj.SetActive(true);
                    }
                }
                if (frame60Objs != null)
                {
                    foreach (var obj in frame60Objs)
                    {
                        obj.SetActive(false);
                    }
                }
                if (frame120Objs != null)
                {
                    foreach (var obj in frame120Objs)
                    {
                        obj.SetActive(false);
                    }
                }
                break;

            // �t���[���i60fps�j
            case 1:
                if (distObjs != null)
                {
                    foreach (var obj in distObjs)
                    {
                        obj.SetActive(false);
                    }
                }
                if (frame60Objs != null)
                {
                    foreach (var obj in frame60Objs)
                    {
                        obj.SetActive(true);
                    }
                }
                if (frame120Objs != null)
                {
                    foreach (var obj in frame120Objs)
                    {
                        obj.SetActive(false);
                    }
                }
                break;

            // �t���[���i120fps�j
            case 2:
                if (distObjs != null)
                {
                    foreach (var obj in distObjs)
                    {
                        obj.SetActive(false);
                    }
                }
                if (frame60Objs != null)
                {
                    foreach (var obj in frame60Objs)
                    {
                        obj.SetActive(false);
                    }
                }
                if (frame120Objs != null)
                {
                    foreach (var obj in frame120Objs)
                    {
                        obj.SetActive(true);
                    }
                }
                break;
        }
    }
}
