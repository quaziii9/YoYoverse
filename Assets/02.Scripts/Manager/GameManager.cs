using System;
using EnumTypes;
using EventLibrary;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private bool _selectedDisk;
    private bool _selectedWire;

    public bool IsReady { get; private set; }

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
            IsReady = true;
            EventManager<GameEvents>.TriggerEvent(GameEvents.IsReady);
        }
    }

    private void SelectWire()
    {
        _selectedWire = true;
        
        if (_selectedDisk) IsReady = true;
        EventManager<GameEvents>.TriggerEvent(GameEvents.IsReady);
    }
}
