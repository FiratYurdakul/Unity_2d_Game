using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.tag == "Player")
        //    GameManager.instance.player.Knockback();

        PLayer1 player = collision.gameObject.GetComponent<PLayer1>();
        player?.Knockback();

    }



}
