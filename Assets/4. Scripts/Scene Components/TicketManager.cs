using System.Collections;
using UnityEngine;

public class TicketManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float speedFactor = 10;
    [SerializeField]
    private int maxTickets = 6;

    [Header("Required Cmponents")]
    [SerializeField]
    private RectTransform[] waypoints;
    [SerializeField]
    private GameObject ticketPrefab;

    [Header("Debugs")]
    [SerializeField]
    private int ticketCount;
    [SerializeField]
    private Ticket[] ticketByQueueNumber;

    private Vector3 spawnPos;
    private Vector3 endPos = new Vector3(2, 0);
    private Vector3 destroyPos = new Vector3(-40, 0);

    private float speed;

    private void Start()
    {

        var canvas = GetComponentInParent<Canvas>();
        spawnPos = new Vector3(2, -canvas.pixelRect.height/canvas.scaleFactor);
        ticketByQueueNumber = new Ticket[maxTickets];
        speed = (canvas.pixelRect.height / canvas.scaleFactor) / speedFactor;
    }

    public void CreateTicket(Demon demon, int queueNumber)
    {
        GameObject ticketObj = Instantiate(ticketPrefab,
            Vector2.zero,
            Quaternion.identity,
            FindEmptyWaypointFromEnd());

        var ticket = ticketObj.GetComponent<Ticket>();
        //ticket.InitializeTicketValues(demon, queueNumber);
        ticketByQueueNumber[queueNumber] = ticket;

        var ticketRect = ticketObj.GetComponent<RectTransform>();
        ticketRect.anchoredPosition = spawnPos;

        StartCoroutine(MoveUI(ticketRect, endPos));
    }

    private Transform FindEmptyWaypointFromEnd()
    {
        for (int i = waypoints.Length - 1; i >= 0; i--)
        {
            if (i - 1 >= 0)
            {
                if (waypoints[i - 1].childCount == 0) 
                    continue;
                else
                    return waypoints[i];
            }
            else
            {
                return waypoints[i];
            }
        }

        return waypoints[0];
    }

    public void RemoveTicket(int queueNumber)
    {
        var ticketToRemove = ticketByQueueNumber[queueNumber];
        ticketByQueueNumber[queueNumber] = null;

        StartCoroutine(DestroyUI(ticketToRemove.GetComponent<RectTransform>()));
    }

    private IEnumerator MoveUI(RectTransform rectTransform, Vector2 end)
    {
        while (rectTransform.anchoredPosition != end)
        {
            rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, end, speed);
            yield return null;
        }
    }

    private IEnumerator DestroyUI(RectTransform rectTransform)
    {
        yield return MoveUI(rectTransform, destroyPos);
        rectTransform.SetParent(null);
        Destroy(rectTransform.gameObject);
        MoveTickestUp();
        foreach(Ticket ticket in ticketByQueueNumber)
        {
            if(ticket != null)
            {
                StartCoroutine(MoveUI(ticket.GetComponent<RectTransform>(), endPos));
            }
        }
    }

    private void MoveTickestUp()
    {
        for (int i = 0; i < maxTickets; i++)
        {
            if (waypoints[i].childCount == 0 && (i + 1 < maxTickets) && waypoints[i + 1].childCount >= 1)
            {
                waypoints[i + 1].GetChild(0).SetParent(waypoints[i]);
            }
        }
    }

    /*public void SetAngryStartTime(int queueNumber, float startTime, float timeTilAngry)
    {
        ticketByQueueNumber[queueNumber].SetAngryStartTime(startTime, timeTilAngry);
    }*/
}
