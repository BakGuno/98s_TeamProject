using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    public Image image;
    public float flashSpeed;

    private Coroutine _coroutine;

    public void Flash()
    {
        if (_coroutine != null) //이전에 코루틴을 돌린 적이 있다는 소리
        {
            StopCoroutine(_coroutine);
        }

        image.GetComponent<Image>().enabled = true;
        image.color = Color.red;
        _coroutine = StartCoroutine(FadeAway());
    }

    private IEnumerator FadeAway()
    {
        float startAlpha = 0.3f;
        float a = startAlpha;

        while (a > 0.0f)
        {
            a -= (startAlpha / flashSpeed) * Time.deltaTime;
            image.color = new Color(1.0f, 0.0f, 0.0f, a);
            yield return null;
        }

        image.GetComponent<Image>().enabled = false;
    }
}
