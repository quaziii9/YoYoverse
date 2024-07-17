using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QTE : MonoBehaviour
{
    private Slider _slider;
    public TMP_Text timeText; // 남은 시간을 표시할 TextMeshPro 오브젝트
    public float decreaseAmount = 3f; // 슬라이더 감소 값
    public float increaseAmount = 5f; // 슬라이더 증가 값
    public float qteDuration = 15f; // QTE 지속 시간 (초)

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        StartCoroutine(StartQTE());
    }

    private IEnumerator StartQTE()
    {
        float elapsedTime = 0f;
        while (elapsedTime < qteDuration)
        {
            // 남은 시간을 업데이트
            UpdateTimeText(qteDuration - elapsedTime);

            // 슬라이더 값 감소
            _slider.value -= decreaseAmount * Time.deltaTime;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 시간이 다 지나면 실패 판정
        FailQTE();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // QTE에 할당된 키 (예: Spacebar)
        {
            PressQTEKey();
        }
    }

    private void PressQTEKey()
    {
        _slider.value += increaseAmount;

        // QTE 성공 판정
        if (_slider.value >= _slider.maxValue)
        {
            SuccessQTE();
            StopAllCoroutines(); // QTE 성공 시 코루틴 종료
        }
    }

    // 남은 시간을 업데이트하는 메서드
    private void UpdateTimeText(float remainingTime)
    {
        timeText.text = remainingTime.ToString("F1") + "초"; // 소수점 한자리까지 표시
    }

    // QTE 성공 시 호출되는 메서드
    private void SuccessQTE()
    {
        DebugLogger.Log("QTE 성공! 암살 및 쿨타임 계산.");
        // 암살 및 쿨타임 계산 로직 추가
    }

    // QTE 실패 시 호출되는 메서드
    private void FailQTE()
    {
        DebugLogger.Log("QTE 실패! 스킬 사용 실패 및 적에게 감지됨.");
        // 스킬 사용 실패 및 적에게 감지됨 로직 추가
    }
}