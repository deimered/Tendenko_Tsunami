using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    //Amplitude do ru�do de Perlin.
    [SerializeField]
    private float amplitudeGain = 1;
    //Frequ�ncia do ru�do de Perlin.
    [SerializeField]
    private float frequencyGain = 1;

    //C�mera virtual.
    CinemachineVirtualCamera vcam;
    //Canal de ru�do da c�mera virtual.
    CinemachineBasicMultiChannelPerlin channelPerlin;

    private void Awake()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        channelPerlin = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    //Fun��o para movimentar a c�mera com o ru�do de Perlin.
    public IEnumerator Shake(float duration)
    {
        channelPerlin.m_AmplitudeGain = amplitudeGain;
        channelPerlin.m_FrequencyGain = frequencyGain;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        channelPerlin.m_AmplitudeGain = 0;
        channelPerlin.m_FrequencyGain = 0;
    }
}
