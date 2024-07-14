using EnumTypes;
using EventLibrary;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class UIManager : Singleton<UIManager>
{
    [HorizontalGroup("Equip UI", Title = "Equip UI")]
    [FoldoutGroup("Equip UI/Selected Disk")] public Image selectedDiskImage;
    [FoldoutGroup("Equip UI/Selected Disk")] public Image selectedDiskStatImage;
    [FoldoutGroup("Equip UI/Selected Disk")] public TMP_Text selectedDiskName;
    [FoldoutGroup("Equip UI/Selected Disk")] public TMP_Text selectedDiskAttack;
    [FoldoutGroup("Equip UI/Selected Disk")] public TMP_Text selectedDiskAttackRange;
    [FoldoutGroup("Equip UI/Selected Disk")] public TMP_Text selectedDiskAttackSpeed;

    [HorizontalGroup("Equip UI")]
    [FoldoutGroup("Equip UI/Selected Wire")] public Image selectedWireImage;
    [FoldoutGroup("Equip UI/Selected Wire")] public Image selectedWireStatImage;
    [FoldoutGroup("Equip UI/Selected Wire")] public TMP_Text selectedWireName;
    [FoldoutGroup("Equip UI/Selected Wire")] public TMP_Text selectedWireAttack;
    [FoldoutGroup("Equip UI/Selected Wire")] public TMP_Text selectedWireAttackRange;
    [FoldoutGroup("Equip UI/Selected Wire")] public TMP_Text selectedWireAttackSpeed;
    
    [HorizontalGroup("Equip UI")]
    [FoldoutGroup("Equip UI/Total Stat")] public TMP_Text totalAttack;
    [FoldoutGroup("Equip UI/Total Stat")] public TMP_Text totalAttackRange;
    [FoldoutGroup("Equip UI/Total Stat")] public TMP_Text totalAttackSpeed;

    private Dictionary<string, string> _nameTranslations = new Dictionary<string, string>()
    {
        { "Normal Disk", "노말 디스크" },
        { "Iron Disk", "철 디스크" },
        { "Blade Disk", "날 디스크" },
        { "Normal Wire", "노말 와이어" },
        { "Thorn Wire", "가시 와이어" },
        { "Electric Wire", "전기 와이어" }
    };

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

    // 이름을 한글로 변환하는 메서드
    private string TranslateName(string englishName)
    {
        if (_nameTranslations.ContainsKey(englishName))
        {
            return _nameTranslations[englishName];
        }
        return englishName; // 매핑이 없으면 영어 이름 그대로 사용
    }

    // 선택된 디스크를 업데이트하는 메서드
    private void UpdateSelectedDisk(YoYoData yoYoData)
    {
        selectedDiskImage.sprite = Resources.Load<Sprite>(yoYoData.imagePath);
        selectedDiskStatImage.sprite = Resources.Load<Sprite>(yoYoData.imagePath); // StatImage 업데이트
        selectedDiskName.text = TranslateName(yoYoData.name);
        selectedDiskAttack.text = $"공격력 : {yoYoData.attack.ToString()}";
        selectedDiskAttackRange.text = $"공격 사거리 : {yoYoData.attackRange.ToString()}";
        selectedDiskAttackSpeed.text = $"공격 속도 : {yoYoData.attackSpeed.ToString()}";
    }

    // 선택된 와이어를 업데이트하는 메서드
    private void UpdateSelectedWire(YoYoData yoYoData)
    {
        selectedWireImage.sprite = Resources.Load<Sprite>(yoYoData.imagePath);
        selectedWireStatImage.sprite = Resources.Load<Sprite>(yoYoData.imagePath); // StatImage 업데이트
        selectedWireName.text = TranslateName(yoYoData.name);
        selectedWireAttack.text = $"공격력 : {yoYoData.attack.ToString()}";
        selectedWireAttackRange.text = $"공격 사거리 : {yoYoData.attackRange.ToString()}";
        selectedWireAttackSpeed.text = $"공격 속도 : {yoYoData.attackSpeed.ToString()}";
    }
}
