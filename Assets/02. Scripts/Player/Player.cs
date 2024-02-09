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

[System.Serializable]
public class Temperature
{
    [HideInInspector]
    public float starttemperature;
    public float curtemperature;
    public float maxTemp;
    public float minTemp;
    
    public Image temperGauge;
    public Image coldUI;
    public GameObject hotUI;
   
    
    public float decayRate;
    //TODO ; 추울 때 화면 진동 주면 괜찮겠지?

    public void Heat(float heat)
    {
        curtemperature += heat;
        if (curtemperature >= maxTemp)
            curtemperature = maxTemp;
        
    }

    public void Cold(float heat)
    {
        curtemperature -= heat;
        if (curtemperature <=minTemp)
            curtemperature = minTemp;
    }

    public void moveGauge()
    {
        temperGauge.transform.rotation = Quaternion.Euler(0, 0, ((curtemperature - starttemperature)/starttemperature) * -90);
        if (curtemperature > 54)
        {
            temperGauge.color = Color.red;
            hotUI.SetActive(true);
            
            //TODO : 데미지 입게 해야될 듯
        }
            
        else if (curtemperature < 18.5)
        {
            temperGauge.color = Color.blue;
            coldUI.enabled = true;
            //TODO : 이동속도 느려지고 데미지?
        }

        else
        {
            temperGauge.color = Color.black;
            hotUI.SetActive(false);
            
            coldUI.enabled = false;
        }
    }
} 

public class Player : MonoBehaviour
{
    public Condition health;
    public Condition stamina;
    public Condition hunger;
    public Condition thirsty;
    public Temperature temperature;

    public bool iswarm; //TODO : 낮 시간대는 따뜻하고 밤 시간대는 춥게
    private bool isDead = false;
    
    public float noHungerHealthDecay;

    //public event Action onTakeDamage; //TODO : 매개변수가 들어갈 수도 있음.
    
    void Start()
    {
        health.curValue = health.startValue;
        hunger.curValue = hunger.startValue;
        stamina.curValue = stamina.startValue;
        thirsty.curValue = thirsty.startValue;
        temperature.curtemperature = temperature.starttemperature;
        iswarm = false;
    }
    
    void Update()
    {
        hunger.Subtract(hunger.decayRate*Time.deltaTime);
        thirsty.Subtract(thirsty.decayRate*Time.deltaTime);
        stamina.Add(stamina.regenRate*Time.deltaTime);
        if (!iswarm)
            temperature.Cold(temperature.decayRate*Time.deltaTime);
        
        if(hunger.curValue == 0.0f || thirsty.curValue == 0.0f)
            health.Subtract(noHungerHealthDecay*Time.deltaTime);
        
        if(health.curValue == 0.0f)
            Die();

        health.uiBar.fillAmount = health.GetPercentage();
        hunger.uiBar.fillAmount = hunger.GetPercentage();
        stamina.uiBar.fillAmount = stamina.GetPercentage();
        thirsty.uiBar.fillAmount = thirsty.GetPercentage();
        temperature.moveGauge();
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
