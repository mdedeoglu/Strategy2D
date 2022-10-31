using System.Collections.Generic;
using UnityEngine;

namespace Strategy2D
{
    // Building Template to place a new building
    public class BuildingTemplate : BuildingMain
    {
        public bool OnBoard = false;    // True, when mouse on gameBoard
        public Transform CellContainer; // All Cell objects of the building

        private List<Cell> _buildingCells;  // All Cells of the building 
        private bool _canPlace;         // True, when the building can place


        protected override void Created()
        {
            // Adjusting placing listener
            _manager.SelectionManager.PlacingOperation += Placing;
            // Filling the building with tempCells
            _buildingCells = CellHelper.SpawnCells(_buildingItem.dimensions, _manager.GameConfig.Cell, CellContainer);

            foreach (var cell in _buildingCells)
                cell.Renderer.sortingOrder = 3;

            AdjustContainerPos();
        }

        private void Placing() { }

        private void AdjustContainerPos()
        {
            float width = Mathf.Round(_buildingItem.itemWidth / 2);
            float height = Mathf.Round(_buildingItem.itemHeight / 2);

            CellContainer.transform.localPosition -= new Vector3(width, height);
        }


        private void Update()
        {
            MovingBuildingTemplate();
        }

        // Moves the buildingTemplate with mouse position
        private void MovingBuildingTemplate()
        {
            var pos = _manager.GameCamera.ScreenToWorldPoint(Input.mousePosition);
            pos.z = transform.position.z;
            transform.position = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), pos.z);

            // Checking for available place
            _canPlace = CheckPlace();

            // When Mouse left-click
            if (Input.GetMouseButtonDown(0))
            {
                // If there is no collision on grid with another building
                if (_canPlace && OnBoard)
                {
                    FillPlace();
                    CreateBuilding();
                }
            }

            // Destroys thi object, When Mouse right-click
            if (Input.GetMouseButtonDown(1))
                SelfDestruction();
        }


        // Checks whether there is colision with another building
        private bool CheckPlace()
        {
            bool canPlace = true;

            // Checking each cell of the building for collision
            foreach (var cell in _buildingCells)
            {
                Vector2 pos = cell.transform.position;
                RaycastHit2D hit = Physics2D.Raycast(cell.transform.position, Vector3.forward, Mathf.Infinity);
                cell.SetValid();

                // If there is a building
                if ((hit.collider && hit.collider.CompareTag("Building")) || IsOutOfGrid(pos))
                {
                    cell.SetInvalid();
                    canPlace = false;
                }
            }
            return canPlace;
        }

        // Checking if the template in grid area
        private bool IsOutOfGrid(Vector2 pos)
        {
            if (pos.x < 0 || pos.y < 0)
                return true;

            GameBoard gameBoard = _manager.GameBoard;
            float maxX = gameBoard.Dimensions[0] - 1;
            float maxY = gameBoard.Dimensions[1] - 1;

            if (pos.x > maxX || pos.y > maxY)
                return true;

            return false;
        }

        // Assigns one corresponding point on grid
        private void FillPlace()
        {
            GameBoard gameBoard = _manager.GameBoard;

            // Turning each cell to 1
            foreach (var cell in _buildingCells)
            {
                Vector2 point = cell.transform.position;
                int gridH = (int)Mathf.Round(point.y);
                int gridW = (int)Mathf.Round(point.x);

                gameBoard.GridContent[gridW, gridH] = 0;
            }
        }

        // Places a building into current mouse position
        private void CreateBuilding()
        {
            // BuildingSolid position
            float width = _buildingItem.itemWidth;
            float height = _buildingItem.itemHeight;
            Vector3 pos = CellContainer.transform.position;
            pos.x += ((width / 2) - .5f);
            pos.y += ((height / 2) - .5f);
            pos.z = 0;

            // Creating a building gameObject
            GameObject buildingObject = Instantiate(_manager.GameConfig.Building, pos, Quaternion.identity) as GameObject;
            BuildingController building = buildingObject.GetComponent<BuildingController>();
            building.CreateBuilding(buildingIndex, _manager);
            building.Sprite.sprite = _buildingItem.itemSprite;
        }

        // Destroys this gameObject
        public void SelfDestruction()
        {
            _manager.SelectionManager.PlacingOperation -= Placing;
            Destroy(gameObject);
        }
    }
}