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
    /// Represents a linguistic variable.
    /// </summary>
    public class LinguisticVariable
    {
        #region Private Properties

        private string name = String.Empty;
        private string direction = String.Empty;
        private string type = String.Empty;
        private double default_value = Double.NaN;
        private string default_NC = String.Empty;
        private double min = Double.NaN;
        private double max = Double.NaN;
        private string varName = String.Empty;
        private MembershipFunctionCollection membershipFunctionCollection = new MembershipFunctionCollection();
        public PointCollection accumulation = new PointCollection();
        private string method = String.Empty;
        private double var_value = Double.NaN;
        private double act_value = Double.NaN;
        private double span = Double.NaN;
        public double area = Double.NaN;
        public double addArea = Double.NaN;
        public double addArea2 = Double.NaN;
        public double center = Double.NaN;


        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LinguisticVariable()
        {
            min = Double.NaN;
            max = Double.NaN;
            area = Double.NaN;
            addArea = Double.NaN;
            addArea2 = Double.NaN;
        }

        /// <param name="name">The name that identificates the linguistic variable.</param>
        /// <param name="direction">The direction of the linguistic variable.</param>
        /// <param name="type">The type of the linguistic variable.</param>
        public LinguisticVariable(string name, string direction, string type)
        {
            this.Name = name;
            this.Direction = direction;
            this.Type = type;
            min = Double.NaN;
            max = Double.NaN;
            addArea = Double.NaN;
            addArea2 = Double.NaN;
        }

        /// <param name="name">The name that identificates the linguistic variable.</param>
        /// <param name="type">The type of the linguistic variable.</param>
        /// <param name="membershipFunctionCollection">A membership functions collection for the lingusitic variable.</param>
        public LinguisticVariable(string name, string type, MembershipFunctionCollection membershipFunctionCollection)
        {
            this.Name = name;
            this.Direction = direction;
            this.Type = type;
            this.MembershipFunctionCollection = membershipFunctionCollection;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The name that identificates the linguistic variable.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// The value that identificates the direction linguistic variable.
        /// </summary>
        public string Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        /// <summary>
        /// The Type of the linguistic variable.
        /// </summary>
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// The VARName of the linguistic variable.
        /// </summary>
        public string VARName
        {
            get { return varName; }
            set { varName = value; }
        }

        /// <summary>
        /// The Method of the linguistic variable.
        /// </summary>
        public string Method
        {
            get { return method; }
            set { method = value; }
        }

        /// <summary>
        /// The default_value of the linguistic variable.
        /// </summary>
        public double Default_value
        {
            get { return default_value; }
            set { default_value = value; }
        }

        /// <summary>
        /// The Range_min of the linguistic variable.
        /// </summary>
        public double Range_min
        {
            get { return min; }
            set { min = value; }
        }

        /// <summary>
        /// The Range_max of the linguistic variable.
        /// </summary>
        public double Range_max
        {
            get { return max; }
            set { max = value; }
        }

        /// <summary>
        /// The Span of the mfunc.
        /// </summary>
        public double Span
        {
            get { return span; }
            set { span = value; }
        }


        /// <summary>
        /// The Default_NC of the linguistic variable.
        /// </summary>
        public string Default_NC
        {
            get { return default_NC; }
            set { default_NC = value; }
        }

        /// <summary>
        /// A membership functions collection for the lingusitic variable.
        /// </summary>
        public MembershipFunctionCollection MembershipFunctionCollection
        {
            get { return membershipFunctionCollection; }
            set { membershipFunctionCollection = value; }
        }

        /// <summary>
        /// The input value for the linguistic variable.
        /// </summary>
        public double Value
        {
            get { return var_value; }
            set { var_value = value; }
        }

        /// <summary>
        /// The Act_value for the linguistic variable.
        /// </summary>
        public double Act_value
        {
            get { return act_value; }
            set { act_value = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Implements the fuzzification of the linguistic variable.
        /// </summary>
        /// <param name="membershipFunctionName">The membership function for which fuzzify the variable.</param>
        /// <returns>The degree of membership.</returns>
        public double Fuzzify(string membershipFunctionName)
        {
            MembershipFunction membershipFunction = this.membershipFunctionCollection.Find(membershipFunctionName);

            /*
                        if ((membershipFunction.X0 <= this.Value) && (this.Value < membershipFunction.X1))
                            return (this.Value - membershipFunction.X0) / (membershipFunction.X1 - membershipFunction.X0);
                        else if ((membershipFunction.X1 <= this.Value) && (this.Value <= membershipFunction.X2))
                            return 1;
                        else if ((membershipFunction.X2 < this.Value) && (this.Value <= membershipFunction.X3))
                            return (membershipFunction.X3 - this.Value) / (membershipFunction.X3 - membershipFunction.X2);
                        else
            */
            return 0;
        }

        /// <summary>
        /// Returns the minimum value of the linguistic variable.
        /// </summary>
        /// <returns>The minimum value of the linguistic variable.</returns>
        public double MinValue()
        {
            double minValue = Double.NaN;
            /*
            this.membershipFunctionCollection[0].X0;

            for (int i = 1; i < this.membershipFunctionCollection.Count; i++)
            {
                if (this.membershipFunctionCollection[i].X0 < minValue)
                    minValue = this.membershipFunctionCollection[i].X0;
            }
            */

            return minValue;
        }

        /// <summary>
        /// Returns the maximum value of the linguistic variable.
        /// </summary>
        /// <returns>The maximum value of the linguistic variable.</returns>
        public double MaxValue()
        {
            double maxValue = Double.NaN;
            /*
            this.membershipFunctionCollection[0].X3;

            for (int i = 1; i < this.membershipFunctionCollection.Count; i++)
            {
                if (this.membershipFunctionCollection[i].X3 > maxValue)
                    maxValue = this.membershipFunctionCollection[i].X3;
            }
            */

            return maxValue;
        }

        /// <summary>
        /// Returns the difference between MaxValue() and MinValue().
        /// </summary>
        /// <returns>The difference between MaxValue() and MinValue().</returns>
        public double Range()
        {
            if (Double.IsNaN(this.MaxValue()) || Double.IsNaN(this.MinValue())) throw new Exception("LinguisticVariable " + this.Name + " MaxValue or MinValue is NaN");
            return this.MaxValue() - this.MinValue();
        }

        /// <summary>
        /// Accumulate the results of Membership functions.
        /// </summary>
        public void Accumulate()
        {
            Console.WriteLine("Accumulate: lvar = " + this.Name);
            double previousLoc = Double.NegativeInfinity;
            double location;
            int index;
            int index2;
            int prevMfunc = -1;
            int mfuncIndex = -1;
            bool finished = false;
            index2 = 0;
            this.accumulation.Clear();
            foreach (MembershipFunction mfunc in this.MembershipFunctionCollection)
            {
                mfunc.index = index2++;
                index = 0;
                foreach (Point pnt in mfunc.activated_pointCollection)
                {
                    pnt.index = index++;
                }
            }
            while (previousLoc < this.Range_max)
            {
                location = Double.PositiveInfinity;
                finished = false;
                while (!finished)
                {
                    foreach (MembershipFunction mfunc in this.MembershipFunctionCollection)
                    {
                        finished = true;
                        if (mfunc.Act_value > 0)
                        {
                            foreach (Point pnt in mfunc.activated_pointCollection)
                            {
                                if (pnt.P1_val >= location) break;
                                if (pnt.P1_val > previousLoc)
                                {
                                    location = pnt.P1_val;
                                    mfunc.pntIndex = pnt.index;
                                    finished = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                double maximum = Double.NegativeInfinity;
                foreach (MembershipFunction mfunc in this.MembershipFunctionCollection)
                {
                    if (mfunc.Act_value == 0) continue;
                    if (mfunc.activated_pointCollection.Count == 0) continue;
                    double value = mfunc.FindValue(ref mfunc.activated_pointCollection, location);
                    if (value >= maximum)
                    {
                        maximum = Math.Round(value,6);
                        mfuncIndex = mfunc.index;
                        mfunc.pntIndex = mfunc.loc1;
                        if (prevMfunc < 0) prevMfunc = mfuncIndex;
                    }
                }
                if (prevMfunc < 0)
                    throw new Exception("Accumulate: LinguisticVariable " + this.Name + " All MembershipFunction.Act_value == 0 !");
                if (mfuncIndex != prevMfunc) // Intersection
                {
                    MembershipFunction mfunc = this.membershipFunctionCollection[mfuncIndex];
                    MembershipFunction mfunc2 = this.membershipFunctionCollection[prevMfunc];
                    Point thePoint = mfunc.activated_pointCollection[mfunc.pntIndex];
                    Point thePoint2 = mfunc2.activated_pointCollection[mfunc2.pntIndex];
                    if (mfunc.pntIndex == 0) // first point
                    {
                        if (location > previousLoc)
                            this.accumulation.Add(new Point(location, maximum));
                    }
                    else if (mfunc2.pntIndex == mfunc2.activated_pointCollection.Count - 1) // last point
                    {
                        if (location > previousLoc) 
                            this.accumulation.Add(new Point(location, maximum));
                    }
                    else // get Intersection
                    {
                        Point thePoint1 = mfunc.activated_pointCollection[mfunc.pntIndex - 1];
                        Point thePoint3 = (mfunc2.pntIndex == 0) ? mfunc2.activated_pointCollection[mfunc2.pntIndex + 1] : mfunc2.activated_pointCollection[mfunc2.pntIndex - 1];
                        double run = Math.Abs(thePoint.P1_val - thePoint1.P1_val);
                        double run2 = Math.Abs(thePoint3.P1_val - thePoint2.P1_val);
                        double rise = thePoint.P2_val - thePoint1.P2_val;
                        double rise2 = thePoint3.P2_val - thePoint2.P2_val;
                        if (run == 0 || run2 == 0 || rise2 == 0)
                        {
                            if (location > previousLoc) 
                                this.accumulation.Add(new Point(location, Math.Max(thePoint.P2_val, thePoint1.P2_val)));
                        }
                        else
                        {
                            double x1 = thePoint2.P1_val;
                            double x2 = thePoint3.P1_val;
                            double x3 = thePoint1.P1_val;
                            double x4 = thePoint.P1_val;
                            double y1 = thePoint2.P2_val;
                            double y2 = thePoint3.P2_val;
                            double y3 = thePoint1.P2_val;
                            double y4 = thePoint.P2_val;
                            double num1 = (x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4);
                            double num2 = (x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4);
                            double den1 = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
                            double den2 = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
                            if (den1 == 0 || den2 == 0)
                                throw new Exception("Accumulate: Intersection is Undefined !");
                            location = Math.Round(num1 / den1, 6);
                            maximum = Math.Round(num2 / den1, 6);
                            if (location > previousLoc) 
                                this.accumulation.Add(new Point(location, maximum));
                        }
                    }
                }
                else // no Intersection
                {
                    if (location > previousLoc) 
                        this.accumulation.Add(new Point(location, maximum));
                }
                prevMfunc = mfuncIndex;
                previousLoc = location;
            }
        }

        #endregion
    }
}
