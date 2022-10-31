using UnityEngine;


namespace Strategy2D
{
    public class BuildingMain : MonoBehaviour
    {
        public int buildingIndex;
        protected GameManager _manager;         
        protected BuildingItem _buildingItem;
        public void CreateBuilding(int buildingIndex, GameManager manager)
        {
            _manager = GameManager.Instance;
            this.buildingIndex = buildingIndex;
            _buildingItem = manager.GameConfig.Buildings[this.buildingIndex];
            name = _buildingItem.name;
            Created();
        }

        protected virtual void Created() { }
    }
}