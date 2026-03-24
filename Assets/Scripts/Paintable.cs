using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Paintable : MonoBehaviour
{
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void ApplyColor(Color newColor)
    {
        sr.color = newColor;
    }
}