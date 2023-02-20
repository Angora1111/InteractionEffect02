using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class ChangeTimeLine : MonoBehaviour
{
    [SerializeField] PlayableDirector playableDirector;//タイムライン
    [SerializeField] PlayableAsset[] songs;//楽曲
    [SerializeField] Dropdown dd;

    public void SetSong()
    {
        playableDirector.playableAsset = songs[dd.value];
    }
}
