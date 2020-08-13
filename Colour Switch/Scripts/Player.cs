using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] float jumpForce = 7.5f;
    Rigidbody2D playerRB;
    string currentBallColour;
    string previousBallColour;
    SpriteRenderer ballSpriteRenderer;
    [SerializeField] public Color[] ballColourArray;

    void SetRandomColour()
    {
        previousBallColour = currentBallColour;
        int colourIndex = Random.Range(0, 4);
        switch(colourIndex)
        {
            case 0:
                currentBallColour = "Cyan";
                ballSpriteRenderer.color = ballColourArray[0];
                break;
            case 1:
                currentBallColour = "Pink";
                ballSpriteRenderer.color = ballColourArray[1];
                break;
            case 2:
                currentBallColour = "Purple";
                ballSpriteRenderer.color = ballColourArray[2];
                break;
            case 3:
                currentBallColour = "Yellow";
                ballSpriteRenderer.color = ballColourArray[3];
                break;
        }
        //checks if new colour is same as previous one
        //if yes, calls SetRandomColour method again
        if (previousBallColour == currentBallColour)
            SetRandomColour();
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            playerRB.velocity = Vector2.up * jumpForce;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag != currentBallColour && col.tag != "Colour Changer")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (col.tag == "Colour Changer")
        {
            SetRandomColour();
            Destroy(col.gameObject);
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerRB = this.gameObject.GetComponent<Rigidbody2D>();
        ballSpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        SetRandomColour();
    }

    // Update is called once per frame
    void Update()
    {
        Jump();
    }
}
