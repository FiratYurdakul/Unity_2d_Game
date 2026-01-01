using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static GameManager instance;

    public PLayer1 player;

    [Header("Fruit Managment")]
    public bool fruitsHaveRandomLook;
    public int fruitsCollected;

    private void Awake()
    {

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
           
    }


    public void AddFruit() => fruitsCollected++;
    public bool getFruitHaveRandomLook() => fruitsHaveRandomLook;
}
