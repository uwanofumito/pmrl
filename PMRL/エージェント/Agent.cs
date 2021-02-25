using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PMRL
{
    class MemoryTable
    {
        private int goal;
        private int step;
        private double reward;
        public MemoryTable(int g = 0, int s = 0, double r = 0)
        {
            goal = g;
            step = s;
            reward = r;
        }
        public int Goal
        { get { return goal; } }
        public int Step
        { get { return step; } }
        public double Reward
        { get { return reward; } }
    }

    class Memory
    {
        private List<State> goalStateSet;
        private List<MemoryTable> stepTableList;
        private int[] stepTable;
        private double[] stanStepTable;
        private int[] rankedStepTable;
        private double[] rewardTable;
        private List<double>[] rewardMemories;
        private int[] goalCountMemories;
        private int sumGoalCountMemories;
        private int pastGoalCount;
        private int iteration;
        private int _iteration;
        private Config config;
        private int i, j, k;
        public Memory(Config config)
        {
            this.config = config;
            goalStateSet = new List<State>();
            stepTableList = new List<MemoryTable>();
            stepTable = new int[100];
            stanStepTable = new double[100];
            rankedStepTable = new int[100];
            rewardTable = new double[100];
            rewardMemories = new List<double>[100];
            goalCountMemories = new int[100];
            for (i = 0; i < 100; i++)
            {
                rewardMemories[i] = new List<double>();
                goalCountMemories[i] = 0;
            }
        }
        public void MemoryReset()
        {
            _iteration = 0;
            sumGoalCountMemories = 0;
            for (i = 0; i < 100; i++)
            {
                goalCountMemories[i] = 0;
            }
        }
        public void SetGoalState(State s)
        {
            goalStateSet.Add(s);
            while (goalStateSet.Count > config.Cooperate_Cycle)
            {
                goalStateSet.RemoveAt(0);
            }
        }
        public void SetStepTableList(MemoryTable mt)
        {
            stepTableList.Add(mt);
            rewardMemories[mt.Goal].Add(mt.Reward);
            while (stepTableList.Count > config.Cooperate_Cycle)
            {
                stepTableList.RemoveAt(0);
            }
            for (i = 0; i < 100; i++)
            {
                while (rewardMemories[i].Count > config.Cooperate_Cycle)
                {
                    rewardMemories[i].RemoveAt(0);
                }
            }
        }
        public void SetGoalCountMemories(bool flag, int g)
        {
            if (flag == false)
            {
                _iteration++;
                iteration++;
            }
            if (_iteration == sumGoalCountMemories)
            {
                if (g > pastGoalCount)
                {
                    goalCountMemories[pastGoalCount]--;
                    goalCountMemories[g]++;
                }
            }
            else
            {
                pastGoalCount = g;
                goalCountMemories[g]++;
                sumGoalCountMemories++;
            }
        }
        public int SendRankedStepIndex(int coopRange)
        {
            return rankedStepTable[coopRange];
        }
        public double GoalCountMemoriesRatio(int index)
        {
            return (double)goalCountMemories[index] / goalCountMemories.Sum();
        }
        public int GoalDetecting(State currentGoal)
        {
            List<State> goalSet = new List<State>();
            for (i = 0; i < goalStateSet.Count; i++)
            {
                if (!goalStateSet[i].StateChar.Comparison(new StateElement(null)))
                {
                    if (goalSet.Count <= 0) goalSet.Add(new State(goalStateSet[i].StateChar));
                    else
                    {
                        bool newgoalFlag = false;
                        for (j = 0; j < goalSet.Count; j++)
                        {
                            if (goalSet[j].StateChar.Comparison(goalStateSet[i].StateChar)) newgoalFlag = true;
                        }
                        if (newgoalFlag != true) goalSet.Add(new State(goalStateSet[i].StateChar));
                    }
                }
            }
            new StateElement(null).Sort(ref goalSet);
            if (goalSet.Count == config.GoalNum)
            {
                for (i = 0; i < goalSet.Count; i++)
                {
                    if (currentGoal.StateChar.Comparison(goalSet[i].StateChar)) return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// ステップテーブルを更新
        /// </summary>
        /// <returns></returns>
        public void updateTables(ref int[] stepTable, ref double[] rewardTable)
        {
            for (j = 0; j < config.GoalNum; j++)
            {
                stepTable[j] = config.MaxStep;
                rewardTable[j] = 0;
            }
            for (i = 0; i < stepTableList.Count; i++)
            {
                if (stepTable[stepTableList[i].Goal] > stepTableList[i].Step)
                {
                    stepTable[stepTableList[i].Goal] = stepTableList[i].Step;
                }
                if (rewardTable[stepTableList[i].Goal] < stepTableList[i].Reward)
                {
                    rewardTable[stepTableList[i].Goal] = stepTableList[i].Reward;
                }
            }
            for (i = 0; i < config.GoalNum; i++)
            {
                //stanStepTable[i] = this.stepTable[i] - Math.Log(/*rewardTable[0]*/10 / (rewardTable[i] + 1), config.Gamma);
                stanStepTable[i] = this.stepTable[i] / (rewardTable[i] + 1);
            }
            bool flag = false;
            for (i = 0; i < stepTable.Length; i++)
            {
                if (this.stepTable[i] != stepTable[i])
                {
                    flag = true;
                    this.stepTable[i] = stepTable[i];
                }
                this.rewardTable[i] = rewardTable[i];
            }
            if (flag)
            {
                for (i = 0; i < config.GoalNum; i++)
                {
                    int minStep = int.MaxValue;
                    rankedStepTable[i] = -1;
                    for (j = 0; j < config.GoalNum; j++)
                    {
                        if (minStep > this.stepTable[j])
                        {
                            bool flag2 = true;
                            for (k = 0; k < i; k++)
                            {
                                if (rankedStepTable[k] == j) flag2 = false;
                            }
                            if (flag2)
                            {
                                minStep = this.stepTable[j];
                                rankedStepTable[i] = j;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 標準ステップテーブルを更新
        /// </summary>
        /// <returns></returns>
        public void updateStanStepTables(ref double[] IstepTable)
        {
            for (i = 0; i < config.GoalNum; i++)
            {
                IstepTable[i] = stanStepTable[i];
            }
        }
        /// <summary>
        /// 期待報酬を更新
        /// </summary>
        /// <returns></returns>
        public void calcRewardStability(int rewardNum, ref double[] meanRewards, ref double[] stdRewards, ref int[] rewardCounts)
        {
            for (i = 0; i < rewardCounts.Length; i++) rewardCounts[i] = 0;
            //rewardCounts = new int[rewardNum];
            for (j = 0; j < rewardNum; j++)
            {
                meanRewards[j] = 0;
                stdRewards[j] = 0;
            }
            for (i = Math.Max(0, stepTableList.Count - 5000); i < stepTableList.Count; i++)
            {
                meanRewards[stepTableList[i].Goal] += stepTableList[i].Reward;
                rewardCounts[stepTableList[i].Goal]++;
            }
            for (j = 0; j < rewardNum; j++)
            {
                meanRewards[j] /= rewardCounts[j];
            }
            for (i = Math.Max(0, stepTableList.Count - 5000); i < stepTableList.Count; i++)
            {
                stdRewards[stepTableList[i].Goal] += Math.Pow(stepTableList[i].Reward - meanRewards[stepTableList[i].Goal], 2);
            }
            for (j = 0; j < rewardNum; j++)
            {
                stdRewards[j] /= rewardCounts[j];
                stdRewards[j] = Math.Sqrt(stdRewards[j]);
            }
        }
        public int Iteration
        {
            get { return iteration; }
        }
        public int _Iteration
        {
            get { return _iteration; }
        }
    }
    /// <summary>
    /// 環境の向こう側、このプログラム最大のクラス
    /// </summary>
    class Agent
    {
        private int agentnum;
        private int actionNum;
        private int maxactionNum;
        private int pastactionNum;
        private int step;
        private int mode;
        private int coopRange;
        private int confAgentNum;
        private int searchRange;
        private bool[] badCoopRange;
        private int[] overlapGoalCount;
        private int[] eachGoalCount;
        private int[] stepTable;
        private bool[] FstepTable;
        private double[] IstepTable;
        private double[] rewardTable;
        private int[,] goalTable;
        private double[] bidValue;
        private int[] bidCount;
        private double randomNum; // ランダムの確率
        private Boolean modeChange; // 学習と評価のモードチェンジ（false:learning, true:evaluation）
        private Boolean absorbingFlag;
        private Boolean goalFlag;
        private Boolean lastlearningFlag;
        private State state;
        private State paststate;
        private Memory memory;
        private ActionSet actionSet;
        private double reward;
        private int iS;
        private int iSflag;
        private int iSCount;
        private double[] meanRewards;
        private double[] stdRewards;
        private int[] rewardCounts;
        // 出力用変数
        private Logger logger;
        private Config config;

        public Agent(Config config)
        {
            this.config = config;
            logger = new Logger(config);
            randomNum = 20;
            step = 0;
            actionNum = 0;
            maxactionNum = this.config.ActionCount;
            pastactionNum = this.config.ActionCount;
            mode = 0;
            coopRange = this.config.AgentNum;// int.MaxValue;
            confAgentNum = 0;
            searchRange = 0;
            badCoopRange = new bool[100];
            overlapGoalCount = new int[100];// Config.GoalNum];
            eachGoalCount = new int[100];// Config.GoalNum];
            stepTable = new int[100];// new int[Config.AgentNum, Config.GoalNum];
            FstepTable = new bool[100];// new bool[Config.AgentNum, Config.GoalNum];
            IstepTable = new double[100];// new double[Config.AgentNum, Config.GoalNum];
            rewardTable = new double[100];
            meanRewards = new double[100];
            stdRewards = new double[100];
            rewardCounts = new int[100];
            Parallel.For(0, 100/*Config.GoalNum*/, j =>
           {
               stepTable[j] = 100;
               rewardTable[j] = 0;
               badCoopRange[j] = false;
           });
            goalTable = new int[this.config.AgentNum, 100];// Config.GoalNum];
            bidValue = new double[100]; // new double[Config.GoalNum];
            bidCount = new int[100]; // new int[Config.GoalNum];
            modeChange = false;
            absorbingFlag = false;
            goalFlag = false;
            lastlearningFlag = false;
            state = new State();
            paststate = new State();
            actionSet = new ActionSet();
            memory = new Memory(config);
            reward = 0;
            iSCount = 0;
        }
        public void AgentReset()
        {
            step = 0;
            mode = 0;
            iSCount = 0;
            absorbingFlag = false;
            goalFlag = false;
            lastlearningFlag = false;
        }
        /// <summary>
        /// エージェントの内部状態を変更する
        /// エージェントの行動の起点
        /// </summary>
        /// <param name="posx"></param>
        /// <param name="posy"></param>
        public void SetState(StateElement se, int confAgentNum, double reward, bool goalflag, bool judgeFlag)
        {
            WallQvalueSetting();
            step++;
            if (state != null && goalFlag == false) paststate = new State(new StateElement(state.StateChar));
            state = new State(se);
            if (goalFlag == false) goalFlag = goalflag;
            this.reward = reward;
            //GoalCheck();
            ChooseAction(judgeFlag);
            if (judgeFlag == false) logger.LogVisitRate(state);
            if (goalFlag == true && absorbingFlag == false) // ゴールできたとき
            {
                absorbingFlag = true;
                if (modeChange == false) // 学習モード
                {
                    logger.LogGoalPosSet(state);
                    memory.SetGoalState(state);
                    //Console.WriteLine("X: " + state.StateChar.X + " Y: " + state.StateChar.Y);
                    for (int i = 0; i < config.GoalNum; i++)
                    {
                        if (memory.GoalDetecting(state) == i)
                        {
                            memory.SetStepTableList(new MemoryTable(i, step, reward));
                            if (config.Lerning == "PMRL")
                            {
                                if (stepTable[i] > step) stepTable[i] = step;
                                if (rewardTable[i] < reward) rewardTable[i] = reward;
                            }
                        }
                    }
                }
            }
            else if (step == config.MaxStep) // ゴールできなかった時
            {
                if (modeChange == false) // 学習モード
                {
                    memory.SetGoalState(new State(null));
                }
            }
        }

        /// <summary>
        /// 行動を選ぶメソッド
        /// </summary>
        /// <returns></returns>
        public void ChooseAction(bool judgeFlag)
        {
            int zeroCount = 0;
            if (actionNum != actionSet.ActionSetSize - 1) pastactionNum = actionNum;
            if (goalFlag == true)
            {
                actionNum = actionSet.ActionSetSize - 1;
                maxactionNum = actionNum;
            }
            else
            {
                for (int i = 0; i < actionSet.ActionSetSize - 1; i++)
                {
                    if (actionSet[i].SendQvalue(state) == 0) zeroCount++;
                    if (actionSet[actionNum].SendQvalue(state) < actionSet[i].SendQvalue(state)) actionNum = i;
                    else if (actionSet[actionNum].SendQvalue(state) == actionSet[i].SendQvalue(state))
                    { if (RandomMan.getRand(2) > 0) actionNum = i; }
                }
                maxactionNum = actionNum;
                if (RandomMan.getRand(100) < randomNum || zeroCount == config.ActionCount - 1) actionNum = RandomMan.getRand(actionSet.ActionSetSize - 1);
                //if (judgeFlag == false) /*BoltzmannChooseAction();/**/
            }
        }
        public void BoltzmannChooseAction()
        {
            double[] actionProbabilities = new double[config.ActionCount - 1];
            double sumProb = 0;
            for (int i = 0; i < actionSet.ActionSetSize - 1; i++)
            {
                actionProbabilities[i] = Math.Exp(actionSet[i].SendQvalue(state) / Tfunction(step));
                sumProb += actionProbabilities[i];
            }
            Parallel.For(0, actionProbabilities.Length, i =>
           {
               actionProbabilities[i] /= sumProb;
           });
            double randnum = RandomMan.getRand(1.0);
            for (int i = 0; i < actionProbabilities.Length; i++)
            {
                if (randnum < actionProbabilities[i])
                {
                    actionNum = i;
                    return;
                }
                else
                { randnum -= actionProbabilities[i]; }
            }
        }
        public double Tfunction(int count)
        {
            return 1.0 / Math.Log(count + 1.1);
        }

        /// <summary>
        /// 環境に渡す行動を選ぶメソッド
        /// </summary>
        /// <returns></returns>
        public Action SendAction()
        {
            return actionSet[actionNum];
        }
        /// <summary>
        /// Q学習
        /// </summary>
        public void QLearning()
        {
            double Qvalue = 0;
            if (state != null && paststate != null && step > 1)
            {
                if ((goalFlag ^ lastlearningFlag) == false)
                //if (goalFlag)
                {
                    if (mode == -1)
                    {
                        Qvalue = (1 - config.Alpha) * actionSet[pastactionNum].SendQvalue(paststate) + config.Alpha * (reward + config.Gamma * actionSet[maxactionNum].SendQvalue(state));
                    }
                    else
                    {
                        int currentGoal = memory.GoalDetecting(state);
                        if (currentGoal == mode)
                        {
                            double Coef_ireward = 0;
                            for (int j = 0; j < config.GoalNum; j++)
                            {
                                if (mode != j && Math.Pow(config.Gamma, (stepTable[j] - stepTable[mode])) * rewardTable[j] > Coef_ireward) // 最大化
                                {
                                    Coef_ireward = Math.Pow(config.Gamma, (stepTable[j] - stepTable[mode])) * rewardTable[j];
                                }
                            }
                            Qvalue = (1 - config.Alpha) * actionSet[pastactionNum].SendQvalue(paststate) + config.Alpha * (config.RewardGap + Coef_ireward + config.Gamma * actionSet[maxactionNum].SendQvalue(state));
                        }
                        else
                        {
                            if (currentGoal != -1) Qvalue = (1 - config.Alpha) * actionSet[pastactionNum].SendQvalue(paststate) + config.Alpha * (rewardTable[currentGoal] + config.Gamma * actionSet[maxactionNum].SendQvalue(state));
                            else Qvalue = (1 - config.Alpha) * actionSet[pastactionNum].SendQvalue(paststate) + config.Alpha * (reward + config.Gamma * actionSet[maxactionNum].SendQvalue(state));
                        }
                    }
                    actionSet[pastactionNum].SetList(paststate, Qvalue);
                }
            }
        }
        public int SendStep(int gnum)
        { return stepTable[gnum]; }
        public void CheckISFlag(int agentNumother, int iSother, int[] stepTableOther)
        {
            if (iS == iSother)
            {
                //iSCount++;
                if (stepTable[iS] < stepTableOther[iS])
                {
                    //iSflag = 1;
                }
                else if (stepTable[iS] == stepTableOther[iS])
                {
                    if (agentnum < agentNumother) ;//iSflag = 1;
                    else if (agentnum > agentNumother) iSCount++; //iSflag = 0;
                    else iSCount += RandomMan.getRand(2);//iSflag = RandomMan.getRand(2);
                }
                else
                {
                    iSCount++;
                    //iSflag = 0;
                }
            }
        }
        public void Goal_determining()
        {
            memory.calcRewardStability(config.GoalNum, ref meanRewards, ref stdRewards, ref rewardCounts);
            memory.updateStanStepTables(ref IstepTable);
            Parallel.For(0, config.GoalNum, i =>
            {
                FstepTable[i] = false;
            });
            for (int j = 0; j < coopRange; j++)
            {
                double tmpIminstep = double.MaxValue; int tmpIminstepIndex = -1;
                for (int i = 0; i < config.GoalNum; i++)
                {
                    if (IstepTable[i] <= tmpIminstep && FstepTable[i] == false)
                    {
                        tmpIminstep = IstepTable[i];
                        tmpIminstepIndex = i;
                    }
                }
                if (tmpIminstepIndex != -1)
                {
                    FstepTable[tmpIminstepIndex] = true;
                }
            }
            int goalNum = 0; double maxBid = 0;// bidValue[0];
            for (int i = 0; i < config.GoalNum; i++)
            {
                if (maxBid < bidValue[i] && FstepTable[i])
                {
                    maxBid = bidValue[i];
                    goalNum = i;
                }
            }
            if (maxBid == 0 || RandomMan.getRand(100) < config.Randomly_Probability_of_Determination)
            {
                while (true)
                {
                    int randGoalnum = RandomMan.getRand(config.GoalNum);
                    if (FstepTable[randGoalnum])
                    {
                        bidCount[randGoalnum]++;
                        iS = randGoalnum;
                        if (config.Communication == "OFF")
                        {
                            if ((meanRewards[iS] > rewardTable[iS] * 0.5) || (RandomMan.getRand(100) < 30)/* && (stdRewards[iS] < rewardTable[iS] * 0.1)/**/) iSflag = 1;
                            else iSflag = 0;/**/
                        }
                        logger.LogMeanReward(meanRewards);
                        logger.LogStdReward(stdRewards);
                        return;// randGoalnum;
                    }
                }
            }
            else
            {
                bidCount[goalNum]++;
                iS = goalNum;
                if (config.Communication == "OFF")
                {
                    if ((meanRewards[iS] > rewardTable[iS] * 0.5) || (RandomMan.getRand(100) < 30)/* && (stdRewards[iS] < rewardTable[iS] * 0.1)/**/) iSflag = 1;
                    else iSflag = 0;
                }
                logger.LogMeanReward(meanRewards);
                logger.LogStdReward(stdRewards);
                return;// goalNum;
            }
        }
        public void updateBid()
        {
            logger.LogBidStack(bidValue);
            double vi = 0;
            if (iSflag >= 1) vi = (double)stepTable[iS];
            else vi = 0;
            if (config.Lerning == "PMRL")
            {
                bidValue[iS] = vi / (double)bidCount[iS] + (double)(bidCount[iS] - 1) / (double)bidCount[iS] * bidValue[iS];
            }
        }
        public void ModeSet(int mode)
        {
            this.lastlearningFlag = true;
            if (mode == -1) this.mode = mode;
            else this.mode = iS;
            logger.LogStepTableStack(step, stepTable);
        }
        public void WallQvalueSetting()
        {
            for (int i = 0; i < actionSet.ActionSetSize; i++)
            {
                if (i == 0) // 上の行動
                {
                    for (int j = 1; j <= config.CellWidth; j++)
                    {
                        actionSet[i].SetList(new State(new StateElement(j, 1)), 0);
                    }
                }
                if (i == 1) // 下の行動
                {
                    for (int j = 1; j <= config.CellWidth; j++)
                    {
                        actionSet[i].SetList(new State(new StateElement(j, config.CellHeight)), 0);
                    }
                }
                if (i == 2) // 右の行動
                {
                    for (int j = 1; j <= config.CellHeight; j++)
                    {
                        actionSet[i].SetList(new State(new StateElement(config.CellWidth, j)), 0);
                    }
                }
                if (i == 3) // 左の行動
                {
                    for (int j = 1; j <= config.CellHeight; j++)
                    {
                        actionSet[i].SetList(new State(new StateElement(1, j)), 0);
                    }
                }
                if (i == 4) // 立ち止まる
                {
                    for (int j = 1; j <= config.CellWidth; j++)
                        for (int k = 1; k <= config.CellHeight; k++)
                        {
                            actionSet[i].SetList(new State(new StateElement(j, k)), 0);
                        }
                }
            }
        }
        public void Output(String name)
        {
            actionSet.Output(name);
            logger.Output(name);
            string[] data = new string[config.AgentNum + 1];
            data[0] = "," + 1 + "," + 2 + ",agentnum is " + agentnum;
            for (int j = 0; j < config.GoalNum; j++)
            {
                data[1] += "," + stepTable[j];
            }
            config.Save<string>(ref data, (name + "_stepTable"));
            data = new string[1];
            for (int i = 0; i < config.GoalNum; i++)
            {
                data[0] += bidValue[i] + ",";
            }
            config.Save<string>(ref data, (name + "_BID"));
        }
        public Boolean GoalFlag
        {
            get { return goalFlag; }
            set { goalFlag = value; }
        }
        public int AgentNum
        {
            get { return agentnum; }
            set { agentnum = value; }
        }
        public int ActCount
        {
            get { return actionSet.ActionSetSize; }
        }
        public double RandomNum
        {
            set { randomNum = value; }
        }
        public Boolean ModeChange
        {
            get { return modeChange; }
            set { modeChange = value; }
        }
        public int Mode
        {
            get { return mode; }
        }
        public int CoopRange
        {
            get { return coopRange; }
            set { coopRange = value; }
        }
        public int GoalCount
        {
            get; set;
        }
        public int[] StepTable
        {
            get { return stepTable; }
        }
        public int IS
        {
            get { return iS; }
        }
        public int ISFlag
        {
            get { return iSflag; }
            set { iSflag = value; }
        }
        public int ISCount
        {
            get { return iSCount; }
            set { iSCount = value; }
        }
        public Config Config
        {
            get { return config; }
            set { config = value; }
        }
    }
}
