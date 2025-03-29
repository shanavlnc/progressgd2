using UnityEngine;

public class ItemDragHandler : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging;
    private SpriteRenderer spriteRenderer;
    private int originalSortOrder;
    private Collider2D col;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        originalSortOrder = spriteRenderer.sortingOrder;
    }

    void OnMouseDown()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsCombining()) return;

        isDragging = true;
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spriteRenderer.sortingOrder = 10;
        if (col != null) col.enabled = false;
        GameManager.Instance.SetDraggedItem(GetComponent<ItemObject>());
    }

    void OnMouseDrag()
    {
        if (!isDragging || col == null) return;

        Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        newPos.z = 0;
        transform.position = newPos;
    }

    void OnMouseUp()
    {
        if (!isDragging) return;

        isDragging = false;
        spriteRenderer.sortingOrder = originalSortOrder;
        if (col != null) col.enabled = true;
        GameManager.Instance.TryCombineItems();
    }
}
