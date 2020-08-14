using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/* Managing sound and sound transitions */
public class SoundManager : MonoBehaviour {

    public static SoundManager core;

    [Tooltip("The ID of the main menu music in the Database.")]
    public string mainMenuMusicID;

    [Tooltip("Should fading be enabled?")]
    public bool fade;

    [Tooltip("Audio fade speed. Suggested to keep at 0.001 - 0.01.")]
    public float fadeSpeed = 0.001f;
    
    public List<AudioInfo> nowPlaying = new List<AudioInfo>();

    public void RestartAudio(List<AudioInfo> b) {
        
        foreach (AudioInfo a in new List<AudioInfo>(nowPlaying)) {
            
            if (!b.Any(x => x.audioclipID == a.audioclipID)) {
                nowPlaying.Remove(a);
                a.busy = false;
                a.stopping = false;
                Destroy(a.audioSource);
            }

        }

        foreach (AudioInfo toPlay in b) {

            if (!nowPlaying.Any(x => x.audioclipID == toPlay.audioclipID)) {
                toPlay.busy = false;
                StartCoroutine(PlayAudio(toPlay));
            }
            
        }

    }

    public void StopAll (bool stopAll, AudioInfo.audioTypeEnum category, bool forceStop, List<AudioInfo> toKeep) {

        var toStop = new List<AudioInfoCompact>();

        foreach (AudioInfo a in new List<AudioInfo>(nowPlaying)) {

            //Making sure that we do not stop any audio from the toKeep list
            if (!toKeep.Any(x => x.audioclipID == a.audioclipID)) {

                if (stopAll || category == a.audioType) {
                    var aic = new AudioInfoCompact();
                    aic.delay = 0f;
                    aic.audioclipID = a.audioclipID;

                    toStop.Add(aic);
                }

            }

        }

        foreach (AudioInfoCompact a in toStop) {
            StartCoroutine(StopAudio(a, forceStop));
        }

    }

    
    public IEnumerator PlayAudio (AudioInfo a) {

        if (!nowPlaying.Any(x => x.audioclipID == a.audioclipID)) {

            if (!nowPlaying.Contains(a)) {
                nowPlaying.Add(a);
            }
            
            while (a.busy) {
                yield return new WaitForEndOfFrame();
            }

            a.busy = true;

            yield return new WaitForSeconds(a.delay);

            var audioClip = getAudioclipById(a.audioclipID);

            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.loop = a.loop;
            audioSource.volume = 0;
            a.audioSource = audioSource;

            if (fade && !SkipAuto.core.skip) {

                if (audioSource != null) {

                    var prevVolume = audioSource.volume;
                    audioSource.volume = 0;
                    audioSource.Play();

                    while (audioSource != null && audioSource.volume < getVolume(a) && a.busy) {
                        audioSource.volume += fadeSpeed;
                        yield return new WaitForEndOfFrame();
                    }

                }

            } else {
                audioSource.volume = getVolume(a);
                audioSource.Play();
            }

        }

    }

    public IEnumerator StopAudio (AudioInfoCompact a, bool forceStop) {

        //Getting all instances of audio
        var playingAll = nowPlaying.FindAll(x => x.audioclipID == a.audioclipID);

        foreach (AudioInfo playing in playingAll) {

            //Removing instance from list
            nowPlaying.Remove(playing);

            var tempAudioSource = playing.audioSource;

            playing.busy = false;
            playing.stopping = true;

            yield return new WaitForSeconds(a.delay);

            //Fading out
            if (fade && !forceStop && !SkipAuto.core.skip) {
                
                while (tempAudioSource != null && tempAudioSource.volume > 0 && playing.stopping) {
                    tempAudioSource.volume -= fadeSpeed;
                    yield return new WaitForEndOfFrame();
                }

            }

            //Removing audio source
            if (tempAudioSource != null) {
                Destroy(tempAudioSource); 
            }

        }

    }

    //Used to update volume levels
    public void updateVolume() {
        foreach (AudioInfo a in nowPlaying) {
            if (!a.stopping) {
                a.busy = false;
                a.audioSource.volume = getVolume(a);
            }
        }
    }

    //Used to obtain volume based on audio type
    public float getVolume(AudioInfo audio) {

        var v = 0f;

        switch (audio.audioType) {

            case AudioInfo.audioTypeEnum.Music:
                v = Settings.core.settings.musicVolume;
                break;
            case AudioInfo.audioTypeEnum.SFx:
                v = Settings.core.settings.SFxVolume;
                break;
            case AudioInfo.audioTypeEnum.Dialogue:
                v = Settings.core.settings.dialogueVolume;
                break;

        }

        //Adjusting based on general volume
        var general = Settings.core.settings.masterVolume;
        return general/(Mathf.Pow(v, -1));

    }

    public AudioClip getAudioclipById(string id) {
        return Database.core.audios.Find(x => x.id == id).audioclip;
    }

    public void PlayMainMenu() {

        var newNowPlaying = new List<AudioInfo>();

        var a = new AudioInfo();
        a.loop = true;
        a.delay = 0f;
        a.audioType = AudioInfo.audioTypeEnum.Music;
        a.audioclipID = mainMenuMusicID;

        newNowPlaying.Add(a);

        RestartAudio(newNowPlaying);
    }

    void Awake() {
        core = core == null ? this : core;
    }

}

[System.Serializable]
public class AudioInfo {
    public string audioclipID;
    public bool loop;

    [Tooltip("Delay before playing audio.")]
    public float delay;

    public enum audioTypeEnum { Music, Dialogue, SFx };

    [Tooltip("What kind of audio is playing? This will influence impact of the volume sliders.")]
    public audioTypeEnum audioType = audioTypeEnum.Music;

    [HideInInspector]
    [System.NonSerialized]
    public AudioSource audioSource;

    [HideInInspector]
    public bool busy = false;

    [HideInInspector]
    public bool stopping = false;
}

[System.Serializable]
public class AudioInfoCompact {
    public string audioclipID;

    [Tooltip("Delay before stopping.")]
    public float delay;
}

// Code by Cination / Tsenkilidis Alexandros.