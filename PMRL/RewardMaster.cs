using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMRL
{
    public class RewardMaster
    {
        private List<Tuple<State, double, int>> rewardSet = new List<Tuple<State, double, int>>();
        private List<int> alreadyReachGoal = new List<int>();
        private Config config = new Config();
        private int rewardCount = 0;
        private List<int> takeRewardCount = new List<int>();

        public void SetRewardPos(State s, double reward)
        {
            rewardSet.Add(new Tuple<State, double, int>(s, reward, 0));
            rewardCount++;
            if (rewardCount > 1) new StateElement(null).Sort(ref rewardSet);
        }
        public State SendRewardPos(int i)
        {
            return rewardSet[i].Item1;
        }
        public double SendRewardValue(int i)
        {
            return rewardSet[i].Item2;
        }
        public Tuple<double, bool> CheckState(State s)
        {
            for (int i = 0; i < rewardCount; i++)
            {
                if (s.StateChar.Comparison(rewardSet[i].Item1.StateChar))
                {
                    if (rewardSet[i].Item3 == 0)
                    {
                        rewardSet[i] = new Tuple<State, double, int>(rewardSet[i].Item1, rewardSet[i].Item2, rewardSet[i].Item3 + 1);
                        return new Tuple<double, bool>(rewardSet[i].Item2, true);
                    }
                    else
                    {
                        return new Tuple<double, bool>(0, true);
                    }
                }
            }
            return new Tuple<double, bool>(0, false);
        }
        public void ResetRewardPos()
        {
            rewardSet = new List<Tuple<State, double, int>>();
            rewardCount = 0;
        }
        public void Restart()
        {
            for (int i = 0; i < rewardSet.Count; i++)
            {
                rewardSet[i] = new Tuple<State, double, int>(rewardSet[i].Item1, rewardSet[i].Item2, 0);
            }
        }
        public int SendRewardNum(State s)
        {
            for (int i = 0; i < config.GoalNum; i++)
            {
                if (rewardSet[i].Item1.StateChar.Comparison(s.StateChar)) return i;
            }
            return -1;
        }
        public int RewardCount
        {
            get { return rewardCount; }
        }
    }
}
