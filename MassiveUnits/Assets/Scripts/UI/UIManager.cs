using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("ShowFPS", 0.0f, 0.5f);
    }
    [SerializeField]
    TextMeshProUGUI _fps;
    // Update is called once per frame
    void Update()
    {
    }
    void ShowFPS()
    {
        _fps.text = $"FPS: {string.Format("{0:0.00}", 1.0f / Time.smoothDeltaTime)}";
    }
}
