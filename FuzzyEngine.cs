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
        private LinguisticVariableCollection linguisticVariableCollection = new LinguisticVariableCollection();
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

        void ReadVariable(XmlNode xmlNode)
        {
            LinguisticVariable linguisticVariable = this.linguisticVariableCollection.Find(xmlNode.Attributes["NAME"].InnerText);

            foreach (XmlNode termNode in xmlNode.ChildNodes)
            {
                string[] points = termNode.Attributes["POINTS"].InnerText.Split();
                linguisticVariable.MembershipFunctionCollection.Add(new MembershipFunction(
                    termNode.Attributes["NAME"].InnerText));
            }
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
        /// Calculates the defuzzification value with the CoG (Center of Gravity) technique.
        /// </summary>
        public void Defuzzify()
        {

            // Reset all VAR_OUTPUT membershipFunction values
            foreach (LinguisticVariable var in this.linguisticVariableCollection)
            {
                if (var.Direction == "VAR_OUTPUT")
                {
                    double numerator = 0;
                    double denominator = 0;
                    foreach (MembershipFunction membershipFunction in var.MembershipFunctionCollection)
                    {
                        membershipFunction.Value = 0;
                    }

                    foreach (RuleBlockCollection ruleBlock in this.fuzzyRuleCollection)
                    {
                        foreach (FuzzyRule fuzzyRule in ruleBlock)
                        {
                            fuzzyRule.Value = Parse(fuzzyRule.Conditions());

                            string[] tokens = fuzzyRule.Text.Split();
                            MembershipFunction membershipFunction = var.MembershipFunctionCollection.Find(tokens[tokens.Length - 1]);

                            if (fuzzyRule.Value > membershipFunction.Value)
                                membershipFunction.Value = fuzzyRule.Value;
                        }
                    }

                    foreach (MembershipFunction membershipFunction in var.MembershipFunctionCollection)
                    {
                        numerator += membershipFunction.Centorid() * membershipFunction.Area();
                        denominator += membershipFunction.Area();
                        if (denominator == 0) throw new Exception(membershipFunction.Name + " area is zero. Divison by zero !");
                    }
                    var.Value += numerator / denominator;
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
                            for (int i = 0; i < membershipFunction.pointCollection.Count; i += 2)
                            {
                                string str = membershipFunction.pointCollection[i].P1_var;
                                if (str == String.Empty)
                                {
                                    str = membershipFunction.pointCollection[i].P1_val.ToString();
                                }
                                if (membershipFunction.pointCollection[i + 1].P2_var != String.Empty)
                                {
                                    str += "," + membershipFunction.pointCollection[i + 1].P2_var;
                                }
                                else if (membershipFunction.pointCollection[i + 1].P2_val.ToString() != "NaN")
                                {
                                    str += "," + membershipFunction.pointCollection[i + 1].P2_val.ToString();
                                }
                                xmlTextWriter.WriteStartElement("POINT", str);
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
                            for (int i = 0; i < membershipFunction.pointCollection.Count; i += 2)
                            {
                                string str = membershipFunction.pointCollection[i].P1_var;
                                if (str == String.Empty)
                                {
                                    str = membershipFunction.pointCollection[i].P1_val.ToString();
                                }
                                if (membershipFunction.pointCollection[i + 1].P2_var != String.Empty)
                                {
                                    str += "," + membershipFunction.pointCollection[i + 1].P2_var;
                                }
                                else if (membershipFunction.pointCollection[i + 1].P2_val.ToString() != "NaN")
                                {
                                    str += "," + membershipFunction.pointCollection[i + 1].P2_val.ToString();
                                }
                                xmlTextWriter.WriteStartElement("POINT", str);
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
                if(ruleBlock.ActivationMethod != string.Empty) xmlTextWriter.WriteAttributeString("ACT", ruleBlock.ActivationMethod);
                xmlTextWriter.WriteAttributeString("ACCU", ruleBlock.AccumulationMethod);
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
                        if (condition.NegatedTerm != String.Empty) xmlTextWriter.WriteAttributeString("NegatedTerm", condition.NegatedTerm);
                        xmlTextWriter.WriteAttributeString("IS", condition.TermName);
                        xmlTextWriter.WriteEndElement();
                    }
                    xmlTextWriter.WriteEndElement();
                    xmlTextWriter.WriteStartElement("Conclusions");
                    xmlTextWriter.WriteAttributeString("Count", fuzzyRule.conclusionCollection.Count.ToString());
                    double weighting = fuzzyRule.conclusionCollection[0].Weighting_factor;
                    if (weighting.ToString() != "NaN")
                    {
                        if(weighting < 1.0) xmlTextWriter.WriteAttributeString("WITH", fuzzyRule.conclusionCollection[0].Weighting_factor.ToString());
                    }
                    foreach (SubCondition conclusion in fuzzyRule.conclusionCollection)
                    {
                        string opr = conclusion.ConditionOper;
                        if (opr == String.Empty) xmlTextWriter.WriteStartElement("THEN");
                        else xmlTextWriter.WriteStartElement(opr);
                        //if (conclusion.NegatedVar != String.Empty) xmlTextWriter.WriteAttributeString("NegatedVar", conclusion.NegatedVar);
                        xmlTextWriter.WriteAttributeString("VarName", conclusion.VarName);
                        //if (condition.NegatedTerm != String.Empty) xmlTextWriter.WriteAttributeString("NegatedTerm", condition.NegatedTerm);
                        xmlTextWriter.WriteAttributeString("IS", conclusion.TermName);
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

            foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("VAR_INPUT"))
            {
                this.LinguisticVariableCollection.Add(new LinguisticVariable(xmlNode.Attributes["NAME"].InnerText, "VAR_INPUT", xmlNode.Attributes["TYPE"].InnerText));
            }

            foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("VAR_OUTPUT"))
            {
                LinguisticVariable var = new LinguisticVariable(xmlNode.Attributes["NAME"].InnerText, "VAR_OUTPUT", xmlNode.Attributes["TYPE"].InnerText);
                this.LinguisticVariableCollection.Add(var);
            }

            foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("FUZZIFY"))
            {
                ReadVariable(xmlNode);
            }

            ReadVariable(xmlDocument.GetElementsByTagName("DEFUZZIFY")[0]);

            foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("RULBLOCK"))
            {
                string blockName = xmlNode.Attributes["NAME"].InnerText;
                this.fuzzyRuleCollection.Add(new RuleBlockCollection(blockName));
                RuleBlockCollection ruleBlock = this.fuzzyRuleCollection.Find(blockName);
                foreach (XmlNode xmlNode2 in xmlDocument.GetElementsByTagName("RULE"))
                {
                    ruleBlock.Add(new FuzzyRule(xmlNode2.Attributes["NUMBER"].InnerText, xmlNode2.Attributes["TEXT"].InnerText));
                }
            }

        }

        #endregion
    }
}
