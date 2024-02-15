using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    private bool isPreviewActivated = false;

    [SerializeField] private GameObject go_BaseUI;

    [SerializeField] private Craft[] craft_build;

    private GameObject go_Preview; //�̸����� ������
    private GameObject go_Prefab; //�ν��Ͻ�ȭ �� ��ų ������

    [SerializeField] private Transform player;


    // Raycast ����
    private RaycastHit hitInfo;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float range;



    public void SlotClick(int slotNum)
    {
        go_Preview = Instantiate(craft_build[slotNum].go_PreviewPrefab, player.position + player.forward, Quaternion.identity);
        go_Prefab = craft_build[slotNum].go_Prefab;
        isPreviewActivated = true;
        go_BaseUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Cancel();

        if (Input.GetKeyDown(KeyCode.T)) //TODO : new input system���� �ٲٱ�
            Window();

        if (isPreviewActivated)
            PreviewPositionUpdate();
    }

    private void Window()
    {
        if (!isActivated)
            OpenWindow();
        else
            CloseWindow();
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
            Debug.Log("���� ������");
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
    }
}
