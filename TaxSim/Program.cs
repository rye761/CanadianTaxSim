using System;

namespace TaxSim
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Wallet wallet = new Wallet();
            wallet.ReceivePaycheck(300000);
            Console.WriteLine(wallet.GetBalance());
        }
    }
}
