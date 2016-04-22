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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DotFuzzy
{
    /// <summary>
    /// ParseFuzzyFile.
    /// </summary>
    public class ParseFuzzyFile
    {
        /// <summary>
        /// The path of the FCL-like XML file in which save the project.
        /// </summary>
        public string FilePath = "";
        private string InputBuffer = "";
        private int numComments = 0;
        private int state = 0;
        private int retState = 0;
        private int fbState = 0;
        RuleBlockCollection ruleBlock = null;
        //ConditionCollection condCollection = null;
        FuzzyRule fuzzyRule = null;
        SubCondition conclusion = null;
        private string conditionOper = String.Empty;
        private string blockName = String.Empty;
        private int rule_number = int.MinValue;
        private int numToken = 0;
        string[] tokens;
        public FuzzyEngine engine;
        LinguisticVariable linguisticVar;
        MembershipFunction mem;
        private string term_name;
        private string[] keywords = {
            "ACCU",
            "ACT",
            "AND",
            "ASUM",
            "BDIF",
            "BSUM",
            "COA",
            "COG",
            "COGS",
            "DEFAULT",
            "DEFUZZIFY",
            "END_DEFUZZIFY",
            "END_FUNCTION_BLOCK",
            "END_FUZZIFY",
            "END_OPTIONS",
            "END_RULEBLOCK",
            "END_VAR",
            "FUNCTION_BLOCK",
            "FUZZIFY",
            "IF",
            "INT",
            "IS",
            "LM",
            "MAX",
            "METHOD",
            "MIN",
            "NC",
            "NOT",
            "NSUM",
            "OPTIONS",
            "OR",
            "PROD",
            "RANGE",
            "REAL",
            "RM",
            "RULE",
            "RULEBLOCK",
            "TERM",
            "THEN",
            "VAR",
            "VAR_INPUT",
            "VAR_OUT[UT",
            "WITH"
        };

        /// <summary>
        /// Sets the FilePath property and loads a project from a FCL-like XML file.
        /// </summary>
        /// <param name="path">Path of the source file.</param>
        //[DllExport("add", CallingConvention = CallingConvention.Cdecl)]
        public void ParseFile(string path)
        {
            this.FilePath = path;
            engine = new FuzzyEngine();
            this.ParseFile();
        }

        /// <summary>
        /// Sets the FilePath property and loads a project from a FCL-like XML file.
        /// </summary>
        private void ParseFile()
        {

            if (FilePath == String.Empty)
                throw new Exception("ParseFile not set");

            try
            {
                if (!(File.Exists(FilePath)))
                {
                    throw new Exception("ParseFile not set!");
                }
                FileStream fs = new FileStream(FilePath, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                InputBuffer = sr.ReadToEnd();
                ParseText();
                sr.Close();
                fs.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
                engine = null;
            }

        }

        private void skipWhiteSpace()
        {
            skipWhiteSpace(false);
        }

        private void skipWhiteSpace(bool eof)
        {
            // skip whitespace characters
            if (numToken < tokens.Count())
            {
                while (string.IsNullOrWhiteSpace(tokens[numToken]))
                {
                    numToken++;
                    if (numToken >= tokens.Count())
                    {
                        if (!eof) throw new Exception("unexpected end of file !");
                        return;
                    }
                }

            }
            else if (!eof) throw new Exception("unexpected end of file !");
        }

        private bool isIdentifier()
        {
            skipWhiteSpace();
            string tok = tokens[numToken];
            if (keywords.Contains(tok)) throw new Exception("unexpected keyword " + tok + " found looking for identifier !");
            if (char.IsLetter(tok, 0) || tok.StartsWith("_")) return true;
            else return false;
        }

        private void ParseText()
        {

            if (InputBuffer == String.Empty)
                throw new Exception("ParseFile is Empty!");

            try
            {
                state = 0;
                //Remove comments
                while (InputBuffer.Contains("(*"))
                {
                    int startComment = InputBuffer.IndexOf("(*");
                    if (startComment >= 0)
                    {
                        int endComment = InputBuffer.IndexOf("*)");
                        int length = endComment - startComment;
                        if (endComment >= 0 && length > 0)
                        {
                            InputBuffer = InputBuffer.Remove(startComment, length);
                            numComments++;
                        }
                        else { throw new Exception("Unmatched comment delimiters!"); }
                    }
                }
                if (InputBuffer.Contains("*)")) throw new Exception("Unmatched comment delimiter *) !");
                state = 1;
                Regex regex = new Regex(@"([ \r\n\t\v]|[:,;=\(\)]|_?\\w+_?\\w*|-?\\d+.?\\d*)");
                tokens = regex.Split(InputBuffer).Where(s => s != String.Empty).ToArray();
                //Char[] delimeters = { ' ', '\t', '\r', '\n', ':', '(', ',', ')', ';', '=', ',' };
                //tokens = InputBuffer.Split(delimeters);
                if (tokens.Count() <= 0) throw new Exception("no tokens found in ParseFile !");
                skipWhiteSpace();
                if (tokens[numToken] == "FUNCTION_BLOCK")
                {
                    state = 2;
                    numToken++;
                    while (state > 0)
                    {
                        Console.WriteLine("state = " + state);
                        skipWhiteSpace();

                        switch (state)
                        {
                            case 2: // after FUNCTION_BLOCK
                                // get function_block_name
                                if (isIdentifier())
                                {
                                    engine.Function_block_name = tokens[numToken];
                                    numToken++;
                                    if (numToken >= tokens.Count()) throw new Exception("unexpected end of file !");
                                    state = 3;
                                }
                                else throw new Exception("Suntax error: Function-block name not found !");
                                break;
                            case 3: // inside function block
                                switch (tokens[numToken])
                                {
                                    case "END_FUNCTION_BLOCK":
                                        numToken++;
                                        state = 0;
                                        break;
                                    case "VAR_INPUT":
                                        numToken++;
                                        state = 4;
                                        break;
                                    case "VAR_OUTPUT":
                                        numToken++;
                                        state = 5;
                                        break;
                                    case "VAR":
                                        numToken++;
                                        state = 6;
                                        break;
                                    default: // expecting function_block_body
                                        state = 7;
                                        break;
                                }
                                break;
                            case 4: // after VAR_INPUT
                                if (tokens[numToken] == "END_VAR")
                                {
                                    numToken++;
                                    state = 3;
                                }
                                else // expecting list of VAR_INPUT
                                {
                                    if (isIdentifier()) // get variable name
                                    {
                                        string varName = tokens[numToken];
                                        numToken++;
                                        skipWhiteSpace();
                                        // get colon
                                        if (numToken >= tokens.Count()) throw new Exception("unexpected end of file !");
                                        if (tokens[numToken] != ":") throw new Exception("colon missing  following VAR_INPUT " + varName + " !");
                                        numToken++;
                                        skipWhiteSpace();
                                        if (numToken >= tokens.Count()) throw new Exception("unexpected end of file !");
                                        switch (tokens[numToken]) // get variable type
                                        {
                                            case "REAL":
                                                engine.LinguisticVariableCollection.Add(new LinguisticVariable(varName, "VAR_INPUT", "REAL"));
                                                break;
                                            case "INT":
                                                engine.LinguisticVariableCollection.Add(new LinguisticVariable(varName, "VAR_INPUT", "INT"));
                                                break;
                                            default:
                                                throw new Exception("REAL or INT type missing in VAR_INPUT " + varName + " !");
                                        }
                                        numToken++;
                                        skipWhiteSpace();
                                        if (tokens[numToken] != ";") throw new Exception("semicolon missing following VAR_INPUT " + varName + " !");
                                        numToken++;
                                    }
                                    else // no VAR_INPUT variable name
                                    {
                                        throw new Exception("name of VAR_INPUT variable missing !");
                                    }
                                }
                                break;
                            case 5: // after VAR_OUTPUT
                                if (tokens[numToken] == "END_VAR")
                                {
                                    numToken++;
                                    state = 3;
                                }
                                else // expecting list of VAR_OUTPUT
                                {
                                    if (isIdentifier()) // get variable name
                                    {
                                        string varName = tokens[numToken];
                                        numToken++;
                                        skipWhiteSpace();
                                        // get colon
                                        if (numToken >= tokens.Count()) throw new Exception("unexpected end of file !");
                                        if (tokens[numToken] != ":") throw new Exception("colon missing  following VAR_OUTPUT " + varName + " !");
                                        numToken++;
                                        skipWhiteSpace();
                                        if (numToken >= tokens.Count()) throw new Exception("unexpected end of file !");
                                        switch (tokens[numToken]) // get variable type
                                        {
                                            case "REAL":
                                                engine.LinguisticVariableCollection.Add(new LinguisticVariable(varName, "VAR_OUTPUT", "REAL"));
                                                break;
                                            case "INT":
                                                engine.LinguisticVariableCollection.Add(new LinguisticVariable(varName, "VAR_OUTPUT", "INT"));
                                                break;
                                            default:
                                                throw new Exception("REAL or INT type missing in VAR_OUTPUT " + varName + " !");
                                        }
                                        numToken++;
                                        skipWhiteSpace();
                                        if (tokens[numToken] != ";") throw new Exception("semicolon missing following VAR_INPUT " + varName + " !");
                                        numToken++;
                                    }
                                    else // no VAR_OUTPUT variable name
                                    {
                                        throw new Exception("name of VAR_OUTPUT variable missing !");
                                    }
                                }
                                break;
                            case 6: // after VAR keyword
                                if (tokens[numToken] == "END_VAR")
                                {
                                    numToken++;
                                    state = 3;
                                }
                                else // expecting list of VAR
                                {
                                    if (isIdentifier()) // get variable name
                                    {
                                        string varName = tokens[numToken];
                                        numToken++;
                                        skipWhiteSpace();
                                        // get colon
                                        if (numToken >= tokens.Count()) throw new Exception("unexpected end of file !");
                                        if (tokens[numToken] != ":") throw new Exception("colon missing  following VAR " + varName + " !");
                                        numToken++;
                                        if (numToken >= tokens.Count()) throw new Exception("unexpected end of file !");
                                        switch (tokens[numToken]) // get variable type
                                        {
                                            case "REAL":
                                                engine.LinguisticVariableCollection.Add(new LinguisticVariable(varName, "VAR", "REAL"));
                                                break;
                                            case "INT":
                                                engine.LinguisticVariableCollection.Add(new LinguisticVariable(varName, "VAR", "INT"));
                                                break;
                                            default:
                                                throw new Exception("REAL or INT type missing in VAR " + varName + " !");
                                        }
                                        numToken++;
                                    }
                                    else // no VAR variable name
                                    {
                                        throw new Exception("name of VAR variable missing !");
                                    }
                                }
                                break;
                            case 7: // function_block_body
                                switch (tokens[numToken])
                                {
                                    case "FUZZIFY":
                                        if (fbState > 8) throw new Exception("syntax error, FUZZIFY keyword detected out of sequence !");
                                        state = 8;
                                        break;
                                    case "DEFUZZIFY":
                                        if (fbState > 9) throw new Exception("syntax error, DEFUZZIFY keyword detected out of sequence !");
                                        state = 9;
                                        break;
                                    case "RULEBLOCK":
                                        if (fbState > 10) throw new Exception("syntax error, RULEBLOCK keyword detected out of sequence !");
                                        state = 10;
                                        break;
                                    //case "OPTIONS":
                                        //numToken++;
                                       // state = 11;
                                       // break;
                                    case "END_FUNCTION_BLOCK":
                                        state = 3;
                                        break;
                                    default:
                                        throw new Exception("syntax error inside FUNCTION_BLOCK " + engine.Function_block_name + " near " + tokens[numToken] + " !");
                                        //break;
                                }
                                break;
                            case 8: // at FUZZIFY
                                retState = state;
                                fbState = state;
                                if (tokens[numToken] != "FUZZIFY")
                                {
                                    state = 7;
                                    break;
                                }
                                numToken++;
                                skipWhiteSpace();
                                if (isIdentifier()) // get variable name
                                {
                                    string varName = tokens[numToken];
                                    linguisticVar = engine.LinguisticVariableCollection.Find(varName);
                                    if (linguisticVar.Direction != "VAR_INPUT") throw new Exception("FUZZIFY variable_name " + varName + " does not match a VAR_INPUT variable !");
                                    numToken++;
                                    state = 12;
                                }
                                else throw new Exception("VAR_INPUT variable name missing following Fuzzify keyword !");
                                break;
                            case 9: // at DEFUZZIFY
                                retState = state;
                                fbState = state;
                                if (tokens[numToken] != "DEFUZZIFY")
                                {
                                    state = 7;
                                    break;
                                }
                                numToken++;
                                skipWhiteSpace();
                                if (isIdentifier()) // get variable name
                                {
                                    string varName = tokens[numToken];
                                    linguisticVar = engine.LinguisticVariableCollection.Find(varName);
                                    if (linguisticVar == null) throw new Exception("VAR_OUTPUT for DEFUZIFY variable_name " + varName + " was not found !");
                                    if (linguisticVar.Direction != "VAR_OUTPUT") throw new Exception("DEFUZIFY variable_name " + varName + " does not match a VAR_OUTPUT variable !");
                                    numToken++;
                                    state = 12;
                                }
                                else throw new Exception("VAR_INPUT variable name missing following Fuzzify keyword !");
                                break;
                            case 10: // at RULEBLOCK
                                retState = state;
                                fbState = state;
                                if (tokens[numToken] != "RULEBLOCK")
                                {
                                    state = 7;
                                    break;
                                }
                                numToken++;
                                skipWhiteSpace();
                                if (isIdentifier()) // get rule_block_name
                                {
                                    blockName = tokens[numToken];
                                    engine.fuzzyRuleCollection.Add(new RuleBlockCollection(blockName));
                                    ruleBlock = engine.fuzzyRuleCollection.Find(blockName);
                                    numToken++;
                                    skipWhiteSpace();
                                    switch (tokens[numToken]) // get operator_definition
                                    {
                                        case "AND":
                                            numToken++;
                                            skipWhiteSpace();
                                            if (tokens[numToken] != ":") throw new Exception("missing ':' following operator_definition 'AND' near RULEBLOCK " + blockName + " !");
                                            numToken++;
                                            skipWhiteSpace();
                                            string operAND = tokens[numToken];
                                            switch (operAND) // get AND activation_method type
                                            {
                                                case "MIN":
                                                    break;
                                                case "PROD":
                                                    break;
                                                case "BDIF":
                                                    break;
                                                default:
                                                    throw new Exception("unrecognized 'AND' operator_definition " + tokens[numToken] + " in RULEBLOCK " + blockName + " !");
                                            }
                                            ruleBlock.OperatorDef = "AND : " + operAND;
                                            break;
                                        case "OR":
                                            numToken++;
                                            skipWhiteSpace();
                                            if (tokens[numToken] != ":") throw new Exception("missing ':' following operator_definition 'OR' near RULEBLOCK " + blockName + " !");
                                            numToken++;
                                            skipWhiteSpace();
                                            string operOR = tokens[numToken];
                                            switch (operOR) // get AND activation_method type
                                            {
                                                case "MAX":
                                                    break;
                                                case "ASUM":
                                                    break;
                                                case "BSUM":
                                                    break;
                                                default:
                                                    throw new Exception("unrecognized 'OR' operator_definition " + tokens[numToken] + " in RULEBLOCK " + blockName + " !");
                                            }
                                            ruleBlock.OperatorDef = "OR : " + operOR;
                                            break;
                                        default:
                                            throw new Exception("missing activation method 'AND' or 'OR' near RULEBLOCK " + blockName + " !");
                                    }
                                    numToken++; // expecting ";"
                                    skipWhiteSpace();
                                    if (tokens[numToken] != ";") throw new Exception("missing ';' following AND/OR operator_definition in RULEBLOCK " + blockName + " !");
                                    numToken++;
                                    state = 20;
                                }
                                else throw new Exception("missing rule_block_name following RULEBLOCK keyword !");
                                break;
                            case 11: // after OPTION
                                break;
                            case 12: // expecting linguistic_term list or END_FUZZIFY or END_DEFUZZIFY
                                string keyword = tokens[numToken];
                                if ((linguisticVar.Direction == "VAR_INPUT" && keyword == "END_FUZZIFY")
                                 || (linguisticVar.Direction == "VAR_OUTPUT" && keyword == "END_DEFUZZIFY"))
                                {
                                    numToken++;
                                    state = retState;
                                }
                                else // not END_FUZIFY or END_DEFUZZIFY
                                {
                                    string name = linguisticVar.Name;
                                    if (linguisticVar.Direction == "VAR_OUTPUT")
                                    {
                                        if (keyword == "METHOD")
                                        {
                                            if (linguisticVar.Method != String.Empty) throw new Exception("syntax error in DEFUZZIFY block " + linguisticVar.Name + " near METHOD. Multiple METHOD keywors not allowed !");
                                            numToken++;
                                            state = 17;
                                            break;
                                        }
                                    }
                                    if (keyword != "TERM") throw new Exception("TERM keyword missing within FUZZIFY/DEFUZIFY block !");
                                    else
                                    {
                                        numToken++;
                                        skipWhiteSpace();
                                        if (isIdentifier())  // get term_name
                                        {
                                            term_name = tokens[numToken];
                                            if (linguisticVar.MembershipFunctionCollection.Find(term_name) != null) throw new Exception("Duplicate term_naem " + term_name + " within FUZZIFY/DEFUZIFY block !");
                                            // look for ':='
                                            numToken++;
                                            skipWhiteSpace();
                                            if (numToken == tokens.Count()) throw new Exception("unexpected End of File near TERM " + term_name + " !");
                                            if ((tokens[numToken] == ":") && (tokens[numToken + 1] == "="))
                                            {
                                                linguisticVar.MembershipFunctionCollection.Add(new MembershipFunction(term_name));
                                                numToken += 2;
                                                state = 13;
                                            }
                                            else throw new Exception("':=' missing within FUZZIFY/DEFUZZIFY block near TERM " + term_name + " !");
                                        }
                                        else throw new Exception("term_name identifier missing within FUZZIFY/DEFUZZIFY block !");
                                    }
                                }
                                break;
                            case 13: // expecting membership_function definition
                                string tok = tokens[numToken];
                                if (keywords.Contains(tok)) throw new Exception("unexpected keyword " + tok + " found within FUZZIFY/DEFUZZIFY block near TERM " + term_name + " !");
                                mem = linguisticVar.MembershipFunctionCollection.Find(term_name);
                                if (mem == null) throw new Exception("term_name " + term_name + " not found within FUZZIFY/DEFUZZIFY block !");
                                if (tok == "(") // points
                                {
                                    state = 14;
                                }
                                else // singleton
                                {
                                    LinguisticVariable var2 = engine.LinguisticVariableCollection.Find(tok);
                                    char first_char = (tok.ToCharArray())[0];
                                    if (first_char == '-' || char.IsDigit(first_char)) // numeric_literal singleton
                                    {
                                        mem.Singleton_value = System.Convert.ToDouble(tok);
                                        numToken++;
                                    }
                                    else if (var2 == null) throw new Exception("variable_name " + tok + " within FUZZIFY/DEFUZZIFY membership_function is not found in VAR list !");
                                    else if (var2.Type != "VAR") throw new Exception("variable_name " + tok + " within FUZZIFY/DFUZIFY membership_function is not found in VAR list !");
                                    else if (var2 != null) // vaiable_name singleton
                                    {
                                        mem.Singleton_varName = tok;
                                        numToken++;
                                    }
                                    skipWhiteSpace();
                                    if (tokens[numToken] != ";") throw new Exception("Syntax error: missing ';' in membership_function within FUZZIFY/DEFUZZIFY block near TERM " + term_name + " !");
                                    numToken++;
                                    state = 12;
                                }
                                break;
                            case 14: // begin points
                                if (tokens[numToken] == "(")
                                {
                                    numToken++;
                                    skipWhiteSpace();
                                    string tok3 = tokens[numToken];
                                    if (isIdentifier()) // variable_name
                                    {
                                        mem.pointCollection.Add(new Point(1, Double.NaN, tok3));
                                        numToken++;
                                        state = 15;
                                        break;
                                    }
                                    else if (tok3.StartsWith("-") || char.IsDigit(tok3, 0)) // numeric_literal
                                    {
                                        double val = System.Convert.ToDouble(tok3);
                                        mem.pointCollection.Add(new Point(1, val, String.Empty));
                                        numToken++;
                                        state = 15;
                                        break;
                                    }
                                }
                                throw new Exception("syntax error in membership_function within FUZZIFY/DFUZIFY block near TERM " + term_name + " !");
                            case 15: // 2nd point
                                if (tokens[numToken] != ",") throw new Exception("syntax error in membership_function within FUZZIFY/DFUZIFY block near TERM " + term_name + ". Missing comma !");
                                numToken++;
                                skipWhiteSpace();
                                string tok2 = tokens[numToken];
                                if (isIdentifier()) // variable_name
                                {
                                    mem.pointCollection.Add(new Point(2, Double.NaN, tok2));
                                    numToken++;
                                    state = 16;
                                    break;
                                }
                                else if (char.IsDigit(tok2, 0)) // numeric_literal
                                {
                                    double val = System.Convert.ToDouble(tok2);
                                    mem.pointCollection.Add(new Point(2, val, String.Empty));
                                    numToken++;
                                    state = 16;
                                    break;
                                }
                                break;
                            case 16: // end points
                                if (tokens[numToken] != ")") throw new Exception("syntax error in membership_function within FUZZIFY/DFUZIFY block near TERM " + term_name + ". Missing ')' !");
                                numToken++;
                                skipWhiteSpace();
                                if (tokens[numToken] == ";") // end of TERM definition
                                {
                                    numToken++;
                                    state = 12;
                                }
                                else state = 14; // excpecting more points
                                break;
                            case 17: // after METHOD
                                if (tokens[numToken] != ":") throw new Exception("syntax error in DFUZIFY block " + linguisticVar.Name + " near METHOD. Missing ':' !");
                                numToken++;
                                skipWhiteSpace();
                                switch (tokens[numToken])
                                {
                                    case "COG":
                                        break;
                                    case "COGS":
                                        break;
                                    case "COA":
                                        break;
                                    case "LM":
                                        break;
                                    case "RM":
                                        break;
                                    default:
                                        throw new Exception("syntax error in DFUZIFY block " + linguisticVar.Name + " near METHOD. Unrecognized defuzzification_method " + tokens[numToken] + " !");
                                }
                                linguisticVar.Method = tokens[numToken];
                                numToken++;
                                skipWhiteSpace();
                                if (tokens[numToken] != ";") throw new Exception("syntax error in DFUZIFY block " + linguisticVar.Name + " near METHOD. Missing ';' !");
                                numToken++;
                                state = 18;
                                break;
                            case 18: // expecting DEFAULT definition
                                if (tokens[numToken] != "DEFAULT") throw new Exception("syntax error in DFUZIFY block " + linguisticVar.Name + ". Missing DEFAULT !");
                                numToken++;
                                skipWhiteSpace();
                                // expecting ":="
                                if (tokens[numToken] != ":" && tokens[numToken + 1] != "=") throw new Exception("syntax error in DFUZIFY block " + linguisticVar.Name + " DEFAULT. Missing ':=' !");
                                numToken += 2;
                                skipWhiteSpace();
                                if (tokens[numToken] == "NC") // DEFAULT := No_Change (maintain value) if no rules fire
                                {
                                    linguisticVar.Default_NC = "NC";
                                    numToken++;
                                    skipWhiteSpace(); // expecting ";"
                                    if (tokens[numToken] != ";") throw new Exception("syntax error in DFUZIFY block " + linguisticVar.Name + " near DEFAULT. Missing ';' !");
                                    numToken++;
                                    state = 19;
                                }
                                else // expecting numeric_literal
                                {
                                    linguisticVar.Default_value = System.Convert.ToDouble(tokens[numToken]);
                                    numToken++;
                                    skipWhiteSpace(); // expecting ";"
                                    if (tokens[numToken] != ";") throw new Exception("syntax error in DFUZIFY block " + linguisticVar.Name + " near DEFAULT. Missing ';' !");
                                    numToken++;
                                    state = 19;
                                }
                                break;
                            case 19: // after default_value; possible RANGE defeinition
                                if (tokens[numToken] == "RANGE")
                                {
                                    numToken++;
                                    skipWhiteSpace(); // expecting ":="
                                    if (tokens[numToken] != ":" && tokens[numToken + 1] != "=") throw new Exception("syntax error in DFUZIFY block " + linguisticVar.Name + " RANGE. Missing ':=' !");
                                    numToken++;
                                    skipWhiteSpace(); // expecting "("
                                    if (tokens[numToken] != "(") throw new Exception("syntax error in DFUZIFY block " + linguisticVar.Name + " RANGE. Missing '(' !");
                                    numToken++;
                                    skipWhiteSpace(); // expecting Range_min
                                    linguisticVar.Range_min = System.Convert.ToDouble(tokens[numToken]);
                                    numToken++;
                                    skipWhiteSpace(); // expecting ".."
                                    if (tokens[numToken] != "." && tokens[numToken + 1] != ".") throw new Exception("syntax error in DFUZIFY block " + linguisticVar.Name + " RANGE. Missing '..' !");
                                    numToken += 2;
                                    skipWhiteSpace(); // expecting Range_max
                                    linguisticVar.Range_max = System.Convert.ToDouble(tokens[numToken]);
                                    numToken++;
                                    skipWhiteSpace(); // expecting ")"
                                    if (tokens[numToken] != ")") throw new Exception("syntax error in DFUZIFY block " + linguisticVar.Name + " RANGE. Missing ')' !");
                                    numToken++;
                                    skipWhiteSpace(); // expecting ";"
                                    if (tokens[numToken] != ";") throw new Exception("syntax error in DFUZIFY block " + linguisticVar.Name + " RANGE. Missing ';' !");
                                    numToken++;
                                    state = 12;
                                }
                                else state = 12; // no RANGE
                                break;
                            case 20: // after operator_definition
                                if (tokens[numToken] == "ACT")
                                {
                                    numToken++;
                                    skipWhiteSpace(); // expecting ":"
                                    if (tokens[numToken] != ":") throw new Exception("missing ':' following ACT in RULEBLOCK " + blockName + " !");
                                    numToken++;
                                    skipWhiteSpace(); // expecting activation_method
                                    string act = tokens[numToken];
                                    switch (act)
                                    {
                                        case "PROD":
                                            break;
                                        case "MIN":
                                            break;
                                        default:
                                            throw new Exception("unrecognized activation_method following ACT: in RULEBLOCK " + blockName + " !");
                                    }
                                    ruleBlock.ActivationMethod = act;
                                    numToken++;
                                    skipWhiteSpace(); // expecting ";"
                                    if (tokens[numToken] != ";") throw new Exception("missing ';' following ACT : " + act + " in RULEBLOCK " + blockName + " !");
                                    numToken++;
                                    state = 21;
                                }
                                else // not ACT
                                {
                                    state = 21;
                                }
                                break;
                            case 21: // after RULEBLOCK, expecting ACCU
                                if (tokens[numToken] == "ACCU")
                                {
                                    numToken++;
                                    skipWhiteSpace(); // expecting ":"
                                    if (tokens[numToken] != ":") throw new Exception("missing ':' following ACCU in RULEBLOCK " + blockName + " !");
                                    numToken++;
                                    skipWhiteSpace(); // get accumulation_method
                                    string accum = tokens[numToken];
                                    switch (accum)
                                    {
                                        case "MAX":
                                            break;
                                        case "BSUM":
                                            break;
                                        case "NSUM’":
                                            break;
                                        default:
                                            break;
                                    }
                                    ruleBlock.AccumulationMethod = accum;
                                    numToken++;
                                    skipWhiteSpace(); // expecting ";"
                                    if (tokens[numToken] != ";") throw new Exception("missing ';' following ACCU : " + accum + " in RULEBLOCK " + blockName + " !");
                                    numToken++;
                                    state = 22;
                                }
                                else  // missing ACCU
                                {
                                    throw new Exception("missing 'ACCU' in RULEBLOCK " + blockName + " !");
                                }
                                break;
                            case 22: // after RULEBLOCK accumulation_method, expecting RULE or END_RULEBLOCK
                                string keyword2 = tokens[numToken];
                                if (keyword2 == "END_RULEBLOCK") // continue with function_block_body
                                {
                                    numToken++;
                                    state = 7;
                                }
                                else
                                {
                                    if (keyword2 == "RULE") // list of rules
                                    {
                                        numToken++;
                                        skipWhiteSpace(); // get RULE INT name
                                        rule_number = System.Convert.ToInt32(tokens[numToken]);
                                        ruleBlock.Add(new FuzzyRule(rule_number.ToString()));
                                        fuzzyRule = ruleBlock.Find(rule_number.ToString());
                                        numToken++;
                                        skipWhiteSpace(); // expecting ':'
                                        if (tokens[numToken] != ":") throw new Exception("missing ':' following RULEBLOCK " + blockName + " RULE " + rule_number + " !");
                                        numToken++;
                                        skipWhiteSpace(); // get RULE statement
                                        if (tokens[numToken] != "IF") throw new Exception("missing 'IF' in RULEBLOCK " + blockName + " RULE " + rule_number + " !");
                                        numToken++;
                                        fuzzyRule.conditionCollection = new ConditionCollection();
                                        //condCollection = fuzzyRule.conditionCollection;
                                        state = 23; // get RULE condition
                                    }
                                    else throw new Exception("unrecoginized token " + keyword2 + " in RULEBLOCK " + blockName + " !");
                                }
                                break;
                            case 23: // RULE condition
                                conditionOper = String.Empty;
                                string tok4 = tokens[numToken];
                                switch (tok4)
                                {
                                    case "THEN":
                                        if (fuzzyRule.conditionCollection.Count < 1) throw new Exception("Syntax error: No conditions in RULEBLOCK " + blockName + " RULE " + rule_number + " !");
                                        numToken++;
                                        state = 24;
                                        break;
                                    default: // condition
                                        if (tok4 == "AND" || tok4 == "OR")
                                        {
                                            conditionOper = tok4;
                                            numToken++;
                                            skipWhiteSpace();
                                        }
                                        if (tok4 == "NOT") // negated subcondition
                                        {
                                            numToken++;
                                            skipWhiteSpace(); // expecting "("
                                            if (tokens[numToken] != "(") throw new Exception("missing '(' following 'NOT' in RULEBLOCK " + blockName + " RULE " + rule_number + " !");
                                            numToken++;
                                            skipWhiteSpace(); // get variable name
                                        }
                                        else tok4 = String.Empty; // not negated
                                        if (!isIdentifier()) throw new Exception("Syntax error: near " + tok4 + "in RULEBLOCK " + blockName + " RULE " + rule_number + " !");
                                        SubCondition subCond = new SubCondition();
                                        fuzzyRule.conditionCollection.Add(subCond);
                                        subCond.NegatedVar = tok4;
                                        subCond.VarName = tokens[numToken];
                                        subCond.ConditionOper = conditionOper;
                                        numToken++;
                                        skipWhiteSpace();
                                        if (tokens[numToken] == "IS") // subcondition
                                        {
                                            if (subCond.NegatedVar != String.Empty) throw new Exception("Syntax error: near NOT " + subCond.VarName + "in RULEBLOCK " + blockName + " RULE " + rule_number + " !");
                                            numToken++;
                                            skipWhiteSpace();
                                            tok4 = tokens[numToken];
                                            if (tok4 == "NOT") // negated Term
                                            {
                                                subCond.NegatedTerm = tok4;
                                                numToken++;
                                                skipWhiteSpace(); // get Term name
                                            }
                                            tok4 = tokens[numToken];
                                            if (!isIdentifier()) throw new Exception("Syntax error near " + tok4 + ". missing term_name in RULEBLOCK " + blockName + " RULE " + rule_number + " !");
                                            subCond.TermName = tok4;
                                            numToken++;
                                            if (subCond.NegatedVar != String.Empty)
                                            {
                                                skipWhiteSpace();
                                                if (tokens[numToken] != ")") throw new Exception("Syntax error: missing ')' following " + tok4 + " in RULEBLOCK " + blockName + " RULE " + rule_number + " !");
                                                numToken++;
                                            }
                                        }
                                        state = 23;
                                        break;
                                }
                                break;
                            case 24: // expecting conclusion
                                string tok5 = tokens[numToken];
                                if (!isIdentifier()) throw new Exception("Syntax error: near " + tokens[numToken] + ". missing conclusion variable name following THEN in RULEBLOCK " + blockName + " RULE " + rule_number + " !");
                                numToken++;
                                skipWhiteSpace();
                                conclusion = new SubCondition();
                                conclusion.VarName = tok5;
                                fuzzyRule.conclusionCollection.Add(conclusion);
                                if (tokens[numToken] == "IS")
                                {
                                    numToken++; // expecting term_name
                                    skipWhiteSpace();
                                    tok5 = tokens[numToken];
                                    if (!isIdentifier()) throw new Exception("Syntax error: near " + tok5 + ". missing conclusion term name following THEN " + conclusion.VarName + "IS in RULEBLOCK " + blockName + " RULE " + rule_number + " !");
                                    conclusion.TermName = tok5;
                                    numToken++;
                                }
                                state = 25;
                                break;
                            case 25: // after conclusion
                                string tok6 = tokens[numToken];
                                switch (tok6)
                                {
                                    case "RULE":
                                        state = 22;
                                        break;
                                    case ",":
                                        numToken++;
                                        state = 24;
                                        break;
                                    case "WITH":
                                        numToken++;
                                        skipWhiteSpace();
                                        tok6 = tokens[numToken];
                                        conclusion.Weighting_factor = System.Convert.ToDouble(tok6);
                                        numToken++;
                                        skipWhiteSpace();
                                        if (tokens[numToken] != ";") throw new Exception("Syntax error: missing ';' following WITH " + tok6 + " in RULEBLOCK " + blockName + " RULE " + rule_number + " !");
                                        numToken++;
                                        state = 22;
                                        break;
                                    case ";":
                                        numToken++;
                                        state = 22;
                                        break;
                                    default:
                                        throw new Exception("Syntax error: unrecognized " + tok6 + " following THEN in RULEBLOCK " + blockName + " RULE " + rule_number + " !");
                                        //break;
                                }
                                break;
                            default:
                                throw new Exception("Parse Error: unexpected parse state " + state + " !");
                        }
                    }
                    skipWhiteSpace(true);
                    if (tokens.Count() > numToken) throw new Exception("tokens after END_FUNCTION_BLOCK not recognized !");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }

        }
    }


}
