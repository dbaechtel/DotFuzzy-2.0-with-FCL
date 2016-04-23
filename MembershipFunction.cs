#region GNU Lesser General Public License
/*
This file is part of DotFuzzy.

DotFuzzy is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

DotFuzzy is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with DotFuzzy.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace DotFuzzy
{
    /// <summary>
    /// Represents a membership function.
    /// </summary>
    public class MembershipFunction
    {
        #region Private Properties

        public int index = Int32.MinValue;
        public int pntIndex = Int32.MinValue;
        private string name = String.Empty;
        private LinguisticVariable lvar = null;
        private string varName = String.Empty;
        private string singleton_varName = String.Empty;
        private double singleton_value = Double.NaN;
        public PointCollection pointCollection = new PointCollection();
        public PointCollection activated_pointCollection = new PointCollection();
        private double value = Double.NaN;
        private double max = Double.NaN;
        private double min = Double.NaN;
        private double degreeAtMax = Double.NaN;
        private double degreeAtMin = Double.NaN;
        private double act_value = Double.NaN;
        //private double area = Double.NaN;
        public double addArea = Double.NaN;
        //private double COA = Double.NaN;
        private DotFuzzy.FuzzyEngine engine;
        public int loc1 = -1;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MembershipFunction()
        {
        }

        /// <param name="name">The name that identificates the membership function.</param>
        public MembershipFunction(string name, ref DotFuzzy.FuzzyEngine engine, ref LinguisticVariable lvar)
        {
            this.Name = name;
            this.lvar = lvar;
            this.engine = engine;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The name that identificates the membership function.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// The singleton_varNmae of the membership function.
        /// </summary>
        public string Singleton_varName
        {
            get { return singleton_varName; }
            set { singleton_varName = value; }
        }

        /// <summary>
        /// The Singleton_value of the membership function.
        /// </summary>
        public double Singleton_value
        {
            get { return singleton_value; }
            set { singleton_value = value; }
        }

        /// <summary>
        /// The VARName for the membership function.
        /// </summary>
        public string VARName
        {
            get { return varName; }
            set { varName = value; }
        }

        /// <summary>
        /// The value of membership function after evaluation process.
        /// </summary>
        public double Value
        {
            get { return value; }
            set { this.value = value; }
        }

        /// <summary>
        /// The Max of membership function after evaluation process.
        /// </summary>
        public double Max
        {
            get { return max; }
            set { this.max = value; }
        }

        /// <summary>
        /// The Degree At Max of membership function after evaluation process.
        /// </summary>
        public double DegreeAtMax
        {
            get { return degreeAtMax; }
            set { this.degreeAtMax = value; }
        }

        /// <summary>
        /// The Min of membership function after evaluation process.
        /// </summary>
        public double Min
        {
            get { return min; }
            set { this.min = value; }
        }

        /// <summary>
        /// The Degree At Min of membership function after evaluation process.
        /// </summary>
        public double DegreeAtMin
        {
            get { return degreeAtMin; }
            set { this.degreeAtMin = value; }
        }

        /// <summary>
        /// The Act_value of membership function after evaluation process.
        /// </summary>
        public double Act_value
        {
            get { return act_value; }
            set { this.act_value = value; }
        }


        #endregion

        #region Public Methods

        /// <summary>
        /// Verify membership function is ordered min to max.
        /// </summary>
        public void Verify()
        {
            double max = double.NegativeInfinity;
            double theValue = double.NaN;
            foreach (Point pnt in this.pointCollection)
            {
                if (!string.IsNullOrEmpty(pnt.P1_var))
                {
                    LinguisticVariable lvar = engine.linguisticVariableCollection.Find(pnt.P1_var);
                    if (lvar == null) throw new Exception("Verify: Membership Function " + this.Name + " Point " + pnt.P1_var + " is not found !");
                    if (Double.IsNaN(lvar.Value)) throw new Exception("Verify: Membership Function " + this.Name + " Point " + pnt.P1_var + " is NaN !");
                    theValue = lvar.Value;
                    pnt.P1_val = theValue; // copy pnt.P1_var.Value to pnt.P1_val
                }
                else
                {
                    if (Double.IsNaN(pnt.P1_val)) throw new Exception("Verify: Membership Function " + this.Name + " Point.Val is NaN !");
                    theValue = pnt.P1_val;
                }
                if (Double.IsNegativeInfinity(max))
                {
                    max = theValue;
                    this.min = theValue;
                }
                else if (theValue >= max) max = theValue;
                else throw new Exception("Verify: Membership Function " + this.Name + " Point " + pnt.P1_var + " Value " + theValue + " is out of order min to max !");
                if (!string.IsNullOrEmpty(pnt.P2_var))
                {
                    LinguisticVariable lvar = engine.linguisticVariableCollection.Find(pnt.P2_var);
                    if (lvar == null) throw new Exception("Verify: Membership Function " + this.Name + " Point " + pnt.P2_var + " is not found !");
                    if (Double.IsNaN(lvar.Value)) throw new Exception("Verify: Membership Function " + this.Name + " Point " + pnt.P2_var + " is NaN !");
                    theValue = lvar.Value;
                    pnt.P2_val = theValue; // copy pnt.P2_var.Value to pnt.P2_val
                }
                if (Double.IsNaN(pnt.P2_val)) throw new Exception("Verify: Membership Function " + this.Name + " Point  is NaN !");
            }
            this.max = theValue;
        }

        /// <summary>
        /// Compute Area of membership function.
        /// </summary>
        public double ComputeArea(PointCollection pnts)
        {
            double area = 0;
            if (Double.IsNaN(lvar.Range_min)) throw new Exception("ComputeArea: LinguisticVariable " + lvar.Name + " Range_min is NaN !");
            if (Double.IsNaN(lvar.Range_max)) throw new Exception("ComputeArea: LinguisticVariable " + lvar.Name + " Range_max is NaN !");
            bool first = true;
            Point previous = null;
            double rise;
            double run;
            foreach (Point pnt in pnts)
            {
                if (Double.IsNaN(pnt.P1_val)) throw new Exception("ComputeArea: LinguisticVariable " + lvar.Name + " Membership Function " + this.Name + " Point.P1_val is NaN !");
                if (Double.IsNaN(pnt.P2_val)) throw new Exception("ComputeArea: LinguisticVariable " + lvar.Name + " Membership Function " + this.Name + " Point.P2_val is NaN !");
                if (first)
                {
                    if (pnt.P2_val > 0) area += Math.Abs(pnt.P1_val - lvar.Range_min) * pnt.P2_val; // extend area to min
                }
                else // not first
                {
                    run = Math.Abs(pnt.P1_val - previous.P1_val);
                    if (run > 0)
                    {
                        rise = Math.Abs(pnt.P2_val - previous.P2_val);
                        if (rise == 0) // equal
                        {
                            area += pnt.P2_val * run; // rectangle
                        }
                        else
                        {
                            area += Math.Min(pnt.P2_val, previous.P2_val) * run; // base
                            area += rise * run / 2; // triangle
                        }
                    }
                }
                first = false;
                previous = pnt;
            }
            if (previous.P1_val < lvar.Range_max)
            {
                if (previous.P2_val > 0) area += Math.Abs(previous.P1_val - lvar.Range_max) * previous.P2_val; // extend area to Range_max
            }
            Console.WriteLine("ComputeArea:  LinguisticVariable " + lvar.Name + " Membership Function " + this.Name + " Area=" + area);
            return area;
        }

        /// <summary>
        /// Compute Activation of membership function.
        /// </summary>
        public void ComputeActivation(double activation)
        {
            if (Double.IsNaN(activation)) throw new Exception("ComputeActivation: LinguisticVariable " + lvar.Name + " Activation is NaN !");
            activated_pointCollection.Clear();
            if (Double.IsNaN(lvar.Range_min)) throw new Exception("ComputeArea: LinguisticVariable " + lvar.Name + " Range_min is NaN !");
            if (Double.IsNaN(lvar.Range_max)) throw new Exception("ComputeArea: LinguisticVariable " + lvar.Name + " Range_max is NaN !");
            this.Act_value = activation;
            bool first = true;
            Point previous = null;
            double rise;
            double run;
            double slope;
            double dist;
            activated_pointCollection.Clear();
            foreach (Point pnt in this.pointCollection)
            {
                if (Double.IsNaN(pnt.P1_val)) throw new Exception("ComputeArea: LinguisticVariable " + lvar.Name + " Membership Function " + this.Name + " Point.P1_val is NaN !");
                if (Double.IsNaN(pnt.P2_val)) throw new Exception("ComputeArea: LinguisticVariable " + lvar.Name + " Membership Function " + this.Name + " Point.P2_val is NaN !");
                if (first)
                {
                    if (pnt.P2_val > 0 && pnt.P1_val > lvar.Range_min)
                    {
                        Point start = new Point(lvar.Range_min, Math.Min(pnt.P2_val, activation));
                        activated_pointCollection.Add(start);
                    }
                    activated_pointCollection.Add(pnt.ClonePoint(activation));
                }
                else // not first
                {
                    rise = Math.Abs(pnt.P2_val - previous.P2_val);
                    run = Math.Abs(pnt.P1_val - previous.P1_val);
                    if ((activation == 0)  // activation == 0
                    || (run == 0)  // pnt.P1_val == previous.P1_val
                    || (rise == 0)) // no slope
                    {
                        activated_pointCollection.Add(pnt.ClonePoint(activation));
                    }
                    else if (previous.P2_val <= activation && pnt.P2_val <= activation) // both <= activation
                        activated_pointCollection.Add(pnt.ClonePoint(activation));
                    else if (previous.P2_val > activation && pnt.P2_val > activation) // both > activation
                        activated_pointCollection.Add(pnt.ClonePoint(activation));
                    else if (previous.P2_val <= activation) // previous below, pnt above activation
                    {
                        slope = rise / run;
                        dist = (activation - previous.P2_val) / slope;
                        Point intersect = new Point(previous.P1_val + dist, activation);
                        activated_pointCollection.Add(intersect);
                        activated_pointCollection.Add(pnt.ClonePoint(activation));
                    }
                    else // previous above, pnt below activation
                    {
                        slope = rise / run;
                        dist = (activation - pnt.P2_val) / slope;
                        Point intersect = new Point(pnt.P1_val - dist, activation);
                        activated_pointCollection.Add(intersect);
                        activated_pointCollection.Add(pnt.ClonePoint(activation));
                    }
                }
                first = false;
                previous = pnt;
            }
            if (previous.P2_val > 0 && previous.P1_val < lvar.Range_max)
            {
                Point last = new Point(lvar.Range_max, Math.Min(previous.P2_val, activation));
                activated_pointCollection.Add(last);

            }
            Console.WriteLine("ComputeActivation:  LinguisticVariable " + lvar.Name + " Membership Function " + this.Name + " Point.Count=" + activated_pointCollection.Count + " activation=" + activation);
            MembershipFunction mfunc = this;
            this.activated_pointCollection.ComputeArea(ref lvar, ref mfunc);
        }

        /// <summary>
        /// FindValue of membership function value corresponding to the location.
        /// </summary>
        public double FindValue(ref PointCollection pnts, double location)
        {
            if (pnts.Count < 1) 
                throw new Exception("FindValue: pnts.Count < 1 !");
            double value = Double.NaN;
            double rise;
            double run;
            double slope;
            bool first = true;
            Point previous = null;
            foreach (Point pnt in pnts)
            {
                if (first)
                {
                    first = false;
                    if (location <= pnt.P1_val)
                    {
                        value = pnt.P2_val;
                        this.loc1 = pnt.index;
                        break;
                    }
                }
                else if (previous.P1_val == pnt.P1_val)
                {
                    run = 0;
                }
                else if (pnt.P1_val == location)
                {
                    value = pnt.P2_val;
                    this.loc1 = pnt.index;
                    break;
                }
                else if (pnt.P1_val > location)
                {
                    run = Math.Abs(pnt.P1_val - previous.P1_val);
                    rise = pnt.P2_val - previous.P2_val;
                    slope = rise / run;
                    value = slope * (location - previous.P1_val) + previous.P2_val;
                    this.loc1 = pnt.index;
                    break;
                }
                previous = pnt;
            }
            if (Double.IsNaN(value)) value = previous.P2_val;
            return value;
        }


        #endregion
    }
}
