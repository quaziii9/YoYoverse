using Gpm.Ui;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DiskListItem : InfiniteScrollItem, IPointerDownHandler
{
    public Image image; // 디스크 이미지

    private bool _isSelected;

    public override void UpdateData(InfiniteScrollData scrollData)
    {
        YoYoData yoyoData = scrollData as YoYoData;
        if (yoyoData == null) return;

        // 텍스트와 이미지 업데이트
        image.sprite = Resources.Load<Sprite>(yoyoData.imagePath);
    }
    
    // 터치 입력을 처리하는 메서드
    public void OnPointerDown(PointerEventData eventData)
    {
       // 선택된 디스크, 와이어에 업데이트
    }
}
