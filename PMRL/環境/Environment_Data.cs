using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PMRL
{
    class Rewards
    {
        private List<double> rewards = new List<double>();

        public void Add(double reward) { rewards.Add(reward); }
        public double this[int index]
        {
            get { return rewards[index]; }
            set { rewards[index] = value; }
        }
    }
    class Environment_Data
    {
        private List<int> startNums = new List<int>();
        private List<int> goalNums = new List<int>();
        private List<int> periods = new List<int>();
        private List<int> widths = new List<int>();
        private List<int> heights = new List<int>();
        private List<List<int>> agentCoopRanges = new List<List<int>>();
        private List<Rewards> rewards = new List<Rewards>();
        private List<States> starts = new List<States>();
        private List<States> goals = new List<States>();
        private List<States> walls = new List<States>();

        public Environment_Data(Config config)
        {
            string envPath = "." + Path.DirectorySeparatorChar + "envData.csv";
            if (File.Exists(envPath))
            {
                int count = 0;
                using (StreamReader sr = new StreamReader(envPath))
                {
                    string s = "";
                    sr.ReadLine();
                    while ((s = sr.ReadLine()) != null)
                    {
                        string[] str = s.Split(',');
                        periods.Add(int.Parse(str[0]));
                        widths.Add(int.Parse(str[1]));
                        heights.Add(int.Parse(str[2]));
                        int startNum = int.Parse(str[3]), goalNum = int.Parse(str[4]);
                        States cstarts = new States();
                        States cgoals = new States();
                        States cwalls = new States();
                        Rewards crewards = new Rewards();
                        List<int> agentCoopRange = new List<int>();
                        for (int i = 5; i < 5 + startNum; i++)
                        {
                            cstarts.Add(new State(new StateElement(int.Parse(str[i].Split('x')[0]), int.Parse(str[i].Split('x')[1]))));
                            agentCoopRange.Add(int.Parse(str[i].Split('x')[2]));
                        }
                        for (int i = 5 + startNum; i < 5 + startNum + goalNum; i++)
                        { cgoals.Add(new State(new StateElement(int.Parse(str[i].Split('x')[0]), int.Parse(str[i].Split('x')[1])))); crewards.Add(double.Parse(str[i].Split('x')[2])); }
                        for (int i = 5 + startNum + goalNum; i < str.Length; i++)
                        { if (str[i] != string.Empty/*int.TryParse(str[i], out walldata)*/) cwalls.Add(new State(new StateElement(int.Parse(str[i].Split('x')[0]), int.Parse(str[i].Split('x')[1])))); }
                        starts.Add(cstarts); goals.Add(cgoals); walls.Add(cwalls); startNums.Add(startNum); goalNums.Add(goalNum); rewards.Add(crewards); agentCoopRanges.Add(agentCoopRange);
                    }
                }
            }
            else
            {
                Console.WriteLine("envData.csv is not found!");
            }
        }
        public int IndexExtraction(int num, int baseNum)
        {
            int count = 0;
            while (num % baseNum == 0) { num /= baseNum; count++; }
            return count;
        }
        public int DataSize()
        { return periods.Count; }
        public int Periods(int t)
        { return periods[t]; }
        public int Widths(int t)
        { return widths[t]; }
        public int Heights(int t)
        { return heights[t]; }
        public int currentStartNum(int t)
        { return startNums[t]; }
        public int currentGoalNum(int t)
        { return goalNums[t]; }
        public Rewards rewardValue(int t)
        { return rewards[t]; }
        public States Start_States(int t)
        { return starts[t]; }
        public States Goal_States(int t)
        { return goals[t]; }
        public States Wall_States(int t)
        { return walls[t]; }
        public List<int> AgentCoopRanges(int t)
        { return agentCoopRanges[t]; }
        public Environment_Data Env_Data
        {
            get { return this; }
        }
    }
}
