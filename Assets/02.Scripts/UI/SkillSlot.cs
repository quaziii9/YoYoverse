using UnityEngine;
using UnityEngine.EventSystems;

public class SkillSlot : MonoBehaviour, IDropHandler
{
    public Transform skillList; // 스킬 리스트를 참조하는 public 변수

    // 드랍 이벤트 처리
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            SkillIcon draggedSkillIcon = eventData.pointerDrag.GetComponent<SkillIcon>();
            if (draggedSkillIcon != null)
            {
                SkillIcon existingSkillIcon = GetComponentInChildren<SkillIcon>();
                if (existingSkillIcon != null && existingSkillIcon != draggedSkillIcon)
                {
                    // 기존 스킬 아이콘의 raycast target 비활성화
                    existingSkillIcon.iconImage.raycastTarget = false;

                    // 기존 스킬 아이콘과 드래그된 스킬 아이콘의 부모와 위치를 스왑
                    Transform originalParent = draggedSkillIcon.originalParent;
                    draggedSkillIcon.originalParent = existingSkillIcon.transform.parent;
                    existingSkillIcon.transform.SetParent(originalParent);
                    existingSkillIcon.transform.localPosition = Vector3.zero;

                    // 기존 스킬 아이콘의 raycast target 활성화
                    existingSkillIcon.iconImage.raycastTarget = true;
                }

                // 드래그된 스킬 아이콘을 새로운 슬롯에 추가
                draggedSkillIcon.transform.SetParent(transform);
                draggedSkillIcon.transform.localPosition = Vector3.zero;
                draggedSkillIcon.originalParent = transform;
            }
        }
    }
}