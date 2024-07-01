using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text.Json;
using System.Data.SqlClient;
using System.IO;
using System.Collections;

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


        public string getFirstname() => firstname;
        public string getLastname() => lastname;
        public string getCardnum() => cardnum;
        public int getPin() => pin;
        public double getBalance() => balance;
        public void setFirstName(string newFirst) => firstname = newFirst;
        public void setLastName(string newLast) => lastname = newLast;
        public void setPin(int newPin) => pin = newPin;
        public void setBalance(double newBalance) => balance = newBalance;

    }

    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; }
    }

    public class Config
    {
        public ConnectionStrings ConnectionStrings { get; set; }
    }


    public class Program
    {
        static void Main(string[] args)
        {
            string filepath = @"C:\Users\Abhay.Anand\FinalConsole(Bank)\trial.json";
            //string fileconnect = @"C:\Users\Abhay.Anand\FinalConsole(Bank)\connect.json";

            string jsonString = File.ReadAllText(filepath);

            //var connectionStringObj = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(fileconnect);
            //string connectstring = File.ReadAllText(fileconnect);
            // Deserialize the JSON into the Config object
            //String config = JsonSerializer.Deserialize(connectstring);

            // Extract the connection string
            //string connectionString = config.ConnectionStrings.DefaultConnection;

            string connectionString = "Data Source=localhost;Initial Catalog=BANK;Integrated Security=True;User Id=sa;password=Ivanti12345";

            List<CardHolder> records = JsonSerializer.Deserialize<List<CardHolder>>(jsonString);

            List<CardHolder> cardHolders = new List<CardHolder>();

            foreach (var record in records)
            {
                cardHolders.Add(record);
                Console.WriteLine(record);
            }


            void PushToDB()
            {
                string query = "INSERT INTO Bank_Console1 (firstname, lastname, cardnum, pin, balance) VALUES (@firstname, @lastname, @cardnum, @pin, @balance)";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    foreach (var record in records)
                    {
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@firstname", record.firstname);
                            command.Parameters.AddWithValue("@lastname", record.lastname);
                            command.Parameters.AddWithValue("@cardnum", record.cardnum);
                            command.Parameters.AddWithValue("@pin", record.pin);
                            command.Parameters.AddWithValue("@balance", record.balance);

                            command.ExecuteNonQuery();
                        }
                    }

                }

            }
            PushToDB(); 

            bool UpdateToDB()
            {
                bool operationSuccessful = true;
                string updateQuery = @"
                                    MERGE Bank_Console1 AS target
                                    USING (SELECT @cardnum AS cardnum, @firstname AS firstname, @lastname AS lastname, @pin AS pin, @balance AS balance) AS source
                                    ON (target.cardnum = source.cardnum)
                                    WHEN MATCHED THEN
                                        UPDATE SET balance = source.balance
                                    WHEN NOT MATCHED THEN
                                        INSERT (firstname, lastname, cardnum, pin, balance)
                                        VALUES (source.firstname, source.lastname, source.cardnum, source.pin, source.balance);";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open ();
                    foreach (var record in records)
                    {
                        using (SqlCommand command = new SqlCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@firstname", record.firstname);
                            command.Parameters.AddWithValue("@lastname", record.lastname);
                            command.Parameters.AddWithValue("@cardnum", record.cardnum);
                            command.Parameters.AddWithValue("@pin", record.pin);
                            command.Parameters.AddWithValue("@balance", record.balance);

                            command.ExecuteNonQuery();
                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected == 0)
                            {
                                operationSuccessful = false;
                                Console.WriteLine($"Operation failed for card number {record.getCardnum()}.");
                            }
                            else if (rowsAffected == 1) // Assuming one row is affected for update or insert
                            {
                                operationSuccessful = true;
                            }

                        }
                    }
                }
                return operationSuccessful;
                
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
            UpdateToDB();

        }
    }
}
