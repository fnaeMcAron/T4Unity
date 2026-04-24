using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MusicBuff
{
    [Header("Настройки трека и баффа")]
    public string buffName;
    public AudioClip audioClip;

    [Header("Модификаторы характеристик")]
    public float damageMultiplier = 1f;
    public float attackSpeedMultiplier = 1f;
    public float moveSpeedMultiplier = 1f;
}

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    public AudioSource audioSource;

    private MusicBuff currentBuff;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayTrack(MusicBuff buff)
    {
        if (buff == null || buff.audioClip == null) return;

        currentBuff = buff;
        audioSource.clip = buff.audioClip;
        audioSource.Play();

        Debug.Log($"Играет: {buff.buffName}");
    }

    public void StopTrack()
    {
        currentBuff = null;
        audioSource.clip = null;
        audioSource.Stop();

        Debug.Log($"Треки остановлены");
    }

    public MusicBuff GetCurrentBuff()
    {
        return currentBuff;
    }

    public AudioClip GetCurrentTrack()
    {
        return currentBuff?.audioClip;
    }

    public float GetDamageMultiplier()
    {
        return currentBuff?.damageMultiplier ?? 1f;
    }

    public float GetAttackSpeedMultiplier()
    {
        return currentBuff?.attackSpeedMultiplier ?? 1f;
    }

    public float GetMoveSpeedMultiplier()
    {
        return currentBuff?.moveSpeedMultiplier ?? 1f;
    }
}