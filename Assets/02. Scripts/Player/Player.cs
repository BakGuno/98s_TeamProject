using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public interface IDamagable
{
    void TakePhysicalDamage(int damageAmount);
    void TakePhysicalBuff(int damageAmount);
}

[System.Serializable]
public class Condition
{
    [HideInInspector]public float curValue;
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
    public float starttemperature;
    public float curtemperature;
    public float maxTemp;
    public float minTemp;


    public Image temperGauge;
    public GameObject coldUI;
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
        if (curtemperature <= minTemp)
            curtemperature = minTemp;
    }

    public void moveGauge()
    {
        temperGauge.transform.rotation =
            Quaternion.Euler(0, 0, ((curtemperature - starttemperature) / starttemperature) * -90);
        if (curtemperature > 54)
        {
            temperGauge.color = Color.red;
            hotUI.SetActive(true);

            //TODO : 데미지 입게 해야될 듯
        }

        else if (curtemperature < 18.5)
        {
            temperGauge.color = Color.blue;
            coldUI.SetActive(true);
            //TODO : 이동속도 느려지고 데미지?
        }

        else
        {
            temperGauge.color = Color.black;
            hotUI.SetActive(false);
            coldUI.SetActive(false);
        }
    }
}

public class Player : MonoBehaviour,IDamagable
{
    public Condition health;
    public Condition mental;
    public Condition stamina;
    public Condition hunger;
    public Condition thirsty;
    public Temperature temperature;

    public bool takeRest = false;
    public bool hasLight = false;

    public bool iswarm; //TODO : 낮 시간대는 따뜻하고 밤 시간대는 춥게
    public bool hasTorch = false;
    [HideInInspector]public bool isDead = false;
    private bool isCold;
    public bool isrun;
    

    public float noHungerHealthDecay;
    public float useSprintStamina;
    public float thirstyMultiplier=1f;

    public CameraShake _cameraShake;
    public Coroutine _coroutine;
    public Animator _animator;

    public UnityEvent onTakeDamage;
    public UnityEvent onTakeButt;//TODO : 데미지 인디케이터 UI 등록하기.

    public event Action OnDieEvnet;

    private PlayerMovements _movements;

    public GameObject mentalWarnUI;

    private void Awake()
    {
        _cameraShake = GetComponent<CameraShake>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        _movements = GetComponent<PlayerMovements>();
        health.curValue = health.startValue;
        hunger.curValue = hunger.startValue;
        stamina.curValue = stamina.startValue;
        thirsty.curValue = thirsty.startValue;
        temperature.curtemperature = temperature.starttemperature;
        mental.curValue = mental.startValue;
        iswarm = false;
        isCold = false;
        isrun = false;
    }

    void Update()
    {
        if (takeRest) //TODO : 리지드바디 넣어서 자식오브젝트로 동작시킬꺼면 여기도 손봐야됨(횃불 꺼질때 takeRest도 꺼지게)
        {
            iswarm = true;
            hasLight = true;
        }
        else
        {
            
            if (GameManager.instance.daytime == _Time.Day)
            {
                
                hasLight = true;
                iswarm = true;
                
            }
            else
            {
                
                hasLight = false;
                iswarm = false;
            }
        }

        hunger.Subtract(hunger.decayRate * Time.deltaTime);
        thirsty.Subtract(thirsty.decayRate*thirstyMultiplier * Time.deltaTime);

        if (!hasLight)
            mental.Subtract(mental.decayRate * Time.deltaTime);

        if (!iswarm)
        {
            temperature.Cold(temperature.decayRate * Time.deltaTime);
        }

        if (temperature.temperGauge.transform.rotation.z >= 0.35)
        {
            isCold = true;
            _movements.speedMultiflier = 0.5f;
            _cameraShake.StartShake();
        }
        else
        {
            if (isCold)
            {
                _movements.speedMultiflier = 1f;
                _cameraShake.StopShake();
                isCold = false;
            }
            
        }
        //TODO : 카메라 떨림 수정해야됨. 밖으로 뺴니까 생각하던것처럼 동작하긴 하는데 코드는 이런게 아님.

        if (temperature.temperGauge.transform.rotation.z <= -0.35)
        {
            thirstyMultiplier = 1.5f;
        }
        else
        {
            thirstyMultiplier = 1f;
        }


        if (hunger.curValue == 0.0f || thirsty.curValue == 0.0f)
            health.Subtract(noHungerHealthDecay * Time.deltaTime);

        if (health.curValue == 0.0f)
            Die();

        if (isrun)
        {
            stamina.Subtract(useSprintStamina*Time.deltaTime);
            if (stamina.curValue == 0)
            {
                isrun = false;
                _movements.curSpeed = _movements.moveSpeed;
                _animator.SetBool("Sprint",false);
            }
        }
        else
        {
            stamina.Add(stamina.regenRate * Time.deltaTime);    
        }
        
        if (mental.GetPercentage() <=0.35f)
            mentalWarnUI.SetActive(true);
        else mentalWarnUI.SetActive(false);

        UIUpdate();
    }

   
    void UIUpdate()
    {
        health.uiBar.fillAmount = health.GetPercentage();
        hunger.uiBar.fillAmount = hunger.GetPercentage();
        stamina.uiBar.fillAmount = stamina.GetPercentage();
        thirsty.uiBar.fillAmount = thirsty.GetPercentage();
        mental.uiBar.fillAmount = mental.GetPercentage();
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

    public void Drink(float amount)
    {
        thirsty.Add(amount);
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
        if (isDead)
        {
            return;
        }
        isDead = true;
        _animator.SetTrigger("Dead");
        enabled = false;
        OnDieEvnet?.Invoke();
        
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        health.Subtract(damageAmount);
        _animator.SetTrigger("Hit");
        onTakeDamage?.Invoke();
    }
    public void TakePhysicalBuff(int damageAmount)
    {
        stamina.Subtract(damageAmount);
        StartBuffShake();
        onTakeButt?.Invoke();
        Invoke(nameof(StopBuffShake),2f);
    }

    void StartBuffShake()
    {
        _cameraShake.StartShake();
    }
    
    void StopBuffShake()
    {
        _cameraShake.StopShake();
    }
}