using UnityEngine;

public class SetEnemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Outline outline;
    public Rigidbody rb;
    void Awake()
    {
        gameObject.AddComponent<EnemyStats>();
        gameObject.AddComponent<HealthBar>();
        gameObject.AddComponent<WalkForward>();
        outline = gameObject.AddComponent<Outline>();
        gameObject.AddComponent<Highlightable>();
        gameObject.AddComponent<BuffController>();
        rb = gameObject.AddComponent<Rigidbody>();

        outline.OutlineColor = new Color(1, 0, 0, 1);
        outline.OutlineWidth = 1f;

        rb.mass = 1f;
        rb.automaticCenterOfMass = true;
        rb.automaticInertiaTensor = true;
        rb.isKinematic = true;
        rb.angularDamping = 0;
        rb.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
