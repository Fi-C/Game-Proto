using UnityEngine;

public class PlayerPaint : MonoBehaviour
{
    public Color currentColor = Color.white;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Paintable paintable = col.gameObject.GetComponent<Paintable>();

        if (paintable != null)
        {
            paintable.ApplyColor(currentColor);
        }
    }

    public void SetColor(Color newColor)
    {
        currentColor = newColor;
        sr.color = newColor;
    }
}