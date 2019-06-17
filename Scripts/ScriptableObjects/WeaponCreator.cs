/* 
 * Oscar Forra Carbonell
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Weapon", menuName ="Weapon/New Weapon",order = 1)]
public class WeaponCreator : ScriptableObject
{
    [Tooltip("The number of levels that the weapon will have")]
    [Header("Number of level that weapon have")]
    public List<Weapon> m_WeaponLevel;                                  // List of levels that weapon have
}
[System.Serializable]
public class Weapon
{
    public float m_Cadence;  
    [Tooltip("Bullet force only used in granade explosion to apply force to RB")]                                       
    public float m_BulletForce;
    public int m_Damage;
    public int m_Ammo;
    public int m_UpgradeCost;
}
