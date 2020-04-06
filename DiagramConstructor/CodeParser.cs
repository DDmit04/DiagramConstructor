using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramConstructor
{
    class CodeParser
    {
        private String codeToParse;
        private CodeThree codeThree = new CodeThree();

        public CodeParser(String codeToParse)
        {
            this.codeToParse = codeToParse;
        }

        public CodeThree ParseCode()
        {
            // TO DO methods(global and in code) location
            codeThree.main = parseNode(codeToParse);
            return codeThree;
        }

        public bool nextCodeIsSimple(String code)
        {
            bool langStateIsNearToBegin = code.IndexOf("if(") == 1 || code.IndexOf("while(") == 1 || code.IndexOf("for(") == 1 || code.IndexOf("else{") == 1;
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
                //if(nextCodeBlock == "")
                //{
                //    nextCodeBlock = nodeCode;
                //}
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
                    String copy = nextCodeBlock;
                    while (copy.Length > 2) {
                        Node node = new Node();
                        nextLineDivider = copy.IndexOf(';');
                        nodeCodeLine = copy.Substring(0, nextLineDivider + 1);
                        copy = copy.Replace(nodeCodeLine, "");
                        node.nodeText = nodeCodeLine.Replace(";", "");
                        if (copy.IndexOf("cin>>") == 0)
                        {
                            nodeCodeLine = nodeCodeLine.Replace("cin>>", "Ввод ");
                            nodeCodeLine = nodeCodeLine.Replace("<<endl", "");

                        } else if(copy.IndexOf("cout<<") == 0)
                        {
                            nodeCodeLine = nodeCodeLine.Replace("cout<<", "Вывод ");
                            nodeCodeLine = nodeCodeLine.Replace("<<endl", "");
                        }
                        else
                        {
                            nodeCodeLine = nodeCodeLine.Replace("}", "").Replace("{", "");
                        }
                        node.nodeText = nodeCodeLine.Replace(";", "");
                        node.nodeType = NodeType.COMMON;
                        resultNodes.Add(node);
                    }
                }
                nodeCode = nodeCode.Replace(nextCodeBlock, "");
            }
            return resultNodes;
        }

    }
}
