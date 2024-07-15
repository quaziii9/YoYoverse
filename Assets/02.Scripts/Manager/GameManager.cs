using EnumTypes;
using EventLibrary;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool _selectedDisk;
    private bool _selectedWire;

    public bool IsEquipReady { get; private set; }
    public bool IsSkillReady { get; private set; }

    private void Awake()
    {
        EventManager<GameEvents>.StartListening(GameEvents.SelectedDisk, SelectDisk);
        EventManager<GameEvents>.StartListening(GameEvents.SelectedWire, SelectWire);
    }
    private void SelectDisk()
    {
        _selectedDisk = true;

        if (_selectedWire)
        {
            IsEquipReady = true;
            EventManager<GameEvents>.TriggerEvent(GameEvents.IsEquipReady);
        }
    }

    private void SelectWire()
    {
        _selectedWire = true;
        
        if (_selectedDisk) IsEquipReady = true;
        EventManager<GameEvents>.TriggerEvent(GameEvents.IsEquipReady);
    }
}
