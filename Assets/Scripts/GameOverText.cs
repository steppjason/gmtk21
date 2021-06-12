using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class GameOverText : MonoBehaviour
{
    TextMeshProUGUI text;

    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }

    public IEnumerator FadeIn(float time){
        yield return text.DOFade(1f, time).WaitForCompletion();
    }

    public IEnumerator FadeOut(float time){
        yield return text.DOFade(0f, time).WaitForCompletion();
    }
}
