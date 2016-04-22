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
