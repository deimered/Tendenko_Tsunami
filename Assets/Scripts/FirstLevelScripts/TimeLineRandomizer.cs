using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeLineRandomizer : MonoBehaviour
{
    //Componenete PlayableDirector.
    private PlayableDirector director;

    //As Timelines a serem escolhidas.
    public PlayableAsset[] timelines;

    //Usar a Timeline durante o Awake
    public bool playOnAwake;

    // Start is called before the first frame update
    void Awake()
    {
        director = GetComponent<PlayableDirector>();
        if (timelines.Length > 0)
        {
            director.playableAsset = timelines[Random.Range(0, timelines.Length)];
            if (playOnAwake)
                director.Play();
        }
    }
}
