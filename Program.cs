using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text.Json;

namespace BankConsole // Corrected namespace declaration
{
    public class CardHolder
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string cardnum { get; set; }
        public int pin { get; set; }
        public double balance { get; set; }

        public CardHolder(string firstname, string lastname, string cardnum, int pin, double balance) // Corrected 'balence' to 'balance'
        {
            this.firstname = firstname;
            this.lastname = lastname;
            this.cardnum = cardnum;
            this.pin = pin;
            this.balance = balance;
        }
        public override string ToString()
        {
            return $"FirstName: {firstname}, LastName: {lastname}, CardNum: {cardnum}, PIN: {pin}, Balance: {balance}";
        }


        public string getFirstname()
        {
            return firstname;
        }

        public string getLastname()
        {
            return lastname;
        }

        public string getCardnum()
        {
            return cardnum;
        }

        public int getPin()
        {
            return pin;
        }

        public double getBalance()
        {
            return balance;
        }

        public void setFirstName(string newFirst)
        {
            firstname = newFirst;
        }

        public void setLastName(string newLast)
        {
            lastname = newLast;
        }

        public void setPin(int newPin)
        {
            pin = newPin;
        }
        public void setBalance(double newbalence){
            balance = newbalence;
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            string filepath = @"C:\Users\Abhay.Anand\FinalConsole(Bank)\trial.json";

            string jsonString = File.ReadAllText(filepath);

            List<CardHolder> records = JsonSerializer.Deserialize<List<CardHolder>>(jsonString);

            List<CardHolder> cardHolders = new List<CardHolder>();

            foreach (var record in records)
            {
                cardHolders.Add(record);
                Console.WriteLine(record);
            }

            


            void printOptions()
            {
                Console.WriteLine("Choose one of the options below:");
                Console.WriteLine("1) Deposit");
                Console.WriteLine("2) Withdraw");
                Console.WriteLine("3) Show Balance"); // Corrected spelling of "Balance"
                Console.WriteLine("4) Exit");
            }

            void Deposit(CardHolder currentUser)
            {
                Console.WriteLine("How much would you like to deposit?");
                string input = Console.ReadLine();
                if (double.TryParse(input, out double newDeposit))
                {
                    currentUser.setBalance(currentUser.getBalance() + newDeposit);
                    Console.WriteLine($"Thank you for your deposit, your new balance is: {currentUser.getBalance()}");
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid amount.");
                }
            }

            void Withdraw(CardHolder currentUser)
            {
                Console.WriteLine("How much would you like to withdraw?");
                string input = Console.ReadLine();
                if (double.TryParse(input, out double newWithdrawal))
                {
                    if (newWithdrawal > currentUser.getBalance())
                    {
                        Console.WriteLine($"Invalid amount withdrawn. Please withdraw less than: {currentUser.getBalance()}");
                    }
                    else
                    {
                        currentUser.setBalance(currentUser.getBalance() - newWithdrawal);
                        Console.WriteLine("Thank you, withdrawal successful!");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid amount.");
                }
            }

            void Balance(CardHolder currentUser)
            {
                Console.WriteLine($"Hi {currentUser.getFirstname()}, your current balance is: {currentUser.getBalance()}");
            }


            Console.WriteLine("Welcome to the simple ATM");
            Console.WriteLine("Please insert card: ");
            string debitnum = Console.ReadLine();

            CardHolder currentUser = cardHolders.Where(p => p.getCardnum() == debitnum).FirstOrDefault();
            if (currentUser == null)
            {
                Console.WriteLine("Card Number not recognised, Please try again");
                return;
            }

            Console.WriteLine("Please enter your PIN: ");
            int userPin = Convert.ToInt32(Console.ReadLine());

            if(currentUser.getPin() != userPin){
                Console.WriteLine("Pin is incorrect, please try again.");
                return;
            }


            Console.WriteLine($"Welcome: {currentUser.getFirstname()}");
            int option = 0;
            do
            {
                printOptions();
                try{
                    option = int.Parse(Console.ReadLine());
                }
                catch{}
                if(option == 1) { Deposit(currentUser);}
                else if(option == 2) {Withdraw(currentUser);}
                else if(option == 3) {Balance(currentUser);}
                else if(option == 4) {break;}
                else {option = 0;}
            }while(option != 0);
            Console.WriteLine("Thank you! Have a nice day.");

        }
    }
}
