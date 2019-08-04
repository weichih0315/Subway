using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

    private Animator animator;

	private void Start () {
        animator = GetComponent<Animator>();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.instance.GetCoin();
            animator.SetTrigger("Collected");
            AudioManager.instance.PlaySound2D("CoinCollected");
        }
    }
}
