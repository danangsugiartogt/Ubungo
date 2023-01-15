using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private int rows;
    [SerializeField] private int column;
    [SerializeField] private int tileSize;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private float offsetCenter = 0.5f;

    private Vector2[,] grids;

    void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        grids = new Vector2[rows, column];

        for(int i = 0; i < rows; i++)
        {
            for(int j = 0; j < column; j++)
            {
                float postX = i * tileSize;
                float postY = j * -tileSize;

                grids[i, j] = new Vector2(postX, postY);

                var tile = Instantiate(tilePrefab, new Vector3(postX, postY, 0), Quaternion.identity, transform);
            }
        }

        float gridWidth = rows * tileSize;
        float gridHeight = column * tileSize;

        transform.position = new Vector3((-gridWidth / 2 + tileSize / 2) + offsetCenter, (gridHeight / 2 - tileSize / 2) - offsetCenter, 0);
    }
}
