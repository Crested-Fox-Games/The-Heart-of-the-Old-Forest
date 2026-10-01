using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    /// <summary>
    /// The sources for the music that will be played in the game
    /// </summary>
    [SerializeField]
    private AudioSource musicSourceA, musicSourceB;

    private AudioSource activeSource;

    /// <summary>
    /// The time it takes for the music to transition
    /// </summary>
    [SerializeField]
    private float crossfadeDuration = 2f;

    /// <summary>
    /// The music that plays while in the menu
    /// </summary>
    [SerializeField]
    private AudioClip menuMusic;

    /// <summary>
    /// The music that plays while its day time
    /// </summary>
    [SerializeField]
    private AudioClip daytimeMusic;

    /// <summary>
    /// The music that plays while its night time
    /// </summary>
    [SerializeField]
    private AudioClip nighttimeMusic;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        //Makes sure this survives between scenes
        DontDestroyOnLoad(gameObject);

        activeSource = musicSourceA;
    }

    private void Start()
    {
        //Plays day time music if we are in gameplay scene when starting & menu music if we are not in gameplay scene
        if(SceneManager.GetActiveScene().name == "Gameplay")
        {
            if(daytimeMusic != null)
            {
                PlayMusic(daytimeMusic);
            }
        }
        else
        {
            if(menuMusic != null)
            {
                PlayMusic(menuMusic);
            }
        }
    }

    /// <summary>
    /// Handles playing music
    /// </summary>
    /// <param name="clip"></param>
    private void PlayMusic(AudioClip clip)
    {
        if (clip == null)
            return;

        if (activeSource.clip == clip)
            return;

        //Checks whether source A or B is the active one
        AudioSource newSource = activeSource == musicSourceA ? musicSourceA : musicSourceB;
        
        StopAllCoroutines();
        StartCoroutine(Crossfade(newSource, clip));
    }

    /// <summary>
    /// Handles the crossfade
    /// </summary>
    /// <param name="newSource"></param>
    /// <param name="newClip"></param>
    /// <returns></returns>
    private IEnumerator Crossfade(AudioSource newSource, AudioClip newClip)
    {
        //Updates the new sources audio settings
        newSource.clip = newClip;
        newSource.volume = 0f;
        newSource.loop = true;
        newSource.Play();

        float timer = 0f;
        float startingVol = activeSource.volume;

        //Loops for crossfade timer
        while(timer < crossfadeDuration)
        {
            timer += Time.deltaTime;

            float t = timer / crossfadeDuration;

            //Lerps the sources
            activeSource.volume = Mathf.Lerp(startingVol, 0f, t);
            newSource.volume = Mathf.Lerp(0f, startingVol, t);

            yield return null;
        }
        //Stops the faded out music
        activeSource.Stop();
        activeSource.volume = 0f;

        //Ensures the new music is at the correct volume
        newSource.volume = startingVol;

        //Sets the active source as the new source
        activeSource = newSource;

    }
}
