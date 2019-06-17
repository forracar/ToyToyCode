/* 
 * Oscar Forra Carbonell
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBlackboard : MonoBehaviour
{
    public EnemyCreator m_EnemyLevels;                                              // Enemy properties. Contains health and damage
    public int m_CoinsToDrop;                                                       // Coins that will be added when player kill enemy                                                    
    public float m_AttackCadence;                                                   // Attack cadence of enemy
    public float m_MinDistanceToAttack;                                             // Minimum distnace that enemy have to be to start attacking
    public float m_MaxDistanceToAttack;                                             // Maximum distance that enemy have to be to start chasing
    public float m_DistanceToGoToObjective;                                         // Minimum distance to go to object
    public float m_DistanceToAttackObjective;                                       // Minimum distance to attack object
    public float m_DistanceToDetectPlayer = 5;                                      // Minimum distance to detect player
    [HideInInspector] public int m_CurrentHealth;                                   // Curent HP
    [HideInInspector] public int m_Damage;                                          // Current Damage
    // Start is called before the first frame update
    public void StatsInit()
    {
        // Set current HP to max hp
        int l_Level = GameController.Instance.m_CurrentRound >= m_EnemyLevels.m_EnemyLevel.Count ? l_Level = m_EnemyLevels.m_EnemyLevel.Count-1 : l_Level = GameController.Instance.m_CurrentRound;
        m_CurrentHealth = m_EnemyLevels.m_EnemyLevel[l_Level].m_Health;
        m_Damage = m_EnemyLevels.m_EnemyLevel[l_Level].m_Damage;
    }
}
