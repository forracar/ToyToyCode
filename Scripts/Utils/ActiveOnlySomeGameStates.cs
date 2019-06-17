/* 
 * Oscar Forra Carbonell
*/
using UnityEngine;
namespace Utils 
{
    public class ActiveOnlySomeGameStates : MonoBehaviour, IActiveDuringGameState
    {
        public TGameState m_ActiveState;                                // State where the chils will be activated
        public GameObject[] m_Childs;                                   // Childs to activate
        public void Start()
        {
            GameController.Instance.m_ActiveDuringGameStateElements.Add(GetComponent<IActiveDuringGameState>());
        }

        public void ActiveGameElement()
        {
            // If the state is the one active otherwise desactive
            if (GameController.Instance.m_CurrentGameState == m_ActiveState)
            {
                foreach (var child in m_Childs)
                {
                    child.SetActive(true);
                }
            }
            else {
                foreach (var child in m_Childs)
                {
                    child.SetActive(false);
                }
                
            }
        }
    }
}

