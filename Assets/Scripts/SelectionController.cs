using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Strategy2D
{
    public class SelectionController : MonoBehaviour
    {
        // AStar Trigger 
        public event Action<Vector2Int> AStarOrder;

        // Controls if there is placing operation currently
        public event PlacingDelegate PlacingOperation;
        public delegate void PlacingDelegate();
        private bool _placingOperation;

        // Building Selection event
        public event Action<RaycastHit2D, List<BuildingController>> SelectBuilding;  // To select a building
        private List<BuildingController> _selectedBuildings;                         // Currently selected building

        // Unit selection events
        public event Action<RaycastHit2D, List<SoldierController>> SelectUnit;       // To select one unit
        public event Action<Vector2, Vector2, List<SoldierController>> SelectUnits;  // To select more than one unit
        private List<SoldierController> _selectedUnits;                              // Currently Selected units

        public event Action<RaycastHit2D, List<SoldierController>> AttackUnit;       // To attack one unit
        public event Action<List<SoldierController>,List<SoldierController>> AttackToUnit;                   // To attack one unit
        private List<SoldierController> _attackedUnits;                              // currently attack units

        public event Action<RaycastHit2D, List<BuildingController>> AttackBuilding;       // To attack one unit
        public event Action<List<SoldierController>, List<BuildingController>> AttackToBuilding;                   // To attack one unit
        private List<BuildingController> _attackedBuildings;                              // currently attack units


        // Deselection Publisher
        public event Action Deselect;       // Deselects all selected

        // Others
        public RectTransform SelectionBox;  // Selection Box

        private GameManager _manager;       // Game Manager
        private InformationPanel _informationMenu;
        private bool _onBoard = true;       // True, when mouse is not on HUD
        private bool _selection;            // True, when selection started
        private bool _boxSelection;         // True, if the selection is made by box
        private Vector2 _startPos;          // Mouse pos, when selection begin

        // Inits SelectionManager
        public void InitSelectionManager(GameManager manager)
        {
            _manager = manager;
            _informationMenu = _manager.InformationPanel;
            _selectedBuildings = new List<BuildingController>();
            _selectedUnits = new List<SoldierController>();
            _attackedUnits = new List<SoldierController>();
            _attackedBuildings = new List<BuildingController>();
            SelectionBox.gameObject.SetActive(false);
        }

        private void Update()
        {
            // When there is placing
            if (PlacingOperation != null)
                return;

            // Selection is available
            if (_onBoard || _boxSelection)
            {
                // Left mouse click
                if (Input.GetMouseButtonDown(0))
                {
                    Deselection();
                    _selection = true;
                    _startPos = Input.mousePosition;
                }
            }

            // Type of selection
            if (_selection)
            {
                // Left mouse held down
                if (Input.GetMouseButton(0))
                    _boxSelection = UpdateSelectionBox(Input.mousePosition);

                // Left mouse up
                if (Input.GetMouseButtonUp(0))
                    RealeaseSelection();
            }

            // When units are selected
            if (_selectedUnits.Count > 0)
                SoldiersReady();
        }

        // Deselection all currently selected
        private void Deselection()
        {
            Deselect?.Invoke();
            _informationMenu.RemoveInformation();
            _selectedBuildings.Clear();
            _selectedUnits.Clear();
            _attackedUnits.Clear();

        }

        // Updates size of the selection box 
        private bool UpdateSelectionBox(Vector2 currentMousePos)
        {
            if (!SelectionBox.gameObject.activeInHierarchy)
                SelectionBox.gameObject.SetActive(true);

            // Adjusting dimensions of selection box
            float width = currentMousePos.x - _startPos.x;
            float height = currentMousePos.y - _startPos.y;

            SelectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
            SelectionBox.anchoredPosition = _startPos + new Vector2(width / 2, height / 2);

            //  SelectionBox.rect.Set(5, 5, 5, 5);
            // If area is greater then box selection true 
            float area = Mathf.Abs(width * height);
            return area > 500 ? true : false;
        }

        // When left mouse up
        private void RealeaseSelection()
        {
            SelectionBox.gameObject.SetActive(false);
            _selection = false;

            // If selection made by box
            if (_boxSelection)
            {
                Vector2 min = SelectionBox.anchoredPosition - (SelectionBox.sizeDelta / 2);
                Vector2 max = SelectionBox.anchoredPosition + (SelectionBox.sizeDelta / 2);

                // If there are units
                SelectUnits?.Invoke(min, max, _selectedUnits);
                _boxSelection = false;
                ShowUnitsInfo();
            }
            // If selection made by left mouse click
            else
            {
                Vector3 worldPos = _manager.GameCamera.ScreenToWorldPoint(_startPos);
                RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector3.back, Mathf.Infinity);
                Debug.DrawRay(worldPos, Vector3.forward, Color.blue, 10f);

                // Publishing selection request to buildings
                SelectBuilding?.Invoke(hit, _selectedBuildings);

                // If there is building
                if (_selectedBuildings.Count > 0)
                {
                    int buildingIndex = _selectedBuildings[0].buildingIndex;
                    BuildingItem buildingData = _manager.GameConfig.Buildings[buildingIndex];
                    _informationMenu.ShowBuildingInfo(buildingData);

                }
                // Else if there is unit
                else
                {                 
                    // Publishing selection request to units 
                    SelectUnit?.Invoke(hit, _selectedUnits);
                    ShowUnitsInfo();
                }
            }
        }

        // Shows information of the units 
        private void ShowUnitsInfo()
        {
            if (_selectedUnits.Count > 0)
            {
                List<string> unitList = new List<string>();
                foreach (var item in _selectedUnits)
                    unitList.Add(item.name);

                _informationMenu.ShowUnitsInfo(unitList);
            }
        }

        // When soldiers are selected
        private void SoldiersReady()
        {
            if (Input.GetMouseButtonDown(1))
            {
                _attackedUnits.Clear();
                _attackedBuildings.Clear();
                Vector3 worldPos = _manager.GameCamera.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector3.back, Mathf.Infinity);
                Debug.DrawRay(worldPos, Vector3.forward, Color.blue, 10f);
                
                Vector3 targetPos = _manager.GameCamera.ScreenToWorldPoint(Input.mousePosition);

                AttackUnit?.Invoke(hit, _attackedUnits);                
                if (_attackedUnits.Count > 0)
                {
                     AttackToUnit?.Invoke(_selectedUnits,_attackedUnits);
                }

                AttackBuilding?.Invoke(hit, _attackedBuildings);
                if (_attackedBuildings.Count > 0)
                {
                   // Debug.Log("AttackBuilding");
                    AttackToBuilding?.Invoke(_selectedUnits, _attackedBuildings);
                    targetPos = _attackedBuildings[0]._spawnPos.position;
                }
               
            
                Vector2Int GoalPos = new Vector2Int(Mathf.RoundToInt(targetPos.x), Mathf.RoundToInt(targetPos.y));
                AStarOrder?.Invoke(GoalPos);
                StartCoroutine(TargetMarker(targetPos));
            }
        }

        // Marks the target position
        IEnumerator TargetMarker(Vector3 pos)
        {
            pos.z = 0;
            SpriteRenderer marker = Instantiate(_manager.GameConfig.Target, pos, Quaternion.identity);
            while (marker.color.a < 1)
            {
                yield return new WaitForSeconds(.01f);
                marker.color += new Color(0, 0, 0, .2f);
            }
            while (marker.color.a > 0)
            {
                yield return new WaitForSeconds(.05f);
                marker.color -= new Color(0, 0, 0, .2f);
            }
            Destroy(marker.gameObject);
        }


        public void MouseOnHUD()
        {
            _onBoard = false;
        }

        public void MouseOnBoard()
        {
            _onBoard = true;
        }

    }
}