using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PMRL
{
    /// <summary>
    /// プログラム全体のパラメータを管理するクラス
    /// </summary>
    public class Config
    {
        private int nullnumber = -1;
        private double normal_Q_value = 0;
        private double reward = 10;
        private double normalreward = 0;
        //private int cellsum = 100;
        private int actioncount = 5;
        private int cellwidth = 8;
        private int cellheight = 3;
        private int maxstate = 100;
        private double alpha = 0.1;
        private double gamma = 0.9;
        private double epsilon = 0.7;
        private double rewardGap = 10;
        private int agentnum = 3;
        private int goalnum = 3;
        private int randombidprobability = 15;
        private int learning_iteration = 500000;
        private int cooperate_cycle = 5000;
        private double cooperate_probability = 0.8;
        private int cycle_bid = 500;
        private int resultLength = 10000;
        private List<int> seeds = new List<int>();
        private List<int> agentsGoalRange = new List<int>();
        private int seedCount = -1;
        private string learningMode;
        private string communicationMode;

        public Config()
        {
            using (StreamReader sr = new StreamReader("config.csv"))
            {
                string line = sr.ReadLine();
                char[] separator = ",".ToCharArray();
                string[] splited = line.Split(separator);
                Dictionary<string, int> caption = new Dictionary<string, int>();
                for(int i = 0; i < splited.Length; ++i)
                {
                    caption[splited[i].Trim()] = i;
                }
                const string initQcaption = "initialQ";
                const string externalrewardcaption = "externalreward";
                const string normalrewardcaption = "normalreward";
                const string actioncountcaption = "actioncount";
                const string cellwidthcaption = "cellwidth";
                const string cellheightcaption = "cellheight";
                const string maxstatecaption = "maxstate";
                const string alphacaption = "alpha";
                const string gammacaption = "gamma";
                const string epsiloncaption = "epsilon";
                const string rewardgapcaption = "rewardgap";
                const string agentnumcaption = "agentnum";
                const string goalnumcaption = "goalnum";
                const string randombidprobabilitycaption = "randombidprobability";
                const string learningiterationcaption = "learningiteration";
                const string cooperatecyclecaption = "cooperatecycle";
                const string cooperateprobabilitycaption = "cooperateprobability";
                const string cyclebidcaption = "cyclebid";
                const string resultlength = "resultlength";
                const string learning = "learning";
                const string communication = "communication";
                double tryResult; int tryResultint;
                while ((line = sr.ReadLine()) != null)
                {
                    splited = line.Split(separator);
                    if (splited[0] == "") break;
                    if(splited[0].Trim() == "seed")
                    {
                        seedCount = splited.Length - 1;
                        for(int i = 1; i < splited.Length; i++)
                        {
                            seeds.Add(int.Parse(splited[i]));
                        }
                        continue;
                    }
                    if (splited[0].Trim() == "agentsGoalRange")
                    {
                        seedCount = splited.Length - 1;
                        for (int i = 1; i < splited.Length; i++)
                        {
                            if (splited[i] == "") continue;
                            agentsGoalRange.Add(int.Parse(splited[i]));
                        }
                        continue;
                    }
                    if (double.TryParse(splited[caption[initQcaption]], out tryResult)) normal_Q_value = tryResult;
                    if (double.TryParse(splited[caption[externalrewardcaption]], out tryResult)) reward = tryResult;
                    if (double.TryParse(splited[caption[normalrewardcaption]], out tryResult)) normalreward = tryResult;
                    if (int.TryParse(splited[caption[actioncountcaption]], out tryResultint)) actioncount = tryResultint;
                    if (int.TryParse(splited[caption[cellwidthcaption]], out tryResultint)) cellwidth = tryResultint;
                    if (int.TryParse(splited[caption[cellheightcaption]], out tryResultint)) cellheight = tryResultint;
                    if (int.TryParse(splited[caption[maxstatecaption]], out tryResultint)) maxstate = tryResultint;
                    if (double.TryParse(splited[caption[alphacaption]], out tryResult)) alpha = tryResult;
                    if (double.TryParse(splited[caption[gammacaption]], out tryResult)) gamma = tryResult;
                    if (double.TryParse(splited[caption[epsiloncaption]], out tryResult)) epsilon = tryResult;
                    if (double.TryParse(splited[caption[rewardgapcaption]], out tryResult)) rewardGap = tryResult;
                    if (int.TryParse(splited[caption[agentnumcaption]], out tryResultint)) agentnum = tryResultint;
                    if (int.TryParse(splited[caption[goalnumcaption]], out tryResultint)) goalnum = tryResultint;
                    if (int.TryParse(splited[caption[randombidprobabilitycaption]], out tryResultint)) randombidprobability = tryResultint;
                    if (int.TryParse(splited[caption[learningiterationcaption]], out tryResultint)) learning_iteration = tryResultint;
                    if (int.TryParse(splited[caption[cooperatecyclecaption]], out tryResultint)) cooperate_cycle = tryResultint;
                    if (double.TryParse(splited[caption[cooperateprobabilitycaption]], out tryResult)) cooperate_probability = tryResult;
                    if (int.TryParse(splited[caption[cyclebidcaption]], out tryResultint)) cycle_bid = tryResultint;
                    if (int.TryParse(splited[caption[resultlength]], out tryResultint)) resultLength = tryResultint;
                    if (splited[caption[learning]].ToString() != "") learningMode = splited[caption[learning]].ToString();
                    if (splited[caption[communication]].ToString() != "") communicationMode = splited[caption[communication]].ToString();
                }
            }
        }

        public int NULL
        {
            get { return nullnumber; }
        }
        public double NormalQValue
        {
            get { return normal_Q_value; }
        }
        public double Reward
        {
            get { return reward; }
        }
        public double NormalReward
        {
            get { return normalreward; }
        }
        public int ActionCount
        {
            get { return actioncount; }
        }
        public int CellWidth
        {
            get { return cellwidth; }
            set { cellwidth = value; }
        }
        public int CellHeight
        {
            get { return cellheight; }
            set { cellheight = value; }
        }
        public int MaxStep
        {
            get { return maxstate; }
        }
        public double Alpha
        {
            get { return alpha; }
        }
        public double Gamma
        {
            get { return gamma; }
        }
        public double Epsilon
        {
            get { return epsilon; }
        }
        public double RewardGap
        {
            get { return rewardGap; }
        }
        public int AgentNum
        {
            get { return agentnum; }
            set { agentnum = value; }
        }
        public int GoalNum
        {
            get { return goalnum; }
            set { goalnum = value; }
        }
        public int Randomly_Probability_of_Determination
        {
            get { return randombidprobability; }
        }
        public int Learning_Iteration
        {
            get { return learning_iteration; }
        }
        public int Cooperate_Cycle
        {
            get { return cooperate_cycle; }
        }
        public double Cooperate_Probability
        {
            get { return cooperate_probability; }
        }
        public int Cycle_Bid
        {
            get { return cycle_bid; }
        }
        public int ResultLength
        {
            get { return resultLength; }
        }
        public int SeedCount
        {
            get { return seedCount; }
        }
        public int Seeds(int i)
        {
            return seeds[i];
        }
        public string Lerning
        {
            get { return this.learningMode; }
        }
        public string Communication
        {
            get { return this.communicationMode; }
        }

        public void Save<Type>(ref Type[] a, string name = "Sample", bool postscriptFlag = false)
        {
            StreamWriter sw = new StreamWriter(name + ".csv", postscriptFlag, Encoding.GetEncoding("UTF-8"));
            for (int i = 0; i < a.Length; i++)
            {
                sw.WriteLine(a[i]);
            }
            sw.Close();
        }
    }
}
