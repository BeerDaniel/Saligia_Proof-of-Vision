using Gamekit3D;
using System.Collections;
using TMPro;
using UnityEngine;

public class CombatText : MonoBehaviour, IPooled<CombatText>
{
    public enum CombatTextType
    {
        Default,
        CrowdControl,
        Damage,
        Healing
    }

    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField, Range(0, 1)] private float _timeToLive;
    [SerializeField] private float _yDelta;
    [SerializeField, Range(0, 1)] private float _alphaEnd;
    
    public int poolID { get; set; }
    public ObjectPooler<CombatText> pool { get; set; }

    public void Init(string text, Color color)
    {
        _text.text = text;
        _text.color = color;
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        StartCoroutine(AnimateText());
    }

    private IEnumerator AnimateText()
    {
        float time = 0;
        Color startColor = _text.color;
        Color endColor = startColor;
        endColor.a = 0;
        float yStart = transform.position.y;

        while (time < _timeToLive)
        {
            _text.color = Color.Lerp(startColor, endColor, time);
            var yD = Mathf.Lerp(yStart, yStart + _yDelta, time);
            transform.position = new Vector3(transform.position.x, yD, transform.position.z);
            time += Time.deltaTime;
            yield return null;
        }
        _text.color = endColor;
        transform.position = new Vector3(transform.position.x, yStart + _yDelta, transform.position.z);
        pool.Free(this);
        gameObject.SetActive(false);
    }
}
