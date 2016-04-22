using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DotFuzzy
{

    /// <summary>
    /// Represents a collection of rules.
    /// </summary>
    public class RuleBlockCollection : Collection<FuzzyRule>
    {
        private string name = String.Empty;
        private string operatorDef = String.Empty;
        private string activationMethod = String.Empty;
        private string accumulationMethod = String.Empty;

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RuleBlockCollection()
        {
        }

        /// <param name="name">The name of the RuleBlockCollection.</param>
        public RuleBlockCollection(string name)
        {
            this.Name = name;
        }

        #endregion


        /// <summary>
        /// The operatorDef of the RuleBlock.
        /// </summary>
        public string OperatorDef
        {
            get { return operatorDef; }
            set { operatorDef = value; }
        }

        /// <summary>
        /// The activationMethod of the RuleBlock.
        /// </summary>
        public string ActivationMethod
        {
            get { return activationMethod; }
            set { activationMethod = value; }
        }

        /// <summary>
        /// The accumulationMethod of the RuleBlock.
        /// </summary>
        public string AccumulationMethod
        {
            get { return accumulationMethod; }
            set { accumulationMethod = value; }
        }

        /// <summary>
        /// The Name of the RuleBlock.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Finds a fuzzy rule in a collection.
        /// </summary>
        /// <param name="ruleName">Rule name.</param>
        /// <returns>The FuzzyRule, if founded.</returns>
        public FuzzyRule Find(string ruleName)
        {
            FuzzyRule fuzzyRule = null;

            foreach (FuzzyRule rule in this)
            {
                if (rule.Name == ruleName)
                {
                    fuzzyRule = rule;
                    break;
                }
            }

            if (fuzzyRule == null)
                throw new Exception("FuzzyRule not found: " + ruleName);
            else
                return fuzzyRule;
        }

    }
}
