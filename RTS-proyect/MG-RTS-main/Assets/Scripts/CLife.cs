using UnityEngine;

public class CLife : MonoBehaviour
{

    public float maxLife = 100f;
    public float currentLife;
    private float _currentNormalized;
    public float currentNormalized
    {
        private set { _currentNormalized = value; }
        get { return _currentNormalized; }
    }

    public bool invincible = false;

    // Start is called before the first frame update
    void Start()
    {
        ResetLife();
    }

    public bool IsAlive()
    {
        return currentLife > 0f;
    }

    public void ResetLife()
    {
        currentLife = maxLife;
        _currentNormalized = currentLife / maxLife;
    }

    public bool Damage(float damageAmount)
    {
        if (!invincible)
        {
            currentLife -= damageAmount;

            if (currentLife <= 0f)
                currentLife = 0f;

            _currentNormalized = currentLife / maxLife;
        }

        if (currentLife <= 0f)
        {
            // die
            SendMessage("UnitDied");

            return true;
        }
        else
            return false;
    }

    public void Heal(float healAmount)
    {
        _currentNormalized = currentLife / maxLife;
    }
}
