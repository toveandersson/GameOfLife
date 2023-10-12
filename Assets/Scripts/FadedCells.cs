using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class FadedCells : MonoBehaviour
{
    public bool alive;
    public bool nPressed = false;
    SpriteRenderer spriteRenderer;
    Color32 color;  //röd,grå,blå,alfa
    float fade = 1;

    public void UpdateStatus()
    {
        spriteRenderer ??= GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = alive;
        
    }
    private void Update()
    {
        if (alive)
        {
            fade -= 0.4f;
            //fade = Mathf.Clamp(fade, 0.4f, 1);
            color = new UnityEngine.Color(0, 0, 0, fade);
        }
    }

}
