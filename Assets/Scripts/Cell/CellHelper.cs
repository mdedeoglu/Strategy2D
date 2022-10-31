using System.Collections.Generic;
using UnityEngine;

namespace Strategy2D
{
    public static class CellHelper
    {
        // Spawns cells into the given container in wanted cell type
        public static List<Cell> SpawnCells(Vector2 dimensions, Cell cell, Transform container)
        {
            var cellList = new List<Cell>();

            // Dimensions(columns, rows)  of the wanted object
            var width = dimensions[0];
            var height = dimensions[1];

            // Container is being filled with cell
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    Cell newCell = Object.Instantiate(cell, container);
                    newCell.transform.localPosition = new Vector2(x, y);
                    cellList.Add(newCell);
                }
            }
            return cellList;
        }
    }
}