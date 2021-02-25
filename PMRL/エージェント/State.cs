using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMRL
{
    /// <summary>
    /// エージェントの内部状態、環境からの入力を管理するクラス
    /// </summary>
    public class State
    {
        private StateElement state; // エージェントの位置をステートとする 
        private int confAgentNum;

        public State(StateElement stateElement = null, int confAgentNum = 0)
        {
            if (stateElement != null) state = new StateElement(stateElement);
            else state = new StateElement(null);
            this.confAgentNum = confAgentNum;
        }
        public void StateTransition(ActionElement act, Config config, States wallStates)
        {
            state.X += act.VX;
            state.Y += act.VY;
            for (int i = 0; i < wallStates.Count; i++)
            {
                if (state.Comparison(wallStates[i].StateChar))
                {
                    state.X -= act.VX;
                    state.Y -= act.VY;
                    return;
                }
            }
            if (state.X > config.CellWidth) state.X = config.CellWidth;
            if (state.X < 1) state.X = 1;
            if (state.Y > config.CellHeight) state.Y = config.CellHeight;
            if (state.Y < 1) state.Y = 1;
        }
        public void CountConfAgentNum(List<State> agentghosts)
        {
            this.confAgentNum = 0;
            for(int i = 0; i < agentghosts.Count; i++)
            {
                if(agentghosts[i].StateChar.Comparison(this.state))
                {
                    this.confAgentNum++; 
                }
            }
        }
        /// <summary>
        /// ステートを返す
        /// </summary>
        public StateElement StateChar
        {
            get { return state; }
        }
        public int ConfAgentNum
        {
            get { return confAgentNum; }
            set { confAgentNum = value; }
        }
    }
    public class StateElement
    {
        private int x;
        private int y;

        public StateElement(StateElement se = null)
        {
            if (se != null)
            {
                x = se.X;
                y = se.Y;
            }
            else
            {
                this.x = 0;
                this.y = 0;
            }
        }
        public StateElement(int x = 0, int y = 0)
        {
            this.x = x;
            this.y = y;
        }
        public bool Comparison(StateElement se, int x = 0, int y = 0)
        {
            if (se != null) return ((this.x == se.X) && (this.y == se.Y));
            else return ((this.x == x) && (this.y == y));
        }
        public int X
        {
            get { return x; }
            set { x = value; }
        }
        public int Y
        {
            get { return y; }
            set { y = value; }
        }
        public void Sort(ref List<State> goalSet)
        {
            goalSet = goalSet.OrderBy(n => n.StateChar.X).ThenBy(n => n.StateChar.Y).ToList();
        }
        public void Sort(ref List<Tuple<State, double, int>> goalSet)
        {
            goalSet = goalSet.OrderBy(n => n.Item1.StateChar.X).ThenBy(n => n.Item1.StateChar.Y).ToList();
        }
    }
    public class States
    {
        private List<State> ss = new List<State>();
        public void Add(State s)
        {
            ss.Add(s);
        }
        public State this[int index]
        {
            get { return ss[index]; }
            set { ss[index] = new State(value.StateChar); }
        }
        public int Count
        { get { return ss.Count; } }
    }
}
