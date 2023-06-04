using Gamekit3D;
using SaligiaProofOfVision;
using System;
using System.Collections.Generic;
using UnityEngine;
using static CombatText;

public class CombatTextCreator : MonoBehaviour
{
    [Serializable]
    public struct CombatTextSettings
    {
        public CombatTextType combatTextType;
        public Color color;
    }

    [SerializeField] private Canvas _combatTextCanvas = null;
    [SerializeField] private GameObject _combatText;
    [SerializeField] private List<CombatTextSettings> _combatTextSettings;
    [SerializeField] private int _poolSize;

    private Dictionary<CombatTextType, CombatTextSettings> _combatTextSettingDictionary;
    private CombatText _loadedCombatText = null;
    private ObjectPooler<CombatText> _combatTextPool;

    private CombatTextSettings _loadedSetting;

    private void Start()
    {
        _combatTextSettingDictionary = new Dictionary<CombatTextType, CombatTextSettings>();
        foreach (var setting in _combatTextSettings)
        {
            if (_combatTextSettingDictionary.ContainsKey(setting.combatTextType))
                continue;
            _combatTextSettingDictionary.Add(setting.combatTextType, setting);
        }
        _combatTextPool = new ObjectPooler<CombatText>();
        _combatTextPool.Initialize(_poolSize, _combatText.GetComponent<CombatText>(), _combatTextCanvas.gameObject);
    }

    private void OnEnable()
    {
        GameEvents.spawnCombatTextEvent += OnSpawnCombatTextEvent;
    }

    private void OnDisable()
    {
        GameEvents.spawnCombatTextEvent -= OnSpawnCombatTextEvent;
    }

    private void OnSpawnCombatTextEvent(CombatTextType type, string text, Vector3 position)
    {
        _loadedCombatText = _combatTextPool.GetNew();

        if (!_combatTextSettingDictionary.TryGetValue(type, out _loadedSetting))
            _loadedSetting = _combatTextSettingDictionary[CombatTextType.Default];

        _loadedCombatText.transform.SetParent(_combatTextCanvas.transform);
        _loadedCombatText.transform.position = position;
        _loadedCombatText.Init(text, _loadedSetting.color);
        _loadedCombatText = null;
    }
}
