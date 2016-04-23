using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotFuzzy
{
    /// <summary>
    /// Represents a SubCondition.
    /// </summary>
    public class SubCondition
    {
        private string negatedVar = String.Empty;
        private string varName = String.Empty;
        private string negatedTerm = String.Empty;
        private string termName = String.Empty;
        private string conditionOper = String.Empty;
        private string weighting_factor = "1.0000";
        private double ruleValue = Double.NaN;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SubCondition()
        {
        }

         /// <summary>
        /// The NegatedVar of the rule.
        /// </summary>
        public string NegatedVar
        {
            get { return negatedVar; }
            set { negatedVar = value; }
        }

        /// <summary>
        /// The VarName of the rule.
        /// </summary>
        public string VarName
        {
            get { return varName; }
            set { varName = value; }
        }

        /// <summary>
        /// The NegatedTerm of the rule.
        /// </summary>
        public string NegatedTerm
        {
            get { return negatedTerm; }
            set { negatedTerm = value; }
        }

        /// <summary>
        /// The TermName of the rule.
        /// </summary>
        public string TermName
        {
            get { return termName; }
            set { termName = value; }
        }

        /// <summary>
        /// The conditionOper of the rule.
        /// </summary>
        public string ConditionOper
        {
            get { return conditionOper; }
            set { conditionOper = value; }
        }

        /// <summary>
        /// The Weighting_factor of the rule.
        /// </summary>
        public string Weighting_factor
        {
            get { return weighting_factor; }
            set { weighting_factor = value; }
        }

        /// <summary>
        /// The Value of the rule.
        /// </summary>
        public double Value
        {
            get { return ruleValue; }
            set { ruleValue = value; }
        }

    }
}
