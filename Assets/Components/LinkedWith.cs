using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkedWith : MonoBehaviour
{
    public enum LinkType { Antecedent, Next,  SubTask, Parent };
    public GameObject link;
    public LinkType type = LinkType.Antecedent;

    private LineRenderer line;

    private int nbPoints = 50;
    private float step1 = 0.33f;
    private float step2 = 0.66f;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
        if (line != null)
        {
            if (link == null)
                line.enabled = false;
            else
            {
                line.enabled = true;
                line.widthCurve = new AnimationCurve(
                     new Keyframe(0, line.startWidth)
                     , new Keyframe(0.5f, line.startWidth)
                     , new Keyframe(0.5001f, line.startWidth + 0.3f)  // max width of arrow head
                     , new Keyframe(0.6f, line.endWidth)  // tip of arrow
                     , new Keyframe(1, line.endWidth));  // tip of arrow
            }
        }
    }

    void OnGUI()
    {
        if (link != null && line != null)
        {
            Vector3 pointA = Camera.main.ScreenToWorldPoint(transform.parent.position);
            pointA = new Vector3(pointA.x, pointA.y, 0);
            Vector3 pointB = Camera.main.ScreenToWorldPoint(link.transform.position);
            pointB = new Vector3(pointB.x, pointB.y, 0);

            Vector3 pointAbis = new Vector3(pointA.x + step1 * (pointB.x - pointA.x), pointA.y + step2 * (pointB.y - pointA.y), 0);
            Vector3 pointBbis = new Vector3(pointB.x + step1 * (pointA.x - pointB.x), pointB.y + step2 * (pointA.y - pointB.y), 0);


            if (type == LinkType.Antecedent)
                DrawCubicBezierCurve(pointB, pointBbis, pointAbis, pointA);
            else
                DrawCubicBezierCurve(pointA, pointAbis, pointBbis, pointB);

        }
    }

    void DrawCubicBezierCurve(Vector3 point0, Vector3 point1, Vector3 point2, Vector3 point3)
    {

        line.positionCount = nbPoints;
        float t = 0f;
        Vector3 B = new Vector3(0, 0, 0);
        for (int i = 0; i < line.positionCount; i++)
        {
            B = (1 - t) * (1 - t) * (1 - t) * point0 + 3 * (1 - t) * (1 - t) *
                t * point1 + 3 * (1 - t) * t * t * point2 + t * t * t * point3;

            line.SetPosition(i, B);
            t += (1 / (float)line.positionCount);
        }
    }
}
