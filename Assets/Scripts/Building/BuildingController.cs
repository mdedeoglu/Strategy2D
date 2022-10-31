using System.Collections.Generic;
using UnityEngine;


namespace Strategy2D
{
    public class BuildingController : BuildingMain
    {
        public SpriteRenderer Sprite;   // Image of the building
        public BoxCollider2D Collider;  // Collider of the building
        public Color SelectedColor;     // When the building selected

        public Transform _spawnPos;  // Spawn Position
        [SerializeField]
        private HealthBar healthBar;
        public float healthPoint;
        public float maxHealthPoint;


        // When Firstly created
        protected override void Created()
        {
            healthPoint = _buildingItem.healthPoint;
            maxHealthPoint = _buildingItem.maxHealthPoint;
            transform.parent = _manager.GameBoard.transform.GetChild(1);

            // Adding Listeners
            _manager.SelectionManager.SelectBuilding += SelectBuildingSubsc;
            _manager.SelectionManager.Deselect += Deselect;
            _manager.SelectionManager.AttackBuilding += AttackBuildingSubsc;


            AdjustCollider();   // Adjusting Collider
            Deselect();       // Coloring the cells with color of selected building

            // Creating spawnPoint
            //if (_buildingItem.isProductive)
                CreateSpawnPos();
        }

        // Adjusts the collider
        private void AdjustCollider()
        {
            float width = _buildingItem.itemWidth;
            float height = _buildingItem.itemHeight;
            Vector2 size = new Vector2(width, height);

            Collider.size = size;
            Sprite.transform.localScale = size;
        }
        void Update()
        {
            HealthControl();
        }
        private void HealthControl()
        {
            if (healthPoint <= 0)
            {
                _manager.SelectionManager.SelectBuilding -= SelectBuildingSubsc;
                _manager.SelectionManager.Deselect -= Deselect;
                _manager.SelectionManager.AttackBuilding -= AttackBuildingSubsc;
                Destroy(this.gameObject);
            }

        }


        private void AttackBuildingSubsc(RaycastHit2D hit, List<BuildingController> attackedBuildings)
        {
            if (hit.collider == Collider)
            {
               
                attackedBuildings.Add(this);
            }

        }

        private void CreateSpawnPos()
        {
            Vector3 pos = transform.position+new Vector3(0,-3,0);            
            _spawnPos = new GameObject().transform;
            _spawnPos.parent = transform;
            _spawnPos.position = pos;
            _spawnPos.name = "Spawn Point";
        }

        // Selection event listener
        private void SelectBuildingSubsc(RaycastHit2D hit, List<BuildingController> selectedBuilding)
        {
            if (hit.collider == Collider)
            {
                // Adding spawn listener
                _manager.InformationPanel.SpawnUnit += SpawnUnit;
                // Coloring the cells with selection color
                Sprite.color = SelectedColor;
                
                selectedBuilding.Add(this);
            }
        }

        // Deselection event Listener
        private void Deselect()
        {
            // Rempveing spawn listener
            _manager.InformationPanel.SpawnUnit -= SpawnUnit;
            // Coloring the cells with color of building
            Sprite.color = _buildingItem.itemColor;
        }

        // Spawns new unit on spawnPoint
        private void SpawnUnit(int unitIndex, Transform parent)
        {
            // Positioning on an interval
            Vector3 spawnPosition = _spawnPos.position;
            spawnPosition.x += Random.Range(-.5f, .5f);
            spawnPosition.y += Random.Range(-.5f, .5f);

            // Creating the unit
            SoldierController unit = Instantiate(_manager.GameConfig.SoldierUnit, spawnPosition, Quaternion.identity, parent);
            unit.InitSoldierUnit(_buildingItem.ProductionUnits[unitIndex], _manager);
        }
    }
}