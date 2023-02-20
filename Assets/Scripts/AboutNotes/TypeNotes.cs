using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeNotes : Notes
{
    protected override void CatchProcess()
    {
        if (JudgementProcess() != EnumData.Judgement.NONE)
        {
            Disappear();
        }
    }

    protected override void JudgeDirection(bool argIsAction = true)
    {
        gm.TypeAction(argIsAction);
    }
}
