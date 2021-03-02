using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogController : Obstacle
{
    [SerializeField] private int detectRange = 5, speed = 3;
    private Transform player;
    private bool hasAggressive = false, isMoving = false;
    private Animator anim;
    private void OnEnable()
    {
        anim = GetComponent<Animator>();
        isMoving = true;
        player = FindObjectOfType<PlayerController>().transform;
    }
    private void Update()
    {
        if (isMoving)
            if (!hasAggressive)
            {
                if (Vector3.Distance(player.position, transform.position) < detectRange)
                {
                    hasAggressive = true;
                    anim.SetTrigger("Run");
                }
            }
            else
                transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
    }
    public override GameObject[] Spawn(Vector3 position, Transform parent)
    {
        GameObject go = Instantiate(gameObject, position, Quaternion.Euler(0, 180, 0), parent);
        return new GameObject[] { go };
    }
    public override void PlayCollisionAnim()
    {
        isMoving = false;
        anim.SetTrigger("Death");
        Destroy(gameObject, 1f);
    }
}
