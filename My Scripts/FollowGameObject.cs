using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowGameObject : MonoBehaviour
{
    [SerializeField] GameObject objectToFollow;
    [SerializeField] Vector2 offset;

    void Update()
    {
        if (objectToFollow == null)
        {
            Debug.LogError("FollowGameObject script in Scene without assigned follow object!");
            return;
        }

        if (offset != Vector2.zero)
        {
            transform.position = Vector2.Lerp(transform.position, (Vector2)objectToFollow.transform.position + offset, 1f);
            return;
        }
        transform.position = Vector2.Lerp(transform.position, (Vector2)objectToFollow.transform.position, 1f);
    }
}
