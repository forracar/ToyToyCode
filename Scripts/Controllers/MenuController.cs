/*
 Oscar Forra Carbonell 
 */
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;
public class MenuController : Singleton<MenuController>
{
    public EventSystem m_EventSystem;                                           // Event system in main scene
    public AudioMixer m_AudioMixer;                                             // Main audio mixer used to change volume in options
    public Animation m_FadeAnimator;                                            // Animator attached to fadepanel
    public AnimationClip m_FadeOutClip;                                         // FadeOut clip
    public AnimationClip m_FadeInClip;                                          // FadeIn clip
    public GameObject m_OptionsFirstSelectionButton;                            // First selection when changes to options menu
    public GameObject m_MainMenuFirstSelectionButton;                           // First selection when changes to main menu sceneç
    public Image m_LoadBarImage;
    public GameObject m_MainMenuPanel;
    
    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    // Function called in play button
    public void LoadGameLevelScene()
    {
        StartCoroutine(LoadSceneWithName(m_FadeInClip.length));
    }
    // Change the main volume of game
    public void SetVolume(float volume)
    {
        m_AudioMixer.SetFloat("Volume",volume);
    }
    // Change the quality graphics
    public void SetQualityGraphics(int qualityID)
    {
        QualitySettings.SetQualityLevel(qualityID);
    }
    // Function used in options panel that changes screen to fullscreen or not
    public void SetFullScreen(bool fullScreen)
    {
        Screen.fullScreen = fullScreen;
    }
    // Load scene and play fadein clip
    private IEnumerator LoadSceneWithName(float animationLength)
    {
        PlayFadeInAnimation();
        yield return new WaitForSeconds(animationLength);
        StartCoroutine(LoadMainSceneAsynchronusly());
        PlayFadeOutAnimation();
    }
    private IEnumerator LoadMainSceneAsynchronusly()
    {
        AsyncOperation l_Operation = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        m_LoadBarImage.gameObject.SetActive(true);
        m_MainMenuPanel.SetActive(false);
        while(!l_Operation.isDone)
        {
            m_LoadBarImage.fillAmount = l_Operation.progress;
            yield return null;
        }
    }
    // Play fade in clip
    private void PlayFadeOutAnimation()
    {
        m_FadeAnimator.clip = m_FadeOutClip;
        m_FadeAnimator.Play();
    }
    // Play fade out clip
    private void PlayFadeInAnimation()
    {
        m_FadeAnimator.clip = m_FadeInClip;
        m_FadeAnimator.Play();
    }
    // Set the object selected in eventsystem (Options)
    public void SelectOptionsButton()
    {
        m_EventSystem.SetSelectedGameObject(m_OptionsFirstSelectionButton);
    }
    // Set the object selected in eventsystem (Main menu)
    public void SelectMainMenuButton()
    {
        m_EventSystem.SetSelectedGameObject(m_MainMenuFirstSelectionButton);
    }
    public void ExitApplication()
    {
        Application.Quit();
    }
}
