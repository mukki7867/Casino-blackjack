using System;
using System.Linq;

namespace Casino_Assessment
{
    static class Program
    {
        #region Global Variables
            //Player cards is an array that holds the cards that were dealt to the user. 
            static string[] CardDeck = new string[11];
            
            //Player cards suites that holds the cards that were dealt to the user. 
            static string[] CardSuiteDeck = new string[20];

            //users to store the player choice (hit or stop)
            static string playerChoice = "";

            static int playerTotal = 0;
            static int Total = 1;
            static int SuiteTotal = 1;
            static bool NaturalBlackJack = false;

            static int wins = 0;
            static int loses = 0;

            static int dealerTotal = 0;
            static Random cardRandomizer = new Random();
            static Random SuiteRandomizer = new Random();

            private static readonly object syncLock = new object();

            static string playAgain = Keys.Yes;
        #endregion

        #region Main Method
            static void Main(string[] args)
            {
                //Main loop: play again is always yes unless they choose no 
                while (string.Equals(playAgain, Keys.Yes, StringComparison.OrdinalIgnoreCase))
                {
                    //Deal cards and display a welcome message
                    StartGame();
                    //handle the loop which plays out the users hand (Hit or stop)
                    StartGameLoop();

                    Console.WriteLine("Would you like to play again? (Y)es or (N)o?");
                    PlayAgain();
                }
            }

        #endregion

        #region Start Game
            //Deals the player and the dealer cards . Displays a welcome message and asks the player if they would like to hit or stop
            static void StartGame()
            {
                dealerTotal = cardRandomizer.Next(17, 22);
                CardDeck[0] = DealCard();
                CardDeck[1] = DealCard();
                
                //Checking cards for a Natural BlackJack
                if ((CardDeck[0] == Keys.Ace || CardDeck[1] ==  Keys.Ace) && (CardDeck[0] == Keys.Ten || CardDeck[1] ==  Keys.Ten || CardDeck[0] == Keys.Jack || CardDeck[1] ==  Keys.Jack || CardDeck[0] == Keys.Queen || CardDeck[1] ==  Keys.Queen || CardDeck[0] == Keys.King || CardDeck[1] ==  Keys.King))   
                {
                    NaturalBlackJack = true;
                    CardSuiteDeck[0] = DealSuite();
                    CardSuiteDeck[1] = DealSuite();

                    if (dealerTotal == 21)
                    {
                        Console.WriteLine("Oops , this is a stand-off, nobody wins ! The dealer's player Total was : {0}", dealerTotal);
                    }

                    else
                    {
                        Console.WriteLine("Congrats , You got BlackJack ! The dealer's player Total was : {0}", dealerTotal);
                    }

                    Console.WriteLine("Would you like to play again? (Y)es or (N)o?");
                    Console.ReadLine();
                    Console.Clear();
                    dealerTotal = 0;
                    Total = 1;
                    SuiteTotal = 1;
                    NaturalBlackJack = false;
                    playerTotal = 0;
                    StartGame();
                }

                CardSuiteDeck[0] = DealSuite();
                CardSuiteDeck[1] = DealSuite();
                
                //Avoiding duplication of cards of same suites since there's only 1 deck of cards
                if ((CardDeck[0] == CardDeck[1]) && (CardSuiteDeck[0] == CardSuiteDeck[1]))
                {
                    SuiteTotal = 1;
                    playerTotal = 0;
                    
                    CardDeck[0] = DealCard();
                    CardDeck[1] = DealCard();

                    CardSuiteDeck[0] = DealSuite();
                    CardSuiteDeck[1] = DealSuite();
                }


                StartGameMessage();
                //Do this until the player chooses a valid option
            }
        #endregion

        #region Start Game Message
            private static void StartGameMessage()
            {
                Console.WriteLine("You are now playing Cards at the Casino!" + Environment.NewLine  + "You have received the following 2 cards from the dealer : {0} and {1}", CardDeck[0] + " of " + CardSuiteDeck[0], CardDeck[1] + " of " + CardSuiteDeck[1]);
                Console.WriteLine("Your player Total is : {0} ", playerTotal);
            }

        #endregion

        #region Start Game Loop
            static void StartGameLoop()
            {

                do
                {
                    Console.WriteLine("Press (H) to Hit or (S) to Stop receiving cards?");
                    playerChoice = Console.ReadLine().ToUpper();
                }
                while (!playerChoice.Equals(Keys.HitMe) && !playerChoice.Equals(Keys.Stop));


                if (playerChoice.Equals(Keys.HitMe))
                {
                    //hit will get them a card / check the total and ask for another hit 
                    Hit();
                }

                if (playerChoice.Equals(Keys.Stop))
                {
                    if (playerTotal > dealerTotal && playerTotal <= 21)
                    {
                        wins++;
                        loses = 0;  
                        Console.WriteLine("Congrats! You won the game! You have " +  wins + " win(s).The dealer's Total was : {0} ", dealerTotal);
                    }
                    else if (playerTotal < dealerTotal)
                    {
                        if (dealerTotal == 21)
                        {
                            Console.WriteLine("Sorry, you've lost! The dealer got BlackJack", dealerTotal);
                        }
                        else
                        {
                            Console.WriteLine("Sorry, you've lost! The dealer's Total was : {0}", dealerTotal);
                        }

                        loses++;

                        if (loses > 2)
                        {
                            Console.WriteLine("You're losing a lot");
                        }
                    }
                    else if (playerTotal == dealerTotal)
                    {
                        Console.WriteLine("You and the dealer got the same total , it's a Stand-Off !");
                    }
            }

            }

        #endregion

        #region Deal Suite
            static string DealSuite()
            {
                int suites = 0;
                lock (syncLock)
                {
                    suites = SuiteRandomizer.Next(1, 5);
                }
                return GetSuiteValue(suites);
            }
        #endregion

        #region Get Suite Value
            static string GetSuiteValue(int suites)
            {
                string Suite;

                switch (suites)
                {
                    case 1:
                        Suite = "Diamonds";
                        break;
                    case 2:
                        Suite = "Hearts";
                        break;
                    case 3:
                        Suite = "Spades";
                        break;
                    case 4:
                        Suite = "Clubs";
                        break;
                    default:
                        Suite = "Diamonds";
                        break;
                }
                return Suite;
            }

        #endregion

        #region Deal Card
            static string DealCard()
            {
                int cards = 0; 
                lock (syncLock)
                {
                  cards = cardRandomizer.Next(1, 14);
                }

            return GetCardValue(cards);
            }

        #endregion

        #region Get Card Value
            static string GetCardValue(int cards)
            {
                string Card;

                switch (cards)
                {
                    case 1:
                        Card = "Ace"; playerTotal++;
                        break;
                    case 2:
                        Card = "Two"; playerTotal += 2;
                        break;
                    case 3:
                        Card = "Three"; playerTotal += 3;
                        break;
                    case 4:
                        Card = "Four"; playerTotal += 4;
                        break;
                    case 5:
                        Card = "Five"; playerTotal += 5;
                        break;
                    case 6:
                        Card = "Six"; playerTotal += 6;
                        break;
                    case 7:
                        Card = "Seven"; playerTotal += 7;
                        break;
                    case 8:
                        Card = "Eight"; playerTotal += 8;
                        break;
                    case 9:
                        Card = "Nine"; playerTotal += 9;
                        break;
                    case 10:
                        Card = "Ten"; playerTotal += 10;
                        break;
                    case 11:
                        Card = "Jack"; playerTotal += 10;
                        break;
                    case 12:
                        Card = "Queen"; playerTotal += 10;
                        break;
                    case 13:
                        Card = "King"; playerTotal += 10;
                        break;
                    default:
                        Card = "Ace"; playerTotal++;
                        break;
                }
                return Card;
            }

        #endregion

        #region Hit
            static void Hit()
            {
                Total++;
                SuiteTotal++;

                CardDeck[Total] = DealCard();
                CardSuiteDeck[SuiteTotal] = DealSuite();

                Console.WriteLine("Your card is a(n) {0} and your new player Total is : {1}. ", CardDeck[Total] + " of " + CardSuiteDeck[SuiteTotal], playerTotal);

                if (NaturalBlackJack)
                {
                    wins++;
                    Console.WriteLine("Congrats! You won the game! You have " + wins + " win(s).The dealer's Total was : {0} ", dealerTotal);
                    Console.WriteLine("You got 21 Blackjack! The dealer's player Total was : {0}. ", dealerTotal);
                    loses = 0;
                }
                else if (playerTotal == dealerTotal)
                {
                    Console.WriteLine("You and the dealer got the same total , it's a Stand-Off !");
                }
                else if (playerTotal == 21)
                {
                    Console.WriteLine("You got 21 ! You should probably stop ! The dealer's player Total was : {0}", dealerTotal);
                    do
                    {
                        Console.WriteLine("Would you like to hit or stop? h for hit s for stop");
                        playerChoice = Console.ReadLine().ToUpper();
                    }
                    while (!playerChoice.Equals(Keys.HitMe) && !playerChoice.Equals(Keys.Stop));
                    if (string.Equals(playerChoice, Keys.HitMe, StringComparison.OrdinalIgnoreCase))
                    {
                        Hit();
                    }
                }
                else if (playerTotal > 21)
                {
                    loses++;
                    Console.WriteLine("You got more than 21 ! You lose ! The dealer's player Total was : {0}", dealerTotal);

                    if (loses > 2)
                    {
                        Console.WriteLine("You're losing a lot , maybe you should take it easy? ");
                    }

                }
                else if (playerTotal < 21)
                {
                    do
                    {
                        Console.WriteLine("Would you like to hit or stop? h for hit s for stop");
                        playerChoice = Console.ReadLine().ToUpper();
                    }
                    while (!playerChoice.Equals(Keys.HitMe) && !playerChoice.Equals(Keys.Stop));
                    if (string.Equals(playerChoice, Keys.HitMe, StringComparison.OrdinalIgnoreCase))
                    {
                        Hit();
                    }

                }
            }

        #endregion

        #region Play Again
            static void PlayAgain()
            {

                //Loop until they make a valid choice 
                do
                {
                    playAgain = Console.ReadLine().ToUpper();
                }
                while (!playAgain.Equals(Keys.Yes) && !playAgain.Equals(Keys.No));

                if (playAgain.Equals(Keys.Yes))
                {
                    Console.WriteLine("Press enter to restart the game!");
                    Console.ReadLine();
                    Console.Clear();
                    dealerTotal = 0;
                    Total = 1;
                    SuiteTotal = 1;
                    NaturalBlackJack = false;
                    playerTotal = 0;
                }
                else if (playAgain.Equals(Keys.No))
                {
                    Environment.Exit(0);
                }
            }

        #endregion
    }
}
