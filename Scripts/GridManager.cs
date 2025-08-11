using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject bitTilePrefab;
    private BinaryTile[,] tiles;

    public int rows = 5;
    public int cols = 5;


    public void GenerateGrid(int maxValue, int requiredSelections)
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        rows = 5;
        cols = 5;
        tiles = new BinaryTile[rows, cols];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                GameObject tileObj = Instantiate(bitTilePrefab, transform);
                var tile = tileObj.GetComponent<BinaryTile>();
                tile.Row = row;
                tile.Col = col;

                string value = Random.Range(0, 2).ToString();
                tile.SetValue(value);

                tiles[row, col] = tile;
            }
        }
    }

    public List<int> GenerateTargetsFromGrid(int requiredSelections, int numTargets)
    {
        List<int> targets = new List<int>();
        System.Random rand = new System.Random();

        for (int t = 0; t < numTargets; t++)
        {
            int startRow = rand.Next(0, rows);
            int startCol = rand.Next(0, cols);

            List<BinaryTile> path = new List<BinaryTile>();
            bool[,] visited = new bool[rows, cols];

            if (FindPathDFS(startRow, startCol, requiredSelections, visited, path))
            {
                string binaryString = string.Join("", path.Select(tile => tile.BitValue));
                int decimalValue = System.Convert.ToInt32(binaryString, 2);


                if (!targets.Contains(decimalValue))
                    targets.Add(decimalValue);
                else
                    t--;
            }
            else
            {
                t--;
            }
        }

        return targets;
    }

    private bool FindPathDFS(int row, int col, int length, bool[,] visited, List<BinaryTile> path)
    {
        if (row < 0 || row >= rows || col < 0 || col >= cols || visited[row, col])
            return false;

        path.Add(tiles[row, col]);
        visited[row, col] = true;

        if (path.Count == length) return true;

        int[] dr = { 1, -1, 0, 0 };
        int[] dc = { 0, 0, 1, -1 };

        foreach (var i in Enumerable.Range(0, 4))
        {
            if (FindPathDFS(row + dr[i], col + dc[i], length, visited, path))
                return true;
        }

        visited[row, col] = false;
        path.RemoveAt(path.Count - 1);
        return false;
    }



}


