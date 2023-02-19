using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearSquare : MonoBehaviour
{
    private GameManager gm;

    private void Awake()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    void OnEnable()
    {
        transform.localScale = new Vector3(0, 0, 1);
        StartCoroutine(Appear());
    }

    IEnumerator Appear()
    {
        float waitTime = 0.8f * gm.GetFixedActionSpeed();
        float exe_times = gm.SetExeTimes(waitTime, 40f);
        Vector3 chanegdScale = new Vector3(120f, 120f, 1.0f);
        Vector3 changedRot = new Vector3(0f, 0f, 360f);
        for (int i = 0; i < exe_times; i++)
        {
            yield return new WaitForSecondsRealtime(waitTime / exe_times);
            transform.localScale += chanegdScale / exe_times;
            transform.eulerAngles += changedRot / exe_times;
        }
    }

    public void SetColor(Color color, int num)
    {
        GetComponent<SpriteRenderer>().color = color;
        var sr = GetComponent<SpriteRenderer>();
        sr.sortingOrder = -100 + num;
    }
}
