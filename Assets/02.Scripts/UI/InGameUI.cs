using EnumTypes;
using EventLibrary;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    public InGameSkillSlot[] inGameSkillSlots;
    public InGameYoYoSlot[] inGameYoYoSlots;

    private void Awake()
    {
        EventManager<GameEvents>.StartListening(GameEvents.StartGame, InitializeSkills);
        EventManager<GameEvents>.StartListening(GameEvents.StartGame, InitializeYoYo);
    }

    private void OnDestroy()
    {
        EventManager<GameEvents>.StopListening(GameEvents.StartGame, InitializeSkills);
        EventManager<GameEvents>.StopListening(GameEvents.StartGame, InitializeYoYo);
    }

    // 스킬 슬롯 초기화
    private void InitializeSkills()
    {
        for (int i = 0; i < inGameSkillSlots.Length; i++)
        {
            SkillData skillData = GameManager.Instance.GetSkillData(i);
            if (skillData != null)
            {
                inGameSkillSlots[i].Initialize(skillData);
            }
        }
    }
    
    // 스킬 슬롯 초기화
    private void InitializeYoYo()
    {
        for (int i = 0; i < inGameYoYoSlots.Length; i++)
        {
            YoYoData yoyoData = GameManager.Instance.GetYoYoData(i);
            if (yoyoData != null)
            {
                inGameYoYoSlots[i].Initialize(yoyoData);
            }
        }
    }

    // 스킬 사용
    public void UseSkill(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < inGameSkillSlots.Length)
        {
            inGameSkillSlots[slotIndex].StartCooldown();
            // 추가적인 스킬 사용 로직을 여기에 추가
        }
    }
}