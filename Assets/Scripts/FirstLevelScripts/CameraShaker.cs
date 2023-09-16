using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    //Amplitude do ruído de Perlin.
    [SerializeField]
    private float amplitudeGain = 1;
    //Frequência do ruído de Perlin.
    [SerializeField]
    private float frequencyGain = 1;

    //Câmera virtual.
    CinemachineVirtualCamera vcam;
    //Canal de ruído da câmera virtual.
    CinemachineBasicMultiChannelPerlin channelPerlin;

    private void Awake()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        channelPerlin = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    //Função para movimentar a câmera com o ruído de Perlin.
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
