using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMRL
{
    /// <summary>
    /// エージェントの向こう側、環境のクラス
    /// </summary>
    class Environment
    {
        private List<int> table;
        private int agentnum;
        private List<State> agentghosts;
        private Environment_Data env_data;
        private Config config;
        private States currentStart;
        private RewardMaster rewardMaster;
        private int currentPeriod;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Environment()//int gsx, int gsy, int gmx, int gmy, int glx, int gly)
        {
            currentPeriod = 0;
            config = new Config();
            rewardMaster = new RewardMaster();
            env_data = new Environment_Data(config);
            table = new List<int>();
            agentnum = 0;
            agentghosts = new List<State>();
            States goalStates = env_data.Goal_States(0);
            for (int i = 0; i < env_data.currentGoalNum(0); i++)
                rewardMaster.SetRewardPos(new State(goalStates[i].StateChar), env_data.rewardValue(0)[i]);
            config.CellWidth = env_data.Widths(currentPeriod);
            config.CellHeight = env_data.Heights(currentPeriod);
            config.AgentNum = env_data.currentStartNum(currentPeriod);
            config.GoalNum = env_data.currentGoalNum(currentPeriod);
        }
        /// <summary>
        /// エージェントの追加（初期値を渡す）
        /// </summary>
        /// <param name="agentposx"></param>
        /// <param name="agentposy"></param>
        public void AddAgent(ref List<AgentControl> agents)
        {
            State agentghost = new State(env_data.Start_States(currentPeriod)[agentnum].StateChar);
            agentghosts.Add(agentghost);
            agents[agentnum].CoopRange = env_data.AgentCoopRanges(currentPeriod)[agentnum];
            agents[agentnum].AgentNum = agentnum++;
        }
        /// <summary>
        /// 行動からエージェントの位置を変更
        /// </summary>
        /// <param name="agentnum"></param>
        /// <param name="str"></param>
        public void SetAction(int agentnum, Action act)
        {
            if (agentnum > this.agentnum) Console.WriteLine("存在しないエージェント番号です。");
            agentghosts[agentnum].StateTransition(act.ActionChar, config, env_data.Wall_States(currentPeriod));
            agentghosts[agentnum].CountConfAgentNum(agentghosts);
        }
        public void EnvironmentalChange(int currentIteration, ref List<AgentControl> agents)
        {
            if (env_data.DataSize() - 1 == currentPeriod || env_data.DataSize() - 1 > currentPeriod && currentIteration < env_data.Periods(currentPeriod + 1)) return;
            currentPeriod++; rewardMaster.ResetRewardPos();
            currentStart = env_data.Start_States(currentPeriod);
            States goals = env_data.Goal_States(currentPeriod);
            for (int i = 0; i < env_data.currentGoalNum(currentPeriod); i++)
                rewardMaster.SetRewardPos(new State(goals[i].StateChar), env_data.rewardValue(currentPeriod)[i]);
            config.CellWidth = env_data.Widths(currentPeriod);
            config.CellHeight = env_data.Heights(currentPeriod);
            config.AgentNum = env_data.currentStartNum(currentPeriod);
            config.GoalNum = env_data.currentGoalNum(currentPeriod);
            int diff = config.AgentNum - agents.Count;
            for (int k = 0; k < diff; k++)
            {
                agents.Add(new AgentControl(config));
                this.AddAgent(ref agents);
            }
            for (int i = 0; i < agentnum; i++)
            {
                agents[i].CoopRange = env_data.AgentCoopRanges(currentPeriod)[i];
                agents[i].Config = config;
            }
        }
        /// <summary>
        /// ステート（現在位置）を与えるメソッド
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="pos"></param>
        public State SendState(int agentnum)
        {
            return new State(agentghosts[agentnum].StateChar, agentghosts[agentnum].ConfAgentNum);
        }
        public Tuple<double, bool> SendReward(int agentnum)
        {
            return rewardMaster.CheckState(agentghosts[agentnum]);
        }
        /// <summary>
        /// エージェントの位置を直すためのメソッド
        /// </summary>
        /// <param name="agentnum"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Restart(int agentnum)
        {
            agentghosts[agentnum] = new State(env_data.Start_States(currentPeriod)[agentnum].StateChar);
            rewardMaster.Restart();
        }

        public State Agent_State(int statenum)
        { return env_data.Start_States(currentPeriod)[statenum]; }
        public State Goal_State(int goalnum)
        { return env_data.Goal_States(currentPeriod)[goalnum]; }
        public double Reward_Value(int goalnum)
        { return env_data.rewardValue(currentPeriod)[goalnum]; }
        public Config Config
        {
            get { return config; }
        }
    }
}
