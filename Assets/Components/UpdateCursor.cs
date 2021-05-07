using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpdateCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // managed by CursorSystem

    public Texture2D cursorTexture;
    public Vector2 hotSpot = Vector2.zero;

    public static bool isOutside = true;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
        isOutside = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOutside = true;
        if (!Input.GetMouseButton(0))
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
