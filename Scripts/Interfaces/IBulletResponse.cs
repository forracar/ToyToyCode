/* 
 * Oscar Forra Carbonell
*/
using UnityEngine;
public interface IBulletResponse
{
  void OnAwake();
  void OnUpdate();
  void OnDealDamage(GameObject collider, string l_tag);
}