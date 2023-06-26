using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashSquare : MonoBehaviour
{
    Color originalColor = default;
    private SpriteRenderer sr;
    private GameManager gm;
    private float waitTime;

    private void Awake()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnEnable()
    {
        sr = transform.GetComponent<SpriteRenderer>();
        originalColor = default;

        StartCoroutine(Action());
    }

    /// <summary>
    /// 色を変更する
    /// </summary>
    /// <param name="argColor"></param>
    public void ChangeColor(Color argColor)
    {
        originalColor = argColor;
    }

    IEnumerator Action()
    {
        // 色が設定されるまで待つ
        yield return new WaitUntil(() => originalColor != default);

        waitTime = 0.15f * gm.GetFixedActionSpeed();
        float exe_times = gm.SetExeTimes(waitTime, 10f);
        //Debug.Log($"実行回数：{exe_times}");
        for (int i = 1; i <= exe_times; i++)
        {
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, (float)i / exe_times);
            yield return new WaitForSecondsRealtime(waitTime / exe_times);
        }
        for (int i = (int)exe_times; i >= 0; i--)
        {
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, (float)i / exe_times);
            yield return new WaitForSecondsRealtime(waitTime / exe_times);
        }

        gameObject.SetActive(false);
    }
}
