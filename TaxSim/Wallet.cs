using System;
using System.Collections.Generic;

namespace TaxSim
{
    public class Wallet
    {
        private Dictionary<string, Account> accounts;
        private Dictionary<string, Account> taxAccounts;
        private float rrspContribution;
        private float tfsaContribution;

        public Wallet()
        {
            accounts = new Dictionary<string, Account>();
            accounts.Add("cash", new Account());
            accounts.Add("rrsp", new RRSPAccount());
            accounts.Add("tfsa", new TFSAAccount());

            taxAccounts = new Dictionary<string, Account>();
            taxAccounts.Add("ei", new Account());
            taxAccounts.Add("cpp", new Account());
            taxAccounts.Add("income", new Account());

            rrspContribution = 0;
            tfsaContribution = 0;
        }


        public float GetBalance()
        {
            return SumAccountDict(accounts);
        }

        public float GetAccountBalance(string identifier)
        {
            if (accounts.ContainsKey(identifier))
            {
                return accounts[identifier].GetBalance();
            }
            else if (taxAccounts.ContainsKey(identifier))
            {
                return taxAccounts[identifier].GetBalance();
            }
            else
            {
                return 0;
            }
        }

        public float GetAnnualChange(string identifier)
        {
            if (accounts.ContainsKey(identifier))
            {
                return accounts[identifier].GetAnnualChange();
            }
            else if (taxAccounts.ContainsKey(identifier))
            {
                return taxAccounts[identifier].GetAnnualChange();
            }
            else
            {
                return 0;
            }
        }

        public float GetTaxPaid()
        {
            return SumAccountDict(taxAccounts);
        }

        public void SetRRSPContribution(float contribution)
        {
            rrspContribution = Math.Min(contribution, Rates.maxRRSPContribution);
        }

        public void SetTFSAContribution(float contribution)
        {
            tfsaContribution = contribution;
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
            float tfsaAmount = Math.Min((accounts["tfsa"] as TFSAAccount).GetAvailableContributionRoom(), amount * tfsaContribution);
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
