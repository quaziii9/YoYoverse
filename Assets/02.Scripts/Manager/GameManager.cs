using EnumTypes;
using EventLibrary;

public class GameManager : Singleton<GameManager>
{
    private bool _selectedDisk;
    private bool _selectedWire;

    public bool IsEquipReady { get; private set; }
    public bool IsSkillReady { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        
        EventManager<GameEvents>.StartListening(GameEvents.SelectedDisk, SelectDisk);
        EventManager<GameEvents>.StartListening(GameEvents.SelectedWire, SelectWire);
        EventManager<GameEvents>.StartListening(GameEvents.IsSkillReady, ChangeSkillReadyState);
    }

    private void OnDestroy()
    {
        EventManager<GameEvents>.StopListening(GameEvents.SelectedDisk, SelectDisk);
        EventManager<GameEvents>.StopListening(GameEvents.SelectedWire, SelectWire);
        EventManager<GameEvents>.StopListening(GameEvents.IsSkillReady, ChangeSkillReadyState);
    }

    // 디스크 선택 시 호출
    private void SelectDisk()
    {
        _selectedDisk = true;
        CheckEquipReady();
    }

    // 와이어 선택 시 호출
    private void SelectWire()
    {
        _selectedWire = true;
        CheckEquipReady();
    }

    // 디스크와 와이어 모두 선택되었는지 확인
    private void CheckEquipReady()
    {
        if (_selectedDisk && _selectedWire)
        {
            IsEquipReady = true;
            EventManager<GameEvents>.TriggerEvent(GameEvents.IsEquipReady);
        }
    }

    // 스킬 준비 상태 변경
    private void ChangeSkillReadyState()
    {
        IsSkillReady = true;
    }
}