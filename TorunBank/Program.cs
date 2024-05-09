using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorunBank
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int newCustomerCount = 2; // vezne bekleyen yeni müşteri sayısı

            List<Customer> customersList = new List<Customer>();

            for (int counter = 1; counter <= newCustomerCount; counter++)
            {
                Console.WriteLine("Hoşgeldiniz, Adınızı giriniz : ");
                Customer customer = new Customer(Console.ReadLine());
                Console.WriteLine("Soyadınızı giriniz : ");
                customer.LastName = Console.ReadLine();
                Console.WriteLine("Cinsiyetiniz Kadın ise 'K', Erkek ise 'E' giriniz : ");
                string gender = Console.ReadLine();
                if (gender == "K" || gender == "k")
                {
                    customer.Gender = Gender.Kadin;
                }
                else if (gender == "E" || gender == "e")
                {
                    customer.Gender = Gender.Erkek;
                }
                else
                {
                    Console.WriteLine("Geçersiz ifade girdiniz.");
                    customer.Gender = Gender.Belirsiz;
                }
                Console.WriteLine("Adresinizi giriniz : ");
                customer.Address = Console.ReadLine();

                Console.WriteLine("Hesap türünüzü seçiniz (1/2/3/4) : ");
                Console.WriteLine("1- Vadeli");
                Console.WriteLine("2- Vadesiz");
                Console.WriteLine("3- Bireysel");
                Console.WriteLine("4- Ticari");
                string selection = Console.ReadLine();
                Account account = new Account(customer);
                switch (selection)
                {
                    case "1":
                        account.AccountType = AccountType.Vadeli;
                        break;
                    case "2":
                        account.AccountType = AccountType.Vadesiz;
                        break;
                    case "3":
                        account.AccountType = AccountType.Bireysel;
                        break;
                    case "4":
                        account.AccountType = AccountType.Ticari;
                        break;
                    default:
                        Console.WriteLine("Geçersiz ifade girdiniz.");
                        account.AccountType = AccountType.Tanimsiz;
                        break;
                }

                Console.WriteLine("Hesap başlangıç para miktarını giriniz : ");
                account.Balance = Convert.ToInt32(Console.ReadLine());

                customersList.Add(customer);
                customer.Accounts.Add(account);
            }
            

            Bank bank = new Bank() { BankName = "Torun Bank", BankAddress = "Nilüfer", BankNumber = 4543, Customers = customersList };
            bank.Customers.ForEach(i => Console.Write("{0}\t {1}\n", i.Email, i.CustomerNumber));

            customersList[0].Accounts[0].Bank = bank;

            Console.WriteLine(customersList[0].Accounts[0].Balance); 
            
            Transaction transaction = new Transaction(2500000) { SenderAccount = customersList[customersList.Count - 1].Accounts[0], ReceiverAccount = customersList[0].Accounts[0], TransactionSource = TransactionSource.BankOffice, TransactionType = TransactionType.Fast};
        }
    }

    public class Bank
    {
        public int BankNumber { get; set; }
        public string BankName { get; set; }
        public string BankAddress { get; set;}
        public List<Customer> Customers { get; set; }
    }
    public class Customer
    {
        public Customer(string name)
        {
            Name = name;
            InitializeMail();
        }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Tc { get; } = DataGenerator.GetRandomTc();
        public Gender Gender { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; } = DataGenerator.GetRandomPhoneNumber();
        public string Email { get; private set; }
        public void InitializeMail()
        {
            Email = DataGenerator.GetRandomEmail(this.Name);
        }
        public string CustomerNumber { get; } = DataGenerator.GetRandomCustomerNumber();
        public List<Account> Accounts { get; set; } = new List<Account>();// one olanda tutmak daha uygun
        public string FullName 
        { 
            get 
            { 
                return Name + " " + LastName;
            }
        }
        
    }

    public class Account
    {
        public Account(Customer customer)
        {
            InitializeIban(customer);
        }
        public AccountType AccountType { get; set; }
        public string CardNumber { get; } = DataGenerator.GetRandomCardNumber();
        public string Iban { get; private set; }
        public void InitializeIban(Customer customer)
        {
            Iban = DataGenerator.GetRandomIBAN(customer.CustomerNumber);
        }
        public double Balance { get; set; }
        public Bank Bank { get; set; }
        public bool isActive { get; set; } = true;
        public void Deposit(double balance) // yatırma
        {
            Balance = Balance + balance;
        }
        public void Withdraw(double balance) // çekme
        {
            Balance = Balance - balance;
        }
        public bool CheckBlackListForCustomer(Customer customer)
        {
            List<string> customerNumbers = new List<string>() { "12345", "46879", "16483", "35648", "12351", "32154" };
            foreach (string cn in customerNumbers)
            {
                if (cn == customer.CustomerNumber)
                {
                    return true;
                }
                
            }
            return false;
        }
    }

    public interface ITax
    {
        double CalculateBsmv(double balance);
        double CalculateKkdf(double balance);

    }
    public class Transaction : ITax
    {
        public Transaction(double transferBalance)
        {
            CalculateBsmv(transferBalance);
            CalculateKkdf(transferBalance);
            TransferBalance = transferBalance;
        }
        public int TransactionId { get; }
        public TransactionSource TransactionSource { get; set; }
        public DateTime TransactionDate { get; } = DateTime.Now;
        public TransactionType TransactionType { get; set; }
        public Account SenderAccount { get; set; }
        public Account ReceiverAccount { get; set; }
        public double TransferBalance { get; set; }
        public double TransferCost { get; } = 4.5;

        public double CalculateBsmv(double balance)
        {
            return balance + (balance*0.05);
        }

        public double CalculateKkdf(double balance)
        {
            return balance + (balance * 0.01);
        }
    }

    public class DataGenerator
    {
        public static string GetRandomEmail(string name)
        {
            Random random = new Random();
            string[] mail = { "@gmail.com", "@hotmail.com", "@outlook.com" };
            return string.Concat(name, mail[random.Next(0, mail.Length)]);
        }

        public static string GetRandomPhoneNumber()
        {
            Random random = new Random();
            string[] starts = { "0535", "0536", "0537", "0538", "0551", "0552", "0553", "0554", "0555" };
            string randomStart = starts[random.Next(starts.Length)];
            string randomEnd = random.Next(1000000, 9999999).ToString();
            return string.Concat(randomStart, randomEnd);
        }

        public static string GetRandomTc()
        {
            Random random = new Random();
            string randomStart = random.Next(100000, 999999).ToString();
            string randomEnd = random.Next(10000, 99999).ToString();
            return string.Concat(randomStart, randomEnd);
        }

        public static string GetRandomCustomerNumber()
        {
            Random random = new Random();
            string start = random.Next(100, 999).ToString();
            string middle = random.Next(1000, 9999).ToString();
            string end = random.Next(10, 99).ToString();
            string result = string.Concat(start, middle, end);
            return result;
        }

        public static string GetRandomCardNumber()
        {
            Random random = new Random();
            
            string part1 = random.Next(1000, 9999).ToString();
            string part2 = random.Next(1000, 9999).ToString();
            string part3 = random.Next(1000, 9999).ToString();
            string part4 = random.Next(1000, 9999).ToString();
            
            return string.Join(" ", part1, part2, part3, part4);
        }

        public static string GetRandomIBAN(string customerNumber)
        {
            Random random = new Random();
            string firstLetter = "TR";
            string ibanStart = random.Next(10, 99).ToString();
            string start = string.Concat(firstLetter, ibanStart);
            string ibanMiddle = random.Next(1000, 9999).ToString();
            string ibanMiddle2 = random.Next(1000, 9999).ToString();
            string ibanMiddle3 = random.Next(1000, 9999).ToString();
            string ibanMiddle4 = random.Next(0, 9).ToString();
            string ibanMiddle5 = string.Concat(ibanMiddle4, customerNumber.Substring(0,3));
            string ibanMiddle6 = customerNumber.Substring(3,4);
            string ibanEnd = customerNumber.Substring(7,2); 
            return string.Join(" ", start, ibanMiddle,ibanMiddle2,ibanMiddle3,ibanMiddle5,ibanMiddle6, ibanEnd);
        }
    }

    public enum TransactionSource
    {
        ATM, Mobile, QRCode, BankOffice, Batch
    }

    public enum Gender
    {
        Kadin, Erkek, Belirsiz
    }

    public enum AccountType
    {
        Vadeli, Vadesiz, Bireysel, Ticari, Tanimsiz
    }

    public enum TransactionType
    {
        Havale, EFT, Fast
    }
}



//Customer,Account,Bank,Transaction,
//vadeli,vadesiz,bireysel,ticari

