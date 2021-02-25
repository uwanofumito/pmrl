using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMRL
{
    class Logger
    {
        private Config config;
        private List<double[]> bidStack;
        private int[,] stepTableStack;
        private int stepTSnumber;
        private int[] goalRank;
        private int[,] visitRate;
        private List<State> goalposSet;
        private List<double[]> meanRewards;
        private List<double[]> stdRewards;
        public Logger(Config config)
        {
            this.config = config;
            stepTableStack = new int[Math.Max(1, this.config.Learning_Iteration / 10000), 100];// new int[Config.Learning_Iteration, Config.AgentNum * Config.GoalNum]; // 1000000学習分
            goalRank = new int[100];// Config.GoalNum];
            stepTSnumber = 0;
            bidStack = new List<double[]>();
            goalposSet = new List<State>();
            meanRewards = new List<double[]>();
            stdRewards = new List<double[]>();
            visitRate = new int[this.config.CellWidth, this.config.CellHeight];
            Parallel.For(0, 100/*Config.GoalNum*/, j =>
            {
                stepTableStack[0, j] = 100;
            });
        }
        public void LogVisitRate(State s)
        {
            return;
            visitRate[s.StateChar.X - 1, s.StateChar.Y - 1]++;
        }
        public void LogGoalPosSet(State s)
        {
            return;
            goalposSet.Add(s);
        }
        public void LogBidStack(double[] bids)
        {
            return;
            bidStack.Add(bids);
        }
        public void LogMeanReward(double[] meanReward)
        {
            return;
            meanRewards.Add(meanReward);
        }
        public void LogStdReward(double[] stdReward)
        {
            return;
            stdRewards.Add(stdReward);
        }
        public void LogStepTableStack(int cIteration, int[] stepTable)
        {
            return;
            if (cIteration % 10000 != 0) return;
            Parallel.For(0, config.GoalNum, j =>
            {
                stepTableStack[stepTSnumber, j] = stepTable[j];
            });
            stepTSnumber++;
        }
        public void Output(String name)
        {
            return;
            string[] data = new string[goalposSet.Count];
            Parallel.For(0, goalposSet.Count, i =>
            { data[i] = goalposSet[i].StateChar.X + "," + goalposSet[i].StateChar.Y; });
            config.Save<string>(ref data, (name + "_goalposition"));
            data = new string[bidStack.Count + 1];
            for (int j = 0; j < config.GoalNum; j++)
            {
                data[0] += j + ",";
            }
            Parallel.For(1, bidStack.Count + 1, i =>
            {
                for (int j = 0; j < config.GoalNum; j++)
                {
                    data[i] += bidStack[i - 1][j] + ",";
                }
            });
            config.Save<string>(ref data, (name + "_bidseq"));

            data = new string[stepTSnumber + 1];
            data[0] = "goal1,goal2,goal3,goal4,goal5";
            Parallel.For(0, stepTSnumber, i =>
            {
                for (int k = 0; k < config.GoalNum; k++)
                {
                    data[i + 1] += stepTableStack[i, k] + ",";
                }
            });
            config.Save<string>(ref data, (name + "_havingSteps"));

            data = new string[config.CellHeight];
            Parallel.For(0, config.CellHeight, i =>
            {
                for (int j = 0; j < config.CellWidth; j++)
                {
                    data[i] += visitRate[j, i] + ",";
                }
            });
            config.Save<string>(ref data, (name + "_visitRate"));

            data = new string[meanRewards.Count];
            Parallel.For(0, meanRewards.Count, i =>
            {
                for (int j = 0; j < meanRewards[i].Length; j++)
                {
                    data[i] += meanRewards[i][j].ToString() + ",";
                }
            });
            config.Save<string>(ref data, (name + "_meanReward"));

            data = new string[stdRewards.Count];
            Parallel.For(0, stdRewards.Count, i =>
            {
                for (int j = 0; j < stdRewards[i].Length; j++)
                {
                    data[i] += stdRewards[i][j].ToString() + ",";
                }
            });
            config.Save<string>(ref data, (name + "_stdReward"));
        }
    }
}
