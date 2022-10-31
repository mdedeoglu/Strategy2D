using UnityEngine.UI;
using UnityEngine;



namespace Strategy2D
{
    public class InformationPanelCell : MonoBehaviour
    {
        public Image unitIcon;  
        public Text unitName;
        private int unitIndex;
        private InformationPanel informationPanel;
        
        public void SetUnitCell(int _unitIndex, Sprite _unitIcon, string _unitName, InformationPanel _informationMenu)
        {
            informationPanel = _informationMenu;
            unitIndex = _unitIndex;
            unitIcon.sprite = _unitIcon;
            unitName.text = _unitName;
        }

        public void SpawnRequest()
        {
            informationPanel.SendSpawnRequest(unitIndex);
        }
    }
}