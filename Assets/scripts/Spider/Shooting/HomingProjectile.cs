using UnityEngine;
using System.Collections;
public class HomingProjectile : MonoBehaviour
{
    public int damage;
    private GameObject _target;
    private float rotateSpeed;
    public float homingDelay;
    private float speed;
    [SerializeField] private float minSpeed = 8f;
    [SerializeField] private float maxSpeed = 11f;
    [SerializeField] private float detectionRange = 10f;  //Detection range for Enemies
    [SerializeField] private float damageRadius = 1f; //range of what the projectile will hit


    [Header("Audio")]
    public string impactSound;
    public GameObject impactFX;

    // Start is called before the first frame update
    void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player");

        speed = Random.Range(minSpeed, maxSpeed);
        rotateSpeed = Random.Range(4f, 6f);
    }

    // Update is called once per frame
    void Update()
    {
        //Move Projectile forward
        if(_target != null) {
            transform.LookAt(_target.transform.position);
        }
        transform.position += transform.forward * (speed * Time.deltaTime);

        GameObject[] targets;
        targets = GameObject.FindGameObjectsWithTag("Player");
        
        //if any target is in range, start homing into target
        if (_target != null)
        {
            foreach (GameObject _target in targets)
            {
                float distance = Vector3.Distance(_target.transform.position, transform.position);

                if (distance <= detectionRange)
                {
                    StartCoroutine(HomingMissile());
                }
            }
        }
        //Need to add something that destroys projectile after certain distance.

        var hitColliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (var hitCollider in hitColliders)
        {
            // Check if colliders are enemies
            var player = hitCollider.GetComponent<PlayerStats>();
            if (player != null)
            {
                //Instantiate(impactFX, transform.position, transform.rotation);
                // For each enemy calculate the distance between enemy and grenade
                var distance = Vector3.Distance(transform.position, hitCollider.gameObject.transform.position);
                // Calculate damag.

                //Debug.Log($"calculated damage <color=red>{damage}</color> - distance <color=green>{distance}</color>");
                var enemyStats = player.GetComponent<PlayerStats>();
                if (enemyStats != null) enemyStats.TakeDamage(damage);

                DestroyProjectile();
            }
        }
    }

    public GameObject FindClosestEnemy()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Player");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {

                closest = go;
                distance = curDistance;
            }
        }

        if(closest != null)
        {
            //move towards closest enemy
        Vector3 direction = (closest.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotateSpeed);
        }

        return closest;
    }

    void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    IEnumerator HomingMissile()
    {
        //time until looking for closest enemy
        yield return new WaitForSeconds(homingDelay);

        FindClosestEnemy();
    }

    void OnDrawGizmosSelected()
    {
        // Display the color
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}