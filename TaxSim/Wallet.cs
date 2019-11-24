using System;
using System.Collections.Generic;

namespace TaxSim
{
    public class Wallet
    {
        private Dictionary<string, Account> accounts;
        private Dictionary<string, Account> taxAccounts;
        private float rrspContribution;

        public Wallet()
        {
            accounts = new Dictionary<string, Account>();
            accounts.Add("cash", new Account());
            accounts.Add("rrsp", new RRSPAccount());

            taxAccounts = new Dictionary<string, Account>();
            taxAccounts.Add("ei", new Account());
            taxAccounts.Add("cpp", new Account());
            taxAccounts.Add("income", new Account());

            rrspContribution = 0;
        }


        public float GetBalance()
        {
            return SumAccountDict(accounts);
        }

        public float GetTaxPaid()
        {
            return SumAccountDict(taxAccounts);
        }

        public void SetRRSPContribution(float contribution)
        {
            rrspContribution = Math.Min(contribution, Rates.maxRRSPContribution);
        }

        public void Tick()
        {
            foreach (Account account in accounts.Values)
            {
                account.Tick();
            }

            foreach (Account account in taxAccounts.Values)
            {
                account.Tick();
            }
        }

        public void ReceivePaycheck(float amount)
        {
            float cppAmount = Math.Min(Math.Max(0, amount - Rates.cppExemption)
                * Rates.cppRate, Rates.cppMaximumContribution);
            float eiAmount = Math.Min(amount * Rates.eiRate, Rates.eiMaximumContribution);
            float rrspAmount = Math.Min(amount * rrspContribution, Rates.maxRRSPContribution);
            float cashAmount = amount - cppAmount - eiAmount - rrspAmount;

            taxAccounts["cpp"].Deposit(cppAmount);
            taxAccounts["ei"].Deposit(eiAmount);

            accounts["rrsp"].Deposit(rrspAmount);

            float federalTaxableIncome = amount - Rates.baseFederalExemption - rrspAmount;
            float provincialTaxableIncome = amount - Rates.baseProvincialExemption - rrspAmount;
            float incomeTaxAmount = Rates.ComputeProvincialTax(provincialTaxableIncome) + Rates.ComputeFederalTax(federalTaxableIncome);
            cashAmount -= incomeTaxAmount;

            taxAccounts["income"].Deposit(incomeTaxAmount);

            accounts["cash"].Deposit(cashAmount);
        }

        public void PrintAccounts()
        {
            Console.WriteLine("Accounts: ");
            foreach (KeyValuePair<string, Account> account in accounts)
            {
                Console.WriteLine(account.Key + ": " + account.Value.GetBalance());
            }

            Console.WriteLine("Tax Payments: ");
            foreach (KeyValuePair<string, Account> account in taxAccounts)
            {
                Console.WriteLine(account.Key + ": " + account.Value.GetBalance());
            }
        }

        public Boolean Spend(float amount)
        {
            if (amount <= accounts["cash"].GetBalance())
            {
                accounts["cash"].Withdraw(amount);
                return true;
            }
            else
            {
                return false;
            }
        }

        private float SumAccountDict(Dictionary<string, Account> dict)
        {
            float balance = 0;
            foreach (Account account in accounts.Values)
            {
                balance += account.GetBalance();
            }

            return balance;
        }
    }
}
