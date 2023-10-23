using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ShowFPS(_updateDelay));
    }
    [SerializeField]
    TextMeshProUGUI _fps, _mobCount;
    [SerializeField]
    float _updateDelay = 0.5f;
    bool _trackingFPS = true;

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator ShowFPS(float delay)
    {
        while (_trackingFPS)
        {
            yield return new WaitForSeconds(delay);
            _fps.text = $"FPS: {string.Format("{0:0.00}", 1.0f / Time.smoothDeltaTime)}";
            _mobCount.text = $"Count: {Spawner.Instance.MobCount}";
        }
    }
}
