using System;
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
            foreach (Method method in methodsToAnylize)
            {
                analyzeNodes(method.methodNodes);
            }
            return methodsToAnylize;
        }

        /// <summary>
        /// Compare short text (PROCESS and IN_OUT_PUT) blocks
        /// </summary>
        /// <param name="nodesToAnylize">nodes for compare</param>
        /// <returns>compared nodes</returns>
        public List<Node> analyzeNodes(List<Node> nodesToAnylize)
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
                    currentNode.childNodes = analyzeNodes(currentNode.childNodes);
                }
                if (currentNode.childIfNodes.Count != 0)
                {
                    currentNode.childIfNodes = analyzeNodes(currentNode.childIfNodes);
                }
                if (currentNode.childElseNodes.Count != 0)
                {
                    currentNode.childElseNodes = analyzeNodes(currentNode.childElseNodes);
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
