using UnityEngine;
using UnityEngine.UI;

public class CookReminderUI : MonoBehaviour
{
    public static CookReminderUI main;

    [Header("Settings")]
    [SerializeField]
    private GameObject holder;
    [SerializeField]
    private Toggle toggle;

    [Header("Debugs")]
    [SerializeField]
    private bool dontShowAgain = true;

    public bool IsShowing => holder.activeInHierarchy;

    private void Awake()
    {
        if(main == null)
            main = this;
        else
            Destroy(this);
    }

    public void Show(bool enable)
    {
        if (enable)
        {
            if (dontShowAgain) return;

            holder.SetActive(true);
            GameManager.main.PauseGame();
        }
        else
        {
            holder.SetActive(false);
            GameManager.main.ResumeGame();
        }
    }

    public void DontShowAgain(bool dontShowAgain)
    {
        //Debug.Log("dontShowAgain = " + dontShowAgain);
        this.dontShowAgain = dontShowAgain;
    }
}
