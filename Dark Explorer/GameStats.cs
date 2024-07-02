using System.Threading.Tasks;

namespace Dark_Explorer
{
    public class GameStats
    {
        public int Score { get; private set; }
        public int Health { get; private set; }

        public GameStats()
        {
            ResetStats();
        }

        public void ResetStats()
        {
            Score = 0;
            Health = 3;
        }

        public void AddScore(int amount)
        {
            Task.Delay(5);
            Score += amount;
        }

        public void TakeDamage(int amount)
        {
            Health -= amount;
            if (Health < 0)
                Health = 0;
        }

        public bool IsGameOver()
        {
            return Health <= 0;
        }
    }
}
