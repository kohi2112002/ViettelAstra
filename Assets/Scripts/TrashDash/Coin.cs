using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Coin : MonoBehaviour
{
    private Transform target;
    private bool alive = false;
    [SerializeField] private int amount = 1;
    [SerializeField] private bool canRotate = false;
    private Transform coinTransform;
    private void OnEnable()
    {
        target = FindObjectOfType<GameController>().fishboneTarget;
        if (Random.Range(0, 10) > 5 && GetComponentInChildren<Animator>() != null)
            GetComponentInChildren<Animator>().Play("idle_02");
        alive = true;
        coinTransform = transform.GetChild(0);
    }
    private IEnumerator FloatUp()
    {
        while (transform.position.y < target.position.y)
        {
            yield return null;
            transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * 7);
            transform.localScale = Vector3.MoveTowards(transform.localScale, target.localScale, Time.time * 17);
        }
        FindObjectOfType<GameController>().Coin += amount;
        Destroy(gameObject);
    }
    private void Update()
    {
        if (canRotate)
        {
            coinTransform.Rotate(new Vector3(0, 120 * Time.deltaTime, 0));
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (alive)
        {
            alive = false;
            StartCoroutine(FloatUp());
        }
    }
}
