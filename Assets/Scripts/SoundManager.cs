using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;
    public AudioSource audioSource;               // AudioSource for sound effects
    public AudioSource BGaudioSource;             

    public AudioClip AmbientHospitalNoise;
    public AudioClip WomanWalking;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        audioSource = gameObject.AddComponent<AudioSource>();    // For sound effects
        PlayBackgroundMusic(AmbientHospitalNoise);
    }
    public void PlaySound(AudioClip clip)
    {
            if (clip != null)
            {
            audioSource.clip = clip;
            audioSource.loop = false;  // Ensure it's not looped
                audioSource.Play();
            }
    }
    
        public IEnumerator PlaySoundLoop(AudioClip clip, float delayBetweenLoops)
        {
            if (clip != null)
            {
                    audioSource.clip = clip;
                    audioSource.Play();

                    // Wait for the clip to finish playing
                    yield return new WaitForSeconds(clip.length);

                    // Wait for the additional delay between loops
                    yield return new WaitForSeconds(delayBetweenLoops);
            }
        }
        public void StopSound()
        {
            audioSource.Stop();
        }
        public bool IsPlaying()
        {
            return audioSource.isPlaying;
        }
        public void PlayBackgroundMusic(AudioClip musicClip)
        {
        if (BGaudioSource.isPlaying)
        {
            BGaudioSource.clip = AmbientHospitalNoise;
            BGaudioSource.Play();
        }
        }
    }
