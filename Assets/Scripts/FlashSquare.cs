using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashSquare : MonoBehaviour
{
    Color originalColor = default;
    [SerializeField] Image colorPreview;
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
        originalColor = colorPreview.color;

        StartCoroutine(Action());
    }

    IEnumerator Action()
    {
        waitTime = 0.15f * gm.GetFixedActionSpeed();
        float exe_times = gm.SetExeTimes(waitTime, 10f);
        Debug.Log($"é¿çsâÒêîÅF{exe_times}");
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
