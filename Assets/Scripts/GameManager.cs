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
  
  private int standClicks = 0;
  private float cardDelaySnd = 0.3f;

  public int cardIndex = 0;

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
  public AudioSource card1Deal;

  // Card hiding dealer's 2nd card
  public GameObject hideCard;
  // How much is bet
  int pot = 0;

  void Start()
  {
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
  private void DealClicked() //mark1
  {
    playerScript.ResetHand();
    dealerScript.ResetHand();

    // Hide dealer last score at start of deal
    mainText.gameObject.SetActive(false); //hide the last Text, you win, you lose, etc.  MANDATORY!

    GameObject.Find("Deck").GetComponent<DeckScript>().Shuffle(); //shuffle deck

    //Player 2 cards begin ------------------------------------------------------------------------
    card1Deal.Play();
    playerScript.StartHand();
    scoreText.text = "Hand: " + playerScript.handValue.ToString(); //show player 1st card value
    StartCoroutine(PlayerDealCardDelay());
    //Player 2 cards end ------------------------------------------------------------------------

    //Dealer 2 cards begin ------------------------------------------------------------------------
    card1Deal.Play();
    dealerScript.StartHand();
    scoreText.text = "Hand: " + dealerScript.handValue.ToString(); //show player 1st card value
    StartCoroutine(DealerCardDelay());
    //dealer 2 cards end ------------------------------------------------------------------------

    // enable do hide one of the dealers cards
    //hideCard.GetComponent<Renderer>().enabled = false; //dunno how it works

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

  private IEnumerator PlayerDealCardDelay()
  {
    // Deal 2 card/s to Player
    for (ulong i = 0; i < 1; i++)
    {
      yield return new WaitForSeconds(cardDelaySnd + 0.2f); // you can extend length +0.2f)
      playerScript.StartHand();
      scoreText.text = "Hand: " + playerScript.handValue.ToString();
      card1Deal.Play();
    }
  }
  private IEnumerator DealerCardDelay()
  {
    // Deal 2 card/s to Dealer
    for (ulong i = 0; i <1; i++)
    {
      yield return new WaitForSeconds(cardDelaySnd +0.2f); // you can extend length +2f)
      dealerScript.StartHand();
      dealerScoreText.text = "Dealer Hand: " + dealerScript.handValue.ToString();
      card1Deal.Play();
  }
}
  private void HitClicked()
  {
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
    bool playerBust = playerScript.handValue  > 21;
    bool dealerBust = dealerScript.handValue  > 21;
    bool player21   = playerScript.handValue == 21;
    bool dealer21   = dealerScript.handValue == 21;

    //if stand has been clicked less than twice, no 21s or busts, quit function
    //if(standClicks < 2 && !playerBust && !dealerBust && !player21 && !dealer21) return;
    bool roundOver = true;

    if (playerBust) //>21 mark3
    {
      mainText.text = "Player busted";
    }
    else if (dealerBust)  //>21 
    {
      mainText.text = "Player wins, dealer busted";
    }

    //if dealer busts, player didnt, or player has more points, Player wins
    else if (playerScript.handValue > dealerScript.handValue && (dealerScript.handValue != 21))  
    {
      mainText.text = "You win again";
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
      roundOver = true;
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
      cashText.text = "€ " +playerScript.GetMoney().ToString();
      standClicks = 0;
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
}


   