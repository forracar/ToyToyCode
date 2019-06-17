/* 
 * Oscar Forra Carbonell
*/
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Enemy",menuName = "Enemies/New enemy",order = 1)]
public class EnemyCreator : ScriptableObject
{
    [Header("Number of levels that enemy have")]
   public List<EnemyProperties> m_EnemyLevel;           // List of enemy properties
}
[System.Serializable]
public class EnemyProperties
{
    [Header("Health")]
    public int m_Health;                                // Max HP that enemy will have
    [Header("Damage")]
    public int m_Damage;                                // Damage that enemy will do
}
