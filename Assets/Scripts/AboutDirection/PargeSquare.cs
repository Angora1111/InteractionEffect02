using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PargeSquare : MonoBehaviour
{
    private bool keeping = false;
    private bool hasExecuted = false;

    private GameManager gm;

    private void Awake()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        if (keeping)
        {
            transform.eulerAngles += new Vector3(0, 0, -1f) * gm.GetFixedActionSpeed();
        }
    }

    void OnEnable()
    {
        hasExecuted = false;
        transform.localScale = new Vector3(100, 100, 1);
        //transform.localScale = new Vector3(20, 20, 1);
        StartCoroutine(Shrink());
    }

    public void StartShrink()
    {
        transform.localScale = new Vector3(100f, 100f, 1f);
        StartCoroutine(Shrink());
    }

    IEnumerator Shrink()
    {
        float waitTime = 0.3f * gm.GetFixedActionSpeed();
        float exe_times = gm.SetExeTimes(waitTime, 20f);
        Vector3 totalScaleGap = new Vector3(80f, 80f, 1f);
        Vector3 totalRotGap = new Vector3(0f, 0f, 360f);
        for (int i = 0; i < exe_times; i++)
        {
            //既に終点ノーツが来たなら、縮小を止める
            if (hasExecuted) { break; }

            yield return new WaitForSecondsRealtime(waitTime / exe_times);
            transform.localScale -= totalScaleGap / exe_times;
            transform.eulerAngles += totalRotGap / exe_times;
        }

        if (!hasExecuted)//まだ終点のノーツが来ていないなら
        {
            keeping = true;
            yield return new WaitUntil(() => !keeping);
        }
        else
        {
            transform.localScale = new Vector3(20f, 20f, 1f);
        }

        for (int i = 0; i < exe_times; i++)
        {
            yield return new WaitForSecondsRealtime(waitTime / exe_times);
            transform.localScale += totalScaleGap * 1.5f / exe_times;
            transform.eulerAngles += totalRotGap * 1.5f / exe_times;
        }
    }

    public void SetColor(Color color, int num)
    {
        GetComponent<SpriteRenderer>().color = color;
        var sr = GetComponent<SpriteRenderer>();
        sr.sortingOrder = -100 + num;
    }

    public void SetKeepingFalse()
    {
        keeping = false;
        hasExecuted = true;
    }
}
