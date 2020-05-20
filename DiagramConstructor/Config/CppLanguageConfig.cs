using System;
using System.Text.RegularExpressions;

namespace DiagramConstructor.Config
{
    class CppLanguageConfig : LanguageConfig
    {

        protected Regex unimportantOutputRegex = new Regex(@"\'\S*\'\,*");
        protected Regex methodSingleCallRegex = new Regex(@"(\S*)\((\S*)\)");
        protected Regex methodReturnCallRegex = new Regex(@"(\S*)(\=)(\S*)\((\S*)\)");
        protected Regex methodCallOnObjectRegex = new Regex(@"\S*\=\S*\.\S*\(\S*\)");

        protected String inputV2Statement = "cout«";
        protected String outputV2Statement = "cin»";

        public override string cleanCodeBeforeParse(string code)
        {
            Regex namespacesRegex = new Regex(@"using\s*namespace\s*\w*\;");
            Regex prepocessorRegex = new Regex(@"#\w*\s*[<|']\w*\.*\w?[\>|']");
            Regex structRegex = new Regex(@"(struct)\s*\S*\s*\{\s*\S*\s*(\};)");
            code = namespacesRegex.Replace(code, "");
            code = prepocessorRegex.Replace(code, "");
            code = structRegex.Replace(code, "");
            return code;
        }

        public override bool isLineStartWithInOutPut(string codeLine)
        {
            return base.isLineStartWithInOutPut(codeLine)
                || codeLine.IndexOf(this.inputV2Statement) == 0
                || codeLine.IndexOf(this.outputV2Statement) == 0;
        }

        public override bool isLineStartWithProgram(string codeLine)
        {
            return methodSingleCallRegex.IsMatch(codeLine)
                || methodReturnCallRegex.IsMatch(codeLine)
                || methodCallOnObjectRegex.IsMatch(codeLine);
        }

        public override string formatFor(string codeLine)
        {
            codeLine = base.formatFor(codeLine);
            codeLine = codeLine.Remove(codeLine.LastIndexOf(')'), 1);
            return codeLine;
        }

        public override string formatIf(string codeLine)
        {
            codeLine = base.formatIf(codeLine);
            codeLine = codeLine.Remove(codeLine.LastIndexOf(')'), 1);
            return codeLine;
        }

        public override string formatWhile(string codeLine)
        {
            codeLine = base.formatWhile(codeLine);
            codeLine = codeLine.Remove(codeLine.LastIndexOf(')'), 1);
            return codeLine;
        }

        public override string formatInOutPut(string codeLine)
        {
            codeLine = base.formatInOutPut(codeLine);
            codeLine = codeLine.Replace(this.inputV2Statement, this.inputReplacement);
            codeLine = codeLine.Replace(this.outputV2Statement, this.outputReplacement);
            //codeLine = codeLine.Replace("<<endl", "").Replace("«endl", "");
            codeLine = codeLine.Replace(">>", ", ").Replace("<<", ", ");
            codeLine = codeLine.Replace("»", ", ").Replace("«", ", ");
            codeLine = unimportantOutputRegex.Replace(codeLine, "");
            return codeLine;
        }
    }
}
