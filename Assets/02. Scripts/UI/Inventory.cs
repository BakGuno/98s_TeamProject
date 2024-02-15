using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class ItemSlot
{
    public ItemData item;
    public int quantity;
}
public class Inventory : MonoBehaviour
{
    public ItemSlotUI[] uiSlots;
    public ItemSlot[] slots;

    public GameObject inventoryWindow;
    public Transform dropPosition;

    [Header("Selected Item")] 
    private ItemSlot selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemstatNames;
    public TextMeshProUGUI selectedItemstatValues;
    public GameObject UseButton;
    public GameObject equipButton;
    public GameObject unEquipButton;
    public GameObject dropButton;

    private int curEquipIndex;

    private PlayerMovements _movements;
    private Player _player;

    [Header("Events")] 
    public UnityEvent onOpenInventory;
    public UnityEvent onCloseInventory;

    public static Inventory instance;
    void Awake()
    {
        instance = this;
        _movements = GetComponent<PlayerMovements>();
        _player = GetComponent<Player>();
    }

    private void Start()
    {
        inventoryWindow.SetActive(false);
        slots = new ItemSlot[uiSlots.Length];

        for (int i = 0; i < slots.Length; i++)//각 UI슬롯들 초기화
        {
            slots[i] = new ItemSlot();
            uiSlots[i].index = i;
            uiSlots[i].Clear();
        }
        
        ClearSelectItemWindow();
    }

    public void OnInventoryButton(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            Toggle();
        }
    }
    
    public void Toggle()
    {
        if (inventoryWindow.activeInHierarchy) //하이어라키상에서 켜져있냐
        {
            inventoryWindow.SetActive(false);
            onCloseInventory?.Invoke();
            _movements.ToggleCursor(false); //커서 잠금 켜기
        }
        else
        {
            inventoryWindow.SetActive(true);
            onOpenInventory?.Invoke();
            _movements.ToggleCursor(true); //커서 잠금 풀기
        }
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    public void AddItem(ItemData item)
    {
        if (item.canStack)
        {
            ItemSlot slotToStackTo = GetItemStack(item); //쌓을수 있는거면 기존에 아이템이 있는지 비교
            if (slotToStackTo != null)
            {
                slotToStackTo.quantity++;
                UpdateUI();
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        if (emptySlot != null)
        {
            emptySlot.item = item;
            emptySlot.quantity = 1;
            UpdateUI();
            return;
        }
        ThrowItem(item); //return을 계속 걸어주는건 예외처리를 다 통과했을 떄 아이템을 못먹는 거라는 느낌
    }

    public void ThrowItem(ItemData item)
    {
        if(item.dropPrefab!=null)
            Instantiate(item.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360f));
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                uiSlots[i].Set(slots[i]); //슬롯에 있는 데이터로 최신화
            }
            else uiSlots[i].Clear();
        }
    }

    ItemSlot GetItemStack(ItemData item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item && slots[i].quantity < item.maxStackAmount)
                return slots[i];
        }

        return null;
    }

    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
                return slots[i];
        }
        return null;
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null)
            return;

        selectedItem = slots[index];
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.item.displayName;
        selectedItemDescription.text = selectedItem.item.description;

        selectedItemstatNames.text = string.Empty;
        selectedItemstatValues.text = string.Empty;

        for (int i = 0; i < selectedItem.item.consumables.Length; i++)
        {
            selectedItemstatNames.text += selectedItem.item.consumables[i].type.ToString() +"\n";
            selectedItemstatValues.text += selectedItem.item.consumables[i].value.ToString() +"\n";
        }
        
        UseButton.SetActive(selectedItem.item.type==ItemType.Consumabable);
        equipButton.SetActive(selectedItem.item.type==ItemType.Equipable && !uiSlots[index].equipped);
        unEquipButton.SetActive(selectedItem.item.type==ItemType.Equipable&& uiSlots[index].equipped);
        dropButton.SetActive(true);
    }
    
    public void ClearSelectItemWindow()
    {
        selectedItem = null;
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;

        selectedItemstatNames.text = string.Empty;
        selectedItemstatValues.text = string.Empty;
        
        UseButton.SetActive(false);
        equipButton.SetActive(false);
        unEquipButton.SetActive(false);
        dropButton.SetActive(false);
    }

    public void OnUseButton()
    {
        if (selectedItem.item.type == ItemType.Consumabable)
        {
            for (int i = 0; i < selectedItem.item.consumables.Length; i++)
            {
                switch (selectedItem.item.consumables[i].type)//enum이 나오면 왠만하면 스위치문
                {
                    case ConsumableType.체력:
                        _player.Heal(selectedItem.item.consumables[i].value);
                        break;
                    case ConsumableType.배고픔:
                        _player.Eat(selectedItem.item.consumables[i].value);
                        break;
                    case ConsumableType.목마름:
                        _player.Drink(selectedItem.item.consumables[i].value);
                        _player.temperature.curtemperature--;
                        break;
                }
            }
        }
        RemoveSelectedItem();
    }

    public void OnEquipButton()
    {
        if (uiSlots[curEquipIndex].equipped)
        {
            UnEquip(curEquipIndex);
        }

        uiSlots[selectedItemIndex].equipped = true;
        curEquipIndex = selectedItemIndex;
        EquipManager.instance.EquipNew(selectedItem.item);
        UpdateUI();
        
        SelectItem(selectedItemIndex);
    }

    void UnEquip(int index)
    {
        uiSlots[index].equipped = false;
        EquipManager.instance.UnEquip();
        UpdateUI();
        
        if (selectedItemIndex == index)
            SelectItem(index);
    }
    
    public void OnUnequipButton()
    {
        UnEquip(selectedItemIndex);
    }

    public void OnDropButton()
    {
        ThrowItem(selectedItem.item);
        RemoveSelectedItem();
    }

    private void RemoveSelectedItem()
    {
        selectedItem.quantity--;
        if (selectedItem.quantity <= 0)
        {
            if (uiSlots[selectedItemIndex].equipped)
            {
                UnEquip(selectedItemIndex); 
            }

            selectedItem.item = null;
            ClearSelectItemWindow();
        }
        UpdateUI();
    }

    public void RemoveItem(ItemData item)
    {
        
    }

    public bool HasItems(ItemData item, int quantity)
    {
        return false;
    }
}
