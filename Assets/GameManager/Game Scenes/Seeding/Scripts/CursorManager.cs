using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture;
    private Vector2 cursorHotspot;

    void Start()
    {
        // Add null checking until textures are added
        if (cursorTexture != null)
        {
            cursorHotspot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 32);
            Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
        }
    }
}
