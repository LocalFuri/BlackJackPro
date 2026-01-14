using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
  // Game Buttons
  public Button dealBtn;
  public Button hitBtn;
  public Button standBtn;
  public Button betBtn;

  public bool playerBust = false;

  private int standClicks = 0;
  public int cardIndex = 0;
  private float cardDelaySnd = 0.227f; //audacity =0,227 got to adjust todo
  private bool dealDone = false;
  

  // Access the player and dealer's script
  public PlayerScript playerScript;
  public PlayerScript dealerScript;

  // public Text to access and update - hud
  public Text scoreText;
  public Text dealerScoreText;
  public Text betsText;
  public Text cashText;
  public Text mainText;
  public Text standBtnText;
  public AudioSource card1DealSnd, loseSnd;

  // Card hiding dealer's 2nd card
  public GameObject hideCard;
  // How much is bet
  int pot = 0;
  private ulong i = 0;

  void Start() //mark1
  {
    ResetGame();
    dealBtn. onClick.AddListener(DealClicked);
    hitBtn.  onClick.AddListener(HitClicked);
    standBtn.onClick.AddListener(StandClicked);
    betBtn.  onClick.AddListener(BetClicked);
  }
  private void DealClicked() //mark2
  {
    // Hide dealer last score at start of deal
    // mainText.gameObject.SetActive(true); //hide the last Text, you win, you lose, etc.  MANDATORY!

    //shuffle deck activate later
    //GameObject.Find("Deck").GetComponent<DeckScript>().Shuffle(); 

   dealDone = true; //2 cards dealt 
  //Player 1 cards begin ------------------------------------------------------------------------
  card1DealSnd.Play();
    playerScript.StartHand();
    scoreText.text = "Hand: " + playerScript.handValue.ToString(); //show player 1st card value
    StartCoroutine(CardDelay());
    //Player 1 cards end ------------------------------------------------------------------------

    // enable do hide one of the dealers cards
    hideCard.GetComponent<Renderer>().enabled = false; //dunno how it works

    // Adjust buttons visibility
    dealBtn.gameObject.SetActive(false);  //hide deal button
    hitBtn.gameObject.SetActive(true);    //make sure it is true
    standBtn.gameObject.SetActive(true);  //make sure it is true

        // Set standard pot size
    pot = 40;
    betsText.text ="€ " + pot.ToString();
    playerScript.AdjustMoney(-20);
    cashText.text ="€ " +playerScript.GetMoney().ToString();
  }

  private IEnumerator CardDelay() //it only works this way
  {
    // Deal 2 card/s to Player
    for (ulong i = 0; i < 1; i++)
    {
      yield return new WaitForSeconds(cardDelaySnd +0.0f);  // you can extend length +0.2f)
      hideCard.GetComponent<Renderer>().enabled = true;     //graphic UI object special handling
      dealerScript.StartHand();
      dealerScoreText.text = "Dealer Hand: " + dealerScript.handValue.ToString(); //show player 1st card value
      card1DealSnd.Play();

      yield return new WaitForSeconds(cardDelaySnd + 0.0f); // you can extend length +0.2f)
      playerScript.StartHand();
      scoreText.text = "Hand: " + playerScript.handValue.ToString(); //show player 1st card value
      card1DealSnd.Play();

      yield return new WaitForSeconds(cardDelaySnd + 0.0f); // you can extend length +0.2f)
      dealerScript.StartHand();
      dealerScoreText.text = "Dealer Hand: " + dealerScript.handValue.ToString(); //show player 1st card value
      card1DealSnd.Play();
    }
  }

  public void HitClicked()
  {
    card1DealSnd.Play();
    playerScript.StartHand();
    scoreText.text = "Hand: " + playerScript.handValue.ToString(); //show players next card

    if (playerScript.handValue > 21)  //player busted
    {
      loseSnd.Play();
      playerBust = true;
      RoundOver();
    }
  }
  private void StandClicked()
  {
    standClicks++;
    if (standClicks > 0) RoundOver();
     HitDealer();
    //standBtnText.text = "Call";
  }

  //HitDealer begin *****************************************************************************************
  private void HitDealer()
  {
    while (dealerScript.handValue < 17 && dealerScript.cardIndex  < 10)
    {
      dealerScript.GetCard();
      dealerScoreText.text = "Dealer Hand: " + dealerScript.handValue.ToString();
      if (dealerScript.handValue > 21) RoundOver ();
    } 
  }  

  //check for winner and loser, hand is over
  void RoundOver()
  {
    //booleans for bust and blackjack /21
    //bool playerBust = playerScript.handValue  > 21;
    bool dealerBust = dealerScript.handValue  > 21;
    bool player21   = playerScript.handValue == 21;
    bool dealer21   = dealerScript.handValue == 21;

    //if stand has been clicked less than twice, no 21s or busts, quit function
    //if(standClicks < 2 && !playerBust && !dealerBust && !player21 && !dealer21) return;
    bool roundOver = true;

    if (playerBust) // >21 
    {
      //disable butts begin ------------------------------------
      dealBtn.gameObject.SetActive(true);
      hitBtn.gameObject.SetActive(false);
      standBtn.gameObject.SetActive(false);
      //disable butts end ------------------------------------

      loseSnd.Play();
      mainText.enabled = true;  
      mainText.color = new Color(255, 0, 0);  //= Color.red;
      mainText.text = "You busted";
      //mark33

      return; // it is safe to exit, else proc could check else conditions
    }
    else if (dealerBust)  //>21 
    {
      mainText.text = "Player wins, dealer busted";
      Debug.Log(mainText.text);
      return;
    }

    //if dealer busts, player didnt, or player has more points, Player wins
    else if (playerScript.handValue > dealerScript.handValue && (dealerScript.handValue != 21))  
    {
      mainText.text = "You win again";
      playerScript.AdjustMoney(pot);
      Debug.Log(mainText.text);
      return;
    }
    // check for tie, return bets
    else if(playerScript.handValue == dealerScript.handValue)
    {
      mainText.text = "Push, bets returned";
      playerScript.AdjustMoney(pot / 2);
      return;
    }
    else
    {
      roundOver = true;
      return;
    }

    //Set ui up for next move / hand / turn
    if (roundOver)
    {
      ResetGame();
      return;
    }
  }
  //add money to pot, if bet clicked
  void BetClicked()
  {
    Text newBet = betBtn.GetComponentInChildren(typeof(Text)) as Text;
    int intBet = int.Parse(newBet.text.ToString().Remove(0,1));
    playerScript.AdjustMoney(-intBet);
    cashText.text="€ " +playerScript.GetMoney().ToString();
    pot += (intBet * 2);
    betsText.text = "€ " + pot.ToString();
  }

  private void ResetGame()
  {
    //button stuff begin -----------------------------------
    hitBtn.gameObject.SetActive(false);
    standBtn.gameObject.SetActive(false);
    dealBtn.gameObject.SetActive(true);
    //button stuff  end   -----------------------------------

    mainText.gameObject.SetActive(true);
    dealerScoreText.gameObject.SetActive(true);
    
    cashText.text = "€ " + playerScript.GetMoney().ToString();
    standClicks = 0;

    //hide object stuff begin -------------------------------------------------------------------
    hideCard.GetComponent<Renderer>().enabled = false; //graphic UI object special handling
    mainText.enabled        = false; //"Add as needed"
    //dealerScoreText.enabled = false; //plain to see

    //hide object stuff end  ----------------------------------------------------------------

    playerScript.ResetHand();
    dealerScript.ResetHand();
  }
}


   
