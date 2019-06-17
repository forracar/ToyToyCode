using UnityEngine;

public class DestroyOnTime : MonoBehaviour
{
    [SerializeField] float m_TimeToDestroy = 5;                                                     // Time to destroy object
    float m_ElaspedTime;                                                                        // Current time since bullet spawn
    // Start is called before the first frame update
    void Start()
    {
        // Set elapsed time to 0
        m_ElaspedTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Compute elapsed time
        m_ElaspedTime += Time.deltaTime;
        // if elpasedtime > timetodestroy: Destroy
        if(m_ElaspedTime > m_TimeToDestroy)
        {
            Destroy(this.gameObject);
        }
    }
}
