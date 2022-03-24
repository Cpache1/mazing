using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    private Slider healthBar;
    private PlayerHealth health;
    Transform player;

    void Awake()
    {
        healthBar = GetComponent<Slider>();
        player = GameObject.Find("PlayerController").transform;
        health = player.GetComponent<PlayerHealth>();
    }

    void Update()
    {
        healthBar.value = health.health;
        transform.position = new Vector3(player.position.x, player.position.y - 1f, -1);
    }
}
