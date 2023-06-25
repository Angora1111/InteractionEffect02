using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeButton : MonoBehaviour
{
    [SerializeField] bool changable = false;
    [SerializeField] GameManager.ChangeMode mode = GameManager.ChangeMode.NONE;
    [SerializeField] GameManager.EffectModeType changedMode_Type = GameManager.EffectModeType.NONE;
    [SerializeField] GameManager.EffectModeHold changedMode_Hold = GameManager.EffectModeHold.NONE;
    private string name_changedMode_Type;
    private string name_changedMode_Hold;
    private Button button;

    private void OnValidate()
    {
        // enumデータが編集されても、人間から見て同じものに設定し直す
        if (!changable)
        {
            for (int i = 0; i < (int)GameManager.EffectModeType.MAX; i++)
            {
                if (changedMode_Type.ToString() != name_changedMode_Type)
                {
                    changedMode_Type++;
                    if (changedMode_Type >= GameManager.EffectModeType.MAX)
                    {
                        changedMode_Type = GameManager.EffectModeType.NONE;
                    }
                }
                else { break; }
            }
            for (int i = 0; i < (int)GameManager.EffectModeHold.MAX; i++)
            {
                if (changedMode_Hold.ToString() != name_changedMode_Hold)
                {
                    changedMode_Hold++;
                    if (changedMode_Hold >= GameManager.EffectModeHold.MAX)
                    {
                        changedMode_Hold = GameManager.EffectModeHold.NONE;
                    }
                }
                else { break; }
            }
        }
        name_changedMode_Type = changedMode_Type.ToString();
        name_changedMode_Hold = changedMode_Hold.ToString();
    }

    private void Start()
    {
        button = GetComponent<Button>();
    }

    public void SetMode()
    {
        switch (mode)
        {
            case GameManager.ChangeMode.TYPE:
                GameManager.effectMode_Type[GameManager.selectingEffectIndex] = changedMode_Type;
                break;
            case GameManager.ChangeMode.HOLD:
                GameManager.effectMode_Hold[GameManager.selectingEffectIndex] = changedMode_Hold;
                break;
        }
    }

    private void Update()
    {
        switch (mode)
        {
            case GameManager.ChangeMode.TYPE:
                if (GameManager.effectMode_Type[GameManager.selectingEffectIndex] == changedMode_Type)
                {
                    button.interactable = false;
                }
                else
                {
                    button.interactable = true;
                }
                break;
            case GameManager.ChangeMode.HOLD:
                if (GameManager.effectMode_Hold[GameManager.selectingEffectIndex] == changedMode_Hold)
                {
                    button.interactable = false;
                }
                else
                {
                    button.interactable = true;
                }
                break;
        }
    }

    public GameManager.ChangeMode Getmode()
    {
        return mode;
    }
}
