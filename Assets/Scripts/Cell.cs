using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Cell : MonoBehaviour
{
    public bool alive;
    SpriteRenderer spriteRenderer;
    public int XIndex { get; set; }
    public int YIndex { get; set; }

    public void UpdateStatus()
    {
        spriteRenderer ??= GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = alive;
    }
}