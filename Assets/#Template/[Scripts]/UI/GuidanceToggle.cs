using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DancingLineFanmade.UI;
using DancingLineFanmade.Gameplay;

[RequireComponent(typeof(Button))]
public class GuidanceToggle : MonoBehaviour
{
    public GameObject 
        toggleOn,
        toggleOff;

    private Button _button;
    private bool _isOn;

    public static event System.Action OnClickToggle;
    public static void ClickToggle() => OnClickToggle?.Invoke();
    private void Start()
    {
        if (FindObjectOfType<GuidanceManager>().GuidanceGroup)
            gameObject.SetActive(true);
        else
        {
            gameObject.SetActive(false);
            return;
        }
        _button = GetComponent<Button>();
        _isOn = GuidanceManager._isUsing;

        HandleToggeDisplay();

        _button.onClick.AddListener(() =>
        {
            ClickToggle();
            _isOn = GuidanceManager._isUsing;
            HandleToggeDisplay();
        });
    }
    private void HandleToggeDisplay()
    {
        toggleOn.SetActive(_isOn);
        toggleOff.SetActive(!_isOn);
    }
}
