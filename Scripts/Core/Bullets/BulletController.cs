/* 
 * Oscar Forra Carbonell
*/
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class BulletController : MonoBehaviour
{
    private IBulletResponse m_BulletResponse;                                   // Bullet behaviour
    public bool m_Granade;
    // Start is called before the first frame update
    private void Awake() 
    {
        // Get Interface
        m_BulletResponse = GetComponent<IBulletResponse>();
        m_BulletResponse.OnAwake();
    }
    void Update() {
        // Update bullet
        m_BulletResponse.OnUpdate();
    }
   private void OnTriggerEnter(Collider other) {
        m_BulletResponse.OnDealDamage( other.gameObject, other.gameObject.tag);
   }
    private  void OnCollisionEnter(Collision other) {
        m_BulletResponse.OnDealDamage( other.gameObject, other.gameObject.tag);
        ContactPoint[] l_ContactPoint = other.contacts;
        if(l_ContactPoint != null)
        {
            for(int i = 0; i<1; i++)
            {
                GameController.Instance.GetGranadeDecals().Initialize(l_ContactPoint[i].point,l_ContactPoint[i].normal);
            }
        }
    }
}
