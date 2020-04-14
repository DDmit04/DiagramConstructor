using DiagramConstructor.actor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DiagramConstructor
{
    class CodeParser
    {
        private String codeToParse;

        private List<Node> methodNodes;

        public CodeParser(String codeToParse)
        {
            this.codeToParse = codeToParse;
        }

        public List<Method> ParseCode()
        {
            List<Method> methods = new List<Method>();
            String nextMethodCode = "";
            int methodCodeBegin = 0;
            String methodBlock = "";
            String methodSignature = "";
            Method newMethod = new Method("no method!", new List<Node>());
            while (!codeIsEmptyMarcks(codeToParse))
            {
                nextMethodCode = getNextCodeBlock(codeToParse);
                methodCodeBegin = nextMethodCode.IndexOf('{');
                methodBlock = nextMethodCode.Substring(methodCodeBegin);
                methodSignature = nextMethodCode.Substring(0, methodCodeBegin);
                codeToParse = codeToParse.Replace(nextMethodCode, "");
                methodBlock = methodBlock.Remove(0, 1).Insert(0, "");
                methodBlock = methodBlock.Remove(methodBlock.Length - 1, 1);
                methodNodes = parseNode(methodBlock);
                newMethod = new Method(methodSignature, methodNodes);
                if (methodSignature.IndexOf("mian(") != 0)
                {
                    methods.Insert(0, newMethod);
                }
                else
                {
                    methods.Add(newMethod);
                }
            }
            return methods;
        }

        /// <summary>
        /// Check is it necessary to delete {} sorrounded string
        /// </summary>
        /// <param name="code">string to check</param>
        /// <returns> string surrounded by {} and contains (if | else | while | ...) as first operand</returns>
        public bool nextCodeIsSimple(String code)
        {
            bool langStateIsNearToBegin = code.IndexOf("if(") == 1 
                || code.IndexOf("while(") == 1 
                || code.IndexOf("for(") == 1 
                || code.IndexOf("else{") == 1;
            bool codeIsSurroundedByMarcks = code[0].Equals('{') && code[code.Length - 1].Equals('}');
            return codeIsSurroundedByMarcks && langStateIsNearToBegin;
        }

        /// <summary>
        /// Delete {} surrounded string if this necessary
        /// </summary>
        /// <param name="code">string to modify</param>
        /// <returns>modified string</returns> 
        public String checkCodeSimplenes(String code)
        {
            if (nextCodeIsSimple(code))
            {
                code = code.Remove(0, 1);
                code = code.Remove(code.Length - 1, 1);
            }
            return code;
        }

        /// <summary>
        /// Returns code block extracted from string (example - while(a > b){a--; b++} cout<<a; cout<<b; --> while(a > b){a--; b++})
        /// </summary>
        /// <param name="code">code to search blocks</param>
        /// <returns>string from begin to index of closing } (count of { and } in result string is equal)</returns>
        public String getNextCodeBlock(String code)
        {
            if (code[0].Equals('{') && code[code.Length - 1].Equals('}'))
            {
                code = code.Remove(0, 1);
                code = code.Remove(code.Length - 1, 1);
            }
            String codeBlock = "";
            int openMarck = 0;
            int closeMarck = 0;
            int endIndex = 0;
            for (endIndex = 0; endIndex < code.Length; endIndex++)
            {
                if (code[endIndex].Equals('}'))
                {
                    openMarck++;
                }
                if (code[endIndex].Equals('{'))
                {
                    closeMarck++;
                }
                if (openMarck == closeMarck && (openMarck != 0 && closeMarck != 0))
                {
                    codeBlock = code.Substring(0, endIndex + 1);
                    break;
                }
            }
            if(codeBlock.Equals(""))
            {
                codeBlock = code;
            }
            return codeBlock;
        }

        /// <summary>
        /// Recursive method which convert code lines to AST nodes 
        /// (if it meets if | else | while | ... it create node and put in it's child nodes result of itself call)
        /// </summary>
        /// <param name="nodeCode">code to convert</param>
        /// <returns>list of code ASTs</returns>
        public List<Node> parseNode(String nodeCode)
        {
            List<Node> resultNodes = new List<Node>();
            int nextLineDivider = 0;
            String nextCodeBlock = "";
            String nodeCodeLine = "";
            while (!codeIsEmptyMarcks(nodeCode)) {
                Node newNode = new Node();
                nextCodeBlock = getNextCodeBlock(nodeCode);
                if (nextCodeBlock.IndexOf("if(") == 0)
                {
                    nextLineDivider = nextCodeBlock.IndexOf('{');
                    nodeCodeLine = nextCodeBlock.Substring(0, nextLineDivider);
                    nodeCodeLine = nodeCodeLine.Replace("if(", "");
                    nodeCodeLine = nodeCodeLine.Replace(")", "");
                    newNode.nodeText = nodeCodeLine;
                    newNode.shapeForm = ShapeForm.IF;

                    newNode.childIfNodes = parseNode(nextCodeBlock.Substring(nextLineDivider));

                    String localCode = replaceFirst(nodeCode, nextCodeBlock, "");

                    if (!codeIsEmptyMarcks(localCode))
                    {
                        String onotherNextBlock = getNextCodeBlock(localCode);
                        if(onotherNextBlock.IndexOf("elseif") == 0)
                        {
                            Regex elseifRegex = new Regex(@"(elseif\(\S+\){\S+;})+(else{\S+;})*");
                            Match match = elseifRegex.Match(localCode);
                            if(match.Success)
                            {
                                onotherNextBlock = match.Value;
                                nodeCode = replaceFirst(nodeCode, onotherNextBlock, "");
                            }
                            localCode = elseifRegex.Replace(localCode, "", 1);
                        }
                        if (onotherNextBlock.IndexOf("else") == 0)
                        {
                            nodeCode = nodeCode.Replace(onotherNextBlock, "");
                            onotherNextBlock = replaceFirst(onotherNextBlock, "else", "");
                            newNode.childElseNodes = parseNode(onotherNextBlock);
                        }
                    }
                    resultNodes.Add(newNode);
                } 
                else if(nextCodeBlock.IndexOf("for(") == 0)
                {
                    nextLineDivider = nextCodeBlock.IndexOf('{');
                    nodeCodeLine = nextCodeBlock.Substring(0, nextLineDivider);
                    nodeCodeLine = nodeCodeLine.Replace("for(", "");
                    nodeCodeLine = nodeCodeLine.Replace(")", "");
                    newNode.nodeText = nodeCodeLine;
                    newNode.shapeForm = ShapeForm.FOR;
                    newNode.childNodes = parseNode(nextCodeBlock.Substring(nextLineDivider));
                    resultNodes.Add(newNode);
                } 
                else if(nextCodeBlock.IndexOf("while(") == 0)
                {
                    nextLineDivider = nextCodeBlock.IndexOf('{');
                    nodeCodeLine = nextCodeBlock.Substring(0, nextLineDivider);
                    nodeCodeLine = nodeCodeLine.Replace("while(", "");
                    nodeCodeLine = nodeCodeLine.Replace(")", "");
                    newNode.nodeText = nodeCodeLine;
                    newNode.shapeForm = ShapeForm.WHILE;
                    newNode.childNodes = parseNode(nextCodeBlock.Substring(nextLineDivider));
                    resultNodes.Add(newNode);
                } 
                else
                {
                    Regex methodSingleCall = new Regex(@"(\S*)\((\S*)\)");
                    Regex methodReturnCall = new Regex(@"(\S*)(\=)(\S*)\((\S*)\)");
                    Regex methodCallOnObject = new Regex(@"\S*\=\S*\.\S*\(\S*\)");
                    String copy = nextCodeBlock;
                    while (!codeIsEmptyMarcks(copy)) {
                        Node node = new Node();
                        nextLineDivider = copy.IndexOf(';');
                        nodeCodeLine = copy.Substring(0, nextLineDivider + 1);
                        if (!lineIsSimple(nodeCodeLine))
                        {
                            break;
                        }
                        copy = replaceFirst(copy, nodeCodeLine, "");
                        if (nodeCodeLine.IndexOf("cin>>") == 0 
                            || nodeCodeLine.IndexOf("cin»") == 0 
                            || nodeCodeLine.IndexOf("cout<<") == 0 
                            || nodeCodeLine.IndexOf("cout«") == 0)
                        {
                            nodeCodeLine = nodeCodeLine.Replace("cin>>", "Ввод ").Replace("cin»", "Ввод ");
                            nodeCodeLine = nodeCodeLine.Replace("cout<<", "Вывод ").Replace("cout«", "Вывод ");
                            nodeCodeLine = nodeCodeLine.Replace("<<endl", "").Replace("«endl", "");
                            nodeCodeLine = nodeCodeLine.Replace(">>", "").Replace("<<", "");
                            nodeCodeLine = nodeCodeLine.Replace("»", "").Replace("«", "");
                            node.shapeForm = ShapeForm.IN_OUT_PUT;

                        }
                        else if(methodSingleCall.IsMatch(nodeCodeLine) 
                            || methodReturnCall.IsMatch(nodeCodeLine) 
                            || methodCallOnObject.IsMatch(nodeCodeLine))
                        {
                            node.shapeForm = ShapeForm.PROGRAM;
                        }
                        else
                        {
                            node.shapeForm = ShapeForm.PROCESS;
                        }

                        node.nodeText = nodeCodeLine.Replace(";", "");
                        resultNodes.Add(node);
                    }
                    if (copy.Length > 0)
                    {
                        nextCodeBlock = replaceFirst(nextCodeBlock, copy, "");
                    }
                }
                nodeCode = replaceFirst(nodeCode, nextCodeBlock, "");
            }
            return resultNodes;
        }

        /// <summary>
        /// Check is text contains code operators
        /// </summary>
        /// <param name="text">text to check</param>
        /// <returns>Is code contains only { and } chars</returns>
        public bool codeIsEmptyMarcks(String text)
        {
            Regex regex = new Regex(@"(\w*\;)");
            return !regex.IsMatch(text);
        }

        /// <summary>
        /// Replace fist ocurence in some string of some string 
        /// </summary>
        /// <param name="text">text to search ocurence</param>
        /// <param name="textToReplace">text for replace</param>
        /// <param name="replace">text to replace </param>
        /// <returns>modifyed text</returns>
        public String replaceFirst(String text, String textToReplace, String replace)
        {
            Regex regex = new Regex(Regex.Escape(textToReplace));
            text = regex.Replace(text, replace, 1);
            return text;
        }

        /// <summary>
        /// Check is code not started with (if | for | while | ...)
        /// </summary>
        /// <param name="line">code to check</param>
        /// <returns>bool</returns>
        public bool lineIsSimple(String line)
        {
            return !(line.IndexOf("if(") == 0
                || line.IndexOf("for(") == 0
                || line.IndexOf("while(") == 0
                || line.IndexOf("switch(") == 0);
        }

    }
}
