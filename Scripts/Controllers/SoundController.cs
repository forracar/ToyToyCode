/* 
 * Oscar Forra Carbonell
*/
using UnityEngine;
using Utils;

public class SoundController : Singleton<SoundController>
{
    private AudioSource m_AudioSource;
    void Start() {
        m_AudioSource = GetComponent<AudioSource>();
    }
    public void StopAudioSource()
    {
        m_AudioSource.Stop();
    }
    public void PlayAudioSource()
    {

        m_AudioSource.Play();
    }
    // Plays One shoot audio
    public void PlayOneShootAudio(string path, Transform l_transform)
    {
        // Create audio instance
        FMOD.Studio.EventInstance l_OneShootAudio = FMODUnity.RuntimeManager.CreateInstance(path);
        // Set audio position
        l_OneShootAudio.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(l_transform));
        // Play OneShoot audio
        l_OneShootAudio.start();
    }
    // Returns a eventInstance
    public FMOD.Studio.EventInstance PlayEvent(string path,Transform l_transform,Rigidbody l_rb)
    {
        // Create audio instance
        FMOD.Studio.EventInstance l_OneShootAudio = FMODUnity.RuntimeManager.CreateInstance(path);
        // Set audio transform
        l_OneShootAudio.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(l_transform));
        // Attach audio to rb
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(l_OneShootAudio,l_transform,l_rb);
        // Play event
        l_OneShootAudio.start();
        return l_OneShootAudio;
    }
    // Stop event
    public void StopEvent(FMOD.Studio.EventInstance l_eventInstance)
    {
        // Stop audio event inmediatly
        l_eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
