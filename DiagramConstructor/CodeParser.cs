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

        public CodeThree ParseCode()
        {
            // TO DO methods(global and in code) location
            // method start - (\w*)\((\w*)\)(\s*)(\{)
            if (codeToParse.IndexOf("main(") == 0)
            {
                codeToParse = codeToParse.Substring(codeToParse.IndexOf('{') + 1);
                codeToParse = codeToParse.Substring(0, codeToParse.Length - 1);
            }
            methodNodes = parseNode(codeToParse);
            CodeThree codeThree = new CodeThree("main()", methodNodes);
            return codeThree;
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
                code = code.Remove(code.Length - 1, 1).Insert(code.Length - 1, "");
            }
            return code;
        }
        public String getNextCodeBlock(String code)
        {
            code = checkCodeSimplenes(code);
            bool codeIsSingleBlock = code.IndexOf('}') == -1 && code.IndexOf('{') == -1;
            if (codeIsSingleBlock)
            {
                return code;
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
            return codeBlock;
        }

        public List<Node> parseNode(String nodeCode)
        {
            List<Node> resultNodes = new List<Node>();
            int nextLineDivider = 0;
            String nextCodeBlock = "";
            String nodeCodeLine = "";
            while (nodeCode.Length > 2) {
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

                    String localCode = nodeCode.Replace(nextCodeBlock, "");

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
                    while (nodeCode.Length > 2) {
                        Node node = new Node();
                        nextLineDivider = nodeCode.IndexOf(';');
                        nodeCodeLine = nodeCode.Substring(0, nextLineDivider + 1);
                        if (!lineIsSimple(nodeCodeLine))
                        {
                            break;
                        }
                        if(nodeCodeLine.IndexOf('{') == 0)
                        {
                            nodeCodeLine = nodeCodeLine.Remove(0, 1);
                        }
                        nodeCode = nodeCode.Substring(nodeCodeLine.Length);
                        node.nodeText = nodeCodeLine.Replace(";", "");
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
                            nodeCodeLine = nodeCodeLine.Replace("}", "").Replace("{", "");
                            node.nodeType = NodeType.COMMON;
                        }

                        Regex regex = new Regex("(\\w*)\\((\\w*)\\)");
                        Regex regex1 = new Regex("(\\w*)(\\=)(\\w*)\\((\\w*)\\)");
                        if (regex.IsMatch(nodeCodeLine))
                        {
                            node.nodeType = NodeType.PROGRAM;
                        }

                        node.nodeText = nodeCodeLine.Replace(";", "");
                        resultNodes.Add(node);
                    }
                    continue;
                }
                nodeCode = nodeCode.Replace(nextCodeBlock, "");
            }
            return resultNodes;
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
