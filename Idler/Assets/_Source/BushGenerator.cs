using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BushGenerator : MonoBehaviour
{
    [SerializeField] private GameObject bushPrefab;
    [SerializeField] private int bushCount;
    [SerializeField] private float bushDuration;
    [SerializeField] private Credits credits;
    
    [Header("Spawn Area")]
    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-5, -2);
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(5, 2);
    [SerializeField] private float minDistance;
    
    private List<Bush> _bushes = new List<Bush>();
    private System.Random _random;
    
    private void Awake()
    {
        _random = new System.Random();
        
        for (int i = 0; i < bushCount; i++)
        {
            GenerateSingleBush();
        }
    }
    
    private void GenerateSingleBush()
    {
        int attempts = 0;
        const int maxAttempts = 100;
        
        while (attempts < maxAttempts)
        {
            float x = Mathf.Lerp(spawnAreaMin.x, spawnAreaMax.x, (float)_random.NextDouble());
            float y = Mathf.Lerp(spawnAreaMin.y, spawnAreaMax.y, (float)_random.NextDouble());
            Vector2 position = new Vector2(x, y);

            if (CanSpawnAt(position))
            {
                CreateBush(position);
                return;
            }
            
            attempts++;
        }

        float fallbackX = Mathf.Lerp(spawnAreaMin.x, spawnAreaMax.x, (float)_random.NextDouble());
        float fallbackY = Mathf.Lerp(spawnAreaMin.y, spawnAreaMax.y, (float)_random.NextDouble());
        CreateBush(new Vector2(fallbackX, fallbackY));
    }
    
    private bool CanSpawnAt(Vector2 position)
    {
        foreach (var bush in _bushes)
        {
            if (bush == null) continue;
            
            float distance = Vector2.Distance(position, bush.transform.position);
            if (distance < minDistance)
            {
                return false;
            }
        }
        return true;
    }
    
    private void CreateBush(Vector2 position)
    {
        GameObject bushObj = Instantiate(bushPrefab, position, Quaternion.identity, transform);
        Bush bush = bushObj.GetComponent<Bush>();
        bush.transform.DOPunchScale(new Vector2(0.5f, 0.5f), 0.2f);
        if (bush != null)
        {
            bush.Initialize(this);
            _bushes.Add(bush);
        }
    }
    
    public void OnBushClicked(Bush bush)
    {
        credits.BushDrop();
        StartCoroutine(RespawnBush(bush));
    }
    
    private IEnumerator RespawnBush(Bush bush)
    {
        _bushes.Remove(bush);
        
        bush.transform.DOPunchScale(new Vector2(0.5f, 0.5f), 0.2f);
        yield return new WaitForSeconds(0.2f);
        Destroy(bush.gameObject);

        yield return new WaitForSeconds(bushDuration);

        GenerateSingleBush();
    }
}