using System.Collections;
using UnityEngine;

public class Pool_Obj : MonoBehaviour
{
    [Header("Debug Info")]
    [SerializeField] private string poolID;
    [SerializeField] private float currentLifeTime;
    [SerializeField] private bool isIndeterminate;

    private Rigidbody rb;
    private Coroutine lifeTimeCoroutine;

    private void OnEnable()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
        }

        if (!isIndeterminate && currentLifeTime > 0) lifeTimeCoroutine = StartCoroutine(LifeRoutine());
    }

    public void SetUp(float lifeTime, string id, bool isIndeterminate)
    {
        this.currentLifeTime = lifeTime;
        this.poolID = id;
        this.isIndeterminate = isIndeterminate;

        if (this.currentLifeTime <= 0) lifeTime = 1;
    }

    public void SetUpNewLifeTime(float lifeTime)
    {
        this.currentLifeTime = lifeTime;
        if (!isIndeterminate && currentLifeTime > 0) lifeTimeCoroutine = StartCoroutine(LifeRoutine());
    }

    private IEnumerator LifeRoutine()
    {
        if (isIndeterminate) yield break;

        yield return new WaitForSeconds(currentLifeTime);
        PollingEnemy.Instance.ReturnToPool(poolID, this);
    }

    public void ReturnToPool()
    {
        if (PollingEnemy.Instance) PollingEnemy.Instance.ReturnToPool(poolID, this);
        else Destroy(gameObject);
    }

    private void OnDisable()
    {
        if (lifeTimeCoroutine != null)
        {
            StopCoroutine(lifeTimeCoroutine);
            lifeTimeCoroutine = null;
        }

        if (!rb) rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }
}
