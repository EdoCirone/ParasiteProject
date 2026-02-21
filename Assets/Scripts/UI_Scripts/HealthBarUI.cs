using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("Reference")]

    [SerializeField] private Entity _player;
    [SerializeField] private Slider _healthBarFill;

    private void Awake()
    {
        if (_healthBarFill == null)
        {
            Debug.LogWarning($"{name}HealtBar Missing");
                return;
        }
        if (_player == null)
        {
            Debug.LogWarning($"{name}Player Missing");
            return;
        }

    }


    private void Start()
    {
        _healthBarFill.maxValue = 1;
        _healthBarFill.minValue = 0;
        _healthBarFill.value = 1;
        _healthBarFill.interactable = false;

    }

    private void Update()
    {
        RefreshHP();
    }

    private void RefreshHP()
    {
        if (_player == null || _healthBarFill == null)
        {
            
                return;
        }
     

        if (_player.MaxHp <= 0)
        {
            Debug.LogWarning($"{name}Player MaxHp Invalid");
            _healthBarFill.value = 0;
            return;
        }

        float fixedhp = Mathf.Clamp(_player.Hp, 0, _player.MaxHp);
        _healthBarFill.value = fixedhp / _player.MaxHp;
    }

}

