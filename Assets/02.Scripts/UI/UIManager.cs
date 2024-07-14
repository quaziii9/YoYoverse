using EnumTypes;
using EventLibrary;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public GameObject selectedDisk;
    public GameObject selectedWire;
    public Image selectedDiskImage;
    public Image selectedWireImage;
    public TMP_Text selectedDiskName;
    public TMP_Text selectedWireName;

    protected override void Awake()
    {
        base.Awake();
        AddEvents(); // 이벤트 리스너 등록
    }

    protected virtual void OnDestroy()
    {
        RemoveEvents(); // 이벤트 리스너 제거
    }

    // 이벤트를 등록하는 메서드
    protected virtual void AddEvents()
    {
        EventManager<UIEvents>.StartListening<YoYoData>(UIEvents.OnClickDiskListItem, UpdateSelectedDisk);
        EventManager<UIEvents>.StartListening<YoYoData>(UIEvents.OnClickWireListItem, UpdateSelectedWire);
    }

    // 이벤트 리스너를 제거하는 메서드
    protected virtual void RemoveEvents()
    {
        EventManager<UIEvents>.StopListening<YoYoData>(UIEvents.OnClickDiskListItem, UpdateSelectedDisk);
        EventManager<UIEvents>.StopListening<YoYoData>(UIEvents.OnClickWireListItem, UpdateSelectedWire);
    }

    // 선택된 디스크를 업데이트하는 메서드
    private void UpdateSelectedDisk(YoYoData yoYoData)
    {
        selectedDiskImage.sprite = Resources.Load<Sprite>(yoYoData.imagePath);
        selectedDiskName.text = yoYoData.name;
    }

    // 선택된 와이어를 업데이트하는 메서드
    private void UpdateSelectedWire(YoYoData yoYoData)
    {
        selectedWireImage.sprite = Resources.Load<Sprite>(yoYoData.imagePath);
        selectedWireName.text = yoYoData.name;
    }
}