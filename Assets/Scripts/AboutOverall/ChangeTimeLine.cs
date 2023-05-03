using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class ChangeTimeLine : MonoBehaviour
{
    [SerializeField] PlayableDirector playableDirector;//�^�C�����C��
    [SerializeField] PlayableAsset[] songs;//�y��
    [SerializeField] Dropdown dd;
    [SerializeField] NotesGenerator ng;

    public void SetSong()
    {
        playableDirector.playableAsset = songs[dd.value];
        ng.SetNotesMapPath(dd.value);
    }
}
