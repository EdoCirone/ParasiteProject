using UnityEngine;

public class UIFollowPlayer : MonoBehaviour
{
    [Header("References")]

    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Vector3 _offset;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (_playerTransform == null || mainCamera == null)
        {
            Debug.Log($"This UI {name} have missing Reference");
            return;
        }

        transform.position = _playerTransform.position + _offset;
    }

}
