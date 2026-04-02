using System;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{
    public bool IsPointerDown { get; private set; }

    public event Action PointerDowned;
    protected override void Start()
    {
        base.Start();
        background.gameObject.SetActive(false);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        background.gameObject.SetActive(true);
        IsPointerDown = true;
        PointerDowned?.Invoke();
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        background.gameObject.SetActive(false);
        IsPointerDown = false;
        base.OnPointerUp(eventData);
    }
}