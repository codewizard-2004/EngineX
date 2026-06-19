using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [SerializeField] private int coinPickupCount = 12;
    [SerializeField] private int fuelPickupCount = 5;
    [SerializeField] private float clearRadius = 0.8f;
    [SerializeField] private int maxPlacementAttempts = 80;
    [SerializeField] private Vector2 fallbackMinBounds = new Vector2(-14f, -2.5f);
    [SerializeField] private Vector2 fallbackMaxBounds = new Vector2(14f, 7f);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (FindFirstObjectByType<PickupSpawner>() != null) return;
        if (FindFirstObjectByType<Lander>() == null) return;

        GameObject spawnerObject = new GameObject("Pickup Spawner");
        spawnerObject.AddComponent<PickupSpawner>();
    }

    private void Start()
    {
        SpawnPickups(FindObjectsByType<CoinPickup>(FindObjectsSortMode.None), coinPickupCount);
        SpawnPickups(FindObjectsByType<FuelPickup>(FindObjectsSortMode.None), fuelPickupCount);
    }

    private void SpawnPickups<T>(T[] existingPickups, int targetCount) where T : Component
    {
        if (existingPickups == null || existingPickups.Length == 0 || targetCount <= 0) return;

        GameObject template = existingPickups[0].gameObject;
        List<GameObject> pickups = new List<GameObject>();

        for (int i = 0; i < existingPickups.Length && pickups.Count < targetCount; i++)
        {
            pickups.Add(existingPickups[i].gameObject);
        }

        while (pickups.Count < targetCount)
        {
            GameObject pickup = Instantiate(template, transform);
            pickup.name = template.name;
            pickups.Add(pickup);
        }

        foreach (GameObject pickup in pickups)
        {
            pickup.transform.position = GetRandomClearPosition();
            pickup.SetActive(true);
        }
    }

    private Vector3 GetRandomClearPosition()
    {
        Rect spawnRect = GetSpawnRect();

        for (int attempt = 0; attempt < maxPlacementAttempts; attempt++)
        {
            Vector3 position = new Vector3(
                Random.Range(spawnRect.xMin, spawnRect.xMax),
                Random.Range(spawnRect.yMin, spawnRect.yMax),
                0f);

            if (IsClearPosition(position))
            {
                return position;
            }
        }

        return new Vector3(
            Random.Range(spawnRect.xMin, spawnRect.xMax),
            Random.Range(spawnRect.yMin, spawnRect.yMax),
            0f);
    }

    private Rect GetSpawnRect()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null || !mainCamera.orthographic)
        {
            return Rect.MinMaxRect(fallbackMinBounds.x, fallbackMinBounds.y, fallbackMaxBounds.x, fallbackMaxBounds.y);
        }

        float height = mainCamera.orthographicSize * 2f;
        float width = height * mainCamera.aspect;
        Vector3 cameraPosition = mainCamera.transform.position;
        float padding = 1.25f;

        return Rect.MinMaxRect(
            cameraPosition.x - width * 0.5f + padding,
            cameraPosition.y - height * 0.5f + padding,
            cameraPosition.x + width * 0.5f - padding,
            cameraPosition.y + height * 0.5f - padding);
    }

    private bool IsClearPosition(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, clearRadius);
        foreach (Collider2D collider2D in colliders)
        {
            if (collider2D == null || collider2D.isTrigger) continue;
            return false;
        }

        Lander lander = FindFirstObjectByType<Lander>();
        if (lander == null) return true;

        return Vector2.Distance(position, lander.transform.position) > clearRadius * 2f;
    }
}
