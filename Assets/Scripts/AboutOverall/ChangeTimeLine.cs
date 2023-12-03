using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class ChangeTimeLine : MonoBehaviour
{
    [System.Serializable]
    public class SongData
    {
        public PlayableAsset song;
        public float timeOffset;
    }

    [SerializeField] PlayableDirector playableDirector;//�^�C�����C��
    [SerializeField] SongData[] songs;//�y��
    [SerializeField] Dropdown dd;
    [SerializeField] NotesGenerator ng;

    public void SetSong()
    {
        playableDirector.playableAsset = songs[dd.value].song;
        ng.SetNotesMapPath(dd.value);
        NotesGenerator.TimeOffset = songs[dd.value].timeOffset;
    }
}
