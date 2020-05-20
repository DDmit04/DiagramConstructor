using DiagramConstructor.utills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramConstructor.Config
{
    abstract class LanguageConfig
    {

        public String ifStatement;
        public String elseStatement;
        public String elseCloseStatement;
        public String elseIfStatement;
        public String forStatement;
        public String whileStatement;
        public String doWhileStatement;
        public String inputStatement;
        public String outputStatement;

        public String inputReplacement;
        public String outputReplacement;

        protected LanguageConfig()
        {
            this.ifStatement = "if(";
            this.elseStatement = "else{";
            this.elseCloseStatement = "}else";
            this.elseIfStatement = "elseif(";
            this.forStatement = "for(";
            this.whileStatement = "while(";
            this.doWhileStatement = "do{";
            this.inputStatement = "cin>>";
            this.outputStatement = "cout<<";
            this.inputReplacement = "Ввод ";
            this.outputReplacement = "Вывод ";
        }

        /// <summary>
        /// Delete code constructions that are not needed to build a diagram
        /// </summary>
        /// <param name="code">code to delete constructions</param>
        /// <returns>code withount optional constructions</returns>
        public virtual string cleanCodeBeforeParse(string code)
        {
            return code;
        }

        /// <summary>
        /// Check is line starts with 'InOutPut' statement
        /// </summary>
        /// <param name="codeLine">line to check</param>
        /// <returns>bool</returns>
        public virtual bool isLineStartWithInOutPut(string codeLine)
        {
            return codeLine.IndexOf(this.inputStatement) == 0 || codeLine.IndexOf(this.outputStatement) == 0;
        }

        /// <summary>
        /// Check is line starts with 'for' statement
        /// </summary>
        /// <param name="codeLine">line to check</param>
        /// <returns>bool</returns>
        public virtual bool isLineStartWithFor(string codeLine)
        {
            return codeLine.IndexOf(this.forStatement) == 0;
        }

        /// <summary>
        /// Check is line starts with 'while' statement
        /// </summary>
        /// <param name="codeLine">line to check</param>
        /// <returns>bool</returns>
        public virtual bool isLineStartWithWhile(string codeLine)
        {
            return codeLine.IndexOf(this.whileStatement) == 0;
        }

        /// <summary>
        /// Check is line starts with 'if' statement
        /// </summary>
        /// <param name="codeLine">line to check</param>
        /// <returns>bool</returns>
        public virtual bool isLineStartWithIf(string codeLine)
        {
            return codeLine.IndexOf(this.ifStatement) == 0;
        }

        /// <summary>
        /// Check is line starts with 'do-while' statement
        /// </summary>
        /// <param name="codeLine">line to check</param>
        /// <returns>bool</returns>
        public virtual bool isLineStartWithDoWhile(string codeLine)
        {
            return codeLine.IndexOf(this.doWhileStatement) == 0;
        }

        /// <summary>
        /// Check is line starts with 'else' statement [else{ | }else | elseif(]
        /// </summary>
        /// <param name="codeLine">line to check</param>
        /// <returns>bool</returns>
        public virtual bool isLineStartWithElse(string codeLine)
        {
            return codeLine.IndexOf(this.elseStatement) == 0  
                || codeLine.IndexOf(this.elseCloseStatement) == 0 
                || codeLine.IndexOf(this.elseIfStatement) == 0;
        }

        /// <summary>
        /// Check is line starts with 'elseif' statement
        /// </summary>
        /// <param name="codeLine">line to check</param>
        /// <returns>bool</returns>
        public virtual bool isLineStartWithElseIf(string codeLine)
        {
            return codeLine.IndexOf(this.elseIfStatement) == 0;
        }

        /// <summary>
        /// Check is line starts with program statement (call lib, call method)
        /// </summary>
        /// <param name="codeLine">line to check</param>
        /// <returns>bool</returns>
        public virtual bool isLineStartWithProgram(string codeLine)
        {
            return false;
        }

        /// <summary>
        /// Format 'for' statement
        /// </summary>
        /// <param name="codeLine">line to format</param>
        /// <returns>formated line (without statement string)</returns>
        public virtual string formatFor(string codeLine)
        {
            codeLine = CodeUtils.replaceFirst(codeLine, this.forStatement, "");
            return codeLine;
        }

        /// <summary>
        /// Format 'if' statement
        /// </summary>
        /// <param name="codeLine">line to format</param>
        /// <returns>formated line (without statement string)</returns>
        public virtual string formatIf(string codeLine)
        {
            codeLine = CodeUtils.replaceFirst(codeLine, this.ifStatement, "");
            return codeLine;
        }

        /// <summary>
        /// Format 'while' statement
        /// </summary>
        /// <param name="codeLine">line to format</param>
        /// <returns>formated line (without statement string)</returns>
        public virtual string formatWhile(string codeLine)
        {
            codeLine = CodeUtils.replaceFirst(codeLine, this.whileStatement, "");
            return codeLine;
        }

        /// <summary>
        /// Format 'inOutPut' statement
        /// </summary>
        /// <param name="codeLine">line to format</param>
        /// <returns>formated line (without statement string)</returns>
        public virtual string formatInOutPut(string codeLine)
        {
            codeLine = codeLine.Replace(this.inputStatement, this.inputReplacement);
            codeLine = codeLine.Replace(this.outputStatement, this.outputReplacement);
            return codeLine;
        }
    }
}
