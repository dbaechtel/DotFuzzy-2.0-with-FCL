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
    /// Represents a rule.
    /// </summary>
    public class FuzzyRule
    {
        #region Private Properties

        private string name = String.Empty;
        private string text = String.Empty;
        private double value = 0;
        public ConditionCollection conditionCollection = new ConditionCollection();
        public ConditionCollection conclusionCollection = new ConditionCollection();

        #endregion

        #region Private Methods

        private string Validate(string text)
        {
            int count = 0;
            int position = text.IndexOf("(");
            string[] tokens = text.Replace("(", "").Replace(")", "").Split();

            while (position >= 0)
            {
                count++;
                position = text.IndexOf("(", position + 1);
            }

            position = text.IndexOf(")");
            while (position >= 0)
            {
                count--;
                position = text.IndexOf(")", position + 1);
            }

            if (count > 0)
                throw new Exception("missing right parenthesis: " + text);
            else if (count < 0)
                throw new Exception("missing left parenthesis: " + text);

            if (tokens[0] != "IF")
                throw new Exception("'IF' not found: " + text);

            if (tokens[tokens.Length - 4] != "THEN")
                throw new Exception("'THEN' not found: " + text);

            if (tokens[tokens.Length - 2] != "IS")
                throw new Exception("'IS' not found: " + text);

            for (int i = 2; i < (tokens.Length - 5); i = i + 2)
            {
                if ((tokens[i] != "IS") && (tokens[i] != "AND") && (tokens[i] != "OR"))
                    throw new Exception("Syntax error: " + tokens[i]);
            }

            return text;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FuzzyRule()
        {
        }

        /// <param name="name">The name of the rule.</param>
        public FuzzyRule(string name)
        {
            this.Name = name;
        }

        /// <param name="name">The name of the rule.</param>
        /// <param name="text">The text of the rule.</param>
        public FuzzyRule(string name, string text)
        {
            this.Name = name;
            this.Text = text;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The Name of the rule.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
 
        /// <summary>
        /// The text of the rule.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = Validate(value); }
        }
 
        /// <summary>
        /// The value of the rule after the evaluation process.
        /// </summary>
        public double Value
        {
            get { return value; }
            set { this.value = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the conditions of the rule.
        /// The part of the rule between IF and THEN.
        /// </summary>
        /// <returns>The conditions of the rule.</returns>
        public string Conditions()
        {
            return this.text.Substring(this.text.IndexOf("IF ") + 3, this.text.IndexOf(" THEN") - 3);
        }

        #endregion
    }
}
