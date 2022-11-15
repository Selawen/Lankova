using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Subtitles : MonoBehaviour
{
    public GameObject subPanel;
    public Text subText;
    static Subtitles Instance { get; set; }

    Coroutine subtitles;

    private void Awake()
    {
        if(Instance != null) { Destroy(Instance.gameObject); Debug.LogWarning("Destroyed old subtitles object"); }
        Instance = this;
    }

    public static void PlaySubtitles(string text)
    {
        Instance.PlaySubs(text);
    }

    void PlaySubs(string text)
    {
        if(subtitles != null) { StopCoroutine(subtitles); }
        subtitles = StartCoroutine(SubtitleCoroutine(text));
    }

    IEnumerator SubtitleCoroutine(string text)
    {
        subText.text = text;
        subPanel.SetActive(true);
        yield return new WaitForSeconds(7.0f);

        subText.text = "";
        subPanel.SetActive(false);
        subtitles = null;
    }
}
