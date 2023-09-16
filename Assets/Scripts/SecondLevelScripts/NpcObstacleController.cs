using LivelyChatBubbles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcObstacleController : MonoBehaviour
{
    //Animation ID
    private int animIDSaving;

    private Animator animator;

    [SerializeField]
    private bool isSave;

    [SerializeField]
    private int subtractMin;
    [SerializeField]
    private int subtractSec;

    [Header("Particles")]
    [SerializeField]
    private ParticleSystem[] particleSystems;

    [SerializeField]
    [Range(0.0f, 1f)]
    private float xPosVariation;

    [SerializeField]
    [Range(0.0f, 1f)]
    private float yPosVariation;

    [SerializeField]
    [Range(0.0f, 1f)]
    private float zPosVariation;

    [Header("LivelyChatBubbles")]
    public ChatMouthpiece chatBubble;
    [SerializeField]
    private string[] chatTexts;

    public bool IsSave
    {
        get { return isSave; }
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animIDSaving = Animator.StringToHash("isSave");

        if (particleSystems.Length > 0)
            StartCoroutine(PlayingParticles());
    }

    public void Save()
    {
        if (!isSave)
        {
            if (chatBubble != null && chatTexts.Length > 0)
                chatBubble.Speak(chatTexts[Random.Range(0, chatTexts.Length)]);
            animator.SetBool(animIDSaving, true);
            isSave = true;
        }
    }

    public void OnThankingEnd()
    {
        Timer.SubtractTimer(subtractMin, subtractSec);
    }

    IEnumerator PlayingParticles()
    {
        while (!isSave)
        {
            ParticleSystem particle = particleSystems[Random.Range(0, particleSystems.Length)];
            if (particle.isStopped)
            {
                particle.gameObject.transform.position = transform.position + new Vector3(Random.Range(-xPosVariation, xPosVariation), 
                                                                                          1 + Random.Range(0f, yPosVariation), 
                                                                                          Random.Range(-zPosVariation, zPosVariation));
                particle.Play();
            }
            yield return new WaitForSeconds(Random.Range(0.7f, 1f));
        }
    }
}
