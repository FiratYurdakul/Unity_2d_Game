using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static GameManager instance;

    public PLayer1 player;

    private void Awake()
    {

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
           
    }




}
