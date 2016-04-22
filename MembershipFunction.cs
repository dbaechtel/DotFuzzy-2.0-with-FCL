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

        private string name = String.Empty;
        private string varName = String.Empty;
        private string singleton_varName = String.Empty;
        private double singleton_value = Double.NaN;
        public PointCollection pointCollection;
        private double value = 0;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MembershipFunction()
        {
        }

        /// <param name="name">The name that identificates the membership function.</param>
        public MembershipFunction(string name)
        {
            this.Name = name;
            pointCollection = new PointCollection();
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

        #endregion

        #region Public Methods

        /// <summary>
        /// Calculate the centroid of a trapezoidal membership function.
        /// </summary>
        /// <returns>The value of centroid.</returns>
        public double Centorid()
        {
            /*
            double a = this.x2 - this.x1;
            double b = this.x3 - this.x0;
            double c = this.x1 - this.x0;

            return ((2 * a * c) + (a * a) + (c * b) + (a * b) + (b * b)) / (3 * (a + b)) + this.x0; 
            */
            return 0;
        }

        /// <summary>
        /// Calculate the area of a trapezoidal membership function.
        /// </summary>
        /// <returns>The value of area.</returns>
        public double Area()
        {
            /*
            double a = this.Centorid() - this.x0;
            double b = this.x3 - this.x0;

            return (this.value * (b + (b - (a * this.value)))) / 2;
            */
            return 0;
        }

        #endregion
    }
}
