using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicketUI : MonoBehaviour
{
    public static TicketUI main;

    [Header("Settings")]
    [SerializeField]
    private int defaultTicketDisplay = 4;
    [SerializeField]
    private float ticketSpacing = 20;
    [SerializeField]
    private float easeInDuration = 2f;
    [SerializeField] 
    private AnimationCurve easeInCurve;
    [SerializeField]
    private float easeOutDuration = 2f;
    [SerializeField] 
    private AnimationCurve easeOutCurve;

    [Header("Required Components")]
    [SerializeField]
    private RectTransform content;
    [SerializeField]
    private GameObject ticketPrefab;

    [Header("Debugs")]
    [SerializeField]
    private List<Ticket> ticketArray = new List<Ticket>();
    private List<Coroutine> reorderCoroutines = new List<Coroutine>();

    private float ticketHeight;
    private float panelHeight;

    private RectTransform panelTransform;

    private void Awake()
    {
        if(main == null)
            main = this;
        else
            Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        panelTransform = GetComponent<RectTransform>();
        panelHeight = panelTransform.rect.height;
        ticketHeight = (panelHeight - (defaultTicketDisplay + 1) * ticketSpacing) / defaultTicketDisplay;
    }

    public Ticket AddTicket(Demon demon, FoodData foodData, int queueNumber,Sprite rugSprite)
    {
        var ticketObj = Instantiate(ticketPrefab, content);
        var ticketSct = ticketObj.GetComponent<Ticket>();
        var ticketRect = ticketObj.GetComponent<RectTransform>();
        var ticketCount = ticketArray.Count;

        // Initialize Ticket
        ticketSct.InitializeTicketValues(demon, foodData, queueNumber, rugSprite);

        // Set position and size
        ticketRect.anchoredPosition = new Vector2(0, -(ticketCount * (ticketHeight + ticketSpacing) + panelHeight));
        ticketRect.sizeDelta = new Vector2(ticketRect.sizeDelta.x, ticketHeight);

        // Add ticket last
        if(ticketCount == 0)
        {
            //Debug.Log("Add as first element");
            ticketArray.Add(ticketSct);
        }
        else
        {
            var index = -1;
            for(int i = 0; i < ticketCount; i++)
            {
                if(ticketSct.TimeLeft <= ticketArray[i].TimeLeft)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                //Debug.Log("Add as last element");
                ticketArray.Add(ticketSct);
            }
            else
            {
                //Debug.Log($"i = {index}, insert {ticketSct.TimeLeft} before {ticketArray[index].TimeLeft}");
                ticketArray.Insert(index, ticketSct);
            }
                
        }
        ReorderTickets();

        return ticketSct;
    }

    public void RemoveTicket(Ticket ticket)
    {
        ticketArray.Remove(ticket);
        var rect = ticket.GetComponent<RectTransform>();
        StartCoroutine(MoveUI(rect, new Vector2(-(rect.rect.width * 1.5f), rect.anchoredPosition.y), easeOutCurve, easeOutDuration, ReorderTickets));
    }

    private void ReorderTickets()
    {
        if(reorderCoroutines.Count > 0)
        {
            foreach (Coroutine coroutine in reorderCoroutines)
                if (coroutine != null) StopCoroutine(coroutine);

            reorderCoroutines.Clear();
        }

        for (int i = 0; i < ticketArray.Count; i++)
        {
            var rect = ticketArray[i].GetComponent<RectTransform>();
            var start = rect.anchoredPosition;
            var end = new Vector2(0, -(i * (ticketHeight + ticketSpacing)));
            reorderCoroutines.Add(StartCoroutine(MoveUI(rect, end, easeInCurve, easeInDuration)));
        }

        // Resize ticket holder
        content.sizeDelta = new Vector2(0, ticketArray.Count * (ticketHeight + ticketSpacing));
    }

    private IEnumerator MoveUI(RectTransform rectTransform, Vector2 end, AnimationCurve curve, float duration, Action OnMovementComplete = null)
    {
        float t = 0;
        while (Vector2.SqrMagnitude(end - rectTransform.anchoredPosition) > 0.01f)
        {
            t += Time.deltaTime;
            var ease = curve.Evaluate(t / duration);
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, end, ease);

            yield return new WaitForFixedUpdate();
        }
        rectTransform.anchoredPosition = end;

        OnMovementComplete?.Invoke();
    }
}
