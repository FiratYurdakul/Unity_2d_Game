using UnityEngine;

public enum FruitType
{
    Apple,
    Banana,
    Cherry,
    Kiwi,
    Orange,
    Strawberry,
    Melon,
    Pineapple,
    Cherries
}


public class Fruit : MonoBehaviour
{
    [SerializeField] private FruitType fruitType;

    [SerializeField] private GameObject pickupVFX;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameManager gameManager;
    private Animator anim;



    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();

    }
    private void Start()
    {
        gameManager = GameManager.instance;
        SetRandomLookIfNeeded();
    }

    private void SetRandomLookIfNeeded()
    {

        if (gameManager.getFruitHaveRandomLook() == false)
        {
            UpdateFruitVisuals();
            return;
        }


        int randomIndex = Random.Range(0, 8);
        anim.SetFloat("fruitIndex", randomIndex);
    }
    


    private void OnTriggerEnter2D(Collider2D collision)
    {
         
        PLayer1 player =  collision.GetComponent<PLayer1>();

        if (player != null)
        {
            gameManager.AddFruit();
            Destroy(gameObject);

            GameObject newVFX = Instantiate(pickupVFX, transform.position, Quaternion.identity );
            Destroy(newVFX, .5f);
        }


    }

    private void UpdateFruitVisuals() => anim.SetFloat("fruitIndex", (int)fruitType);
         




}
