
/* 
Oscar Forra Carbonell 
*/
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    public Animation m_Animation;                                  // Animation attached to button
    public AnimationClip m_SelectedAnimationClip;                   // Animation clip when button  is selected
    public AnimationClip m_DeselectedAnimationClip;                 // Animation clip when button is deselected
    private void Awake()
    {
        // Get animation attached to button
        if(!m_Animation)
            m_Animation = GetComponent<Animation>();
    }
    // Function called when object is deselected by user
    public void OnDeselect(BaseEventData eventData)
    {
        PlayOnDeselectedAnimation();
    }
    // Function called when object is selected by user
    public void OnSelect(BaseEventData eventData)
    {
        PlayOnSelectAnimation();
    }
    // Plays the selected animation clip
    private void PlayOnSelectAnimation()
    {
        SoundController.Instance.PlayOneShootAudio("event:/UI/ChangeButton",this.transform);
        m_Animation.clip = m_SelectedAnimationClip;
        m_Animation.Play();
    }
    // Plays the deselected animation clip
    private void PlayOnDeselectedAnimation()
    {
        m_Animation.clip = m_DeselectedAnimationClip;
        m_Animation.Play();
    }
    public void OnSubmit(BaseEventData eventData)
    {
        SoundController.Instance.PlayOneShootAudio("event:/UI/Click",Camera.main.transform);
        PlayOnDeselectedAnimation();
    }
}
