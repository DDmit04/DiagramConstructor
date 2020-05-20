using DiagramConstructor.Config;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DiagramConstructor.actor
{
    class CodeAnalyzer
    {

        private LanguageConfig languageConfig;

        public CodeAnalyzer(LanguageConfig languageConfig)
        {
            this.languageConfig = languageConfig;
        }

        /// <summary>
        /// Modificate all method nodes
        /// </summary>
        /// <param name="methodsToAnylize">list of methods to modificate</param>
        /// <returns>list of modificated methods</returns>
        public List<Method> analyzeMethods(List<Method> methodsToAnylize)
        {
            int methodArgsIndex = -1;
            int lastGlobalVaribleEndIndex = -1;
            foreach (Method method in methodsToAnylize)
            {
                Regex arraySizeRegexp = new Regex(@"(\[\d*\])*");
                method.methodSignature = arraySizeRegexp.Replace(method.methodSignature, "", 1);
                methodArgsIndex = method.methodSignature.IndexOf('(');
                lastGlobalVaribleEndIndex = method.methodSignature.LastIndexOf(';');
                if (methodArgsIndex != -1)
                {
                    method.methodSignature = method.methodSignature.Insert(methodArgsIndex, " ");
                }
                if (method.methodSignature.LastIndexOf(';') != -1)
                {
                    method.methodSignature = clearMethodSignature(method.methodSignature);
                }
                anylizeNodesTexts(method.methodNodes);
                compareNodes(method.methodNodes);
            }
            return methodsToAnylize;
        }

        /// <summary>
        /// Delete globar args from method signature
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public String clearMethodSignature(String methodName)
        {
            int lastGlobalVaribleEndIndex = methodName.LastIndexOf(';');
            while (lastGlobalVaribleEndIndex != -1)
            {
                methodName = methodName.Substring(lastGlobalVaribleEndIndex + 1);
                lastGlobalVaribleEndIndex = methodName.LastIndexOf(';');
            }
            return methodName;
        }

        /// <summary>
        /// Modificate or delete node of it's text
        /// </summary>
        /// <param name="nodesToAnylize">nodes to modificate or delete</param>
        /// <returns>list of modificated nodes</returns>
        private List<Node> anylizeNodesTexts(List<Node> nodesToAnylize)
        {
            Node currentNode = nodesToAnylize[0];
            for (int i = nodesToAnylize.Count - 1; i >= 0; i--)
            {
                currentNode = nodesToAnylize[i];

                if (currentNode.shapeForm == ShapeForm.FOR)
                {
                    currentNode.nodeText = extractTextFromOperator(currentNode.nodeText, currentNode.shapeForm);
                    currentNode.nodeText = anylizeForNodeText(currentNode.nodeText);
                }
                else if (currentNode.shapeForm == ShapeForm.WHILE || currentNode.shapeForm == ShapeForm.IF)
                {
                    currentNode.nodeText = extractTextFromOperator(currentNode.nodeText, currentNode.shapeForm);
                    currentNode.nodeText = currentNode.nodeText.Replace("||", " or ").Replace("|", " or ");
                    currentNode.nodeText = currentNode.nodeText.Replace("&&", " and ").Replace("&", " and ");
                }
                else if(currentNode.shapeForm == ShapeForm.IN_OUT_PUT)
                {
                    currentNode.nodeText = extractTextFromOperator(currentNode.nodeText, currentNode.shapeForm);
                    if(isUnimportantOutput(currentNode.nodeText))
                    {
                        nodesToAnylize.RemoveAt(i);
                        continue;
                    }
                }

                if (nodeBranchNeedAnylize(currentNode.childNodes))
                {
                    currentNode.childNodes = anylizeNodesTexts(currentNode.childNodes);
                }
                if (nodeBranchNeedAnylize(currentNode.childIfNodes))
                {
                    currentNode.childIfNodes = anylizeNodesTexts(currentNode.childIfNodes);
                }
                if (nodeBranchNeedAnylize(currentNode.childElseNodes))
                {
                    currentNode.childElseNodes = anylizeNodesTexts(currentNode.childElseNodes);
                }
            }
            return nodesToAnylize;
        }

        /// <summary>
        /// Compare short text (PROCESS and IN_OUT_PUT) blocks
        /// </summary>
        /// <param name="nodesToAnylize">nodes for compare</param>
        /// <returns>list of compared nodes</returns>
        private List<Node> compareNodes(List<Node> nodesToAnylize)
        {
            int bothNodesTextLength = 0;
            int bothNodesLineBrakersCount = 0;
            foreach(Node node in nodesToAnylize)
            {
                if (nodeBranchNeedAnylize(node.childNodes))
                {
                    node.childNodes = compareNodes(node.childNodes);
                }
                if (nodeBranchNeedAnylize(node.childIfNodes))
                {
                    node.childIfNodes = compareNodes(node.childIfNodes);
                }
                if (nodeBranchNeedAnylize(node.childElseNodes))
                {
                    node.childElseNodes = compareNodes(node.childElseNodes);
                }
            }
            Node currentNode = null;
            Node lastNode = nodesToAnylize[nodesToAnylize.Count - 1];
            // nodesToAnylize.Count - 2 !!!
            for (int i = nodesToAnylize.Count - 2; i >= 0; i--)
            {
                currentNode = nodesToAnylize[i];
                
                bothNodesTextLength = lastNode.nodeText.Length + currentNode.nodeText.Length;
                bothNodesLineBrakersCount = new Regex("\n").Matches(currentNode.nodeText).Count + new Regex("\n").Matches(lastNode.nodeText).Count;
                if (currentNode.shapeForm == ShapeForm.PROCESS
                    && lastNode.shapeForm == ShapeForm.PROCESS
                    && bothNodesTextLength < 50
                    && bothNodesLineBrakersCount < 5)
                {
                    lastNode.nodeText = currentNode.nodeText + "\n" + lastNode.nodeText;
                    nodesToAnylize.RemoveAt(i);
                }
                else if (currentNode.shapeForm == ShapeForm.PROGRAM
                    && lastNode.shapeForm == ShapeForm.PROGRAM
                    && bothNodesTextLength < 35
                    && bothNodesLineBrakersCount < 5)
                {
                    lastNode.nodeText = currentNode.nodeText + "\n" + lastNode.nodeText;
                    nodesToAnylize.RemoveAt(i);
                }
                else if (currentNode.shapeForm == ShapeForm.IN_OUT_PUT
                    && lastNode.shapeForm == ShapeForm.IN_OUT_PUT
                    && bothNodesTextLength < 50
                    && bothNodesLineBrakersCount < 5)
                {
                    if (lastNode.nodeText.Length > 15 || currentNode.nodeText.Length > 15)
                    {
                        lastNode.nodeText = currentNode.nodeText + ",\n" + lastNode.nodeText.Replace(this.languageConfig.outputReplacement, "").Replace(this.languageConfig.inputReplacement, "");
                    }
                    else
                    {
                        lastNode.nodeText = currentNode.nodeText + ", " + lastNode.nodeText.Replace(this.languageConfig.outputReplacement, "").Replace(this.languageConfig.inputReplacement, "");
                    }
                    nodesToAnylize.RemoveAt(i);
                }
                else
                {
                    lastNode = currentNode;
                }
            }
            return nodesToAnylize;
        }

        /// <summary>
        /// Check is console output just constant string
        /// </summary>
        /// <param name="startText">text to anylize</param>
        /// <returns>is console output just constant string</returns>
        private bool isUnimportantOutput(String startText)
        {
            Regex unimportantOutput = new Regex(@"^" + this.languageConfig.outputStatement + @"\s*(\'\s*\S*\s*\'|\s*)$");

            return unimportantOutput.IsMatch(startText) && startText.IndexOf(',') == -1;
        }

        /// <summary>
        /// Check is node branch need to be anylized
        /// </summary>
        /// <param name="nodes">nodes to check</param>
        /// <returns>is neccesary to analyze node branch</returns>
        private bool nodeBranchNeedAnylize(List<Node> nodes)
        {
            return nodes.Count > 1 || (nodes.Count > 0 && nodes[0].shapeForm != ShapeForm.IN_OUT_PUT);
        }

        /// <summary>
        /// Modificate text in FOR shape
        /// </summary>
        /// <param name="startText">text to anylize</param>
        /// <returns>modificated text</returns>
        private String anylizeForNodeText(String startText)
        {
            //exampe - startText = 'i=0;i<10;i++'
            int index = startText.LastIndexOf(';');
            // i++
            String incrementText = startText.Substring(index + 1);
            // i
            Regex incrementName = new Regex(@"(\d*|\w*)[^\W*]");
            // ++
            incrementText = incrementName.Replace(incrementText, "", 1);
            // ++ (in case of incrementText - '+=10' incrementAction - '+=')
            String incrementAction = incrementText.Substring(0, 2);
            // '' (in case of incrementText - '+=10' incrementArg - '10')
            String incrementArg = incrementText.Replace(incrementAction, "");

            if (incrementAction.Equals("++"))
            {
                incrementText = "1";
            }
            else if (incrementAction.Equals("--"))
            {
                incrementText = "-1";
            }
            else if (incrementAction.Equals("+="))
            {
                incrementText = incrementArg;
            }
            else if (incrementAction.Equals("-="))
            {
                incrementText = "-" + incrementArg;
            }
            else if (incrementAction.Equals("*="))
            {
                incrementText = "*" + incrementArg;
            }
            else if (incrementAction.Equals("/="))
            {
                incrementText = "/" + incrementArg;
            }

            startText = startText.Substring(0, index);
            startText = startText.Replace(";", " (" + incrementText + ") ");
            return startText;
        }

        /// <summary>
        /// Gets operator text from code line
        /// </summary>
        /// <param name="nodeCode">operator code to extract text</param>
        /// <param name="nodeType">operator type to extract</param>
        /// <returns>extracted operator text</returns>
        String extractTextFromOperator(string nodeCode, ShapeForm nodeType)
        {
            if (nodeType == ShapeForm.IF)
            {
                nodeCode = this.languageConfig.formatIf(nodeCode);
            }
            else if (nodeType == ShapeForm.FOR)
            {
                nodeCode = this.languageConfig.formatFor(nodeCode);
            }
            else if (nodeType == ShapeForm.WHILE)
            {
                nodeCode = this.languageConfig.formatWhile(nodeCode);
            }
            else if(nodeType == ShapeForm.IN_OUT_PUT)
            {
                nodeCode = this.languageConfig.formatInOutPut(nodeCode);
            }
            return nodeCode;
        }

    }
}
