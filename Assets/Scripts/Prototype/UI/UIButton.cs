using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UIInputReceiver))]
public class UIButton : Button
{
    private InputReceiver receiver;

    protected override void Awake()
    {
        base.Awake();
        receiver = GetComponent<UIInputReceiver>();
        onClick.AddListener(() => receiver.OnInputReceived());
    }
}
