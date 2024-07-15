using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using EventLibrary;
using EnumTypes;

public class SkillIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image iconImage;
    [HideInInspector] public Transform originalParent;
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private Vector3 _startPosition;

    private void Start()
    {
        _canvas = FindObjectOfType<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void OnEnable()
    {
        EventManager<UIEvents>.StartListening(UIEvents.StartDraggingSkillIcon, DisableRaycastTarget);
        EventManager<UIEvents>.StartListening(UIEvents.StopDraggingSkillIcon, EnableRaycastTarget);
    }

    private void OnDisable()
    {
        EventManager<UIEvents>.StopListening(UIEvents.StartDraggingSkillIcon, DisableRaycastTarget);
        EventManager<UIEvents>.StopListening(UIEvents.StopDraggingSkillIcon, EnableRaycastTarget);
    }

    // 드래그 시작 시 호출
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        _startPosition = transform.position;
        _canvasGroup.blocksRaycasts = false;

        // Canvas의 최상위 자식으로 이동
        transform.SetParent(_canvas.transform, true);
        transform.SetAsLastSibling();

        // 모든 스킬 아이콘의 raycast target 비활성화
        EventManager<UIEvents>.TriggerEvent(UIEvents.StartDraggingSkillIcon);
    }

    // 드래그 중에 호출
    public void OnDrag(PointerEventData eventData)
    {
        RectTransform rt = GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToWorldPointInRectangle(_canvas.transform as RectTransform, eventData.position, _canvas.worldCamera, out Vector3 globalMousePos);
        rt.position = globalMousePos;
    }

    // 드래그 종료 시 호출
    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;

        GameObject pointerEnterObject = eventData.pointerEnter;
        if (pointerEnterObject != null)
        {
            SkillSlot slot = FindParentSkillSlot(pointerEnterObject.transform);
            if (slot != null)
            {
                HandleDropInSlot(slot);
            }
            else
            {
                ReturnToOriginalList();
            }
        }
        else
        {
            ReturnToOriginalList();
        }

        // 모든 스킬 아이콘의 raycast target 활성화
        EventManager<UIEvents>.TriggerEvent(UIEvents.StopDraggingSkillIcon);
    }

    private void HandleDropInSlot(SkillSlot slot)
    {
        SkillIcon existingSkillIcon = slot.GetComponentInChildren<SkillIcon>();
        if (existingSkillIcon != null && existingSkillIcon != this)
        {
            // 기존 스킬 아이콘과 드래그된 스킬 아이콘의 부모와 위치를 교환
            Transform tempParent = existingSkillIcon.originalParent;
            existingSkillIcon.transform.SetParent(originalParent);
            existingSkillIcon.transform.localPosition = Vector3.zero;
            existingSkillIcon.originalParent = originalParent;

            originalParent = slot.transform;
            // originalIndex = tempIndex;

            // 기존 스킬 아이콘의 raycast target 활성화
            existingSkillIcon.iconImage.raycastTarget = true;
        }

        // 드래그된 스킬 아이콘을 새로운 슬롯에 추가
        transform.SetParent(slot.transform);
        transform.localPosition = Vector3.zero;
        originalParent = slot.transform;
    }

    private void ReturnToOriginalList()
    {
        // originalParent가 스킬 리스트인지 확인
        SkillSlot originalSlot = originalParent.GetComponent<SkillSlot>();
        if (originalSlot != null && originalSlot.skillList != null)
        {
            transform.SetParent(originalSlot.skillList);
            transform.localPosition = Vector3.zero;
        }
        else
        {
            transform.SetParent(originalParent);
            transform.localPosition = Vector3.zero;
        }
    }

    // 주어진 자식 Transform에서 부모 SkillSlot을 찾음
    private SkillSlot FindParentSkillSlot(Transform child)
    {
        while (child != null)
        {
            SkillSlot slot = child.GetComponent<SkillSlot>();
            if (slot != null)
            {
                return slot;
            }
            child = child.parent;
        }
        return null;
    }

    private void DisableRaycastTarget()
    {
        iconImage.raycastTarget = false;
    }

    private void EnableRaycastTarget()
    {
        iconImage.raycastTarget = true;
    }
}
