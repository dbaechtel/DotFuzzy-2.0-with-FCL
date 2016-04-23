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
using System.Collections.ObjectModel;
using System.Text;

namespace DotFuzzy
{
    /// <summary>
    /// Represents a collection of membership function Points.
    /// </summary>
    public class PointCollection : Collection<Point>
    {

        #region Public Methods


        /// <summary>
        /// Compute Area of membership function.
        /// </summary>
        public double ComputeArea(ref LinguisticVariable lvar, ref MembershipFunction mfunc)
        {
            double area = 0;
            if (Double.IsNaN(lvar.Range_min)) throw new Exception("ComputeArea: LinguisticVariable " + lvar.Name + " Range_min is NaN !");
            if (Double.IsNaN(lvar.Range_max)) throw new Exception("ComputeArea: LinguisticVariable " + lvar.Name + " Range_max is NaN !");
            bool first = true;
            bool extend = false;
            bool extend2 = false;
            Point previous = null;
            double rise;
            double run;
            foreach (Point pnt in this)
            {
                if (Double.IsNaN(pnt.P1_val)) throw new Exception("ComputeArea: LinguisticVariable " + lvar.Name + " Membership Function "
                    + ((Object.ReferenceEquals(mfunc, null)) ? "" : mfunc.Name) + " Point.P1_val is NaN !");
                if (Double.IsNaN(pnt.P2_val)) throw new Exception("ComputeArea: LinguisticVariable " + lvar.Name + " Membership Function "
                    + ((Object.ReferenceEquals(mfunc, null)) ? "" : mfunc.Name) + " Point.P2_val is NaN !");
                if (first)
                {
                    if (pnt.P2_val > 0)
                    {
                        area += Math.Abs(pnt.P1_val - lvar.Range_min) * pnt.P2_val; // extend area to min
                        if (!Object.ReferenceEquals(mfunc, null)) 
                            extend = true;
                    }
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
            } // foreach pnt
            if (previous.P1_val <= lvar.Range_max)
            {
                if (previous.P2_val > 0)
                {
                    area += Math.Abs(previous.P1_val - lvar.Range_max) * previous.P2_val; // extend area to Range_max
                    if (!Object.ReferenceEquals(mfunc, null)) 
                        extend2 = true;
                }
            }
            if (!Object.ReferenceEquals(mfunc, null))
            {
                if ((extend || extend2) && lvar.Method == "COG_EXTEND")
                {
                    mfunc.addArea = area; // Additional area
                    Console.WriteLine("ComputeArea:  LinguisticVariable " + lvar.Name + " Membership Function " + mfunc.Name + " addArea=" + area);
                    if (extend) lvar.addArea = area;
                    else lvar.addArea2 = area;
                }
            }
            Console.WriteLine("ComputeArea:  LinguisticVariable " + lvar.Name + " Membership Function "
                + ((Object.ReferenceEquals(mfunc, null)) ? "" : mfunc.Name) + " Area=" + area);
            return area;
        }

        /// <summary>
        /// Compute Area of membership function.
        /// </summary>
        public double LocateCenterArea(ref LinguisticVariable lvar, string mfuncName)
        {
            double location = Double.NaN;
            double rise = Double.NaN;
            double run = Double.NaN;
            double dist = Double.NaN;
            double sumArea = 0;
            Point previous = null;
            bool first = true;
            double area = lvar.area + ((Double.IsNaN(lvar.addArea)) ? 0 : lvar.addArea) + ((Double.IsNaN(lvar.addArea2)) ? 0 : lvar.addArea2);
            if (lvar == null) throw new Exception("LocateCenterArea: LinguisticVariable lvar is null !");
            if (Double.IsNaN(area)) throw new Exception("LocateCenterArea: LinguisticVariable " + lvar.Name + " Membership Function " + mfuncName + " area is NaN !");
            if (this.Count < 1) throw new Exception("LocateCenterArea: LinguisticVariable " + lvar.Name + " Membership Function " + mfuncName + " PointCollection Count < 1 !");
            if (this.Count == 1)
            {
                Point thePoint = this.Items[0];
                location = thePoint.P1_val;
            }
            else // this.Count > 1
            {
                foreach (Point pnt in this)
                {
                    if (first)
                    {
                        if (!Double.IsNaN(lvar.addArea)) sumArea += lvar.addArea;
                    }
                    else
                    {
                        if (Double.IsNaN(previous.P2_val))
                            throw new Exception("LocateCenterArea: LinguisticVariable " + lvar.Name + " Membership Function " + mfuncName + " P2_val is NaN!");
                        run = pnt.P1_val - previous.P1_val;
                        if (run <= 0) continue;
                        rise = pnt.P2_val - previous.P2_val;
                        switch (Math.Sign(rise))
                        {
                            case 0: // rise == zero
                                sumArea += run * pnt.P2_val;
                                break;
                            case 1: // rise > zero
                                sumArea += run * (previous.P2_val + rise / 2);
                                break;
                            case -1: // rise < zero
                                sumArea += run * (pnt.P2_val - rise / 2);
                                break;
                            default:
                                throw new Exception("LocateCenterArea: LinguisticVariable " + lvar.Name + " Membership Function " + mfuncName + " Math.Sign(rise) is invalid !");
                        }
                        if (sumArea >= area / 2) // pnt is at or past area/2
                        {
                            switch (Math.Sign(rise))
                            {
                                case 0: // rise == zero
                                    if (pnt.P2_val == 0)
                                        throw new Exception("LocateCenterArea: LinguisticVariable " + lvar.Name + " Membership Function " + mfuncName + " pnt.P2_val == 0 !");
                                    dist = (sumArea - area / 2) / pnt.P2_val;
                                    break;
                                case 1: // rise > zero
                                    if (previous.P2_val + rise / 2 <= 0)
                                        throw new Exception("LocateCenterArea: LinguisticVariable " + lvar.Name + " Membership Function " + mfuncName + " (previous.P2_val + rise / 2) <= 0 !");
                                    dist = (sumArea - area / 2) / (previous.P2_val + rise / 2);
                                    break;
                                case -1: // rise < zero
                                    if (pnt.P2_val - rise / 2 <= 0)
                                        throw new Exception("LocateCenterArea: LinguisticVariable " + lvar.Name + " Membership Function " + mfuncName + " (pnt.P2_val - rise / 2) <= 0 !");
                                    dist = (sumArea - area / 2) / (pnt.P2_val - rise / 2);
                                    break;
                                default:
                                    throw new Exception("LocateCenterArea: LinguisticVariable " + lvar.Name + " Membership Function " + mfuncName + " Math.Sign(rise) is invalid !");
                            } // switch
                            if (dist > run || dist < 0)
                                throw new Exception("LocateCenterArea: LinguisticVariable " + lvar.Name + " Membership Function " + mfuncName + " dist " + dist + " is < 0 or > run " + run + " !");
                            location = Math.Min(Math.Max( pnt.P1_val - dist, lvar.Range_min), lvar.Range_max);
                            break; // foreach
                        } // if (sumArea >= area / 2)
                    } // first
                    previous = pnt;
                    first = false;
                } // foreach (Point pnt in this)
            } // this.Count > 1
            return location;
        }


        #endregion
    }
}
