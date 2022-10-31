using UnityEngine;



namespace Strategy2D
{
    [CreateAssetMenu(fileName = "New Building", menuName = "Building")]
    public class BuildingItem : Item 
    { 
        public Color itemColor = Color.white;        
        public SoldierItem[] ProductionUnits;  

        public Vector2 dimensions
        {
            get { return new Vector2(itemWidth, itemHeight); }
        }

        public void GetDamage()
        {
            throw new System.NotImplementedException();
        }
    }
}