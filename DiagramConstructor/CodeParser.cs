using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            while (codeToParse.Length > 2)
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
                methods.Add(newMethod);
            }
            return methods;
        }

       
        public bool nextCodeIsSimple(String code)
        {
            bool langStateIsNearToBegin = code.IndexOf("if(") == 1 
                || code.IndexOf("while(") == 1 
                || code.IndexOf("for(") == 1 
                || code.IndexOf("else{") == 1;
            bool codeIsSurroundedByMarcks = code[0] == '{' && code[code.Length - 1] == '}';
            return codeIsSurroundedByMarcks && langStateIsNearToBegin;
        }

        
        public String checkCodeSimplenes(String code)
        {
            if (nextCodeIsSimple(code))
            {
                code = code.Remove(0, 1).Insert(0, "");
                code = code.Remove(code.Length - 1, 1);
            }
            return code;
        }

        
        public String getNextCodeBlock(String code)
        {
            if (code[0] == '{' && code[code.Length - 1] == '}')
            {
                code = code.Remove(0, 1).Insert(0, "");
                code = code.Remove(code.Length - 1, 1).Insert(code.Length - 1, "");
            }
            String codeBlock = "";
            int openMarck = 0;
            int closeMarck = 0;
            int endIndex = 0;
            for (endIndex = 0; endIndex < code.Length; endIndex++)
            {
                if (code[endIndex] == '}')
                {
                    openMarck++;
                }
                if (code[endIndex] == '{')
                {
                    closeMarck++;
                }
                if (openMarck == closeMarck && (openMarck != 0 && closeMarck != 0))
                {
                    codeBlock = code.Substring(0, endIndex + 1);
                    break;
                }
            }
            if(codeBlock == "")
            {
                codeBlock = code;
            }
            return codeBlock;
        }

        public List<Node> parseNode(String nodeCode)
        {
            List<Node> resultNodes = new List<Node>();
            int nextLineDivider = 0;
            String nextCodeBlock = "";
            String nodeCodeLine = "";
            while (codeIsEmptyMarcks(nodeCode)) {
                Node newNode = new Node();
                nextCodeBlock = getNextCodeBlock(nodeCode);
                if (nextCodeBlock.IndexOf("if(") == 0)
                {
                    nextLineDivider = nextCodeBlock.IndexOf('{');
                    nodeCodeLine = nextCodeBlock.Substring(0, nextLineDivider);
                    nodeCodeLine = nodeCodeLine.Replace("if(", "");
                    nodeCodeLine = nodeCodeLine.Replace(")", "");
                    newNode.nodeText = nodeCodeLine;
                    newNode.nodeType = NodeType.IF;

                    newNode.childIfNodes = parseNode(nextCodeBlock.Substring(nextLineDivider));

                    String localCode = replaceFirst(nodeCode, nextCodeBlock, "");

                    if (localCode.Length > 2)
                    {
                        String onotherNextBlock = getNextCodeBlock(localCode);
                        if (onotherNextBlock.IndexOf("else") == 0)
                        {
                            nodeCode = nodeCode.Replace(onotherNextBlock, "");
                            onotherNextBlock = onotherNextBlock.Replace("else", "");
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
                    newNode.nodeType = NodeType.FOR;
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
                    newNode.nodeType = NodeType.WHILE;
                    newNode.childNodes = parseNode(nextCodeBlock.Substring(nextLineDivider));
                    resultNodes.Add(newNode);
                } 
                else
                {
                    bool blockIsSimple = codeBlockIsSimple(nodeCode);
                    String copy = nextCodeBlock;
                    while (codeIsEmptyMarcks(copy)) {
                        Node node = new Node();
                        nextLineDivider = copy.IndexOf(';');
                        nodeCodeLine = copy.Substring(0, nextLineDivider + 1);
                        if (!lineIsSimple(nodeCodeLine))
                        {
                            break;
                        }
                        copy = replaceFirst(copy, nodeCodeLine, "");
                        if (nodeCodeLine.IndexOf("cin>>") == 0 || nodeCodeLine.IndexOf("cin»") == 0)
                        {
                            nodeCodeLine = nodeCodeLine.Replace("cin>>", "Ввод ");
                            nodeCodeLine = nodeCodeLine.Replace("cin»", "Ввод ");
                            nodeCodeLine = nodeCodeLine.Replace("«endl", "");
                            nodeCodeLine = nodeCodeLine.Replace("<<endl", "");
                            node.nodeType = NodeType.INOUTPUT;

                        }
                        else if(nodeCodeLine.IndexOf("cout<<") == 0 || nodeCodeLine.IndexOf("cout«") == 0)
                        {
                            nodeCodeLine = nodeCodeLine.Replace("cout<<", "Вывод ");
                            nodeCodeLine = nodeCodeLine.Replace("cout«", "Ввод ");
                            nodeCodeLine = nodeCodeLine.Replace("«endl", "");
                            nodeCodeLine = nodeCodeLine.Replace("<<endl", "");
                            node.nodeType = NodeType.INOUTPUT;
                        }
                        else
                        {
                            node.nodeType = NodeType.COMMON;
                        }

                        Regex methodSingleCall = new Regex(@"(\w*)\((\w*)\)");
                        Regex methodReturnCall = new Regex(@"(\w*)(\=)(\w*)\((\w*)\)");
                        if (methodSingleCall.IsMatch(nodeCodeLine) || methodReturnCall.IsMatch(nodeCodeLine))
                        {
                            node.nodeType = NodeType.PROGRAM;
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

        public bool codeIsEmptyMarcks(String text)
        {
            Regex regex = new Regex(@"(\w*\;)");
            return regex.IsMatch(text);
        }

        public String replaceFirst(String text, String textToReplace, String replace)
        {
            Regex regex = new Regex(Regex.Escape(textToReplace));
            text = regex.Replace(text, replace, 1);
            return text;
        }

        public bool codeBlockIsSimple(String line)
        {
            return line.IndexOf("if(") == -1 
                && line.IndexOf("for(") == -1
                && line.IndexOf("while(") == -1 
                && line.IndexOf("switch(") == -1;
        }

        public bool lineIsSimple(String line)
        {
            return !(line.IndexOf("if(") == 0
                || line.IndexOf("for(") == 0
                || line.IndexOf("while(") == 0
                || line.IndexOf("switch(") == 0);
        }

    }
}
