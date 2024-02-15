using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

[System.Serializable]
public class Craft
{
    public string craftName;
    public GameObject go_Prefab;
    public GameObject go_PreviewPrefab;
}

public class CraftMenu : MonoBehaviour
{
    private bool isActivated = false;
    public bool isPreviewActivated { get; set; }

    [SerializeField] private GameObject go_BaseUI;

    [SerializeField] private Craft[] craft_build;

    private GameObject go_Preview; //미리보기 프리팹
    private GameObject go_Prefab; //인스턴스화 할 시킬 프리팹

    [SerializeField] private Transform player;


    // Raycast 변수
    private RaycastHit hitInfo;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float range;

    private PlayerMovements _movements;

    private void Start()
    {
        _movements = GetComponent<PlayerMovements>();
        isPreviewActivated = false;
    }

    public void SlotClick(int slotNum)
    {
        go_Preview = Instantiate(craft_build[slotNum].go_PreviewPrefab, player.position + player.forward, Quaternion.identity);
        go_Prefab = craft_build[slotNum].go_Prefab;
        isPreviewActivated = true;
        go_BaseUI.SetActive(false);
        _movements.ToggleCursor(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Cancel();

        /*if (Input.GetKeyDown(KeyCode.T)) //TODO : new input system으로 바꾸기
            Window();*/

        if (isPreviewActivated)
            PreviewPositionUpdate();
    }

    public void OnStructureButton(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            Window();
        }
    }

    private void Window()
    {
        if (!isActivated)
        {
            if (isPreviewActivated)
            {
                Destroy(go_Preview);
                isPreviewActivated = false;
            }
            OpenWindow();
            _movements.ToggleCursor(true);
        }
        else
        {
            CloseWindow();
            _movements.ToggleCursor(false);
        }
    }

    private void OpenWindow()
    {
        isActivated = true;
        go_BaseUI.SetActive(true);
    }

    private void CloseWindow()
    {
        isActivated = false;
        go_BaseUI.SetActive(false);
    }

    private void PreviewPositionUpdate()
    {
        if(Physics.Raycast(player.position, player.forward, out hitInfo ,range, layerMask))
        {
            go_Preview.SetActive(true);
            Debug.Log("레이 도달함");
            if (hitInfo.transform != null)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                    go_Preview.transform.Rotate(0,-90f,0);
                else if (Input.GetKeyDown(KeyCode.E))
                    go_Preview.transform.Rotate(0, 90f, 0);

                Vector3 point = hitInfo.point;
                point.Set(Mathf.Round(point.x), Mathf.Round(point.y/0.1f)*0.1f, Mathf.Round(point.z));
                go_Preview.transform.position = point;
            }
        }
        else
        { go_Preview.SetActive(false); }
    }

    public void OnBuildInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            Build();
    }

    public void Build()
    {
        if(isPreviewActivated && go_Preview.GetComponent<PreviewObject>().IsBuildable())
        {
            Instantiate(go_Prefab, go_Preview.transform.position, go_Preview.transform.rotation);
            Destroy(go_Preview);

            isActivated = false;
            isPreviewActivated = false;
            go_Preview = null;
            go_Prefab = null;
        }
    }

    private void Cancel()
    {
        if (isPreviewActivated)
            Destroy(go_Preview);

        isActivated = false;
        isPreviewActivated = false;

        go_Preview = null;

        go_Prefab = null;

        go_BaseUI.SetActive(false);

        _movements.ToggleCursor(false);
    }
}
