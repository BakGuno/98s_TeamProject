using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Condition
{
    [HideInInspector]
    public float curValue;
    public float maxValue;
    public float startValue;
    public float regenRate;
    public float decayRate;
    public Image uiBar;

    public void Add(float amount)
    {
        curValue = Mathf.Min(curValue + amount, maxValue); //maxValue를 안 넘기 위해 비교해서 사용
    }

    public void Subtract(float amount)
    {
        curValue = Mathf.Max(curValue - amount, 0.0f); //0보다는 낮아지지 않게
    }
    public float GetPercentage()
    {
        return curValue / maxValue;
    }
}

public class Player : MonoBehaviour
{
    public Condition health;
    public Condition stamina;
    public Condition hunger;
    public Condition thirsty;
    public float temperature;
    
    private bool isDead = false;
    
    public float noHungerHealthDecay;

    //public event Action onTakeDamage; //TODO : 매개변수가 들어갈 수도 있음.
    
    void Start()
    {
        health.curValue = health.startValue;
        hunger.curValue = hunger.startValue;
        stamina.curValue = stamina.startValue;
        thirsty.curValue = thirsty.startValue;
        temperature = 36.5f;
    }
    
    void Update()
    {
        hunger.Subtract(hunger.decayRate*Time.deltaTime);
        thirsty.Subtract(thirsty.decayRate*Time.deltaTime);
        stamina.Add(stamina.regenRate*Time.deltaTime);
        
        if(hunger.curValue == 0.0f)
            health.Subtract(noHungerHealthDecay*Time.deltaTime);
        
        if(health.curValue == 0.0f)
            Die();

        health.uiBar.fillAmount = health.GetPercentage();
        hunger.uiBar.fillAmount = hunger.GetPercentage();
        stamina.uiBar.fillAmount = stamina.GetPercentage();
        thirsty.uiBar.fillAmount = thirsty.GetPercentage();
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void Eat(float amount)
    {
        hunger.Add(amount);
    }

    public bool UseStamina(float amount)
    {
        if (stamina.curValue - amount < 0)
            return false;
        
        stamina.Subtract(amount);
        return true;
    }

    public void Die()
    {
        isDead = true;
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        health.Subtract(damageAmount);
        //onTakeDamage?.Invoke();
    }
}
