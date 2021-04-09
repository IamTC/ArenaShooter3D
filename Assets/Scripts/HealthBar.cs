using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider healthBar;
    public PlayerHealth health;
    // Start is called before the first frame update
    void Start()
    {
        health = GameObject.FindWithTag("PlayerTag").GetComponent<PlayerHealth>();
        healthBar = GetComponent<Slider>();
        healthBar.maxValue = health.MAX_HEALTH;
        healthBar.value = health.MAX_HEALTH;

        var healthBarCanvas = healthBar.GetComponent<RectTransform>();
        healthBarCanvas.anchoredPosition = new Vector3((healthBarCanvas.rect.width / 2) - Screen.width + 20, Screen.height - (healthBarCanvas.rect.height / 2) - 20);

    }

    public void SetHealth(int health)
    {
        healthBar.value = health;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
