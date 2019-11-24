using System;
namespace TaxSim
{
    public class TFSAAccount : Account
    {
        private float _contributionRoom;
        public TFSAAccount()
        {
            _contributionRoom = 6000;
            _interestRate = 0.10F;
        }

        public void Tick()
        {
            base.Tick();
            _contributionRoom += 6000;
        }

        public float GetAvailableContributionRoom()
        {
            return _contributionRoom - _balance;
        }
    }
}
