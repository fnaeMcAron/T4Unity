using System.Collections;
using TMPro;
using UnityEngine;

public class YoukiController : CharacterBase
{
    [Header("Музыкальные баффы Еки")]
    public MusicBuff[] availableTracks;

    [Header("Настройки Еки")]
    public WeaponRaycast raycast;
    public TMP_Text songText;
    public GameObject defaultBuffEffect;
    public CharacterBase[] allCharacters;

    private int currentTrackIndex = 0;
    private MusicManager musicManager;

    void Start()
    {
        musicManager = MusicManager.Instance;
    }

    private void OnWeaponChanged(int newWeaponIndex)
    {
        Debug.Log($"Еки сменил оружие на: {weaponSlots[newWeaponIndex].slotName}");

        // Дополнительная логика при смене оружия для Еки
        switch (newWeaponIndex)
        {
            case 0: // Ножи
                Debug.Log("Еки готов к ближнему бою!");
                break;
            case 1: // Револьвер
                Debug.Log("Еки готов к дальнему бою!");
                break;
        }
    }

    public override void PerformMeleeAttack()
    {
        Debug.Log($"Еки: атака ножами в ближнем бою");
        PlayKnifeAttack();
    }

    public override void PerformRangedAttack()
    {
        Debug.Log("Еки: выстрел из револьвера");
        ShootRevolver();
    }

    public override void UseAbility(bool isHold)
    {
        if (isHold)
        {
            Debug.Log("Еки: смена музыки и применение баффов");
            SwitchMusicTrack();
            ApplyAreaBuffToAllPlayers();
        }
        else
        {
            musicManager.StopTrack();
            CancelBuffsToAllPlayers();
        }
    }

    public override void Dodge()
    {
        Debug.Log("Еки: уворот");
    }

    private void PlayKnifeAttack()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject knife = GameObject.CreatePrimitive(PrimitiveType.Cube);
            knife.transform.position = transform.position + transform.right * (i == 0 ? -0.5f : 0.5f) + transform.forward;
            knife.transform.localScale = new Vector3(0.1f, 0.1f, 0.3f);
            knife.GetComponent<Renderer>().material.color = Color.cyan;
            Destroy(knife, 0.3f);
        }
    }

    private void ShootRevolver()
    {
        if (raycast != null)
        {
            raycast.Shoot();
        }
        else
        {
            Debug.LogWarning("WeaponRaycast не назначен для Еки!");
        }
    }

    private void SwitchMusicTrack()
    {
        int nextTrackIndex = (currentTrackIndex + 1) % availableTracks.Length;
        PlayTrack(nextTrackIndex);
    }

    private void PlayTrack(int trackIndex)
    {
        if (musicManager == null || availableTracks.Length == 0) return;

        currentTrackIndex = trackIndex;
        MusicBuff selectedBuff = availableTracks[currentTrackIndex];

        musicManager.PlayTrack(selectedBuff);
        songText.text = $"Now playing:\n{selectedBuff.buffName}";
    }

    private void ApplyAreaBuffToAllPlayers()
    {
        if (availableTracks.Length == 0) return;

        MusicBuff currentBuff = availableTracks[currentTrackIndex];
        Debug.Log($"Пытаюсь применить бафф: {currentBuff.buffName}");

        int buffsApplied = 0;
        foreach (CharacterBase character in allCharacters)
        {
            if (character != null)
            {
                character.ApplyMusicBuff(currentBuff);
                buffsApplied++;
            }
        }
        Debug.Log($"Бафф применен к {buffsApplied} персонажам");
    }

    private void CancelBuffsToAllPlayers()
    {
        if (availableTracks.Length == 0) return;

        foreach (CharacterBase character in allCharacters)
        {
            if (character != null)
            {
                character.ResetBuffs();
            }
        }
        Debug.Log($"Баффы сняты");
        songText.text = $"Now playing:\n";
    }

    public void PlaySpecificTrack(int trackIndex)
    {
        if (trackIndex >= 0 && trackIndex < availableTracks.Length)
        {
            PlayTrack(trackIndex);
        }
    }

    public MusicBuff GetCurrentTrackInfo()
    {
        if (availableTracks.Length > 0 && currentTrackIndex < availableTracks.Length)
        {
            return availableTracks[currentTrackIndex];
        }
        return null;
    }

    public string GetCurrentTrackName()
    {
        MusicBuff current = GetCurrentTrackInfo();
        return current != null ? current.buffName : "No track";
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.3f, 0.5f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position, 8f);
    }
}