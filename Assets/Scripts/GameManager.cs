using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UnityEngine.XR;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class GameManager : MonoBehaviour
{
  // Game Buttons
  public Button dealBtn;
  public Button hitBtn;
  public Button standBtn;
  public Button betBtn;
  public Sprite[] cardSprites;

  //public const string goldColor  = "#FFD700"; //Standard Gold RGB: 255, 215, 0)
  public static readonly Color32 gold = new Color32(255, 215, 0, 255);

  public bool playerBust = false;
  private int standClicks = 0;
  //public int cardIndex = 0;
  public int cardOffset = 0; //use to rig dealers score
  //private float cardDelaySnd = 0.066f; //audacity =0,227 got to adjust todo
  private float cardDelaySnd = 0.20f; //audacity =0,227 got to adjust todo


  //private float cardDelaySnd = 0.093f; //audacity =0,227 got to adjust todo

  private string tieText    = "Push, Bets returned!";
  private string loseText   = "You Lose!";
  private string winText    = "You Win!";
  private string bustedText = "You Busted!";

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
  public AudioSource card1DealSnd, loseSnd, tieSnd, harpupSnd,blackJackSnd;

  // Card hiding dealer's 2nd card
  public GameObject hideCard;
  // How much is bet
  int pot = 0;
  private ulong i = 0;

  void Start() //mark1
  {
    dealBtn. onClick.AddListener(DealClicked);
    hitBtn.  onClick.AddListener(HitClicked);
    standBtn.onClick.AddListener(HitDealer);
    betBtn.  onClick.AddListener(BetClicked);
  }

  private void DealClicked() //mark2
  {
    //hideCard.GetComponent<Renderer>().enabled = false; //graphic UI object special handling
    hideCard.GetComponent<Renderer>().enabled = true; //graphic UI object special handling ?????????


    mainText.text =""; // reset: win, lose, tie

    playerScript.ResetHand(); //mandatory after each round
    dealerScript.ResetHand(); //mandatory after each round

    //shuffle deck activate later
    GameObject.Find("Deck").GetComponent<DeckScript>().Shuffle();
    //GameObject.Find("Deck").GetComponent<DeckScript>().RestoreDeck(); //just restores the card from 1s init

    //Player 1 cards begin ------------------------------------------------------------------------
    card1DealSnd.Play();
    playerScript.StartHand();
    scoreText.text = "Hand: " + playerScript.handValue.ToString(); //show player 1st card value
    StartCoroutine(CardDelay());

    //Player 1 cards end ------------------------------------------------------------------------

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


    Debug.Log("Player :" + playerScript.handValue);
    Debug.Log("Dealer :" + dealerScript.handValue);


    //6. player has a natural Blackjack, Dealer has not begin ---------------------------------------
    if (playerScript.handValue == 21 && dealerScript.handValue != 21 && playerScript.cardIndex == 2)
    {
      Debug.Log("6. player has a natural Blackjack, Dealer has not");
      blackJackSnd.Play();
      mainText.color = new Color(255, 0, 255); //red #FF0000
      mainText.text = "BLACK JACK!";
      playerScript.AdjustMoney(+60);
    }
    //6. player has a natural Blackjack, Dealer has not end ---------------------------------------

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
      yield return null;

      yield return new WaitForSeconds(cardDelaySnd + 0.0f); // you can extend length +0.2f)
      playerScript.StartHand();
      scoreText.text = "Hand: " + playerScript.handValue.ToString(); //show player 1st card value
      card1DealSnd.Play();
      yield return null;

      yield return new WaitForSeconds(cardDelaySnd + 0.0f); // you can extend length +0.2f)
      dealerScript.StartHand();
      dealerScoreText.text = "Dealer Hand: " + dealerScript.handValue.ToString(); //show player 1st card value
      card1DealSnd.Play();

      // IMPORTANT: wait a frame so score updates
      yield return null;

      //Debug.Log(playerScript.cardIndex);
    }

    PlayerBlackJack(); //check if player got natural Blackjack
  }

  public void PlayerBlackJack()
  {
    Debug.Log("from Ienumerator");
    Debug.Log("Player :" + playerScript.handValue);
    Debug.Log("Dealer :" + dealerScript.handValue);

    //6. player has a natural Blackjack, Dealer has not begin ---------------------------------------
    if (playerScript.handValue == 21 && dealerScript.handValue != 21)
    {
      Debug.Log("6. player has a natural Blackjack, Dealer has not");
      blackJackSnd.Play();
      mainText.color = new Color(255, 0, 255); //red #FF0000
      mainText.text = "BLACK JACK!";
      playerScript.AdjustMoney(+60);
    }
    //6. player has a natural Blackjack, Dealer has not end ---------------------------------------
  }


  public void HitClicked()
  {
    card1DealSnd.Play();
    playerScript.StartHand();
    scoreText.text = "Hand: " + playerScript.handValue.ToString(); //show players next card
    Debug.Log(playerScript.cardIndex);

    if (playerScript.handValue > 21)  //player busted
    {
      Debug.Log(901);
      loseSnd.Play();
      playerBust = true;
      RoundOver();
    }
  }

  private IEnumerator DealerCardDelay() //it only works this way
  {
    while (dealerScript.handValue < 17)
    {
      card1DealSnd.Play();
      dealerScript.GetCard();

      /*
      if (dealerScript.handValue >=17)
      {
        dealerScoreText.text = "Dealer Hand: " + dealerScript.handValue.ToString();
        Debug.Log("IEnumerator DealerCardDelay =>exit cause >=17 ");
        yield break;  // stops the coroutine immediately
      }
      */
      // IMPORTANT: wait a frame so score updates
      yield return null;

      dealerScoreText.text = "Dealer Hand: " + dealerScript.handValue.ToString();
      yield return new WaitForSeconds(cardDelaySnd + 0F);
    }
    DetermineWinner();
  }

  private void DetermineWinner()
  {
    Debug.Log("from Ienumerator");
    Debug.Log("Player :" + playerScript.handValue);
    Debug.Log("Dealer :" + dealerScript.handValue);


    //6. player has a natural Blackjack, Dealer has not begin ---------------------------------------
    if (playerScript.handValue == 21 && dealerScript.handValue != 21 && playerScript.cardIndex == 2)
    {
      Debug.Log("6. player has a natural Blackjack, Dealer has not");
      blackJackSnd.Play();
      mainText.color = new Color(255, 0, 255); //red #FF0000
      mainText.text = "BLACK JACK!";
      playerScript.AdjustMoney(+60);
    }
    //6. player has a natural Blackjack, Dealer has not end ---------------------------------------


    //1. it is a tie begin-------------------------------------------------------------------------------
    else if (dealerScript.handValue == playerScript.handValue) //a tie
    {
      Debug.Log("1. it is a tie");
      Debug.Log("Player :" + playerScript.handValue);
      Debug.Log("Dealer :" + dealerScript.handValue);

      tieSnd.Play();
      mainText.color = new Color(255, 215, 0);  //gold #FFD700
      mainText.text = tieText;                  // "Push, Bets returned";
      playerScript.AdjustMoney(+20);
    }
    //1. it is a tie end  -------------------------------------------------------------------------------

    //2. dealer has better and not busted begin--------------------------------------------------------------
    else if ((dealerScript.handValue > playerScript.handValue) && dealerScript.handValue < 22)
    {
      Debug.Log("2. dealer has better and not busted");
      Debug.Log("Player :" + playerScript.handValue);
      Debug.Log("Dealer :" + dealerScript.handValue);

      loseSnd.Play();
      mainText.color = new Color(255, 0, 0); //red #FF0000
      mainText.text = loseText; // "You Lose!";
      playerScript.AdjustMoney(-pot / 2);
    }
    //2. dealer has better and not busted begin--------------------------------------------------------------

    //3. if play has better than dealer and player did not bust begin -------------------------------------------
    else if ((playerScript.handValue > dealerScript.handValue) && playerScript.handValue <= 21)
    {

      Debug.Log("3. if play has better than dealer and player did not bust");
      Debug.Log("Player :" + playerScript.handValue);
      Debug.Log("Dealer :" + dealerScript.handValue);
      harpupSnd.Play();
      mainText.color = new Color(0, 255, 0);  //green
      mainText.text = winText;                //"You win";
      playerScript.AdjustMoney(+40);
    }
    //3. if play has better than deal and play did not bust begin -------------------------------------------

    //4. dealer busted begin ----------------------------------------------------------------------------
    else if (dealerScript.handValue > 21)
    {
      Debug.Log("4. dealer busted");
      Debug.Log("Player :" + playerScript.handValue);
      Debug.Log("Dealer :" + dealerScript.handValue);

      harpupSnd.Play();
      mainText.color = new Color(0, 255, 0);  //green
      mainText.text = winText;                //"You win";
      playerScript.AdjustMoney(+40);
    }
    //4.dealer busted end ----------------------------------------------------------------------------

    //5. player busted begin ----------------------------------------------------------------------------
    else if (playerScript.handValue > 21)
    {
      Debug.Log("5. player busted");
      Debug.Log("Player :" + playerScript.handValue);
      Debug.Log("Dealer :" + dealerScript.handValue);

      loseSnd.Play();
      mainText.color = new Color(255, 0, 0); //red #FF0000
      mainText.text = loseText; // "You Lose!";
      playerScript.AdjustMoney(-pot / 2);
    }
    //5. player busted end ----------------------------------------------------------------------------

    else
    {
      mainText.color = new Color(255, 0, 0); //red #FF0000
      mainText.text = "Unfixed Error";
      Debug.Log("Dealer: " + dealerScript.handValue);
      Debug.Log("Player: " + playerScript.handValue);
    }


    cashText.text = "€ " + playerScript.GetMoney().ToString();

    //adjust butts begin ------------------------------------
    dealBtn.gameObject.SetActive(true);
    hitBtn.gameObject.SetActive(false);
    standBtn.gameObject.SetActive(false);
    //adjust butts end ------------------------------------

    //    dealBtn.image.sprite = normalSprite;
    //    dealBtn.image.sprite = normalSprite;
    //dealBtn.SetNormal();

    //dealButtonController.SetNormal(); // 
  }


  //HitDealer begin *****************************************************************************************
  private void HitDealer()
  {
    StartCoroutine(DealerCardDelay());

    /*
    cardOffset = 0; //rig deales score
    dealerScript.handValue = dealerScript.handValue + cardOffset; //use for testing
    dealerScoreText.text = "Dealer Hand: " + dealerScript.handValue.ToString();
    */

    return;
    Debug.Log(dealerScript.handValue);
    mainText.enabled = true; //show: tie, lose, win

    //6. player has a natural Blackjack, Dealer has not begin ---------------------------------------
    if (playerScript.handValue == 21 && dealerScript.handValue != 21 && playerScript.cardIndex == 2)
    {
      Debug.Log("6. player has a natural Blackjack, Dealer has not");
      blackJackSnd.Play();
      mainText.color = new Color(255, 0, 255); //red #FF0000
      mainText.text = "BLACK JACK!";
      playerScript.AdjustMoney(+60);
    }
    //6. player has a natural Blackjack, Dealer has not end ---------------------------------------


    //1. it is a tie begin-------------------------------------------------------------------------------
    else if (dealerScript.handValue == playerScript.handValue) //a tie
    {
      Debug.Log("1. it is a tie");
      Debug.Log("Player :" + playerScript.handValue);
      Debug.Log("Dealer :" + dealerScript.handValue);

      tieSnd.Play();
      mainText.color = new Color(255, 215, 0);  //gold #FFD700
      mainText.text = tieText;                  // "Push, Bets returned";
      playerScript.AdjustMoney(+20);
    }
    //1. it is a tie end  -------------------------------------------------------------------------------

    //2. dealer has better and not busted begin--------------------------------------------------------------
    else if ((dealerScript.handValue > playerScript.handValue) && dealerScript.handValue <22)
    {
      Debug.Log("2. dealer has better and not busted");
      Debug.Log("Player :" + playerScript.handValue);
      Debug.Log("Dealer :" + dealerScript.handValue);

      loseSnd.Play();
      mainText.color = new Color(255, 0, 0); //red #FF0000
      mainText.text = loseText; // "You Lose!";
      playerScript.AdjustMoney(-pot /2 );
    }
    //2. dealer has better and not busted begin--------------------------------------------------------------

    //3. if play has better than dealer and player did not bust begin -------------------------------------------
    else if ((playerScript.handValue > dealerScript.handValue) && playerScript.handValue <= 21)
    {

      Debug.Log("3. if play has better than dealer and player did not bust");
      Debug.Log("Player :" +playerScript.handValue);
      Debug.Log("Dealer :" + dealerScript.handValue);
      harpupSnd.Play();
      mainText.color = new Color(0, 255, 0);  //green
      mainText.text = winText;                //"You win";
      playerScript.AdjustMoney(+40);
    }
    //3. if play has better than deal and play did not bust begin -------------------------------------------
    
    //4. dealer busted begin ----------------------------------------------------------------------------
    else if (dealerScript.handValue >21)
    {
      Debug.Log("4. dealer busted");
      Debug.Log("Player :" + playerScript.handValue);
      Debug.Log("Dealer :" + dealerScript.handValue);

      harpupSnd.Play();
      mainText.color = new Color(0, 255, 0);  //green
      mainText.text = winText;                //"You win";
      playerScript.AdjustMoney(+40);
    }
    //4.dealer busted end ----------------------------------------------------------------------------

    //5. player busted begin ----------------------------------------------------------------------------
    else if (playerScript.handValue > 21)
    {
      Debug.Log("5. player busted");
      Debug.Log("Player :" + playerScript.handValue);
      Debug.Log("Dealer :" + dealerScript.handValue);

      loseSnd.Play();
      mainText.color = new Color(255, 0, 0); //red #FF0000
      mainText.text = loseText; // "You Lose!";
      playerScript.AdjustMoney(-pot / 2);
    }
    //5. player busted end ----------------------------------------------------------------------------

    //7. dealer has a natural Blackjack, player has not begin ---------------------------------------
    if (playerScript.handValue == 21 && dealerScript.handValue != 21 && playerScript.cardIndex == 2)
    {
      Debug.Log("7. dealer has a natural Blackjack, player has not");
      loseSnd.Play();
      mainText.color = new Color(255, 0, 0); //red #FF0000
      mainText.text = loseText; // "You Lose!";
      playerScript.AdjustMoney(-pot / 2);
    }
    //7. dealer has a natural Blackjack, player has not end ---------------------------------------

    else
    {
      mainText.color = new Color(255, 0, 0); //red #FF0000
      mainText.text = "Unfixed Error";
      Debug.Log("Dealer: "  +dealerScript.handValue);
      Debug.Log("Player: "  + playerScript.handValue);
    }


      cashText.text = "€ " + playerScript.GetMoney().ToString();

    //adjust butts begin ------------------------------------
    dealBtn.gameObject  .SetActive(true);
    hitBtn.gameObject   .SetActive(false);
    standBtn.gameObject .SetActive(false);
    //adjust butts end ------------------------------------

    //    dealBtn.image.sprite = normalSprite;
    //    dealBtn.image.sprite = normalSprite;
    //dealBtn.SetNormal();

    //dealButtonController.SetNormal(); // 



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
      
      return;
    }

    //if dealer busts, player didnt, or player has more points, Player wins
    else if (playerScript.handValue > dealerScript.handValue && (dealerScript.handValue != 21))  
    {
      mainText.text = "You win again";
      playerScript.AdjustMoney(pot);
      return;
    }
    // check for tie, return bets
    else if(playerScript.handValue == dealerScript.handValue)
    {
      mainText.text = tieText;          // "Push, bets returned";
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
      //ResetGame();
      return;
    }
  }
  //add money to pot, if bet clicked

  void BetClicked()
  {
    Text newBet = betBtn.GetComponentInChildren(typeof(Text)) as Text;
    int intBet = int.Parse(newBet.text.ToString().Remove(0, 1));
    playerScript.AdjustMoney(-intBet);
    cashText.text = "€ " + playerScript.GetMoney().ToString();
    pot += (intBet * 2);
    betsText.text = "€ " + pot.ToString();
  }

}



