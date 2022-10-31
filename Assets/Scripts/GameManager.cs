using UnityEngine;

namespace Strategy2D
{
    public class GameManager :Singleton<GameManager>
    {
        public Camera GameCamera;              
        public GameConfig GameConfig;      
        public GameBoard GameBoard;             
        public ProductionMenu ProductionMenu;   
        public InformationPanel InformationPanel; 
        public SelectionController SelectionManager; 

        private void Start()
        {
            GameBoard.InitGameBoard(this);                 
            ProductionMenu.InitProductionMenu(this);       
            SelectionManager.InitSelectionManager(this);   
            InformationPanel.InitInformationPanel(this);    
        }
    }
}