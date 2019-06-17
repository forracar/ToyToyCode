/* 
 * Oscar Forra Carbonell
*/
using UnityEngine;

public interface IEnemy
{
    #region Stat init
    void OnAwake();
    TEnemyStates CurrentState();
    void OnInit(Vector3 position);
    void OnUpdate();
    #endregion
    // State setters
    #region  State Setters
    void SetIdle();
    void SetChasePlayer();
    void SetChaseObjective();
    void SetAttackPlayer();
    void SetAttackObjective();
    void SetHit();
    void SetDie();
    #endregion
    #region State Updaters
    // State updaters
    void UpdateIdle();
    void UpdateChasePlayer();
    void UpdateChaseObjective();
    void UpdateAttackPlayer();
    void UpdateAttackObjective();
    void UpdateHit();
    void UpdateDie();
    void DealDamage(int damage);
    void SetChasingAnimation(bool active);
    void SetHitAnimation(bool active);
    void SetDieAnimation(bool active);
    void SetAttackAnimation(bool active);
    #endregion
}