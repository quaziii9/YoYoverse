using EnumTypes;
using EventLibrary;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class UIManager : Singleton<UIManager>
{
    #region Variable
    
    // Equip UI
    [FoldoutGroup("Equip UI")] [PropertySpace(0f, 5f)] public GameObject equipUI;
    
    [HorizontalGroup("Equip UI/Horizontal")]
    [FoldoutGroup("Equip UI/Horizontal/Selected Disk")] public Image selectedDiskImage;
    [FoldoutGroup("Equip UI/Horizontal/Selected Disk")] public Image selectedDiskStatImage;
    [FoldoutGroup("Equip UI/Horizontal/Selected Disk")] public TMP_Text selectedDiskName;
    [FoldoutGroup("Equip UI/Horizontal/Selected Disk")] public TMP_Text selectedDiskAttack;
    [FoldoutGroup("Equip UI/Horizontal/Selected Disk")] public TMP_Text selectedDiskAttackRange;
    [FoldoutGroup("Equip UI/Horizontal/Selected Disk")] public TMP_Text selectedDiskAttackSpeed;

    [HorizontalGroup("Equip UI/Horizontal")]
    [FoldoutGroup("Equip UI/Horizontal/Selected Wire")] public Image selectedWireImage;
    [FoldoutGroup("Equip UI/Horizontal/Selected Wire")] public Image selectedWireStatImage;
    [FoldoutGroup("Equip UI/Horizontal/Selected Wire")] public TMP_Text selectedWireName;
    [FoldoutGroup("Equip UI/Horizontal/Selected Wire")] public TMP_Text selectedWireAttack;
    [FoldoutGroup("Equip UI/Horizontal/Selected Wire")] public TMP_Text selectedWireAttackRange;
    [FoldoutGroup("Equip UI/Horizontal/Selected Wire")] public TMP_Text selectedWireAttackSpeed;
    
    [HorizontalGroup("Equip UI/Horizontal")]
    [FoldoutGroup("Equip UI/Horizontal/Total Stat")] public TMP_Text totalAttack;
    [FoldoutGroup("Equip UI/Horizontal/Total Stat")] public TMP_Text totalAttackRange;
    [FoldoutGroup("Equip UI/Horizontal/Total Stat")] public TMP_Text totalAttackSpeed;

    public Button decisionButton;
    public GameObject decisionButtonAlpha;

    private float _diskAttack, _wireAttack;
    private float _diskAttackRange, _wireAttackRange;
    private float _diskAttackSpeed, _wireAttackSpeed;

    private const float baseRange = 5f;

    #endregion

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
        AddButtonEvents();
    }

    protected virtual void OnDestroy()
    {
        RemoveEvents(); // 이벤트 리스너 제거
        RemoveButtonEvents();
    }

    // 이벤트를 등록하는 메서드
    protected virtual void AddEvents()
    {
        EventManager<UIEvents>.StartListening<YoYoData>(UIEvents.OnClickDiskListItem, UpdateSelectedDisk);
        EventManager<UIEvents>.StartListening<YoYoData>(UIEvents.OnClickWireListItem, UpdateSelectedWire);
        EventManager<GameEvents>.StartListening(GameEvents.IsReady, EnableDecisionButton);
    }

    // 이벤트 리스너를 제거하는 메서드
    protected virtual void RemoveEvents()
    {
        EventManager<UIEvents>.StopListening<YoYoData>(UIEvents.OnClickDiskListItem, UpdateSelectedDisk);
        EventManager<UIEvents>.StopListening<YoYoData>(UIEvents.OnClickWireListItem, UpdateSelectedWire);
        EventManager<GameEvents>.StopListening(GameEvents.IsReady, EnableDecisionButton);
    }

    private void AddButtonEvents()
    {
        decisionButton.onClick.AddListener(OnClickDecisionButton);
    }

    private void RemoveButtonEvents()
    {
        decisionButton.onClick.RemoveListener(OnClickDecisionButton);
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
        selectedDiskAttack.text = $"공격력 : {yoYoData.attack}";
        selectedDiskAttackRange.text = $"공격 사거리 : {yoYoData.attackRange}";
        selectedDiskAttackSpeed.text = $"공격 속도 : {yoYoData.attackSpeed}";

        _diskAttack = yoYoData.attack;
        _diskAttackRange = CalculateDiskRange(yoYoData.attackRange);
        _diskAttackSpeed = yoYoData.attackSpeed;

        UpdateTotalStats();
    }

    // 선택된 와이어를 업데이트하는 메서드
    private void UpdateSelectedWire(YoYoData yoYoData)
    {
        selectedWireImage.sprite = Resources.Load<Sprite>(yoYoData.imagePath);
        selectedWireStatImage.sprite = Resources.Load<Sprite>(yoYoData.imagePath); // StatImage 업데이트
        selectedWireName.text = TranslateName(yoYoData.name);
        selectedWireAttack.text = $"공격력 : {yoYoData.attack}";
        selectedWireAttackRange.text = $"공격 사거리 : {yoYoData.attackRange}";
        selectedWireAttackSpeed.text = $"공격 속도 : {yoYoData.attackSpeed}";

        _wireAttack = yoYoData.attack;
        _wireAttackRange = CalculateWireRange(yoYoData.attackRange);
        _wireAttackSpeed = yoYoData.attackSpeed;

        UpdateTotalStats();
    }

    // 종합 스탯을 업데이트하는 메서드
    private void UpdateTotalStats()
    {
        totalAttack.text = $"공격력 : <color=#ff0000>{_diskAttack} (디스크)</color> + <color=#00ff00>{_wireAttack} (와이어)</color> = {( _diskAttack + _wireAttack)}";
        totalAttackRange.text = $"공격 사거리 : <color=#ff0000>{_diskAttackRange} (디스크)</color> + <color=#00ff00>{_wireAttackRange} (와이어)</color> = {( _diskAttackRange + _wireAttackRange)}";
        totalAttackSpeed.text = $"공격 속도 : <color=#ff0000>{_diskAttackSpeed} (디스크)</color> + <color=#00ff00>{_wireAttackSpeed} (와이어)</color> = {( _diskAttackSpeed + _wireAttackSpeed)}";
    }

    // 디스크의 공격 사거리를 계산하는 메서드
    private float CalculateDiskRange(float attackRange)
    {
        return baseRange * (1f / 5f) * attackRange;
    }

    // 와이어의 공격 사거리를 계산하는 메서드
    private float CalculateWireRange(float attackRange)
    {
        return baseRange * (4f / 5f) * attackRange;
    }

    private void EnableDecisionButton()
    {
        decisionButton.enabled = true;
        decisionButtonAlpha.SetActive(false);
    }

    private void OnClickDecisionButton()
    {
        equipUI.SetActive(false);
        // 기술 세팅 UI 활성화
    }
}
