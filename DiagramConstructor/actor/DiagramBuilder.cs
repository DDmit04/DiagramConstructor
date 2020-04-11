using DiagramConstructor.actor;
using Microsoft.Office.Interop.Visio;
using System;
using System.Collections.Generic;

namespace DiagramConstructor
{
    class DiagramBuilder
    {
        private ShapeWrapper globalLastDropedShapeV2;
        private bool globalIsSameBranch = false;

        private double startX = 2.25;
        private double coreX = 2.25;

        private double startY = 11;
        private double coreY = 11;

        private List<Method> codeThree;
        private ShapeManipulator shapeManipulator;

        public DiagramBuilder(List<Method> codeThree)
        {
            shapeManipulator = new ShapeManipulator();
            this.codeThree = codeThree;
        }

        /// <summary>
        /// Main builer method iterrate on methods ASTs and build each of them
        /// </summary>
        public void buildDiagram()
        {

            shapeManipulator.openDocument();

            foreach (Method method in codeThree)
            {

                shapeManipulator.addTextField(method.methodSignature, coreX, coreY);

                coreY -= 0.4;

                placeBeginShape();

                for (int i = 0; i < method.methodNodes.Count; i++)
                {
                    Node node = method.methodNodes[i];
                    globalLastDropedShapeV2 = buildTreeV2(node, coreX, coreY);
                }

                placeEndShape(method.methodNodes);

                updateGlobalValuesBeforeRecursion(globalIsSameBranch, null);

                coreX += 5;
                coreY = startY;
            }

            if(Configuration.testRun)
            {
                Console.ReadKey();
            }

            Configuration.finalFilePath = shapeManipulator.closeDocument();
        }

        /// <summary>
        /// Place first method shape (begin)
        /// </summary>
        public void placeBeginShape()
        {
            globalLastDropedShapeV2 = shapeManipulator.dropShapeV2(ShapeForm.BEGIN, "Начало", coreX, coreY);
            coreY -= 0.95;
        }

        /// <summary>
        /// Change global valus
        /// </summary>
        /// <param name="sameBranch">new inSameBranch value</param>
        /// <param name="lastBranchShape">new globalLastDropedShapeV2 value</param>
        public void updateGlobalValuesBeforeRecursion(bool sameBranch, ShapeWrapper lastBranchShape)
        {
            globalIsSameBranch = sameBranch;
            globalLastDropedShapeV2 = lastBranchShape;
        }

        /// <summary>
        /// Place last method shape (end)
        /// </summary>
        public void placeEndShape(List<Node> mainBranch)
        {
            ShapeWrapper endShape = shapeManipulator.dropShapeV2(ShapeForm.BEGIN, "Конец", coreX, coreY);
            ShapeConnectionType shapeConnection = BuilderUtills.defineConnectionType(globalLastDropedShapeV2, endShape, globalIsSameBranch);
            shapeManipulator.connectShapes(globalLastDropedShapeV2.shape, endShape.shape, ShapeForm.LINE, shapeConnection);
        }

        /// <summary>
        /// Recursive method connect place new shapes from node and connect them if node have childe nodes - build them
        /// </summary>
        /// <param name="node">node to create shape</param>
        /// <param name="x">new shape x</param>
        /// <param name="y">new shape y</param>
        /// <returns>last places shape in nodes AST branch</returns>
        public ShapeWrapper buildTreeV2(Node node, double x, double y)
        {
            ShapeWrapper currentNodeShape = shapeManipulator.dropShapeV2(node, x, y);
            if (globalLastDropedShapeV2 != null)
            {
                ShapeConnectionType shapeConnectionType = BuilderUtills.defineConnectionType(globalLastDropedShapeV2, currentNodeShape, globalIsSameBranch);
                shapeManipulator.connectShapes(globalLastDropedShapeV2.shape, currentNodeShape.shape, ShapeForm.LINE, shapeConnectionType);
            }
            y--;
            ShapeWrapper lastBranchShape = currentNodeShape;
            if (node.shapeForm == ShapeForm.IF)
            {
                lastBranchShape = startIfElseBranch(node, currentNodeShape, x, y);
                //IMPORTANT (here method must be after)
                updateGlobalValuesBeforeRecursion(true, lastBranchShape);
                coreY--;
            } 
            else if (node.shapeForm == ShapeForm.WHILE)
            {
                y -= 0.2;
                lastBranchShape = currentNodeShape;
                //IMPORTANT
                updateGlobalValuesBeforeRecursion(true, lastBranchShape);
                startWhileBranch(node, currentNodeShape, x, y);
                coreY -= 0.2;
            }
            else if(node.shapeForm == ShapeForm.FOR)
            {
                lastBranchShape = currentNodeShape;
                //IMPORTANT
                updateGlobalValuesBeforeRecursion(true, lastBranchShape);
                buildTreeBranchV2(node, currentNodeShape, x, y);
                coreY -= 0.2;
            }
            if (y < coreY)
            {
                coreY = y;
            }
            //call with false!!!
            updateGlobalValuesBeforeRecursion(false, lastBranchShape);
            return lastBranchShape;
        }

        /// <summary>
        /// Place "yes" and "no" text fields before build node AST branch started with "while"
        /// </summary>
        /// <param name="node">"while" node</param>
        /// <param name="currentNodeShape">placed "while" shape</param>
        /// <param name="x">new branch x</param>
        /// <param name="y">new branch y</param>
        /// <returns>last shape in branch</returns>
        public ShapeWrapper startWhileBranch(Node node, ShapeWrapper currentNodeShape, double x, double y)
        {
            shapeManipulator.adSmallTextField("Да", x + 0.38, y + 1 - 0.5);
            shapeManipulator.adSmallTextField("Нет", x + 0.7, y + 1 + 0.2);
            y -= 0.2;
            ShapeWrapper lastBranchShape = buildTreeBranchV2(node, currentNodeShape, x, y);
            return lastBranchShape;
        }

        /// <summary>
        /// Place "yes" and "no" text fields before build node AST branch started with "if"
        /// </summary>
        /// <param name="node">"if" node</param>
        /// <param name="currentNodeShape">placed "if" shape</param>
        /// <param name="x">new branch x</param>
        /// <param name="y">new branch y</param>
        /// <returns>last shape in branch (in this case - invisible block connecting if and else branches)</returns>
        public ShapeWrapper startIfElseBranch(Node node, ShapeWrapper currentNodeShape, double x, double y)
        {
            shapeManipulator.adSmallTextField("Да", x - 0.7, y + 1.2);
            shapeManipulator.adSmallTextField("Нет", x + 0.7, y + 1.2);

            ShapeWrapper lastBranchShape = buildIfElseTreeBranchV2(node.childIfNodes, currentNodeShape, ShapeConnectionType.FROM_LEFT_TO_TOP, x - 1.2, y);

            int statementHeight = Math.Max(node.childIfNodes.Count, node.childElseNodes.Count);

            ShapeWrapper invisibleBlock = shapeManipulator.dropShapeV2(ShapeForm.INVISIBLE_BLOCK, "", x, y - statementHeight);

            if (lastBranchShape.isCommonShape())
            {
                shapeManipulator.connectShapes(invisibleBlock.shape, lastBranchShape.shape, ShapeForm.LINE, ShapeConnectionType.FROM_BOT_TO_CENTER);
            }
            else
            {
                shapeManipulator.connectShapes(invisibleBlock.shape, lastBranchShape.shape, ShapeForm.LINE, ShapeConnectionType.FROM_RIGHT_TO_CENTER);
            }

            if (node.childElseNodes.Count != 0)
            {
                lastBranchShape = buildIfElseTreeBranchV2(node.childElseNodes, currentNodeShape, ShapeConnectionType.FROM_RIGHT_TO_TOP, x + 1.2, y);
                shapeManipulator.connectShapes(invisibleBlock.shape, lastBranchShape.shape, ShapeForm.LINE, ShapeConnectionType.FROM_BOT_TO_CENTER);
            }
            else
            {
                shapeManipulator.connectShapes(invisibleBlock.shape, currentNodeShape.shape, ShapeForm.LINE, ShapeConnectionType.FROM_RIGHT_TO_CENTER);
            }
            return invisibleBlock;
        }

        /// <summary>
        /// Iterate node chiled nodes to build new branch or place branch shape
        /// </summary>
        /// <param name="node">node to build</param>
        /// <param name="chainParentShape">branch parent shape (to connect branch last shape to it)</param>
        /// <param name="x">branch x</param>
        /// <param name="y">branch y</param>
        /// <returns>last branch shape</returns>
        public ShapeWrapper buildTreeBranchV2(Node node, ShapeWrapper chainParentShape, double x, double y)
        {
            ShapeWrapper lastBranchShape = null;
            ShapeWrapper prevShape = chainParentShape;
            if(node.childNodes.Count == 0)
            {
                lastBranchShape = buildTreeV2(node, x, y);
                //IMPORTANT (global droped shape is changes every time when recursion call)!!!
                updateGlobalValuesBeforeRecursion(true, lastBranchShape);
                y--;
            }
            foreach(Node currentNode in node.childNodes)
            {
                if (currentNode.isSimpleNode())
                {
                    lastBranchShape = buildTreeV2(currentNode, x, y);
                }
                else
                {
                    lastBranchShape = buildTreeV2(currentNode, x, y);
                    //IMPORTANT (global droped shape is changes every time when recursion call)!!!
                    updateGlobalValuesBeforeRecursion(true, lastBranchShape);
                    y -= Math.Max(currentNode.childNodes.Count, Math.Max(currentNode.childIfNodes.Count, currentNode.childElseNodes.Count));
                    y -= 0.7;
                }
                prevShape = lastBranchShape;
                y--;
            }
            // if parent shape exists connect last branch shape to it
            if (chainParentShape != null)
            {
                ShapeConnectionType shapeConnectionType = BuilderUtills.defineConnectionTypeWithBranchParent(chainParentShape, lastBranchShape);
                shapeManipulator.connectShapes(chainParentShape.shape, lastBranchShape.shape, ShapeForm.ARROW_LEFT, shapeConnectionType);
            }
            if (y < coreY)
            {
                coreY = y;
            }
            return lastBranchShape;
        }

        /// <summary>
        /// Iterate node chiled nodes (in case if statement) to build new branch or place branch shape
        /// </summary>
        /// <param name="chainParentShape">branch parent shape (to connect branch last shape to it)</param>
        /// <param name="ifElseConnectionType">connection type for first if or else branch shape</param>
        /// <param name="x">branch x</param>
        /// <param name="y">branch y</param>
        /// <returns>last branch shape</returns>
        public ShapeWrapper buildIfElseTreeBranchV2(List<Node> nodes, ShapeWrapper chainParentShape, ShapeConnectionType ifElseConnectionType, double x, double y)
        {
            ShapeWrapper lastBranchShape = null;
            for (int i = 0; i < nodes.Count; i++)
            {
                Node node = nodes[i];
                if (node.isSimpleNode())
                {
                    if (i == 0)
                    {
                        lastBranchShape = shapeManipulator.dropShapeV2(node, x, y);
                        //IMPORTANT (global droped shape is changes every time when recursion call)!!!
                        updateGlobalValuesBeforeRecursion(true, lastBranchShape);
                        //connect first if/else branch shape manualy 
                        shapeManipulator.connectShapes(lastBranchShape.shape, chainParentShape.shape, ShapeForm.LINE, ifElseConnectionType);
                    }
                    else
                    {
                        lastBranchShape = buildTreeV2(node, x, y);
                    }
                }
                else
                {
                    lastBranchShape = buildTreeV2(node, x, y);
                    //IMPORTANT (global droped shape is changes every time when recursion call)!!!
                    updateGlobalValuesBeforeRecursion(true, lastBranchShape);
                    y -= Math.Max(node.childNodes.Count, Math.Max(node.childIfNodes.Count, node.childElseNodes.Count));
                }
                y--;
                if (y < coreY)
                {
                    coreY = y;
                } 
                else
                {
                    y = coreY;
                }
            }
            return lastBranchShape;
        }
    }
}
