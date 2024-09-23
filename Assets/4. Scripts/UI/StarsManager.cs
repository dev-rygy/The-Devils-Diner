using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/*
 * 
 * Note on how this script will work:
 * 
 * The StarsManager script handles the "Stars" of the game.
 * When a player loses a star, the object that causes the loss of star MUST call the "LoseStar"
 * function for every star the player is going to lose. This will automatically update the 
 * UI that shows the stars on screen.
 * 
 * TODO:
 *      Implement a system that shows a set of 5 stars that either:
 *      a) Gray out as stars are lost
 *      b) Disappear as stars are lost
 * 
 * CURRENT IMPLEMENTATION:
 *      Currently the UI generates text that shows at the top-right of the screen. The text follows this format:
 *      "Stars: " + stars
 */

public class StarsManager : MonoBehaviour
{
    public static StarsManager main;

    public TMP_Text TextStars;
    private int stars;

    // Star Objects
    [SerializeField] private Image[] starArray;
    public Sprite LitStar;
    public Sprite DimStar;

    private void Awake()
    {
        if(main == null)
            main = this;
        else
            Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        stars = 5;

        for(int i = 0; i < 5; i++)
        {
            starArray[i].sprite = LitStar;
        }
        /*
        TextStars.rectTransform.localPosition = new Vector3(-300, 200);
        TextStars.text = "Stars: " + stars;
        */
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (stars > 0)
        {
            
            TextStars.text = "Stars: " + stars;
        }
        else
        {
            
            TextStars.text = "GAME OVER";
        }
        */
    }

    // Outside function call for losing a star
    public void LoseStar()
    {
        stars--;
        updateStar();
    }

    // Updates the image of a star when lost
    private void updateStar()
    {
        // Check to avoid out of bounds error
        if(stars < 5 && stars > 0)
        {
            //CanvasRenderer renderer = starArray[stars].GetComponent<CanvasRenderer>();
            //renderer.SetColor(Color.gray);
            starArray[stars].sprite = DimStar;
        }
        else
        {
            SceneManager.LoadScene(2);
        }
    }
}
