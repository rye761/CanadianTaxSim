using System;
using System.Collections.Generic;

namespace TaxSim
{
    public class Wallet
    {
        private Dictionary<string, Account> accounts;
        private Dictionary<string, Account> taxAccounts;

        public Wallet()
        {
            accounts = new Dictionary<string, Account>();
            accounts.Add("cash", new Account());

            taxAccounts = new Dictionary<string, Account>();
            taxAccounts.Add("ei", new Account());
            taxAccounts.Add("cpp", new Account());
            taxAccounts.Add("income", new Account());
        }


        public float GetBalance()
        {
            return SumAccountDict(accounts);
        }

        public float GetTaxPaid()
        {
            return SumAccountDict(taxAccounts);
        }

        public void ReceivePaycheck(float amount)
        {
            float cppAmount = Math.Min(Math.Max(0, amount - Rates.cppExemption)
                * Rates.cppRate, Rates.cppMaximumContribution);
            float eiAmount = Math.Min(amount * Rates.eiRate, Rates.eiMaximumContribution);
            float cashAmount = amount - cppAmount - eiAmount;

            taxAccounts["cpp"].Deposit(cppAmount);
            taxAccounts["ei"].Deposit(eiAmount);

            float federalTaxableIncome = amount - Rates.baseFederalExemption;
            float provincialTaxableIncome = amount - Rates.baseProvincialExemption;
            float incomeTaxAmount = Rates.ComputeProvincialTax(provincialTaxableIncome) + Rates.ComputeFederalTax(federalTaxableIncome);
            cashAmount -= incomeTaxAmount;

            taxAccounts["income"].Deposit(incomeTaxAmount);

            accounts["cash"].Deposit(cashAmount);

            Console.WriteLine("Paycheck received: Cash: " + cashAmount + ", CPP: " + cppAmount + ", EI: " + eiAmount + ", Tax: " + incomeTaxAmount);
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
