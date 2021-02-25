using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PMRL
{
    class Routine
    {
        private List<Agent> agentSet = new List<Agent>();

        static void Main(/*string[] args*/)
        {
            Config config = new Config();
            for (int s = 0; s < config.SeedCount; s++)
            {
                string[] args = new string[1];
                args[0] = "Sample";
                int i = 0, j = 0;
                int[,] stepTable = new int[100, 100];
                i = config.Seeds(s);
                string path = "." + Path.DirectorySeparatorChar + "iteration_" + config.Learning_Iteration + "alpha_" + config.Alpha + "gamma_" + config.Gamma + "epsilon_" + config.Epsilon + "reward_" + config.Reward + "rewardgap_" + config.RewardGap + "randomdet_" + config.Randomly_Probability_of_Determination;
                System.IO.Directory.CreateDirectory(path);
                System.IO.Directory.CreateDirectory(path + Path.DirectorySeparatorChar + args[0] + "_" + i);

                Environment maze = new Environment();
                config = maze.Config;
                List<AgentControl> agents = new List<AgentControl>();
                int[] currentGoalnum = new int[100];
                bool[] currentConflag = new bool[100];
                while (true)
                {
                    RandomMan.setSeed(i);
                    Boolean[] resultduringtime = new Boolean[config.Learning_Iteration];
                    int[] MinCount = new int[config.ResultLength];
                    double[] EvalCount = new double[config.ResultLength];
                    int rlength = 0;
                    Console.Clear();
                    for (j = 0; j < config.AgentNum; j++)
                    {
                        agents.Add(new AgentControl(maze.Config));
                        maze.AddAgent(ref agents);
                    }
                    for (j = 0; j < config.Learning_Iteration; j++)
                    {
                        Console.SetCursorPosition(0, 0);
                        Console.WriteLine(j + "学習");
                        maze.EnvironmentalChange(j, ref agents);
                        for (int k = 0; k < config.AgentNum; k++)
                        {
                            maze.Restart(agents[k].AgentNum);
                            agents[k].AgentReset();
                            agents[k].RandomNum = 100 * config.Epsilon; // (double)200 / 3.0; // ランダム行動を行う確率
                        }
                        for (int k = 0; k < 100; k++)
                        {
                            for (int l = 1; l <= 3; l++)
                            {
                                for (int m = 0; m < config.AgentNum; m++)
                                {
                                    agents[m].act(l, ref maze, false);
                                }
                            }
                            int count = 0;
                            for (int l = 0; l < config.AgentNum; l++) if (agents[l].GoalFlag) count++;
                            if (count - config.AgentNum >= 0) break;
                        }
                        for (int k = 0; k < config.AgentNum; k++)
                        {
                            Parallel.For(0, config.GoalNum, l =>
                           {
                               stepTable[agents[k].AgentNum, l] = agents[k].SendStep(l);
                           });
                        }
                        for (int k = 0; k < config.AgentNum; k++)
                        {
                            if (config.Lerning == "PMRL")
                            {
                                agents[k].Goal_determining();
                            }
                        }
                        if (config.Communication == "ON")
                        {
                            for (int k = 0; k < config.AgentNum; k++)
                            {
                                agents[k].ISCount = 0; agents[k].ISFlag = 1;
                                for (int l = 0; l < config.AgentNum; l++)
                                {
                                    if(k != l) agents[k].CheckISFlag(agents[l].AgentNum, agents[l].IS, agents[l].StepTable);
                                }
                                Console.WriteLine("agents[" + k + "]_" + "ISCount: " + agents[k].ISCount);
                                if (agents[k].ISCount == 0) agents[k].ISFlag = 1;
                                else agents[k].ISFlag = 0;
                            }
                        }
                        // 最初はQ学習を行うように改良
                        for (int k = 0; k < config.AgentNum; k++)
                        {
                            agents[k].PMRL(); //ここを変えるとPMRL
                        }
                        for(int k = 0; k < config.AgentNum; k++)
                        {
                            Console.WriteLine("Agent" + k + "_coopRange: " + agents[k].CoopRange + " iS: " + agents[k].IS + " iSflag: " + agents[k].ISFlag);
                        }
                        if (j > config.Learning_Iteration - 10)
                        {
                            System.IO.Directory.CreateDirectory(path + Path.DirectorySeparatorChar + args[0] + "_" + i + "_" + (j - (config.Learning_Iteration - 10000)));
                            for (int k = 0; k < config.AgentNum; k++)
                            {
                                agents[k].Output(path + Path.DirectorySeparatorChar + args[0] + "_" + i + "_" + (j - (config.Learning_Iteration - 10000)) + Path.DirectorySeparatorChar + "_" + k + "_Qvalue");
                            }
                        }
                        /* 検証 /**/
                        for (int k = 0; k < config.AgentNum; k++)
                        {
                            agents[k].ModeChange = true;
                            maze.Restart(agents[k].AgentNum);
                            agents[k].AgentReset();
                            agents[k].RandomNum = 0;
                        }
                        int minStep = 100;
                        for (int k = 0; k < 100; k++)
                        {
                            for (int l = 1; l <= 3; l++)
                            {
                                for (int m = 0; m < config.AgentNum; m++)
                                {
                                    if (l != 2) agents[m].act(l, ref maze, true);
                                }
                            }
                            int count = 0;
                            for (int l = 0; l < config.AgentNum; l++) if (agents[l].GoalFlag) count++;
                            if (count - config.AgentNum >= 0)
                            {
                                minStep = k + 1;
                                break;
                            }
                        }
                        bool[] flag = new bool[config.AgentNum];
                        Parallel.For(0, config.AgentNum, l => { flag[l] = false; });
                        Parallel.For(0, config.AgentNum, l => { if (flag[l]) { agents[l].GoalCount++; } });
                        Parallel.For(0, config.AgentNum, k => { currentGoalnum[k] = -1; });
                        for (int k = 0; k < config.AgentNum; k++)
                        {
                            for (int m = 0; m < config.GoalNum; m++)
                            {
                                if (maze.Goal_State(m).StateChar.Comparison(maze.SendState(agents[k].AgentNum).StateChar))
                                {
                                    currentGoalnum[k] = m;
                                }
                            }
                        }
                        /* 最短ステップによるジャッジ /**/
                        bool trueFlag = true;
                        for (int l = 0; l < config.AgentNum - 1; l++)
                        {
                            for (int m = l + 1; m < config.AgentNum; m++)
                            {
                                if (maze.SendState(agents[l].AgentNum).StateChar.Comparison(maze.SendState(agents[m].AgentNum).StateChar))
                                {
                                    trueFlag = false;
                                }
                            }
                        }
                        double sumResult = 0;
                        for (int k = 0; k < config.AgentNum; k++)
                        {
                            if (currentGoalnum[k] != -1)
                            {
                                bool checkFlag = false;
                                for (int l = 0; l < k; l++)
                                {
                                    if (currentGoalnum[k] == currentGoalnum[l]) checkFlag = true;
                                }
                                if (checkFlag == false) sumResult += maze.Reward_Value(currentGoalnum[k]);
                            }
                        }
                        if (trueFlag)
                        {
                            MinCount[rlength] = minStep;
                            EvalCount[rlength++] = sumResult / minStep;
                        }
                        else
                        {
                            MinCount[rlength] = 100;
                            EvalCount[rlength++] = sumResult / 100;
                        }
                        Parallel.For(0, config.AgentNum, l =>
                       {
                           agents[l].ModeChange = false;
                       });
                        if (rlength == config.ResultLength)
                        {
                            config.Save<int>(ref MinCount, (path + Path.DirectorySeparatorChar + "MinimumStep_" + i), true);
                            config.Save<double>(ref EvalCount, (path + Path.DirectorySeparatorChar + "Evalue_" + i), true);
                            rlength = 0;
                        }
                    }
                    if (rlength != 0)
                    {
                        int[] tmpCount = new int[rlength]; double[] dtmpCount = new double[rlength];
                        for (int l = 0; l < rlength; l++) { tmpCount[l] = MinCount[l]; dtmpCount[l] = EvalCount[l]; }
                        config.Save<int>(ref tmpCount, (path + Path.DirectorySeparatorChar + "MinimumStep_" + i), true);
                        config.Save<double>(ref dtmpCount, (path + Path.DirectorySeparatorChar + "Evalue_" + i), true);
                        rlength = 0;
                    }
                    Parallel.For(0, config.AgentNum, k =>
                   {
                       agents[k].Output(path + Path.DirectorySeparatorChar + args[0] + "_" + i + Path.DirectorySeparatorChar + "_" + k + "_Qvalue");
                   });
                    break;
                }
            }
        }
    }
}
