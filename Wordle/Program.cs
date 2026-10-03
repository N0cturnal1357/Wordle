using System.Collections.Generic;
using System.Drawing;

namespace Wordle
{
    internal class Program
    {
        // GME-1020 A1 - Wordle - Jessica Daley
        // I forgot to put this ↑ info in the repo title... 

        // Current avg guesses to solve: 5-6

        enum LetterStatus
        {
            Correct,
            WrongPlace,
            Incorrect,
        }


        static void Main(string[] args)
        {

            //The game needs these variables. You can create some of your own if you need them, but don't remove these ones.
            Random _rng = new Random();   //RNG generator

            List<String> wordList = LoadWords();  //List of words loaded from file.

            String mysteryWord = wordList[_rng.Next(wordList.Count)];  //select the initial mystery word from the list - at random.
            string currentGuessWord = "";  //human or AI guess word
            int wordGuessCount = 0;  //count how many guesses
            int totalGuessCount = 0; //keep track of total guesses for average guess count
            int wordCount = 1;  //keep track of how many words have been guessed correctly
            float avgGuessCount = 0;  //keep track of average guess count

            List<String> AIWordList = LoadWords();  //**AI NOTE: i created this for you to use. It's a copy of the word list that the AI can access and/or change.
            LetterStatus[] AIStatusTracker;  //This is an array of 5 elements that tracks the status of each letter in a guessed word. You can use this to help your AI figure out what to guess next.


            //You can create your own variables here if needed.
            List<char> greens = new List<char>();
            List<char> yellows = new List<char>();

            while (wordCount < 1000)  //change this to 100 when ready to flex your AI. 10 is just for testing.
            {
                //Leave this alone. It counts how many words have been guessed.
                wordGuessCount++;
                //currentGuessWord = GetWord("What is your guess: ");

                //AI START
                //I believe most of your AI code could go here. The AI here needs to intelligently choose the next word to guess. Use AIWordList to help you. 
                //Comment out the line above before uncommenting this line.

               
                Console.WriteLine($"{AIWordList.Count} words remaining in the list.");
                if (wordGuessCount == 1) currentGuessWord = "slate"; // first guess is the best word
                else currentGuessWord = AIWordList[_rng.Next(AIWordList.Count)]; // random guess after first guess
                char[] guessChars = currentGuessWord.ToCharArray();

                //Leave this alone. It checks to see if the word you guessed is a valid word. This will work for your AI too. If the guessed word
                //isn't a real word, the loop will restart without costing a guess.
                if (wordList.Contains(currentGuessWord) == false)
                {
                    Console.WriteLine("Not a valid word, try again.");
                    wordGuessCount--;
                    continue;
                }

                //**AI Note: You can use AIStatusTracker to "see" what's right and what's wrong in your guessed word before checking your next word.
                //ALSO CheckWord is what checks the guessed word against the mystery word. Try not to remove or change this unless you REALLY know what you're doing.
                AIStatusTracker = CheckWord(mysteryWord, currentGuessWord);

                // filter words

                // better explanation of this -->> "word => word[i] != guessChars[i]" after much googling
                // since RemoveAll is going through every index of AIWordList, 'word' represents the current index being checked
                // and everything on the right of => represents the condition needed to be met to return true

                if (AIWordList.Count >= 1)
                {
                    for (int i = 0; i < guessChars.Length; i++)
                    {
                        if (AIStatusTracker[i] == LetterStatus.Correct) // remove words without green
                        {
                            // prevent duplicates in greens list
                            if (!greens.Contains(guessChars[i])) greens.Add(guessChars[i]); // add to list of greens
                            AIWordList.RemoveAll(word => !word.Contains(guessChars[i])); // remove words without green
                            AIWordList.RemoveAll(word => word[i] != guessChars[i]); // remove words without green at [i] position
                        }
                        else if (AIStatusTracker[i] == LetterStatus.WrongPlace) // remove words without yellow
                        {
                            // prevent duplicates in yellows list
                            if (!yellows.Contains(guessChars[i])) yellows.Add(guessChars[i]); // add to list of yellows
                            AIWordList.RemoveAll(word => !word.Contains(guessChars[i])); // remove words without yellow
                            AIWordList.RemoveAll(word => word[i] == guessChars[i]); // remove words with yellow at [i] position
                        }
                        //Console.WriteLine($"{guessChars[i]} checked");
                    }

                    for (int i = 0; i < guessChars.Length; i++)
                    {
                        // remove all blanks at [i] position
                        if (AIStatusTracker[i] == LetterStatus.Incorrect)
                        {
                            AIWordList.RemoveAll(word => word[i] == guessChars[i]); // remove words with blanks at [i] position
                            if (!yellows.Contains(guessChars[i]) && !greens.Contains(guessChars[i]))
                            {
                                AIWordList.RemoveAll(word => word.Contains(guessChars[i])); // remove words with blanks anywhere
                            }
                            Console.WriteLine($"Removed all words with {guessChars[i]}");
                        }
                    }
                }



                if (currentGuessWord != mysteryWord) AIWordList.Remove(currentGuessWord); // remove the last word guessed to prevent repeats

                Console.WriteLine($"List updated: {AIWordList.Count} words remaining.");

                //AI FINISH
                //Leave this alone. It tells you when you found the word, updates the counters, and moves on to the next word.
                if (mysteryWord.Equals(currentGuessWord))
                {
                    totalGuessCount += wordGuessCount;
                    avgGuessCount = (float)totalGuessCount / wordCount;
                    Console.WriteLine("Congrats, you got it in " + wordGuessCount + "! Avg guess count = " + avgGuessCount + " Resetting the mystery word.\n");
                    mysteryWord = wordList[_rng.Next(wordList.Count)];
                    AIWordList = LoadWords();
                    wordCount++;
                    wordGuessCount = 0;
                }
            }
        }


        //Check the guessed word against the mystery word.
        //Don't change this function without asking me first.
        static LetterStatus[] CheckWord(String word, String guess)
        {
            char[] wordChars = word.ToCharArray();      //convert mystery word and guessed word to char arrays for easier comparison.
            char[] guessChars = guess.ToCharArray();

            LetterStatus[] statusTracker = new LetterStatus[5]; //an array to hold the status of each guessed letter.
            //assume all the guessed letters start "incorrect"
            statusTracker[0] = LetterStatus.Incorrect;
            statusTracker[1] = LetterStatus.Incorrect;
            statusTracker[2] = LetterStatus.Incorrect;
            statusTracker[3] = LetterStatus.Incorrect;
            statusTracker[4] = LetterStatus.Incorrect;

            //This is part of the world rule check. This is used to deal with multiple green or yellow letters. Don't remove it.
            List<char> letterCounter = word.ToList();

            //green letters - right letter, right place
            for (int i = 0; i < guessChars.Length; i++)
            {
                for (int j = 0; j < wordChars.Length; j++)
                {
                    if (guessChars[i] == wordChars[j] && i == j)  //right letter, right place
                    {
                        statusTracker[i] = LetterStatus.Correct;  //set the status in the array
                        letterCounter.Remove(guessChars[i]);      //remove the letter from the counter so it can't be flagged yellow later
                        break;                                    //stop the loop since we found a match for this letter
                    }
                }
            }

            //yellow letters - right letter wrong place
            for (int i = 0; i < guessChars.Length; i++)
            {
                for (int j = 0; j < wordChars.Length; j++)
                {
                    if (statusTracker[i] == LetterStatus.Correct)       //if the letter is already flagged green, don't check it again
                        break;

                    if (guessChars[i] == wordChars[j] && i != j && statusTracker[j] != LetterStatus.Correct)    //right letter, wrong place and not already flagged green (correct)
                    {
                        //count occurences of that letter, only flag yellow if flags < actual occurences
                        if (letterCounter.Contains(guessChars[i]))
                        {
                            statusTracker[i] = LetterStatus.WrongPlace;  //set the status in the array
                            letterCounter.Remove(guessChars[i]);        //remove the letter from future consideration so it can't be flagged yellow again
                        }
                    }
                }
            }

            //print the guessed word with colors for correct and wrong place letters. 
            for (int i = 0; i < guessChars.Length; i++)
            {
                //set the color
                if (statusTracker[i] == LetterStatus.Correct)
                    Console.ForegroundColor = ConsoleColor.Green;
                else if (statusTracker[i] == LetterStatus.WrongPlace)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else
                    Console.ForegroundColor = ConsoleColor.White;
                                                                            // this just makes a more saturated yellow
                if (statusTracker[i] == LetterStatus.WrongPlace) Console.Write($"\u001b[38;2;255;255;0m{guessChars[i]}");  //print the letter
                else Console.Write(guessChars[i]);  //print the letter
                Console.ForegroundColor = ConsoleColor.White;  //reset back to white for the next letter
            }
            Console.WriteLine();        //print a new line after the guessed word is printed

            return statusTracker;       //return the arrray - mostly of use for the AI
        }


        //This method loads the words from file. Don't change
        //this without asking me first.
        static List<string> LoadWords()
        {
            List<string> wordList = new List<string>();
            string[] lines = File.ReadAllLines("../../../words.txt");
            foreach (string line in lines)
            {
                if (line.Length == 5)
                {
                    wordList.Add(line.ToLower());
                }
            }
            return wordList;
        }



        //This is the original humamn player GetWord method.
        //You can write an AI version, if it's helpful for
        //you.
        static string GetWord(String prompt)
        {
            Console.Write(prompt);
            string inputWord = Console.ReadLine();
            while (inputWord.Length != 5)
            {
                Console.Write("Word must be 5 letters. Try again:");
                inputWord = Console.ReadLine();
            }
            return inputWord.ToLower();
        }
    }
}
