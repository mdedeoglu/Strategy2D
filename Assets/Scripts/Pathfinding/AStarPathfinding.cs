using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Strategy2D
{
	public class AStarPathfinding : MonoBehaviour
	{
		public Vector2Int StartPos, GoalPos;// Start and Goal positions
		public Node Current;                // current position of the soldier
		public Stack<Vector2Int> Path
		{       // Path that must be completed
			get { return _path; }
			set { _path = value; }
		}
		private GameBoard _gameBoard;       // GameBoard that holds the grid
		private HashSet<Node> _openList, _closedList;
		private Stack<Vector2Int> _path;    // Path
		private Dictionary<Vector2Int, Node> _allNodes = new Dictionary<Vector2Int, Node>();

		// Inits
		private void Initialize()
		{
			Current = GetNode(StartPos);

			_openList = new HashSet<Node>();
			_closedList = new HashSet<Node>();

			_openList.Add(Current);
		}

		public void PathFinding(GameBoard gameBoard)
		{   
			_gameBoard = gameBoard;
			if (Current == null)
				Initialize();

			while (_openList.Count > 0 && _path == null)
			{
				List<Node> neighbors = FindNeighbors(Current.Position);

				ExamineNeighbors(neighbors, Current);

				UpdateCurrentTile(ref Current);

				_path = GeneratePath(Current);
			}

				
		}


		private List<Node> FindNeighbors(Vector2Int parentPos)
		{
			List<Node> neighbors = new List<Node>();
			for (int x = -1; x <= 1; x++)
			{
				for (int y = -1; y <= 1; y++)
				{
					if (x != 0 || y != 0)
					{ 
						Vector2Int neighborPos = new Vector2Int(parentPos.x + x, parentPos.y + y);

						if (!IsOutOfGrid(neighborPos))
						{
							if (neighborPos != StartPos && _gameBoard.GridContent[neighborPos.x, neighborPos.y] == 1)
							{
								Node neighbor = GetNode(neighborPos);
								neighbors.Add(neighbor);
							}
						}
					}
				}
			}

			return neighbors;
		}

		// Checking if the template in grid area
		private bool IsOutOfGrid(Vector2 pos)
		{
			if (pos.x < 0 || pos.y < 0)
				return true;

			float maxX = _gameBoard.Dimensions[0] - 1;
			float maxY = _gameBoard.Dimensions[1] - 1;

			if (pos.x > maxX || pos.y > maxY)
				return true;

			return false;
		}

		// Rating the selected neigbors
		private void ExamineNeighbors(List<Node> neighbors, Node current)
		{
			for (int i = 0; i < neighbors.Count; i++)
			{
				Node neighbor = neighbors[i];

				if (!ConnectedDiagonally(current, neighbor))
					continue;

				int gScore = DetermineGScore(neighbors[i].Position, current.Position);      // calculating G score

				if (_openList.Contains(neighbor))
				{
					if (Current.G + gScore < neighbor.G)
					{
						CalculateValues(current, neighbor, gScore);
					}
				}
				else if (!_closedList.Contains(neighbor))
				{
					CalculateValues(current, neighbor, gScore);

					_openList.Add(neighbor);
				}
			}
		}

		// Gives rate
		private void CalculateValues(Node parent, Node neighbor, int cost)
		{
			neighbor.Parent = parent;

			neighbor.G = parent.G + cost;

			neighbor.H = (Mathf.Abs(neighbor.Position.x - GoalPos.x) + Mathf.Abs(neighbor.Position.y - GoalPos.y)) * 10;

			neighbor.F = neighbor.G + neighbor.H;
		}

		// calculating G score using the nodes
		private int DetermineGScore(Vector2Int neighbor, Vector2Int current)
		{
			int gScore = 0;

			int x = current.x - neighbor.x;
			int y = current.y - neighbor.y;

			if (Mathf.Abs(x - y) % 2 == 1)
				gScore = 10;        // horizontal or vertical
			else
				gScore = 14;        // cross direction

			return gScore;
		}

		private void UpdateCurrentTile(ref Node current)
		{
			_openList.Remove(current);
			_closedList.Add(current);

			if (_openList.Count > 0)
				Current = _openList.OrderBy(x => x.F).First();      // sort the list and find the node with lowest F value
		}

		private Node GetNode(Vector2Int position)
		{
			if (_allNodes.ContainsKey(position))
				return _allNodes[position];
			else
			{
				Node node = new Node(position);
				_allNodes.Add(position, node);
				return node;
			}
		}

		private bool ConnectedDiagonally(Node current, Node neighbor)
		{
			Vector2Int direction = current.Position - neighbor.Position;

			Vector2Int first = new Vector2Int(current.Position.x + (direction.x * -1), current.Position.y);
			Vector2Int second = new Vector2Int(current.Position.x, current.Position.y + (direction.y * -1));

			if (_gameBoard.GridContent[first.x, first.y] != 1 || _gameBoard.GridContent[second.x, second.y] != 1)
				return false;

			return true;
		}

		private Stack<Vector2Int> GeneratePath(Node current)
		{
			if (Current.Position == GoalPos)
			{
				Stack<Vector2Int> finalPath = new Stack<Vector2Int>();

				while (current.Position != StartPos)
				{
					finalPath.Push(current.Position);
					current = current.Parent;
				}

				return finalPath;
			}
			return null;
		}
	}
}