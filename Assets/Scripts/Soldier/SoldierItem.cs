using UnityEngine;



namespace Strategy2D
{
    [CreateAssetMenu(fileName = "Soldier", menuName = "Soldier Unit")]
    public class SoldierItem : Item
    {        
        public Color UnitColor = Color.white;
        public int attackHP = 0;
    }
}