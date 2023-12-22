using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputWindow2 : InputWindow
{
    [SerializeField] bool notLineUp = false;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    protected override void SetObjects()
    {
        if (notLineUp) return;
        base.SetObjects();
    }

    public void SetInteractable(bool setBool)
    {
        if (transform.childCount == 0) return;

        foreach(Transform child in transform)
        {
            if(child.gameObject.TryGetComponent<InputField>(out var inf))
            {
                inf.interactable = setBool;
            }
        }
    }
}
