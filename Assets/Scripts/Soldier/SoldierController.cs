using System.Collections.Generic;
using UnityEngine;



namespace Strategy2D
{
    public class SoldierController : MonoBehaviour
    {
        public CircleCollider2D Collider;
        public SpriteRenderer Sprite;
        public Color SelectedColor;
        public AStarPathfinding AStar;

        private GameManager manager;
        private SoldierItem soldierItem;
        private float healthPoint;
        private float attackHP;
        private SoldierController targetUnit;
        private BuildingController targetBuilding;
        [SerializeField]
        private HealthBar healthBar;

        [SerializeField]
        private float speed = 1f;
        private Vector2 targetPos;


        public void InitSoldierUnit(SoldierItem _soldierItem, GameManager _manager)
        {
            enabled = false;
            this.manager = _manager;
            // Adding this as a subscribers to SelectionManager
            this.manager.SelectionManager.AttackToUnit += AttackToUnitSubsc;
            this.manager.SelectionManager.AttackToBuilding += AttackToBuildingSubsc;
            this.manager.SelectionManager.SelectUnit += SelectUnitSubsc;
            this.manager.SelectionManager.AttackUnit += AttackUnitSubsc;
            this.manager.SelectionManager.SelectUnits += SelectUnitsSubsc;
            this.manager.SelectionManager.Deselect += DeselectMe;

            // Setting soldier unit information
            this.soldierItem = _soldierItem;
            name = this.soldierItem.name;
            Sprite.sprite = this.soldierItem.itemSprite;
            attackHP = this.soldierItem.attackHP;
            healthPoint = this.soldierItem.maxHealthPoint;
            Sprite.color = this.soldierItem.UnitColor;
        }


        void Update()
        {
            Movement();
            HealthControl();
        }


        private void Movement()
        {
            if (AStar.Path != null && AStar.Path.Count > 0)
            {
                if (targetPos == new Vector2Int(1000, 0) || transform.position == (Vector3)targetPos)
                    targetPos = AStar.Path.Pop();

            }
            else 
            {
                // arrive target point 
                enabled = false;
                if (targetUnit) AttackUnit();
                if (targetBuilding) AttackBuilding();
            }

            // moving the soldier towards to target
            if (targetPos != new Vector2Int(1000, 0))
                transform.position = Vector2.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);

        }
        private void HealthControl()
        {
            if (healthPoint <= 0)
            {
                this.manager.SelectionManager.SelectUnit -= SelectUnitSubsc;
                this.manager.SelectionManager.SelectUnits -= SelectUnitsSubsc;
                this.manager.SelectionManager.AttackToUnit -= AttackToUnitSubsc;
                this.manager.SelectionManager.AttackUnit -= AttackUnitSubsc;
                this.manager.SelectionManager.Deselect -= DeselectMe;
                Destroy(this.gameObject);
            }

        }
        private void AttackBuilding()
        {

            if (targetBuilding.healthPoint > 0)
            {
                targetBuilding.enabled = true;
                targetBuilding.healthPoint -= attackHP;
                targetBuilding.transform.Find("HealthBar").GetComponent<HealthBar>().Damage(targetBuilding.maxHealthPoint, targetBuilding.healthPoint);
            }
            targetBuilding = null;
        }
        private void AttackUnit()
        {

            if (targetUnit.healthPoint > 0)
            {
                targetUnit.enabled = true;
                targetUnit.healthPoint -= attackHP;
                targetUnit.transform.Find("HealthBar").GetComponent<HealthBar>().Damage(targetUnit.soldierItem.maxHealthPoint, targetUnit.healthPoint);
               
            }
            targetUnit = null;
        }

        private void AttackToBuildingSubsc(List<SoldierController> selectedUnits, List<BuildingController> attackedBuildings)
        {
            foreach (SoldierController unit in selectedUnits)
            {
                targetBuilding = attackedBuildings[0];
            }

        }
        private void AttackToUnitSubsc(List<SoldierController> selectedUnits, List<SoldierController> attackedUnits)
        {
            foreach (SoldierController unit in selectedUnits)
            {
                if (unit == this && unit != attackedUnits[0])
                {
                    targetUnit = attackedUnits[0];
                   
                }
            }
 
        }

        private void AttackUnitSubsc(RaycastHit2D hit, List<SoldierController> attackedUnits)
        {
            if (hit.collider == Collider)
            {
                AttackMe(attackedUnits);
            }

        }
        private void AttackMe(List<SoldierController> attackedUnits)
        {
            attackedUnits.Add(this);
        }

        // Selection Listener
        private void SelectUnitSubsc(RaycastHit2D hit, List<SoldierController> selectedUnits)
        {
            if (hit.collider == Collider)
                SelectMe(selectedUnits);
        }

        // Selection Listener
        private void SelectUnitsSubsc(Vector2 min, Vector2 max, List<SoldierController> selectedUnits)
        {
            Vector3 screenPos = manager.GameCamera.WorldToScreenPoint(transform.position);
            if (screenPos.x > min.x && screenPos.x < max.x)
            {
                if (screenPos.y > min.y && screenPos.y < max.y)
                    SelectMe(selectedUnits);
            }
        }

        // Selection
        private void SelectMe(List<SoldierController> selectedUnits)
        {
   
            manager.SelectionManager.AStarOrder += StartAStar;
            Sprite.color = SelectedColor;
            selectedUnits.Add(this);
        }

        // Deselection event Listener
        private void DeselectMe()
        {
            manager.SelectionManager.AStarOrder -= StartAStar;
            Sprite.color = soldierItem.UnitColor;
        }

        // AStar Listener
        private void StartAStar(Vector2Int targetPos)
        {

            enabled = true;
            this.targetPos = new Vector2Int(1000, 0);        // null value
            AStar.Current = null;
            AStar.StartPos = new Vector2Int((int)transform.position.x, (int)transform.position.y);
            AStar.GoalPos = targetPos;
            AStar.Path = null;
            AStar.PathFinding(manager.GameBoard);
        }
    }
}