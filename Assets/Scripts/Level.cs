using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [System.Serializable]
    public class Tile
    {
        public Vector2 position;
        public bool isClaimed;
    }

    private List<Tile> tileList = new List<Tile>();

    private void Awake()
    {
        CollectTiles();
    }

    private void CollectTiles()
    {
        foreach(Transform tf in transform)
        {
            Tile tile = new Tile()
            {
                position = tf.position,
                isClaimed = false
            };

            tileList.Add(tile);
        }
    }

    public bool TryToClaimTiles(Vector2[] positions)
    {
        List<Tile> validTiles = new List<Tile>();
        for(int i = 0; i < positions.Length; i++)
        {
            var position = positions[i];

            var validTile = tileList.Find(tile => tile.position == position && tile.isClaimed == false);
            if (validTile == null)
            {
                return false;
            }

            validTiles.Add(validTile);
        }

        foreach(var tile in validTiles)
        {
            tile.isClaimed = true;
        }

        return true;
    }

    public void RemoveClaimedTiles(Vector2[] positions)
    {
        for(int i = 0; i < positions.Length; i++)
        {
            var position = positions[i];
            var validTile = tileList.Find(tile => tile.position == position);
            if (validTile != null)
            {
                validTile.isClaimed = false;
            }
        }
    }
}
