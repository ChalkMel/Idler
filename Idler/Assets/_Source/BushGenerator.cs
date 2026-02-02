using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BushGenerator : MonoBehaviour
{
    [SerializeField] private Tile bushTile;
    [SerializeField] private int bushCount;
    [SerializeField] private float bushDuration;
    [SerializeField] private Credits credits;
        
    private Tilemap _tilemap;
    private System.Random _random;
        
    private const int MAX_X = 9;
    private const int MAX_Y = 6;
    private const int MIN = 0;

    private void Awake()
    {
        _random = new System.Random();
        _tilemap = GetComponent<Tilemap>();
        for (int i = 0; i < bushCount; i++)
        {
            GenerateSingleBush();
        }
    }

    private void GenerateSingleBush()
    {
        int attempts = 0;
        const int maxAttempts = 10;

        while (attempts < maxAttempts)
        {
            int x = _random.Next(MIN, MAX_X);
            int y = _random.Next(MIN, MAX_Y);

            Vector3Int position = new Vector3Int(x, y, 0);

            if (!_tilemap.HasTile(position))
            {
                if (HasTouch(position))
                {
                    attempts++;
                    continue;
                }

                _tilemap.SetTile(position, bushTile);
                return;
            }

            attempts++;
        }
    }

    private bool HasTouch(Vector3Int position)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                Vector3Int neighborPos = new Vector3Int(
                    position.x + dx,
                    position.y + dy,
                    0
                );

                if (_tilemap.HasTile(neighborPos))
                {
                    return true;
                }
            }
        }

        return false;
    }
        

    private void OnMouseDown()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = _tilemap.WorldToCell(worldPos);
        cellPos.z = 0;

        credits.BushDrop();
            
        if (_tilemap.HasTile(cellPos))
        {
            StartCoroutine(RespawnBush(cellPos));
        }
    }

    private IEnumerator RespawnBush(Vector3Int removedPosition)
    {
        _tilemap.SetTile(removedPosition, null);
        yield return new WaitForSeconds(bushDuration);
        GenerateSingleBush();
    }
}