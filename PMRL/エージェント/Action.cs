using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PMRL
{
    /// <summary>
    /// エージェントの行動を管理するクラス
    /// </summary>
    public class Action
    {
        private ActionElement action;
        private SVList svList;

        public Action(int vx, int vy)
        {
            action = new ActionElement(null, vx, vy);
            svList = new SVList();
        }

        public void SetList(State state, double qvalue)
        {
            svList.SetSVList(state, qvalue);
        }

        public double SendQvalue(State state)
        {
            return svList.SendQvalue(state);
        }

        public int SendUpdateCount(State state)
        {
            return svList.SendUpdateCount(state);
        }

        public ActionElement ActionChar
        {
            get { return action; }
            set { action = value; }
        }
        /// <summary>
        /// 具体的な行動を作る
        /// </summary>
        /// <param name="vx"></param>
        /// <param name="vy"></param>
        public void SetAction(int vx, int vy)
        {
            action.VX = vx;
            action.VY = vy;
            action.ActFlag = true;
        }
        public void Save(String name)
        { svList.Save(name); }
    }

    public class ActionSet
    {
        private List<Action> actionSet;
        public ActionSet()
        {
            actionSet = new List<Action>();
            Action up = new Action(0, -1);
            Action down = new Action(0, 1);
            Action right = new Action(1, 0);
            Action left = new Action(-1, 0);
            Action stay = new Action(0, 0);
            actionSet.Add(up); actionSet.Add(down); actionSet.Add(right); actionSet.Add(left); actionSet.Add(stay);
        }
        public int ActionSetSize
        {
            get { return actionSet.Count; }
        }
        public Action this[int index]
        {
            get { return actionSet[index]; }
        }
        public void Output(String name)
        {
            Parallel.For(0, actionSet.Count, i =>
            {
                if (i == 0) actionSet[i].Save(name + "_up");
                else if (i == 1) actionSet[i].Save(name + "_down");
                else if (i == 2) actionSet[i].Save(name + "_right");
                else if (i == 3) actionSet[i].Save(name + "_left");
                else if (i == 4) actionSet[i].Save(name + "_stay");
            });
        }
    }

    public class ActionElement
    {
        private int vx;
        private int vy;
        private Boolean actflag;
        
        public ActionElement(ActionElement ae = null, int vx = 0, int vy = 0)
        {
            if (ae != null)
            {
                vx = ae.VX;
                vy = ae.VY;
                actflag = ae.ActFlag;
            }
            else
            {
                this.vx = vx;
                this.vy = vy;
            }
        }
        public int VX
        {
            get { return vx; }
            set { vx = value; }
        }
        public int VY
        {
            get { return vy; }
            set { vy = value; }
        }
        public Boolean ActFlag
        {
            get { return actflag; }
            set { actflag = value; }
        }
    }

    public class SVList
    {
        private List<State> stateList = new List<State>();
        private List<double> qvalueList = new List<double>();
        private List<int> updateCountList = new List<int>();
        private Config config = new Config();

        public void SetSVList(State state, double qvalue)
        {
            for (int i = 0; i < stateList.Count; i++)
            {
                if (state != null && stateList[i] != null && stateList[i].StateChar.Comparison(state.StateChar))
                {
                    qvalueList[i] = qvalue;
                    updateCountList[i]++;
                    return;
                }
            }
            stateList.Add(state);
            qvalueList.Add(qvalue);
            updateCountList.Add(1);
        }
        public double SendQvalue(State state)
        {
            if (stateList.Count > 0)
            {
                for (int i = 0; i < stateList.Count; i++)
                {
                    if (state != null && stateList[i] != null && stateList[i].StateChar.Comparison(state.StateChar))
                    {
                        return qvalueList[i];
                    }
                }
            }
            return config.NormalQValue;
        }
        public int SendUpdateCount(State state)
        {
            if (stateList.Count > 0)
            {
                for (int i = 0; i < stateList.Count; i++)
                {
                    if (state != null && stateList[i] != null && stateList[i].StateChar.Comparison(state.StateChar))
                    {
                        return updateCountList[i];
                    }
                }
            }
            return 0;
        }
        public void Discount(int index)
        {
            for(int i = 0; i < qvalueList.Count; i++)
            {
                if (i == index) continue;
                qvalueList[i] *= 0.99;
            }
        }
        public void Save(String name)
        {
            string[] data = new string[Math.Min(stateList.Count, qvalueList.Count)];
            Parallel.For(0, data.Length, i =>
           {
               data[i] = stateList[i].StateChar.X + "," + stateList[i].StateChar.Y + "," + qvalueList[i] + "," + updateCountList[i];
           });
            config.Save<string>(ref data, (name));
            Parallel.For(0, data.Length, i =>
           {
               data[i] = stateList[i].StateChar.X + "," + stateList[i].StateChar.Y + "," + Math.Round(qvalueList[i], 3);
           });
            config.Save<string>(ref data, (name + "_data"));
        }
    }
}