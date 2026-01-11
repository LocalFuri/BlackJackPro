using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine; 
using UnityEngine.UI;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
  // Game Buttons
  public Button dealBtn;
  public Button hitBtn;
  public Button standBtn;
  public Button betBtn;
  
  private int standClicks = 0;

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
    
  // Card hiding dealer's 2nd card
  public GameObject hideCard;
  // How much is bet
  int pot = 0;

  void Start()
  {
   // GameManager.DealClicked()(Assets / Scripts / GameManager.cs.:51

    
    


    // Add on click listeners to the buttons
    //dealBtn.onClick.AddListener(()  => DealClicked());
    //hitBtn.onClick.AddListener(()   => HitClicked());
    //standBtn.onClick.AddListener(() => StandClicked());
    //betBtn.onClick.AddListener(()   => BetClicked());

    //KI recommended, since we do not pass parameters
    dealBtn. onClick.AddListener(DealClicked);
    hitBtn.  onClick.AddListener(HitClicked);
    standBtn.onClick.AddListener(StandClicked);
    betBtn.  onClick.AddListener(BetClicked);
  }
  private void DealClicked()
  {
    //reset round, hide text, prep for new hand
    playerScript.ResetHand();
    Debug.Log(101);

    //dealerScript = playerScript, can maybe cause an failure, check this !!!
    //dealerScript.ResetHand();
    Debug.Log(102);

    // Hide dealer last score at start of deal
    mainText.gameObject.SetActive(false); //hide the last Text, you win, you lose, etc.
    GameObject.Find("Deck").GetComponent<DeckScript>().Shuffle();
    Debug.Log(103);


    // Update the scores displayed
    playerScript.StartHand();   //deal 1st card player
    scoreText.text = "Hand: " + playerScript.handValue.ToString(); //show player 1st card value
    Debug.Log(playerScript.handValue.ToString());


    dealerScript.StartHand();   //deal 1st card dealer
    dealerScoreText.text = "Dealer Hand: " + dealerScript.handValue.ToString();
    Debug.Log(dealerScript.handValue.ToString());

    // enable do hide one of the dealers cards
    hideCard.GetComponent<Renderer>().enabled = true;
    Debug.Log(106);
    
    // Adjust buttons visibility
    dealBtn.gameObject.SetActive(false);  //hide deal button
    hitBtn.gameObject.SetActive(true);    //make sure it is true
    standBtn.gameObject.SetActive(true);  //make sure it is true

    // Set standard pot size
    pot = 40;
    betsText.text ="€" + pot.ToString();
    playerScript.AdjustMoney(-20);
    cashText.text ="€" +playerScript.GetMoney().ToString();
  }

  private void HitClicked()

  {
    Debug.Log("HitClicked -01");
    //return;
    // Check that there is still room on the table
    if (playerScript.cardIndex <= 10)
    {
      playerScript.GetCard();
      scoreText.text = "Hand: " +playerScript.handValue.ToString() ;
      if (playerScript.handValue > 20) RoundOver();
    }
  }
  private void StandClicked()
  {
    standClicks++;
    if (standClicks > 1) RoundOver();
     HitDealer();
    standBtnText.text = "Call";
  }

  //HitDealer begin *****************************************************************************************
  private void HitDealer()
  {
    while (dealerScript.handValue < 16 && dealerScript.cardIndex < 10)
    {
      dealerScript.GetCard();
      dealerScoreText.text = "Dealer Hand: " + dealerScript.handValue.ToString();
      if (dealerScript.handValue > 20) RoundOver ();
    } 
  }  

  //check for winner and loser, hand is over
  void RoundOver()
  {
    //booleans for bust and blackjack /21
    bool playerBust = playerScript.handValue  > 21;
    bool dealerBust = dealerScript.handValue  > 21;
    bool player21   = playerScript.handValue == 21;
    bool dealer21   = dealerScript.handValue == 21;

    //if stand has been clicked less than twice, no 21s or busts, quit function
    if(standClicks < 2 && !playerBust && !dealerBust && !player21 && !dealer21) return;
    bool roundOver = true;

    //all bust, bets returned
    if (playerBust && dealerBust)
    {
      mainText.text = "All Bust, Bets returned"; //k
      playerScript.AdjustMoney(pot / 2); //k  
    }
    //if player busts, dealer didnt, or dealer has more points, Dealer wins
    else if (playerBust || (!dealerBust && playerScript.handValue > dealerScript.handValue))
    {
      mainText.text = "Dealer wins";
    }

    //if dealer busts, player didnt, or player has more points, Player wins
    else if (dealerBust || playerScript.handValue > dealerScript.handValue)
    {
      mainText.text = "You win";
      playerScript.AdjustMoney(pot);
    }
    // check for tie, return bets
    else if(playerScript.handValue == dealerScript.handValue)
    {
      mainText.text = "Push, bets returned";
      playerScript.AdjustMoney(pot / 2);
    }
    else
    {
      roundOver = false;
    }

    //Set ui up for next move / hand / turn
    if (roundOver)
    {
      hitBtn.gameObject.         SetActive(false);
      standBtn.gameObject.       SetActive(false);
      dealBtn.gameObject.        SetActive(true);
      mainText.gameObject.       SetActive(true);
      dealerScoreText.gameObject.SetActive(true);
      hideCard.GetComponent<Renderer>().enabled=false;
      cashText.text = "€" +playerScript.GetMoney().ToString();
      standClicks = 0;
    }
  }
  //add money to pot, if bet clicked
  void BetClicked()
  {
    Text newBet = betBtn.GetComponentInChildren(typeof(Text)) as Text;
    int intBet = int.Parse(newBet.text.ToString().Remove(0,1));
    playerScript.AdjustMoney(-intBet);
    cashText.text="€" +playerScript.GetMoney().ToString();
    pot += (intBet * 2);
    betsText.text = "€" + pot.ToString();
  }
}


   