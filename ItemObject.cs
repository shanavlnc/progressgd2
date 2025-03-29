using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(ItemDragHandler))]
public class ItemObject : MonoBehaviour
{
    public string itemName;
    private SpriteRenderer spriteRenderer;
    private PolygonCollider2D polyCollider;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        polyCollider = GetComponent<PolygonCollider2D>();
        if (polyCollider == null)
        {
            polyCollider = gameObject.AddComponent<PolygonCollider2D>();
            polyCollider.isTrigger = true;
        }
    }

    public void Setup(string foodName)
    {
        itemName = foodName;
        spriteRenderer.sprite = Resources.Load<Sprite>($"FoodIcons/{foodName}");
        gameObject.name = $"Item_{foodName}";
        UpdateCollider();
    }

    void UpdateCollider()
    {
        if (polyCollider != null && spriteRenderer.sprite != null)
        {
            polyCollider.pathCount = 0;
            var path = new List<Vector2>();
            spriteRenderer.sprite.GetPhysicsShape(0, path);
            polyCollider.pathCount = 1;
            polyCollider.SetPath(0, path.ToArray());
        }
    }

    void OnMouseEnter()
    {
        if (Input.GetMouseButton(0) && GameManager.Instance != null)
        {
            GameManager.Instance.SetHoveredItem(this);
        }
    }

    void OnMouseExit()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetHoveredItem(null);
        }
    }
}
