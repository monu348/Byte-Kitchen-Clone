using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private const string PLAYER_PREFS_SOUND_EFEECTS_VOLUME = "SoundEffectsVolume";
    [SerializeField] private AudioClipsRefsSO audioClipsRefsSO;
    public static SoundManager Instance{get; private set;}
    private float volume = 8f;
    private void Awake()
    {
        Instance = this;
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFEECTS_VOLUME, volume = 1f);
    }
    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        Player.Instance.OnPickedSomething += Player_OnPickedSomething;
        BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnAnyObjectPlacedHere;
        TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
    }

    private void TrashCounter_OnAnyObjectTrashed(object sender, System.EventArgs e)
    {
        TrashCounter trashCounter = sender as TrashCounter;
        PlaySound(audioClipsRefsSO.trash, trashCounter.transform.position);
    }

    private void BaseCounter_OnAnyObjectPlacedHere(object sender, System.EventArgs e)
    {
        BaseCounter baseCounter = sender as BaseCounter;
        PlaySound(audioClipsRefsSO.objectDrop,baseCounter.transform.position);
    }

    private void Player_OnPickedSomething(object sender, System.EventArgs e)
    {
        PlaySound(audioClipsRefsSO.objectPickup,Player.Instance.transform.position);
    }

    private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e)
    {
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlaySound(audioClipsRefsSO.chop, cuttingCounter.transform.position);
    }

    private void DeliveryManager_OnRecipeFailed(object sender, System.EventArgs e)
    {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(audioClipsRefsSO.deliveryFail,deliveryCounter.transform.position);
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e)
    {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(audioClipsRefsSO.deliverySuccess,deliveryCounter.transform.position);
    }

    private void PlaySound(AudioClip[] audioClipArray , Vector3 position , float volume=4f)
    {
        // the randomness isnt among all the sounds but rather the arrays , such as we have chop1 , chop2 , chop3 so we want the chopping sound to be random
        AudioSource.PlayClipAtPoint(audioClipArray[Random.Range(0,audioClipArray.Length)] , position , volume );
    }
    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 4f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier*volume);
    }
    public void PlayFootstepsSound(Vector3 position, float volume)
    {
        PlaySound(audioClipsRefsSO.footstep,position,volume);
    }
    public void PlayCountDownSound()
    {
        PlaySound(audioClipsRefsSO.warning, Vector3.zero);
    }
    public void PlayWarningSound(Vector3 position)
    {
        PlaySound(audioClipsRefsSO.warning, position);
    }
    public void ChangeVolume()
    {
        volume += 0.1f;
        if (volume > 4f)
        {
            volume = 0f;
        }
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFEECTS_VOLUME,volume);
        PlayerPrefs.Save();
    }
    public float GetVolume()
    {
        return volume;
    }
}
