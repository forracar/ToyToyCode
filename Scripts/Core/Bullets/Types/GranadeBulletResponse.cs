/* 
 * Oscar Forra Carbonell
*/
using UnityEngine;

public class GranadeBulletResponse : MonoBehaviour, IBulletResponse
{
    public enum TOwner {Player, Enemy};                                     // Enum used to know whitch owner is instantiating the bullet
    public TOwner m_Owner;                                                  // Current owner
    public int m_CurrentExplosionRadius;                                    // Explosion radius
    public LayerMask m_ExplosionLayerMask;                                  // Layer that explosion will affect
    public GameObject m_ExplosionParticle;                               // Partciles that will be instantiated when graanadee explodes
    public bool m_Active;
    private int m_Damage;                                                   // Damage that bullet will do
    private float m_Force;                                                  // Speed that rb will move
    private Rigidbody m_RigidBody;                                          // Rigidbody attached to bullet
    public GameObject m_TrailParticles;
    private Vector3 m_Direction;                                            // Direction that bullet will move
    private float m_ElapsedTime;                                            // Elapsed time
    private Transform m_BulletTrasnform;

    private Vector3 m_StartPosition;
    public void OnAwake()
    {
        m_BulletTrasnform = this.transform;
        m_StartPosition = transform.position;
        m_Active = false;
        m_RigidBody = GetComponent<Rigidbody>();
    }
     // Function called when bullet is isntantiated
    public void OnInit(int l_Damage, float l_Speed, Vector3 l_Direction, Vector3 l_Position)
    {
        // Set damage, speed and direction depending on player weapon. This function is called when the bullet is instantiated in PlayerWeaponController
        m_Damage = l_Damage;
        m_Force = l_Speed;
        m_Direction = l_Direction;
        m_BulletTrasnform.position = l_Position;
        m_RigidBody.isKinematic = false;
        m_TrailParticles.SetActive(true);
        m_Active = true;
        // Impulse RB
        m_RigidBody.AddForce((m_Direction).normalized * m_Force , ForceMode.Impulse);
        
    }
    public void OnBulletDestroy()
    {
        // Deal damage and instaantiate particles
        GranadeBulletExplosion();
        Desactive();
    }
    public void Desactive()
    {
        SoundController.Instance.PlayOneShootAudio("event:/Croco/Explosion",m_BulletTrasnform);
        m_BulletTrasnform.position = m_StartPosition;
        m_RigidBody.isKinematic = true;
        m_TrailParticles.SetActive(false);
        m_Active = false;
    }
    // Called in Updaate in Bullet Controller
    public void OnUpdate()
    {
        m_ElapsedTime += Time.deltaTime;
    }
    // This function deal damage and instantiate a random PS
    private void GranadeBulletExplosion()
    {
        // Instantiate PS
        Instantiate(m_ExplosionParticle,transform.position,transform.rotation);
        // Get all gameObjects that have to collide with granade
        Collider[] l_Objects = Physics.OverlapSphere(m_BulletTrasnform.position,m_CurrentExplosionRadius,m_ExplosionLayerMask);
        // If some object have collisioned deal damage to it
        if(l_Objects != null)
        {
            foreach(Collider go in l_Objects)
            {
                // IF tag is enemy
                if(go.tag == "Enemy")
                {
                    // Deaal damage to enemy
                    go.GetComponent<IEnemy>().DealDamage(m_Damage);
                }
                if(go.tag == "DestroyableObject")
                {
                    // Deal damage to destroyable object
                    go.GetComponent<IDamageable>().DealDamage(m_Damage);
                }
                if(go.tag == "BoxAmmo")
                {
                    // Desactive box ammo
                    BoxAmmo l_Box = go.GetComponent<BoxAmmo>();
                    l_Box.DropObject();
                }
            }
        }
    }
    // Function called in OnTriggerEnter in BulletController
    public void OnDealDamage(GameObject collider, string l_tag)
    {
        OnBulletDestroy();
    }
    // Instantiate randomm PS
}
