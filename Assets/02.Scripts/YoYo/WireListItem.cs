using EnumTypes;
using EventLibrary;
using Gpm.Ui;
using UnityEngine;
using UnityEngine.UI;

public class WireListItem : InfiniteScrollItem
{
    public Image image; // 와이어 이미지
    public Button button;

    private YoYoData _yoYoData;
    private bool _isSelected;

    private void Awake()
    {
        button.onClick.AddListener(OnClickEquipListItem);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClickEquipListItem);
    }

    public override void UpdateData(InfiniteScrollData scrollData)
    {
        _yoYoData = scrollData as YoYoData;
        if (_yoYoData == null)
        {
            Debug.LogError("YoYoData is null in UpdateData");
            return;
        }

        // 텍스트와 이미지 업데이트
        image.sprite = Resources.Load<Sprite>(_yoYoData.imagePath);
    }

    // 버튼 클릭하면 선택된 아이템 업데이트
    private void OnClickEquipListItem()
    {
        if (_yoYoData == null)
        {
            Debug.LogError("YoYoData is null in OnClickEquipListItem");
            return;
        }

        Debug.Log($"Triggering OnClickWireListItem event with YoYoData: {_yoYoData.name}, {_yoYoData.imagePath}");
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickWireListItem, _yoYoData);
    }
}