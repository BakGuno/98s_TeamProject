using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraShake : MonoBehaviour
{
    private Camera _camera; // 카메라의 Transform 컴포넌트
    private Transform _cameraTransform;

    // 흔들림의 강도와 빈도
    public float shakeIntensity = 0.1f;
    public float shakeSpeed = 50.0f;

    private WaitForSeconds wait = new WaitForSeconds(2f);
    private float shakeDuration = 2f;
    private bool isShake = false;

    // 원래 카메라의 위치
    private Vector3 originalPos;

    void Awake()
    {
        _camera = Camera.main;
        if (_camera != null)
        {
            _cameraTransform = _camera.GetComponent<Transform>();
        }
    }

    void Start()
    {
        originalPos = _cameraTransform.localPosition; // 원래 위치 저장
    }

    public void StartShake()
    {
        if (GameManager.instance.player._coroutine != null)
        {
            return;
        }
        GameManager.instance.player._coroutine = StartCoroutine(nameof(Shake));
        isShake = true;
    }

    public void StopShake()
    {
        //StopCoroutine(nameof(Shake));
        isShake = false;
        GameManager.instance.player._coroutine = null;
    }

    // 카메라 흔들기 함수
    public IEnumerator Shake() //TODO : 좋아보이진 않는다. 다시 검토할 필요가 있음
    {
        while (isShake) //요기 / 코루틴은 사용하면 항상 파괴점이 있어야하는데 여기는 없음.
        {
            
            float shakeTime = 0f;
            while (shakeTime < shakeDuration)
            {
                shakeTime += Time.deltaTime;
                Vector3 shakePos = originalPos + Random.insideUnitSphere * shakeIntensity;
                _cameraTransform.localPosition =
                    Vector3.Lerp(_cameraTransform.localPosition, shakePos, Time.deltaTime * shakeSpeed);
                yield return null;    
            }
            yield return wait;
        }
        _cameraTransform.localPosition = originalPos;
    }
}