using UnityEngine;
using FYFY;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CursorSystem : FSystem {

    public static CursorSystem instance;

    public CursorSystem()
    {
        if (Application.isPlaying)
        {
            
        }
        instance = this;
    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount)
    {
        if (Input.GetMouseButtonUp(0) && UpdateCursor.isOutside)
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}