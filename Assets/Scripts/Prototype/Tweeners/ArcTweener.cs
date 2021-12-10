using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ArcTweener : MonoBehaviour, IObjectTweener
{
    [SerializeField] private float time = 1.5f;
    [SerializeField] private float height;

    public void MoveTo(Transform transform, Vector3 targetPosition)
    {
        float distance = Vector3.Distance(targetPosition, transform.position);
        transform.DOJump(targetPosition, height, 1, time);
    }
}
