using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int Health = 0;
    public int MAX_HEALTH = 100;

    public HealthBar healthBar;
    // Start is called before the first frame update
    void Start()
    {
        healthBar = GameObject.FindWithTag("HealthBar").GetComponent<HealthBar>();
        Health = MAX_HEALTH;
    }

    public void TakeDamage(int dmg)
    {
        Health -= dmg;
        healthBar.SetHealth(Health);
    }
}
