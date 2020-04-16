﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiagramConstructor.actor
{
    class CodeAnalyzer
    {

        public List<Method> analyzeMethods(List<Method> methodsToAnylize)
        {
            int methodArgsIndex = -1;
            foreach (Method method in methodsToAnylize)
            {
                Regex arraySizeRegexp = new Regex(@"(\[\d*\])*");
                method.methodSignature = arraySizeRegexp.Replace(method.methodSignature, "", 1);
                methodArgsIndex = method.methodSignature.IndexOf('(');
                if (methodArgsIndex != -1)
                {
                    method.methodSignature = method.methodSignature.Insert(methodArgsIndex, " ");
                }
                anylizeNodesTexts(method.methodNodes);
                compareNodes(method.methodNodes);
            }
            return methodsToAnylize;
        }

        public String anylizeProcessNodeText(String startText)
        {
            int equalSignIndex = startText.IndexOf('=');
            if (equalSignIndex != -1)
            {
                startText = startText.Insert(equalSignIndex, " ");
                equalSignIndex = startText.IndexOf('=');
                startText = startText.Insert(equalSignIndex + 1, " ");
            }
            return startText;
        }

        public bool isUnimportantOutput(String startText)
        {
            Regex unimportantOutput = new Regex(@"Вывод\s*\'\S*\'");

            return unimportantOutput.IsMatch(startText) && startText.IndexOf(',') == -1;
        }

        public String anylizeForNodeText(String startText)
        {
            //exampe - startText = 'i=0;i<10;i++'
            int index = startText.LastIndexOf(';');
            // i++
            String incrementText = startText.Substring(index + 1);
            // i
            Regex incrementName = new Regex(@"(\d*|\w*)[^\W*]");
            // ++
            incrementText = incrementName.Replace(incrementText, "", 1);
            // ++ (in case of incrementText = '+=10' incrementAction = '+=')
            String incrementAction = incrementText.Substring(0, 2);
            // '' (in case of incrementText = '+=10' incrementArg = '10')
            String incrementArg = incrementText.Replace(incrementAction, "");

            if(incrementAction.Equals("++"))
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
            else if (incrementAction.Equals("-=") )
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

        public List<Node> anylizeNodesTexts (List<Node> nodesToAnylize)
        {
            Node currentNode = nodesToAnylize[0];
            for (int i = nodesToAnylize.Count - 1; i >= 0; i--)
            {
                currentNode = nodesToAnylize[i];
                if (currentNode.shapeForm == ShapeForm.PROCESS)
                {
                    currentNode.nodeText = anylizeProcessNodeText(currentNode.nodeText);
                }
                else if (currentNode.shapeForm == ShapeForm.FOR)
                {
                    currentNode.nodeText = anylizeForNodeText(currentNode.nodeText);
                }
                else if (currentNode.shapeForm == ShapeForm.IN_OUT_PUT && isUnimportantOutput(currentNode.nodeText))
                {
                    nodesToAnylize.RemoveAt(i);
                    continue;
                }

                if (currentNode.childNodes.Count != 0)
                {
                    currentNode.childNodes = compareNodes(currentNode.childNodes);
                }
                if (currentNode.childIfNodes.Count != 0)
                {
                    currentNode.childIfNodes = compareNodes(currentNode.childIfNodes);
                }
                if (currentNode.childElseNodes.Count != 0)
                {
                    currentNode.childElseNodes = compareNodes(currentNode.childElseNodes);
                }
            }
            return nodesToAnylize;
        }

        /// <summary>
        /// Compare short text (PROCESS and IN_OUT_PUT) blocks
        /// </summary>
        /// <param name="nodesToAnylize">nodes for compare</param>
        /// <returns>compared nodes</returns>
        public List<Node> compareNodes(List<Node> nodesToAnylize)
        {
            Node lastNode = null;
            Node currentNode = null;
            int bothNodesTextLength = 0;
            int bothLineBrakersCount = 0;
            for(int i = nodesToAnylize.Count - 1; i >= 0; i--)
            { 
                currentNode = nodesToAnylize[i];
                if (currentNode.childNodes.Count != 0)
                {
                    currentNode.childNodes = compareNodes(currentNode.childNodes);
                }
                if (currentNode.childIfNodes.Count != 0)
                {
                    currentNode.childIfNodes = compareNodes(currentNode.childIfNodes);
                }
                if (currentNode.childElseNodes.Count != 0)
                {
                    currentNode.childElseNodes = compareNodes(currentNode.childElseNodes);
                }
                if (i == nodesToAnylize.Count - 1)
                {
                    lastNode = currentNode;
                    continue;
                }
                bothNodesTextLength = lastNode.nodeText.Length + currentNode.nodeText.Length;
                bothLineBrakersCount = new Regex("\n").Matches(currentNode.nodeText).Count + new Regex("\n").Matches(lastNode.nodeText).Count;
                if (currentNode.shapeForm == ShapeForm.PROCESS 
                    && lastNode.shapeForm == ShapeForm.PROCESS 
                    && bothNodesTextLength < 50 
                    && bothLineBrakersCount < 5)
                {
                    lastNode.nodeText += "\n" + currentNode.nodeText;
                    nodesToAnylize.RemoveAt(i);
                } 
                else if(currentNode.shapeForm == ShapeForm.IN_OUT_PUT
                    && lastNode.shapeForm == ShapeForm.IN_OUT_PUT
                    && bothNodesTextLength < 50
                    && bothLineBrakersCount < 5)
                {
                    if (lastNode.nodeText.Length > 15 || currentNode.nodeText.Length > 15)
                    {
                        lastNode.nodeText += ",\n" + currentNode.nodeText.Replace("Вывод", "").Replace("Ввод", "");
                    }
                    else
                    {
                        lastNode.nodeText += "," + currentNode.nodeText.Replace("Вывод", "").Replace("Ввод", "");
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

    }
}
