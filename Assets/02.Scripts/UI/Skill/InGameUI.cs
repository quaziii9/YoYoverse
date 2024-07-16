using EnumTypes;
using EventLibrary;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    public InGameSkillSlot[] inGameSkillSlots; // InGameSlot 오브젝트 배열

    private void Awake()
    {
        EventManager<GameEvents>.StartListening(GameEvents.StartGame, InitializeSkills);
    }

    private void OnDestroy()
    {
        EventManager<GameEvents>.StopListening(GameEvents.StartGame, InitializeSkills);
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