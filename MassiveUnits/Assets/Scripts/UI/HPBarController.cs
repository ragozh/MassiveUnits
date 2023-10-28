using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBarController : MonoBehaviour
{
    [SerializeField]
    Image _bar;
    RectTransform _rect;
    private void Start()
    {
        Setup();
    }
    public void Setup()
    {
        _rect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }
    public void UpdatePosition(Vector3 position)
    {
        var screenPoint = Camera.main.WorldToViewportPoint(position);
        var rectPos = new Vector2(screenPoint.x * _rect.sizeDelta.x, screenPoint.y * _rect.sizeDelta.y);
        transform.localPosition = rectPos - _rect.sizeDelta * 0.5f + Vector2.up * 100;
    }
    public void UpdateValue(int hp, int maxHP)
    {
        var ratio = (float)hp / maxHP;
        if (ratio < 0)
        {
            ratio = 0;
        }
        _bar.fillAmount = ratio;
    }
}
