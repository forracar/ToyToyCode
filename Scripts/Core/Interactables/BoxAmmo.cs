/* 
Oscar Forra Carbonell
*/
using UnityEngine;

public class BoxAmmo : MonoBehaviour, IRestartGameElement
{
    public GameObject m_ObjectToDrop;                                   // Object that will drop
    private IRestartGameElement m_RestartElement;                       // Component attached to gameobject
    private void Awake() 
    {
        // Get component attached to gameobject
        m_RestartElement = GetComponent<IRestartGameElement>();
    }
    private void Start() 
    {
        // Add to gamecontroller list of RestartElements
        if(GameController.Instance)
            GameController.Instance.m_RestartGameElements.Add(m_RestartElement);
    }
    public void DropObject()
    {
        //Play sound
        SoundController.Instance.PlayOneShootAudio("event:/GamePlay/AmmoBoxDestroy",this.transform);
        // Instantiate object to drop
        Instantiate(m_ObjectToDrop,transform.position,Quaternion.identity);
        // Set box to gameobject inactive
        gameObject.SetActive(false);
    }

    public void RestartGameElement()
    {
        // Set gameobject to active
        gameObject.SetActive(true);
    }
}
