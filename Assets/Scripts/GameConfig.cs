using UnityEngine;



namespace Strategy2D
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Game Config")]
    public class GameConfig : ScriptableObject
    {
        // Grid dimensions
        public int MapGridWidth;
        public int MapGridHeight;

        public Cell Cell;           // Cell Prefab for buildingTemplate
        public Pool Pool;           // Pool of Production Menu

        // Buildings
        public GameObject BuildingTemplate;     // Template to place selected building
        public GameObject Building;        // Building that will be placed
        public BuildingItem[] Buildings;        // All distinct buildings on the game

        public SoldierController SoldierUnit;       // Military unit object

        public SpriteRenderer Target;   // Target marker
    }
}