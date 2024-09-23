using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TransitionUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float pauseDuration = 2f;
    [SerializeField]
    private float panelFadeDuration = 2f;
    [SerializeField]
    private float textFadeDuration = 1f;
    [SerializeField]
    private Color mainTextDefaultColor;
    [SerializeField]
    private Color subText1DefaultColor;
    [SerializeField]
    private Color subText2DefaultColor;

    [Header("Required Components")]
    [SerializeField]
    private Image transitionPanel;
    [SerializeField]
    private TMP_Text mainText;
    [SerializeField]
    private TMP_Text subText1;
    [SerializeField]
    private TMP_Text subText2;
    [SerializeField]
    private Button nextButton;

    private Color blackTrans = new Color(0, 0, 0, 0);

    public IEnumerator IntroCoroutine(string mainText)
    {
        this.mainText.text = mainText;
        subText1.enabled = false;
        subText2.enabled = false;
        nextButton.enabled = false;

        transitionPanel.gameObject.SetActive(true);
        // Pause for dramatic effect
        yield return StartCoroutine(FadeUI(this.mainText, blackTrans, mainTextDefaultColor, textFadeDuration));

        yield return new WaitForSeconds(pauseDuration);

        // Fade out the panel
        StartCoroutine(FadeUI(transitionPanel, Color.black, blackTrans, panelFadeDuration));
        yield return (FadeUI(this.mainText, mainTextDefaultColor, blackTrans, panelFadeDuration));

        transitionPanel.gameObject.SetActive(false);
    }

    public IEnumerator NightEndInCoroutine(string mainText, string subText1, string subText2)
    {
        this.mainText.text = mainText;
        this.subText1.text = subText1;
        this.subText2.text = subText2;

        this.subText1.enabled = true;
        this.subText2.enabled = true;
        nextButton.enabled = true;

        transitionPanel.gameObject.SetActive(true);

        // Fade out the panel
        StartCoroutine(FadeUI(transitionPanel, blackTrans, Color.black, panelFadeDuration));
        StartCoroutine(FadeUI(this.subText1, blackTrans, subText1DefaultColor, panelFadeDuration));
        StartCoroutine(FadeUI(this.subText2, blackTrans, subText2DefaultColor, panelFadeDuration));
        yield return StartCoroutine(FadeUI(this.mainText, blackTrans, mainTextDefaultColor, panelFadeDuration));
    }

    public void NightEndOut()
    {
        StartCoroutine(NightEndOutCoroutine());
    }

    public IEnumerator NightEndOutCoroutine()
    {
        StartCoroutine(FadeUI(this.subText1, subText1DefaultColor, blackTrans, textFadeDuration));
        StartCoroutine(FadeUI(this.subText2, subText2DefaultColor, blackTrans, textFadeDuration));
        yield return StartCoroutine(FadeUI(this.mainText, mainTextDefaultColor, blackTrans, textFadeDuration));

        GameManager.main.NextNight();
    }

    private IEnumerator FadeUI(Graphic graphicObj,Color startColor, Color endColor, float duration)
    {
        var t = 0f;

        graphicObj.color = startColor;

        while (t < duration)
        {
            graphicObj.color = Color.Lerp(startColor, endColor, t / duration);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        graphicObj.color = endColor;
    }
}
