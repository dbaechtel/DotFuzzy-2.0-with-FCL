using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotFuzzy
{
    /// <summary>
    /// Represents a membership function Point.
    /// </summary>
    public class Point
    {
        public int index = Int32.MinValue;
        private double p1_val = Double.NaN;
        private string p1_var = String.Empty;
        private double p2_val = Double.NaN;
        private string p2_var = String.Empty;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Point()
        {
        }

        /// <param name="index">P1_val.</param>
        /// <param name="d">P1_val.</param>
        /// <param name="v">P1_var.</param>
        public Point(int index, double d, string v)
        {
            if (index == 1)
            {
                this.p1_val = d;
                this.p1_var = v;
            }
            else
            {
                this.p2_val = d;
                this.p2_var = v;
            }
        }

        /// <param name="P1">P1_val.</param>
        /// <param name="P2">P2_val.</param>
        public Point(double P1, double P2)
        {
            this.p1_val = Math.Round(P1,6);
            this.p1_var = "";
            this.p2_val = Math.Round(P2,6);
            this.p2_var = "";
        }

        /// <summary>
        /// Clone this Point and return new Point.
        /// </summary>
        public Point ClonePoint()
        {
            Point clone = new Point();
            clone.p1_val = this.p1_val;
            clone.p1_var = this.p1_var;
            clone.p2_val = this.p2_val;
            clone.p2_var = this.p2_var;
            return clone;
        }

        /// <summary>
        /// Clone this Point and return new Point.
        /// </summary>
        public Point ClonePoint(double activation)
        {
            Point clone = new Point();
            clone.p1_val = this.p1_val;
            clone.p1_var = this.p1_var;
            clone.p2_val = Math.Min(this.p2_val, activation);
            clone.p2_var = this.p2_var;
            return clone;
        }

        /// <summary>
        /// The p1_val of the linguistic variable.
        /// </summary>
        public double P1_val
        {
            get { return p1_val; }
            set { p1_val = value; }
        }

        /// <summary>
        /// The p2_val of the linguistic variable.
        /// </summary>
        public double P2_val
        {
            get { return p2_val; }
            set { p2_val = value; }
        }

        /// <summary>
        /// The p1_var of the linguistic variable.
        /// </summary>
        public string P1_var
        {
            get { return p1_var; }
            set { p1_var = value; }
        }

        /// <summary>
        /// The p2_var of the linguistic variable.
        /// </summary>
        public string P2_var
        {
            get { return p2_var; }
            set { p2_var = value; }
        }


    }
}
