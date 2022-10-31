using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



namespace Strategy2D
{
    public class InformationPanel : MonoBehaviour
    {
        public Image BuildingImage;     // Image of the selectedBuilding
        public Text BuildingName;       // Name of the selectedBuilding
        public InformationPanelCell InformationMenuCell; // A cell object to create unit production panel 
        public Transform SpawnParent;

        public event Action<int, Transform> SpawnUnit;  // To send spawn request


        [SerializeField]
        protected RectTransform _header;       // Rectangle of Content object of ScrollBar
        [SerializeField]
        protected RectTransform _content;       // Rectangle of Content object of ScrollBar
        private GameManager _manager;           // Gane Manager
        private List<SoldierItem> _productionUnits;// Unit object samples of selected building to spawn
        private BuildingItem _buildingData;     // BuildingData on showing 


        // Inits the menu
        public void InitInformationPanel(GameManager manager)
        {
            _manager = manager;
            _header.gameObject.SetActive(false);
            _content.gameObject.SetActive(false);
        }

        // Shows information of the selected building on the menu
        public void ShowBuildingInfo(BuildingItem buildingData)
        {
            // Deselecting previously selected building
            if (_buildingData)
                RemoveInformation();

            // Unhiding the information menu
            _header.gameObject.SetActive(true);
            _content.gameObject.SetActive(false);

            // Assigning newly selected building
            _buildingData = buildingData;

            // Setting Information Panel
            BuildingName.text = _buildingData.name;
            BuildingImage.sprite = _buildingData.itemSprite;

            // Listing the units
            if (_buildingData.isProductive)
                ShowUnitsInfo(null);
        }

        // Listing units on information panel
        public void ShowUnitsInfo(List<string> unitList)
        {
            // Unhiding the production content
            _content.gameObject.SetActive(true);

            // Listing the units
            int unitCount = _buildingData.ProductionUnits.Length;
            for (int i = 0; i < unitCount; i++)
            {
                var unitData = _buildingData.ProductionUnits[i];
                int unitAmount = 1;

                // If selected units wanted 
                if (unitList != null)
                {
                    unitAmount = 0;
                    _header.gameObject.SetActive(false);
                    foreach (var unit in unitList)
                    {
                        if (unit == unitData.name)
                            unitAmount++;
                    }
                }

                if (unitAmount > 0)
                {
                    // Listing military units on information bar
                    InformationPanelCell cell = Instantiate(InformationMenuCell, Vector3.zero, Quaternion.identity, _content.transform);
                    cell.SetUnitCell(i, unitData.itemIcon, unitData.itemName, this);
                }
            }

        }

        // Deselects current selectedBuilding
        public void RemoveInformation()
        {
            // Hiding Objects
            _header.gameObject.SetActive(false);
            _content.gameObject.SetActive(false);

            // Destroying Old data
            foreach (Transform child in _content.transform)
                Destroy(child.gameObject);
        }

        // Sending spawn request to selected building
        public void SendSpawnRequest(int unitIndex)
        {
            SpawnUnit?.Invoke(unitIndex, SpawnParent);
        }
    }
}