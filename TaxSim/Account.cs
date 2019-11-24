using System;
namespace TaxSim
{
    public class Account
    {
        protected float _balance;
        protected float _interestRate;

        public Account()
        {
            _balance = 0;
            _interestRate = 0;
        }

        public float GetBalance()
        {
            return _balance;
        }

        public void Deposit(float amount)
        {
            _balance += amount;
        }

        public void Withdraw(float amount)
        {
            _balance -= amount;
        }

        public static void Transfer(Account from, Account to, float amount)
        {
            from.Withdraw(amount);
            to.Deposit(amount);
        }

        public void Tick()
        {
            _balance = _balance * (1 + _interestRate);
        }
    }
}
