using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Prefabs")]
    public GameObject foodItemPrefab;
    public ParticleSystem discoveryEffect;

    [Header("UI Elements")]
    public TextMeshProUGUI discoveryText;
    public Button clearButton;
    public GameObject victoryPanel;

    [Header("Game Data")]
    public List<FoodRecipe> recipes = new();
    public string[] starterItems = { "Bread", "Cheese", "Egg", "Rice", "Chicken" };
    public Vector2 spawnAreaMin = new(-5f, -3f);
    public Vector2 spawnAreaMax = new(5f, 3f);

    private readonly List<string> discoveredItems = new();
    private ItemObject draggedItem;
    private ItemObject hoveredItem;
    private const int TOTAL_DISCOVERIES = 30;
    private bool isProcessingCombination;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (clearButton != null) clearButton.onClick.AddListener(ClearAllItems);
        discoveredItems.Clear();
        UpdateDiscoveryUI();
        SpawnStarterItems();
    }

    #region Drag System
    public void SetDraggedItem(ItemObject item) => draggedItem = item;
    public void SetHoveredItem(ItemObject item) => hoveredItem = item;
    public bool IsCombining() => isProcessingCombination;

    public void TryCombineItems()
    {
        if (isProcessingCombination || draggedItem == null || hoveredItem == null)
        {
            Debug.Log(draggedItem == null ? "No dragged item" : hoveredItem == null ? "No hovered item" : "Already processing");
            return;
        }

        if (draggedItem == hoveredItem)
        {
            Debug.Log("Self-combination blocked");
            return;
        }

        isProcessingCombination = true;
        Debug.Log($"Attempting combination: {draggedItem.itemName} + {hoveredItem.itemName}");

        foreach (FoodRecipe recipe in recipes)
        {
            if (IsValidCombination(recipe, draggedItem.itemName, hoveredItem.itemName))
            {
                HandleSuccessfulCombination(recipe);
                break;
            }
        }

        isProcessingCombination = false;
        draggedItem = null;
        hoveredItem = null;
    }
    #endregion

    #region Game Logic
    void HandleSuccessfulCombination(FoodRecipe recipe)
    {
        Vector2 spawnPos = (draggedItem.transform.position + hoveredItem.transform.position) / 2f;
        bool isNew = !discoveredItems.Contains(recipe.result);

        Destroy(draggedItem.gameObject);
        Destroy(hoveredItem.gameObject);
        SpawnItem(recipe.result, spawnPos);

        if (isNew)
        {
            discoveredItems.Add(recipe.result);
            UpdateDiscoveryUI();
            PlayDiscoveryEffect(spawnPos);
            CheckForVictory();
        }
    }

    void SpawnStarterItems()
    {
        foreach (string item in starterItems)
        {
            SpawnItem(item, GetRandomSpawnPosition(), false);
        }
    }

    Vector2 GetRandomSpawnPosition()
    {
        return new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );
    }

    public void SpawnItem(string itemName, Vector2? pos = null, bool countAsDiscovered = true)
    {
        Vector2 spawnPos = pos ?? GetRandomSpawnPosition();
        GameObject newItem = Instantiate(foodItemPrefab, spawnPos, Quaternion.identity);
        ItemObject itemObj = newItem.GetComponent<ItemObject>();
        itemObj.Setup(itemName);

        if (countAsDiscovered && !discoveredItems.Contains(itemName))
        {
            discoveredItems.Add(itemName);
            UpdateDiscoveryUI();
        }
    }

    bool IsValidCombination(FoodRecipe recipe, string item1, string item2)
    {
        return (recipe.item1 == item1 && recipe.item2 == item2) ||
               (recipe.item1 == item2 && recipe.item2 == item1);
    }

    void PlayDiscoveryEffect(Vector2 position)
    {
        if (discoveryEffect != null)
        {
            ParticleSystem effect = Instantiate(discoveryEffect, position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration);
        }
    }
    #endregion

    #region UI Management
    void UpdateDiscoveryUI()
    {
        if (discoveryText != null)
        {
            discoveryText.text = $"{discoveredItems.Count} / {TOTAL_DISCOVERIES}";
        }
    }

    void CheckForVictory()
    {
        if (discoveredItems.Count >= TOTAL_DISCOVERIES && victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
    }

    public void ClearAllItems()
    {
        ItemObject[] items = FindObjectsOfType<ItemObject>();
        foreach (ItemObject item in items)
        {
            Destroy(item.gameObject);
        }

        discoveredItems.Clear();
        UpdateDiscoveryUI();
        SpawnStarterItems();
    }
    #endregion
}

[System.Serializable]
public class FoodRecipe
{
    public string item1;
    public string item2;
    public string result;
}
