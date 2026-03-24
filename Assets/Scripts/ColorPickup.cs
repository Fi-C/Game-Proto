using UnityEngine;

public class ColorPickup : MonoBehaviour
{
    public Color pickupColor = Color.red;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerPaint player = other.GetComponent<PlayerPaint>();

        if (player != null)
        {
            player.SetColor(pickupColor);
            Destroy(gameObject);
        }
    }
}