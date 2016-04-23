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
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace DotFuzzy
{
    /// <summary>
    /// Represents the inferential engine.
    /// </summary>
    public class FuzzyEngine
    {
        #region Private Properties

        private string function_block_name = String.Empty;
        public LinguisticVariableCollection linguisticVariableCollection = new LinguisticVariableCollection();
        public FuzzyRuleCollection fuzzyRuleCollection = new FuzzyRuleCollection();
        private string filePath = String.Empty;

        #endregion

        #region Private Methods

        private double Parse(string text)
        {
            int counter = 0;
            int firstMatch = 0;

            if (!text.StartsWith("("))
            {
                string[] tokens = text.Split();
                return this.linguisticVariableCollection.Find(tokens[0]).Fuzzify(tokens[2]);
            }

            for (int i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case '(':
                        counter++;
                        if (counter == 1)
                            firstMatch = i;
                        break;

                    case ')':
                        counter--;
                        if ((counter == 0) && (i > 0))
                        {
                            string substring = text.Substring(firstMatch + 1, i - firstMatch - 1);
                            string substringBrackets = text.Substring(firstMatch, i - firstMatch + 1);
                            int length = substringBrackets.Length;
                            text = text.Replace(substringBrackets, Parse(substring).ToString());
                            i = i - (length - 1);
                        }
                        break;

                    default:
                        break;
                }
            }

            return Evaluate(text);
        }

        private double Evaluate(string text)
        {
            string[] tokens = text.Split();
            string connective = "";
            double value = 0;

            for (int i = 0; i <= ((tokens.Length / 2) + 1); i = i + 2)
            {
                double tokenValue = Convert.ToDouble(tokens[i]);

                switch (connective)
                {
                    case "AND":
                        if (tokenValue < value)
                            value = tokenValue;
                        break;

                    case "OR":
                        if (tokenValue > value)
                            value = tokenValue;
                        break;

                    default:
                        value = tokenValue;
                        break;
                }

                if ((i + 1) < tokens.Length)
                    connective = tokens[i + 1];
            }

            return value;
        }

         #endregion

        #region Public Properties

        /// <summary>
        /// A collection of linguistic variables.
        /// </summary>
        public LinguisticVariableCollection LinguisticVariableCollection
        {
            get { return linguisticVariableCollection; }
            set { linguisticVariableCollection = value; }
        }

        /// <summary>
        /// The path of the FCL-like XML file in which save the project.
        /// </summary>
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the VAR_INPUT value.
        /// </summary>
        public void Set_VAR_INPUT(string varName, double value)
        {
            LinguisticVariable lvar = this.linguisticVariableCollection.Find(varName.Trim());
            if (lvar == null) throw new Exception("Set_VAR_INPUT: LinguisticVariable " + varName + " was not found !");
            if (lvar.Direction != "VAR_INPUT") throw new Exception("Set_VAR_INPUT: LinguisticVariable " + varName + " is not a VAR_INPUT !");
            lvar.Value = value;
            Console.WriteLine("Set_VAR_INPUT: " + varName + " = " + value.ToString());
        }

        /// <summary>
        /// Calculates the fuzzification value of each VAR_INPUT.
        /// </summary>
        public void FuzzifyAll()
        {
            foreach (LinguisticVariable lvar in this.linguisticVariableCollection)
            {
                if (string.IsNullOrEmpty(lvar.Name)) throw new Exception("LinguisticVariable is missing Name !");
                Fuzzify(lvar.Name);
            }
        }

        /// <summary>
        /// Calculates the fuzzification value of each VAR_INPUT.
        /// </summary>
        public void Fuzzify(string ling_VARNAME)
        {
            LinguisticVariable lvar = this.linguisticVariableCollection.Find(ling_VARNAME);
            if (lvar != null)
            {
                if (lvar.Direction == "VAR_INPUT")
                {
                    foreach (MembershipFunction mfunc in lvar.MembershipFunctionCollection)
                    {
                        if (!Double.IsNaN(mfunc.Singleton_value))
                        {
                            mfunc.Value = Math.Min(Math.Max(mfunc.Singleton_value, 0), 1.0);
                        }
                        else if (!String.IsNullOrEmpty(mfunc.Singleton_varName))
                        {
                            LinguisticVariable lvar2 = this.linguisticVariableCollection.Find(mfunc.Singleton_varName);
                            if (lvar2 == null) throw new Exception("Fuzzify: LinguisticVariable " + lvar.Name + " MembershipFunction " + mfunc + " Point = " + mfunc.Singleton_varName + " not found !");
                            if (Double.IsNaN(lvar2.Value)) throw new Exception("Fuzzify: LinguisticVariable " + lvar.Name + " MembershipFunction " + mfunc + " Point = " + mfunc.Singleton_varName + " is NaN !");
                            mfunc.Value = Math.Min(Math.Max(lvar2.Value, 0), 1.0);
                        }
                        else if (mfunc.pointCollection.Count > 0)
                        {
                            if (Double.IsNaN(lvar.Value)) throw new Exception("Fuzzify: LinguisticVariable " + lvar.Name + " Value = NaN. Cannot Fuzzify !");
                            mfunc.Min = double.NegativeInfinity;
                            mfunc.Max = double.PositiveInfinity;
                            foreach (Point pnt in mfunc.pointCollection)
                            {
                                if (!Double.IsNaN(pnt.P1_val))
                                {
                                    if (pnt.P1_val > mfunc.Min && pnt.P1_val <= lvar.Value)
                                    {
                                        mfunc.Min = pnt.P1_val;
                                        mfunc.DegreeAtMin = pnt.P2_val;
                                    }
                                    if (pnt.P1_val < mfunc.Max && pnt.P1_val >= lvar.Value)
                                    {
                                        mfunc.Max = pnt.P1_val;
                                        mfunc.DegreeAtMax = pnt.P2_val;
                                    }
                                }
                            }
                            if (!Double.IsNegativeInfinity(mfunc.Min))
                            {
                                if (!Double.IsPositiveInfinity(mfunc.Max))
                                {
                                    if (mfunc.Min == mfunc.Max)
                                    {
                                        mfunc.Value = Math.Max(Math.Min(Math.Max(mfunc.DegreeAtMin, mfunc.DegreeAtMax), 1.0), 0.0);

                                    }
                                    else
                                    {
                                        double run = Math.Abs(mfunc.Max - mfunc.Min);
                                        double slope = (mfunc.DegreeAtMax - mfunc.DegreeAtMin) / run;
                                        double dist = Math.Abs(lvar.Value - mfunc.Min);
                                        mfunc.Value = Math.Max(Math.Min(mfunc.DegreeAtMin + dist * slope, 1.0), 0.0);
                                    }
                                }
                                else // Double.IsPositiveInfinity(mfunc.Max)
                                {
                                    mfunc.Value = Math.Max(Math.Min(mfunc.DegreeAtMin, 1.0), 0.0);
                                }
                            }
                            else // Double.IsNegativeInfinity(mfunc.Min)
                            {
                                if (!Double.IsPositiveInfinity(mfunc.Max))
                                {
                                    mfunc.Value = Math.Max(Math.Min(mfunc.DegreeAtMax, 1.0), 0.0);
                                }
                                else throw new Exception("Fuzzify: LinguisticVariable " + lvar.Name + " Membership function " + mfunc.Name + " Value = " + mfunc.Value + ". Both Points are not defined. Cannot Fuzzify !");
                            }
                        }
                        else throw new Exception("Fuzzify: LinguisticVariable " + lvar.Name + " Membership function " + mfunc.Name + ". No points in pointCollection. Cannot Fuzzify !");
                        Console.WriteLine("Fuzzify: LinguisticVariable " + lvar.Name + " lvar.Value = " + lvar.Value + ". Membership function " + mfunc.Name + " Value = " + mfunc.Value);
                    }
                }
            }
            else throw new Exception("Fuzzify: LinguisticVariable " + ling_VARNAME + " was not found !");
        }

        /// <summary>
        /// Calculates the defuzzification value with the CoG (Center of Gravity) technique.
        /// </summary>
        public void DefuzzifyAll()
        {

            // Reset all VAR_OUTPUT membershipFunction values
            foreach (LinguisticVariable var in this.linguisticVariableCollection)
            {
                if (var.Direction == "VAR_OUTPUT")
                {
                    if (string.IsNullOrEmpty(var.Name)) throw new Exception("Defuzzify: var.Name is empty !");
                    Defuzzify(var.Name);
                }
            }
        }

        /// <summary>
        /// Calculates the defuzzification value of LinguisticVariable lvar_name.
        /// <param name="lvar_name"> Name of the LinguisticVariable to Defuzzify.</param>
        /// </summary>
        public double Defuzzify(string lvar_name)
        {
            LinguisticVariable lvar = linguisticVariableCollection.Find(lvar_name);
            if (lvar == null) throw new Exception("Defuzzify: " + lvar_name + " is not found !");
            if (lvar.Direction != "VAR_OUTPUT") throw new Exception("Defuzzify: " + lvar_name + " is not VAR_OUTPUT !");
            lvar.addArea = Double.NaN;
            lvar.addArea2 = Double.NaN;
            Console.WriteLine("Defuzzify: " + lvar.Name + " mfunc.Count = " + lvar.MembershipFunctionCollection.Count);
            foreach (MembershipFunction mfunc in lvar.MembershipFunctionCollection)
            {
                if (string.IsNullOrEmpty(mfunc.Name)) 
                    throw new Exception("Defuzzify: " + lvar_name + " MembershipFunction Name is empty !");
                {
                    mfunc.Verify();
                    if (!Double.IsNaN(mfunc.Act_value))
                    {
                        if (mfunc.pointCollection.Count > 0)
                            mfunc.ComputeActivation(mfunc.Act_value);
                        else
                            Console.WriteLine("Defuzzify: " + lvar_name + " MembershipFunction " + mfunc.Name + " pointCollection.Count=" + mfunc.pointCollection.Count + " !");
                        }
                }
            }
            // MembershipFunction mfunc2 = lvar.MembershipFunctionCollection[0];
            // double val = mfunc2.FindValue(ref mfunc2.activated_pointCollection, 2.0);
            lvar.Accumulate();
            MembershipFunction none = null;
            lvar.area = lvar.accumulation.ComputeArea(ref lvar, ref none);
            lvar.center = lvar.accumulation.LocateCenterArea(ref lvar, "");
            Console.WriteLine("Defuzzify: VAR_OUTPUT " + lvar_name + " Min=" + lvar.Range_min + " Max=" + lvar.Range_max + " Area=" + lvar.area + " Center=" + lvar.center);
            return lvar.center;
        }

        /// <summary>
        /// Calculates the Inference of  all RuleBlocks.
        /// </summary>
        public void Inference()
        {
            Aggregation();
            Activation();
        }

        /// <summary>
        /// Calculates the Aggregation of  all RuleBlocks.
        /// </summary>
        public void Aggregation()
        {

            foreach (RuleBlockCollection ruleBlock in this.fuzzyRuleCollection)
            {
                foreach (FuzzyRule fuzzyRule in ruleBlock)
                {
                    fuzzyRule.Value = Double.NaN;
                    fuzzyRule.Aggregation = Double.NaN;
                    foreach (SubCondition cond in fuzzyRule.conditionCollection)
                    {
                        if (string.IsNullOrEmpty(cond.VarName)) throw new Exception("RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name + " Condition VarName is null !");
                        LinguisticVariable condVar = linguisticVariableCollection.Find(cond.VarName);
                        if (condVar == null) throw new Exception("RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name + " Condition " + cond.VarName + " is not found !");
                        double theValue = Double.NaN;
                        if (string.IsNullOrEmpty(cond.TermName))  // Singleton
                        {
                            if (Double.IsNaN(condVar.Value)) throw new Exception("RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name + " Condition " + cond.VarName + " is NaN !");
                            theValue = Math.Min(Math.Max(condVar.Value, 0), 1);
                        }
                        else // not Singleton
                        {
                            MembershipFunction termVar = condVar.MembershipFunctionCollection.Find(cond.TermName);
                            if (termVar == null) throw new Exception("RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name + " Term " + cond.TermName + " is not found !");
                            if (Double.IsNaN(termVar.Value)) throw new Exception("RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name + " Term " + cond.TermName + " is NaN !");
                            theValue = Math.Min(Math.Max(termVar.Value, 0), 1);
                            if (!string.IsNullOrEmpty(cond.NegatedTerm)) theValue = 1 - theValue;
                        }
                        switch (cond.ConditionOper)
                        {
                            case "":
                                fuzzyRule.Aggregation = theValue;
                                break;
                            case "OR":
                                fuzzyRule.Aggregation = Math.Max(theValue, fuzzyRule.Aggregation);
                                break;
                            case "AND":
                                fuzzyRule.Aggregation = Math.Min(theValue, fuzzyRule.Aggregation);
                                break;
                            default:
                                throw new Exception("");

                        }
                        if (!string.IsNullOrEmpty(cond.NegatedVar)) fuzzyRule.Aggregation = 1 - fuzzyRule.Aggregation;
                        Console.WriteLine("Aggregation: RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name
                            + ((string.IsNullOrEmpty(cond.ConditionOper)) ? " IF " : " " + cond.ConditionOper)
                            + ((!string.IsNullOrEmpty(cond.NegatedVar)) ? " NOT " : " ") + cond.VarName
                            + ((!string.IsNullOrEmpty(cond.TermName)) ? " IS " + ((!string.IsNullOrEmpty(cond.NegatedTerm)) ? "NOT " : "") + cond.TermName : "")
                            + " Value=" + fuzzyRule.Aggregation);
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the Activation of all RuleBlocks.
        /// </summary>
        public void Activation()
        {
            foreach (RuleBlockCollection ruleBlock in this.fuzzyRuleCollection)  // reset activation
            {
                foreach (FuzzyRule fuzzyRule in ruleBlock)
                {
                    foreach (SubCondition conclusion in fuzzyRule.conclusionCollection)
                    {
                        if (string.IsNullOrEmpty(conclusion.VarName)) throw new Exception("RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name + " Conclusion VarName is null !");
                        LinguisticVariable conclVar = linguisticVariableCollection.Find(conclusion.VarName);
                        if (conclVar == null) throw new Exception("RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name + " Conclusion " + conclusion.VarName + " is not found !");
                        if (!string.IsNullOrEmpty(conclusion.TermName)) // TermName
                        {
                            MembershipFunction termVar = conclVar.MembershipFunctionCollection.Find(conclusion.TermName);
                            if (termVar == null)
                                throw new Exception("RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name + " Conclusion Term " + conclusion.TermName + " is not found !");
                            termVar.Act_value = Double.NaN;

                        }
                    }
                }
            }
            foreach (RuleBlockCollection ruleBlock in this.fuzzyRuleCollection)
            {
                foreach (FuzzyRule fuzzyRule in ruleBlock)
                {
                    foreach (SubCondition conclusion in fuzzyRule.conclusionCollection)
                    {
                        if (string.IsNullOrEmpty(conclusion.VarName)) throw new Exception("RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name + " Conclusion VarName is null !");
                        LinguisticVariable conclVar = linguisticVariableCollection.Find(conclusion.VarName);
                        if (conclVar == null) throw new Exception("RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name + " Conclusion " + conclusion.VarName + " is not found !");
                        if (string.IsNullOrEmpty(conclusion.TermName)) // Singleton_Var
                        {
                            if (Double.IsNaN(conclVar.Value)) conclVar.Value = Math.Min(Math.Max(1.0 * fuzzyRule.Aggregation, 0), 1);
                            else conclVar.Value = Math.Min(Math.Max(conclVar.Value * fuzzyRule.Aggregation, 0), 1);
                            if (Double.IsNaN(conclVar.Act_value)) conclVar.Act_value = conclVar.Value;
                            else conclVar.Act_value = Math.Min(Math.Max(Math.Max(conclVar.Act_value, conclVar.Value), 0), 1);
                            Console.WriteLine("Activation: RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name + " Conclusion " + conclVar.Name + " Act_value=" + conclVar.Act_value);
                        }
                        else // TermName
                        {
                            MembershipFunction termVar = conclVar.MembershipFunctionCollection.Find(conclusion.TermName);
                            if (termVar == null)
                                throw new Exception("RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name + " Conclusion Term " + conclusion.TermName + " is not found !");
                            if (Double.IsNaN(termVar.Act_value)) termVar.Act_value = fuzzyRule.Aggregation;
                            else termVar.Act_value = Math.Min(Math.Max(Math.Max(termVar.Act_value, fuzzyRule.Aggregation), 0), 1);
                            Console.WriteLine("Activation: RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name + " Lvar " + conclVar.Name + " Term " + termVar.Name + " Act_value=" + termVar.Act_value);
                            /*
                            if (!Double.IsNaN(termVar.Singleton_value)) // Singleton_value
                            {
                                conclVar.Value = Math.Min(Math.Max(termVar.Singleton_value * fuzzyRule.Aggregation, 0), 1);
                                if (Double.IsNaN(conclVar.Act_value)) conclVar.Act_value = conclVar.Value;
                                else conclVar.Act_value = Math.Min(Math.Max(Math.Max(conclVar.Act_value, conclVar.Value), 0), 1);
                            }
                            else if (!string.IsNullOrEmpty(termVar.Singleton_varName)) //Singleton_varName
                            {
                                LinguisticVariable clVar = this.linguisticVariableCollection.Find(termVar.Singleton_varName);
                                if (clVar == null)
                                    throw new Exception("RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name + " Conclusion Term " + conclusion.TermName + " Memfunction " + termVar.Singleton_varName + " is not found !");
                                if (Double.IsNaN(clVar.Value))
                                    throw new Exception("RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name + " Conclusion Term " + conclusion.TermName + " Memfunction " + termVar.Singleton_varName + " is NaN !");
                                conclVar.Value = Math.Min(Math.Max(clVar.Value * fuzzyRule.Aggregation, 0), 1);
                                if (Double.IsNaN(conclVar.Act_value)) conclVar.Act_value = conclVar.Value;
                                else conclVar.Act_value = Math.Min(Math.Max(Math.Max(conclVar.Act_value, conclVar.Value), 0), 1);
                            }
                            else // Points
                            {
                                if (termVar.pointCollection.Count < 1)
                                    throw new Exception("RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name + " Conclusion Term " + conclusion.TermName + " Memfunction Point Count=0 is NaN !");
                               
                                double maxPoint = double.PositiveInfinity;
                                double minPoint = double.NegativeInfinity;
                                double maxPointVal = double.NaN;
                                double minPointVal = double.NaN;
                                foreach (Point pnt in termVar.pointCollection)
                                {
                                    Double P1val = double.NaN;
                                    if (!string.IsNullOrEmpty(pnt.P1_var))
                                    {
                                        LinguisticVariable P1_var = this.linguisticVariableCollection.Find(pnt.P1_var);
                                        if (P1_var == null)
                                            throw new Exception("RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name
                                                + " Conclusion Term " + conclusion.TermName + " Memfunction " + termVar.Name + " Point " + pnt.P1_var + " was not found !");
                                        if (Double.IsNaN(P1_var.Value))
                                            throw new Exception("RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name
                                                + " Conclusion Term " + conclusion.TermName + " Memfunction " + termVar.Name + " Point " + pnt.P1_var + " is NaN !");
                                        P1val = P1_var.Value;
                                    }
                                    else
                                    {
                                        if (Double.IsNaN(pnt.P1_val))
                                            throw new Exception("RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name
                                                + " Conclusion Term " + conclusion.TermName + " Memfunction " + termVar.Name + " Point is NaN !");
                                        P1val = pnt.P1_val;
                                    }
                                    if (P1val < maxPoint && P1val >= fuzzyRule.Aggregation)
                                    {
                                        maxPoint = P1val;
                                        if (string.IsNullOrEmpty(pnt.P2_var))
                                        {
                                            if (Double.IsNaN(pnt.P2_val))
                                                throw new Exception("RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name
                                                    + " Conclusion Term " + conclusion.TermName + " Memfunction " + termVar.Name + " Point is NaN !");
                                            maxPointVal = pnt.P2_val;
                                        }
                                        else
                                        {
                                            LinguisticVariable P2_var = this.linguisticVariableCollection.Find(pnt.P2_var);
                                            if (P2_var == null)
                                                throw new Exception("RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name
                                                    + " Conclusion Term " + conclusion.TermName + " Memfunction " + termVar.Name + " Point " + pnt.P2_var + " was not found !");
                                            if (Double.IsNaN(P2_var.Value))
                                                throw new Exception("RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name
                                                    + " Conclusion Term " + conclusion.TermName + " Memfunction " + termVar.Name + " Point " + pnt.P2_var + " is NaN !");
                                            maxPointVal = P2_var.Value;
                                        }
                                    }
                                    if (P1val > minPoint && P1val <= fuzzyRule.Aggregation)
                                    {
                                        minPoint = P1val;
                                        if (string.IsNullOrEmpty(pnt.P2_var))
                                        {
                                            if (Double.IsNaN(pnt.P2_val))
                                                throw new Exception("RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name
                                                    + " Conclusion Term " + conclusion.TermName + " Memfunction " + termVar.Name + " Point is NaN !");
                                            minPointVal = pnt.P2_val;
                                        }
                                        else
                                        {
                                            LinguisticVariable P2_var = this.linguisticVariableCollection.Find(pnt.P2_var);
                                            if (P2_var == null)
                                                throw new Exception("RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name
                                                    + " Conclusion Term " + conclusion.TermName + " Memfunction " + termVar.Name + " Point " + pnt.P2_var + " was not found !");
                                            if (Double.IsNaN(P2_var.Value))
                                                throw new Exception("RuleBlock " + ruleBlock.Name + " FuzzyRule " + fuzzyRule.Name
                                                    + " Conclusion Term " + conclusion.TermName + " Memfunction " + termVar.Name + " Point " + pnt.P2_var + " is NaN !");
                                            minPointVal = P2_var.Value;
                                        }
                                    }
                                }
                                if (double.IsPositiveInfinity(maxPoint))
                                {
                                    conclVar.Value = Math.Min(Math.Max(minPointVal * fuzzyRule.Aggregation, 0), 1);
                                }
                                else if (double.IsNegativeInfinity(minPoint))
                                {
                                    conclVar.Value = Math.Min(Math.Max(maxPointVal * fuzzyRule.Aggregation, 0), 1);
                                }
                                else
                                {
                                    double run = maxPoint - minPoint;
                                    if (run != 0)
                                    {
                                        double rise = maxPointVal - minPointVal;
                                        double slope = rise / run;
                                        double dist = fuzzyRule.Aggregation - minPoint;
                                        conclVar.Value = Math.Min(Math.Max((dist * slope + minPointVal) * fuzzyRule.Aggregation, 0), 1);
                                    }
                                    else conclVar.Value = Math.Min(Math.Max(maxPointVal * fuzzyRule.Aggregation, 0), 1);
                                }
                            }
                          */
                        }
                    }
                }
            }
        }


        /// <summary>
        /// The Function_block_name of the FB.
        /// </summary>
        public string Function_block_name
        {
            get { return function_block_name; }
            set { function_block_name = value; }
        }

        /// <summary>
        /// Sets the FilePath property and saves the project into a FCL-like XML file.
        /// </summary>
        /// <param name="path">Path of the destination document.</param>
        public void Save(string path)
        {
            this.FilePath = path;
            this.Save();
        }

        /// <summary>
        /// Saves the project into a FCL-like XML file.
        /// </summary>
        public void Save()
        {
            if (this.filePath == String.Empty)
                throw new Exception("FilePath not set");

            XmlTextWriter xmlTextWriter = new XmlTextWriter(this.filePath, Encoding.UTF8);
            xmlTextWriter.Formatting = Formatting.Indented;
            xmlTextWriter.WriteStartDocument(true);
            xmlTextWriter.WriteStartElement("FUNCTION_BLOCK");
            xmlTextWriter.WriteAttributeString("NAME", function_block_name);


            foreach (LinguisticVariable linguisticVariable in this.linguisticVariableCollection)
            {
                if (linguisticVariable.Direction == "VAR_INPUT")
                {
                    xmlTextWriter.WriteStartElement("VAR_INPUT");
                    xmlTextWriter.WriteAttributeString("NAME", linguisticVariable.Name);
                    xmlTextWriter.WriteAttributeString("TYPE", linguisticVariable.Type);
                    if (linguisticVariable.Range_min.ToString() != "NaN")
                    {
                        xmlTextWriter.WriteAttributeString("RANGE ",
                            linguisticVariable.Range_min.ToString() + ".." +
                            linguisticVariable.Range_max.ToString());
                    }
                    xmlTextWriter.WriteEndElement();
                }
            }

            foreach (LinguisticVariable linguisticVariable in this.linguisticVariableCollection)
            {
                if (linguisticVariable.Direction == "VAR_OUTPUT")
                {
                    xmlTextWriter.WriteStartElement("VAR_OUTPUT");
                    xmlTextWriter.WriteAttributeString("NAME", linguisticVariable.Name);
                    xmlTextWriter.WriteAttributeString("TYPE", linguisticVariable.Type);
                    if (linguisticVariable.Range_min.ToString() != "NaN")
                    {
                        xmlTextWriter.WriteAttributeString("RANGE ",
                            linguisticVariable.Range_min.ToString() + ".." +
                            linguisticVariable.Range_max.ToString());
                    }
                    xmlTextWriter.WriteEndElement();
                }
            }

            foreach (LinguisticVariable linguisticVariable in this.linguisticVariableCollection)
            {
                if (linguisticVariable.Direction == "VAR")
                {
                    xmlTextWriter.WriteStartElement("VAR");
                    xmlTextWriter.WriteAttributeString("NAME", linguisticVariable.Name);
                    xmlTextWriter.WriteAttributeString("TYPE", linguisticVariable.Type);
                    if (!Double.IsNaN(linguisticVariable.Value))
                    {
                        xmlTextWriter.WriteAttributeString("VALUE", linguisticVariable.Value.ToString());
                    }
                    if (linguisticVariable.Range_min.ToString() != "NaN")
                    {
                        xmlTextWriter.WriteAttributeString("RANGE ",
                            linguisticVariable.Range_min.ToString() + ".." +
                            linguisticVariable.Range_max.ToString());
                    }
                    xmlTextWriter.WriteEndElement();
                }
            }

            // save FUZZIFY
            foreach (LinguisticVariable linguisticVariable in this.linguisticVariableCollection)
            {
                if (linguisticVariable.Direction == "VAR_INPUT")
                {
                    xmlTextWriter.WriteStartElement("FUZZIFY");
                    xmlTextWriter.WriteAttributeString("NAME", linguisticVariable.Name);
                    foreach (MembershipFunction membershipFunction in linguisticVariable.MembershipFunctionCollection)
                    {
                        xmlTextWriter.WriteStartElement("TERM");
                        xmlTextWriter.WriteAttributeString("NAME", membershipFunction.Name);
                        if (membershipFunction.Singleton_varName != String.Empty)
                        {
                            xmlTextWriter.WriteAttributeString("Singleton_varName", membershipFunction.Singleton_varName);
                        }
                        else if (membershipFunction.Singleton_value.ToString() != "NaN")
                        {
                            xmlTextWriter.WriteAttributeString("Singleton_value", membershipFunction.Singleton_value.ToString());
                        }
                        else // Points
                        {
                            xmlTextWriter.WriteAttributeString("POINTS", (membershipFunction.pointCollection.Count / 2).ToString());
                            for (int i = 0; i < membershipFunction.pointCollection.Count; i++)
                            {
                                string str = membershipFunction.pointCollection[i].P1_var;
                                if (str == String.Empty)
                                {
                                    str = membershipFunction.pointCollection[i].P1_val.ToString();
                                }
                                if (membershipFunction.pointCollection[i].P2_var != String.Empty)
                                {
                                    str += "," + membershipFunction.pointCollection[i].P2_var;
                                }
                                else if (!Double.IsNaN(membershipFunction.pointCollection[i].P2_val))
                                {
                                    str += "," + membershipFunction.pointCollection[i].P2_val.ToString();
                                }
                                xmlTextWriter.WriteStartElement("POINT");
                                xmlTextWriter.WriteAttributeString("POINT_str", str);
                                xmlTextWriter.WriteEndElement();
                            }
                        }
                        xmlTextWriter.WriteEndElement();
                    }
                    xmlTextWriter.WriteEndElement();
                }
            }

            // save DEFUZZIFY
            foreach (LinguisticVariable linguisticVariable in this.linguisticVariableCollection)
            {
                if (linguisticVariable.Direction == "VAR_OUTPUT")
                {
                    xmlTextWriter.WriteStartElement("DEFUZZIFY");
                    xmlTextWriter.WriteAttributeString("NAME", linguisticVariable.Name);
                    xmlTextWriter.WriteAttributeString("METHOD", linguisticVariable.Method);
                    if (linguisticVariable.Default_NC != String.Empty)
                    {
                        xmlTextWriter.WriteAttributeString("DEFAULT", linguisticVariable.Default_NC);
                    }
                    else
                    {
                        xmlTextWriter.WriteAttributeString("DEFAULT", linguisticVariable.Default_value.ToString());

                    }
                    if (linguisticVariable.Range_min.ToString() != "NaN")
                    {
                        xmlTextWriter.WriteAttributeString("RANGE ",
                            linguisticVariable.Range_min.ToString() + ".." +
                            linguisticVariable.Range_max.ToString());
                    }
                    foreach (MembershipFunction membershipFunction in linguisticVariable.MembershipFunctionCollection)
                    {
                        xmlTextWriter.WriteStartElement("TERM");
                        xmlTextWriter.WriteAttributeString("NAME", membershipFunction.Name);
                        if (membershipFunction.Singleton_varName != String.Empty)
                        {
                            xmlTextWriter.WriteAttributeString("Singleton_varName", membershipFunction.Singleton_varName);
                        }
                        else if (membershipFunction.Singleton_value.ToString() != "NaN")
                        {
                            xmlTextWriter.WriteAttributeString("Singleton_value", membershipFunction.Singleton_value.ToString());
                        }
                        else // Points
                        {
                            xmlTextWriter.WriteAttributeString("POINTS", (membershipFunction.pointCollection.Count / 2).ToString());
                            for (int i = 0; i < membershipFunction.pointCollection.Count; i++)
                            {
                                string str = membershipFunction.pointCollection[i].P1_var;
                                if (str == String.Empty)
                                {
                                    str = membershipFunction.pointCollection[i].P1_val.ToString();
                                }
                                if (membershipFunction.pointCollection[i].P2_var != String.Empty)
                                {
                                    str += "," + membershipFunction.pointCollection[i].P2_var;
                                }
                                else if (membershipFunction.pointCollection[i].P2_val.ToString() != "NaN")
                                {
                                    str += "," + membershipFunction.pointCollection[i].P2_val.ToString();
                                }
                                xmlTextWriter.WriteStartElement("POINT");
                                xmlTextWriter.WriteAttributeString("POINT_str", str);
                                xmlTextWriter.WriteEndElement();
                            }
                        }
                        xmlTextWriter.WriteEndElement();
                    }
                    xmlTextWriter.WriteEndElement();
                }
            }

            foreach (RuleBlockCollection ruleBlock in this.fuzzyRuleCollection)
            {
                xmlTextWriter.WriteStartElement("RULEBLOCK");
                xmlTextWriter.WriteAttributeString("NAME", ruleBlock.Name);
                xmlTextWriter.WriteAttributeString("Operdef", ruleBlock.OperatorDef);
                if (ruleBlock.ActivationMethod != string.Empty) xmlTextWriter.WriteAttributeString("ACT", ruleBlock.ActivationMethod);
                xmlTextWriter.WriteAttributeString("Accumulation", "ACCU : " + ruleBlock.AccumulationMethod);
                foreach (FuzzyRule fuzzyRule in ruleBlock)
                {
                    xmlTextWriter.WriteStartElement("RULE");
                    xmlTextWriter.WriteAttributeString("NUMBER", fuzzyRule.Name.ToString());
                    xmlTextWriter.WriteStartElement("Conditions");
                    xmlTextWriter.WriteAttributeString("Count", fuzzyRule.conditionCollection.Count.ToString());
                    foreach (SubCondition condition in fuzzyRule.conditionCollection)
                    {
                        string opr = condition.ConditionOper;
                        if (opr == String.Empty) xmlTextWriter.WriteStartElement("IF");
                        else xmlTextWriter.WriteStartElement(opr);
                        if (condition.NegatedVar != String.Empty) xmlTextWriter.WriteAttributeString("NegatedVar", condition.NegatedVar);
                        xmlTextWriter.WriteAttributeString("VarName", condition.VarName);
                        //if (condition.NegatedTerm != String.Empty) xmlTextWriter.WriteAttributeString("NegatedTerm", condition.NegatedTerm);
                        if (condition.TermName != String.Empty) xmlTextWriter.WriteAttributeString("IS",
                            (condition.NegatedTerm != String.Empty) ? (condition.NegatedTerm + " : " + condition.TermName) : condition.TermName);
                        xmlTextWriter.WriteEndElement();
                    }
                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.WriteStartElement("Conclusions");
                    xmlTextWriter.WriteAttributeString("Count", fuzzyRule.conclusionCollection.Count.ToString());
                    foreach (SubCondition conclusion in fuzzyRule.conclusionCollection)
                    {
                        string opr = conclusion.ConditionOper;
                        if (opr == String.Empty) xmlTextWriter.WriteStartElement("THEN");
                        else xmlTextWriter.WriteStartElement(opr);
                        //if (conclusion.NegatedVar != String.Empty) xmlTextWriter.WriteAttributeString("NegatedVar", conclusion.NegatedVar);
                        xmlTextWriter.WriteAttributeString("VarName", conclusion.VarName);
                        //if (condition.NegatedTerm != String.Empty) xmlTextWriter.WriteAttributeString("NegatedTerm", condition.NegatedTerm);
                        if (conclusion.TermName != String.Empty) xmlTextWriter.WriteAttributeString("IS", conclusion.TermName);
                        string weighting = conclusion.Weighting_factor;
                        if (weighting != "1.0000") xmlTextWriter.WriteAttributeString("WITH", weighting);
                        xmlTextWriter.WriteEndElement();
                    }
                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.WriteEndElement();
                }
                xmlTextWriter.WriteEndElement();

            }

            xmlTextWriter.WriteEndDocument();
            xmlTextWriter.Close();
        }



        /// <summary>
        /// Sets the FilePath property and loads a project from a FCL-like XML file.
        /// </summary>
        /// <param name="path">Path of the source file.</param>
        public void Load(string path)
        {
            this.FilePath = path;
            function_block_name = String.Empty;
            linguisticVariableCollection = new LinguisticVariableCollection();
            fuzzyRuleCollection = new FuzzyRuleCollection();
            this.Load();
        }

        /// <summary>
        /// Loads a project from a FCL-like XML file.
        /// </summary>
        public void Load()
        {
            if (this.filePath == String.Empty)
                throw new Exception("FilePath not set");

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(this.filePath);

            XmlNodeList elemList = xmlDocument.GetElementsByTagName("FUNCTION_BLOCK");
            Function_block_name = elemList[0].Attributes.GetNamedItem("NAME").Value;

            this.LinguisticVariableCollection.Clear();
            foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("VAR_INPUT"))
            {
                string lvName = xmlNode.Attributes["NAME"].InnerText;
                this.LinguisticVariableCollection.Add(new LinguisticVariable(lvName, "VAR_INPUT", xmlNode.Attributes["TYPE"].InnerText));
                LinguisticVariable lvar = this.LinguisticVariableCollection.Find(lvName);
                if (lvar != null)
                {
                    lvar.MembershipFunctionCollection.Clear();
                }

            }

            foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("VAR_OUTPUT"))
            {
                string lvName = xmlNode.Attributes["NAME"].InnerText;
                LinguisticVariable var = new LinguisticVariable(lvName, "VAR_OUTPUT", xmlNode.Attributes["TYPE"].InnerText);
                this.LinguisticVariableCollection.Add(var);
                LinguisticVariable lvar = this.LinguisticVariableCollection.Find(lvName);
                if (lvar != null)
                {
                    XmlAttribute range = xmlNode.Attributes["RANGE"];
                    if (range != null)
                    {
                        string str = range.Value;
                        Regex regex = new Regex(@"([ \r\n\t\v]|\:\=|[:,;=\(\)]|\.\.|_?\\w+_?\\w*|-?\\d+.?\\d*)");
                        string[] toks = regex.Split(str).Where(s => s != String.Empty).ToArray();
                        if (toks.Length != 3) throw new Exception(" range format " + str + " for VAR_OUTPUT " + lvName + " is unrecognized !");
                        lvar.Range_min = System.Convert.ToDouble(toks[0]);
                        lvar.Range_max = System.Convert.ToDouble(toks[2]);
                        if (lvar.Range_min > lvar.Range_max) throw new Exception(" range format " + str + " for VAR_OUTPUT " + lvName + " order is incorrect !");
                    }
                    lvar.MembershipFunctionCollection.Clear();
                }
            }

            foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("VAR"))
            {
                string lvName = xmlNode.Attributes["NAME"].InnerText;
                LinguisticVariable lvar = new LinguisticVariable(lvName, "VAR", xmlNode.Attributes["TYPE"].InnerText);
                this.LinguisticVariableCollection.Add(lvar);
                XmlNode node = xmlNode.Attributes.GetNamedItem("VALUE");
                if (node != null)
                {
                    string lvar_value = xmlNode.Attributes.GetNamedItem("VALUE").Value;
                    lvar.Value = System.Convert.ToDouble(lvar_value);
                }
                lvar.MembershipFunctionCollection.Clear();
            }

            foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("FUZZIFY"))
            {
                string varName = xmlNode.Attributes.GetNamedItem("NAME").Value;
                LinguisticVariable lvar = this.LinguisticVariableCollection.Find(varName);
                if (lvar != null)
                {
                    if (lvar.Direction == "VAR" || lvar.Direction == "VAR_INPUT")
                    {
                        foreach (XmlNode xmlNode2 in xmlNode.SelectNodes("TERM"))
                        {
                            string tName = xmlNode2.Attributes.GetNamedItem("NAME").Value;
                            DotFuzzy.FuzzyEngine engine = this;
                            lvar.MembershipFunctionCollection.Add(new MembershipFunction(tName, ref engine, ref lvar));
                            MembershipFunction mFunc = lvar.MembershipFunctionCollection.Find(tName);
                            mFunc.pointCollection.Clear();
                            if (mFunc == null) throw new Exception("Membership function not found for TERM " + tName + " !");
                            XmlNode node = xmlNode2.Attributes.GetNamedItem("Singleton_value");
                            XmlNode node2 = xmlNode2.Attributes.GetNamedItem("Singleton_varName");
                            if (node != null)
                            {
                                string valStr = node.Value;
                                mFunc.Singleton_value = System.Convert.ToDouble(valStr);
                            }
                            else if (node2 != null)
                            {
                                string valStr = node2.Value;
                                mFunc.Singleton_varName = valStr;
                            }
                            else
                            {
                                foreach (XmlNode xmlNode3 in xmlNode2.SelectNodes("POINT"))
                                {
                                    string pStr = xmlNode3.Attributes.GetNamedItem("POINT_str").Value;
                                    char[] charSeparators = new char[] { ',' };
                                    String[] vals = pStr.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                                    bool isVar = char.IsLetter(vals[0], 0) || vals[0].StartsWith("_");
                                    Point pnt = new Point(1, (!isVar) ? System.Convert.ToDouble(vals[0]) : Double.NaN, (isVar) ? vals[0] : "");
                                    if (vals.GetLength(0) > 1)
                                    {
                                        isVar = char.IsLetter(vals[1], 0) || vals[1].StartsWith("_");
                                        if (isVar) pnt.P2_var = vals[1];
                                        else pnt.P2_val = System.Convert.ToDouble(vals[1]);
                                    }
                                    mFunc.pointCollection.Add(pnt);
                                }
                            }
                        }
                    }
                    else throw new Exception("cannot find VAR or VAR_INPUT for LinguisticVariable " + varName + " !");
                }
                else throw new Exception("cannot find VAR or VAR_INPUT for LinguisticVariable " + varName + " !");
            }

            foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("DEFUZZIFY"))
            {
                string varName = xmlNode.Attributes.GetNamedItem("NAME").Value;
                LinguisticVariable lvar = this.LinguisticVariableCollection.Find(varName);
                if (lvar != null)
                {
                    if (lvar.Direction == "VAR_OUTPUT")
                    {
                        lvar.Method = xmlNode.Attributes.GetNamedItem("METHOD").Value;
                        string defaultStr = xmlNode.Attributes.GetNamedItem("DEFAULT").Value;
                        bool isVar = char.IsLetter(defaultStr, 0) || defaultStr.StartsWith("_");
                        if (isVar) lvar.Default_NC = defaultStr;
                        else lvar.Default_value = System.Convert.ToDouble(defaultStr);
                        foreach (XmlNode xmlNode2 in xmlNode.SelectNodes("TERM"))
                        {
                            string tName = xmlNode2.Attributes.GetNamedItem("NAME").Value;
                            DotFuzzy.FuzzyEngine engine = this;
                            lvar.MembershipFunctionCollection.Add(new MembershipFunction(tName, ref engine, ref lvar));
                            MembershipFunction mFunc = lvar.MembershipFunctionCollection.Find(tName);
                            if (mFunc == null) throw new Exception("Membership function not found for TERM " + tName + " !");
                            mFunc.pointCollection.Clear();
                            XmlNode node = xmlNode2.Attributes.GetNamedItem("Singleton_value");
                            XmlNode node2 = xmlNode2.Attributes.GetNamedItem("Singleton_varName");
                            if (node != null)
                            {
                                string valStr = node.Value;
                                mFunc.Singleton_value = System.Convert.ToDouble(valStr);
                            }
                            else if (node2 != null)
                            {
                                string valStr = node2.Value;
                                mFunc.Singleton_varName = valStr;
                            }
                            else
                            {
                                foreach (XmlNode xmlNode3 in xmlNode2.SelectNodes("POINT"))
                                {
                                    string pStr = xmlNode3.Attributes.GetNamedItem("POINT_str").Value;
                                    char[] charSeparators = new char[] { ',' };
                                    String[] vals = pStr.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                                    isVar = char.IsLetter(vals[0], 0) || vals[0].StartsWith("_");
                                    Point pnt = new Point(1, (!isVar) ? System.Convert.ToDouble(vals[0]) : Double.NaN, (isVar) ? vals[0] : "");
                                    if (vals.GetLength(0) > 1)
                                    {
                                        isVar = char.IsLetter(vals[1], 0) || vals[1].StartsWith("_");
                                        if (isVar) pnt.P2_var = vals[1];
                                        else pnt.P2_val = System.Convert.ToDouble(vals[1]);
                                    }
                                    mFunc.pointCollection.Add(pnt);
                                }
                            }
                        }
                    }
                    else throw new Exception("cannot find VAR_OUTPUT for LinguisticVariable " + varName + " !");
                }
                else throw new Exception("cannot find VAR_OUTPUT for LinguisticVariable " + varName + " !");
            }

            foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("RULEBLOCK"))
            {
                XmlNode xmlNode3 = xmlNode.Attributes["NAME"];
                if (xmlNode3 == null) throw new Exception("RULEBLOCK Name not found !");
                string blockName = xmlNode3.Value;
                xmlNode3 = xmlNode.Attributes["Operdef"];
                if (xmlNode3 == null) throw new Exception("Operdef missing in RULEBLOCK " + blockName + " !");
                RuleBlockCollection ruleBlock = new RuleBlockCollection(blockName);
                ruleBlock.OperatorDef = xmlNode3.Value;
                xmlNode3 = xmlNode.Attributes["ACT"];
                if (xmlNode3 != null) ruleBlock.ActivationMethod = xmlNode3.Value;
                xmlNode3 = xmlNode.Attributes["Accumulation"];
                if (xmlNode3 == null) throw new Exception("Accumulation missing in RULEBLOCK " + blockName + " !");
                string accuStr = xmlNode3.Value;
                char[] charSeparators = new char[] { ':', ' ' };
                String[] vals = accuStr.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                if (vals.Length != 2) throw new Exception("Accumulation string '" + accuStr + "' in RULEBLOCK " + blockName + " does not contain 'ACCU : accumulation_method' !");
                ruleBlock.AccumulationMethod = vals[1];
                this.fuzzyRuleCollection.Add(ruleBlock);
                foreach (XmlNode xmlNode2 in xmlNode.SelectNodes("RULE"))
                {
                    xmlNode3 = xmlNode2.Attributes["NUMBER"];
                    if (xmlNode3 == null) throw new Exception("Rule NUMBER attribute not found in RULEBLOCK " + blockName + " !");
                    string ruleNum = xmlNode3.Value;
                    ruleBlock.Add(new FuzzyRule(ruleNum));
                    FuzzyRule fuzzyRule = ruleBlock.Find(ruleNum);
                    if (fuzzyRule != null)
                    {
                        XmlNode condition = xmlNode2.SelectSingleNode("Conditions");
                        if (condition == null) throw new Exception("Condition in RULEBLOCK " + blockName + " Rule " + ruleNum + " is missing !");
                        foreach (XmlNode xmlNode4 in condition.ChildNodes)
                        {
                            if (xmlNode4 != null)
                            {
                                string condOp = xmlNode4.Name;
                                if (condOp == "IF" || condOp == "AND" || condOp == "OR")
                                {
                                    SubCondition subCond = new SubCondition();
                                    subCond.ConditionOper = (condOp == "IF") ? String.Empty : condOp;
                                    XmlNode xmlNode6 = xmlNode4.Attributes.GetNamedItem("NegatedVar");
                                    if (xmlNode6 != null)
                                    {
                                        subCond.NegatedVar = xmlNode6.Value;
                                    }
                                    xmlNode6 = xmlNode4.Attributes.GetNamedItem("VarName");
                                    if (xmlNode6 != null)
                                    {
                                        subCond.VarName = xmlNode6.Value;
                                    }
                                    xmlNode6 = xmlNode4.Attributes.GetNamedItem("IS");
                                    if (xmlNode6 != null)
                                    {
                                        string termStr = xmlNode6.Value;
                                        if (termStr.Length < 1) throw new Exception("Term string in RULEBLOCK " + blockName + " Rule " + ruleNum + " is missing !");
                                        if (termStr.Contains(":"))
                                        {
                                            char[] charSeparators2 = new char[] { ':', ' ' };
                                            String[] vals2 = termStr.Split(charSeparators2, StringSplitOptions.RemoveEmptyEntries);
                                            if (vals2.Length != 2) throw new Exception("Term string '" + termStr + "' in RULEBLOCK " + blockName + " Rule " + ruleNum + " does not contain 'NOT : termName' !");
                                            subCond.NegatedTerm = vals2[0];
                                            subCond.TermName = vals2[1];
                                        }
                                        else
                                        {
                                            if (termStr == String.Empty) throw new Exception("Term string '" + termStr + "' in RULEBLOCK " + blockName + " Rule " + ruleNum + " does not contain termName !");
                                            subCond.NegatedTerm = string.Empty;
                                            subCond.TermName = termStr;
                                        }
                                    } // if (xmlNode6 != null)
                                    fuzzyRule.conditionCollection.Add(subCond);
                                } // if (xmlNode5 != null)
                                else throw new Exception("IF not found for RULBLOCK " + blockName + " Rule " + ruleNum + " !");
                            }
                            else throw new Exception("Condition not found for RULBLOCK " + blockName + " Rule " + ruleNum + " !");
                        }

                        XmlNode conclusions = xmlNode2.SelectSingleNode("Conclusions");
                        if (conclusions == null) throw new Exception("Conclusion in RULEBLOCK " + blockName + " Rule " + ruleNum + " is missing !");
                        XmlNodeList nodes = conclusions.SelectNodes("THEN");
                        if (nodes.Count < 1) throw new Exception("No Conclusions in RULEBLOCK " + blockName + " Rule " + ruleNum + " listed !");
                        foreach (XmlNode xmlNode6 in nodes)
                        {
                            XmlNode xmlNode7 = xmlNode6.Attributes.GetNamedItem("VarName");
                            if (xmlNode7 == null) throw new Exception("VarName missing in Conclusion in RULEBLOCK " + blockName + " Rule " + ruleNum + " !");
                            SubCondition conclusion = new SubCondition();
                            conclusion.VarName = xmlNode7.Value;
                            XmlNode xmlNode8 = xmlNode6.Attributes.GetNamedItem("IS");
                            if (xmlNode8 != null) conclusion.TermName = xmlNode8.Value;
                            xmlNode8 = xmlNode6.Attributes.GetNamedItem("WITH");
                            if (xmlNode8 != null) conclusion.Weighting_factor = xmlNode8.Value;
                            fuzzyRule.conclusionCollection.Add(conclusion);
                        } // foreach (XmlNode xmlNode6 in nodes)
                    } // if (fuzzyRule != null)
                } // foreach (XmlNode xmlNode2 in xmlNode.SelectNodes("RULE"))
            } // foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("RULEBLOCK"))
            Console.WriteLine(" Load complete successfully.");

        }

        #endregion
    }
}
