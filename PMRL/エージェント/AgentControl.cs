using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMRL
{
    class AgentControl : Agent
    {
        public AgentControl(Config config) : base(config)
        {  }

        public void act(int mode, ref Environment env, bool judgeFlag)
        {
            switch (mode)
            {
                case 1:
                    Tuple<double, bool> tmp = env.SendReward(this.AgentNum);
                    this.SetState(env.SendState(this.AgentNum).StateChar, env.SendState(this.AgentNum).ConfAgentNum, tmp.Item1, tmp.Item2, judgeFlag);
                    break;
                case 2:
                    QLearning();
                    break;
                case 3:
                    env.SetAction(this.AgentNum, this.SendAction());
                    break;
            }
        }

        public void normalLearning()
        {
            //this.Mode = -1;
            this.ModeSet(-1);
            this.QLearning();
        }
        public void PMRL()
        {
            //this.updateBidForFar(iS, iSflag);
            this.updateBid();
            //this.Mode = iS;
            this.ModeSet(1);
            this.QLearning();
        }
    }
}
