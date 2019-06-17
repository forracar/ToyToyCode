/* 
 * Oscar Forra Carbonell
*/
using UnityEngine;

public class TableController : MonoBehaviour
{
    private ITableResponse m_TableResponse;
    private void Awake() 
    {
        // Get table behaviour
        m_TableResponse = GetComponent<ITableResponse>();    
    }
    void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Player"){
            // Enter behaviour
            m_TableResponse.OnEnter();
        }
    }
    void OnTriggerStay(Collider other) 
    {
        if(other.tag == "Player"){
            // On stay behaviour
            m_TableResponse.OnStay();
        }   
    }
    void OnTriggerExit(Collider other) {
        if(other.tag == "Player"){
            // On stay behaviour
            m_TableResponse.OnExit();
        } 
    }
}

