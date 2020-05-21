using DiagramConstructor.actor;
using System;
using System.Collections.Generic;

namespace DiagramConstructor
{
    class DiagramBuilder
    {
        private ShapeWrapper globalLastDropedShape;
        private bool globalIsSameBranch = false;

        private double startX = 1.25;
        private double coreX = 1.25;

        private double startY = 11;
        private double coreY = 11;

        private VisioManipulator visioManipulator;

        public DiagramBuilder()
        {
            this.visioManipulator = new VisioManipulator();
            setDefaultValues();
        }

        /// <summary>
        /// Turn class varibles to degfault
        /// </summary>
        private void setDefaultValues()
        {
            this.globalLastDropedShape = null;
            this.globalIsSameBranch = false;
            this.startX = 1.25;
            this.coreX = 1.25;
            this.startY = 11;
            this.coreY = 11;
        }

        /// <summary>
        /// Main builer method iterrate on methods ASTs and build each of them
        /// </summary>
        public void buildDiagram(List<Method> allMethods)
        {

            setDefaultValues();

            visioManipulator.openDocument();

            foreach (Method method in allMethods)
            {
                if (!method.methodSignature.Equals("main ()"))
                {
                    visioManipulator.addTextField(method.methodSignature, coreX, coreY);
                    coreY -= 0.4;
                }
                placeBeginShape();
                for (int i = 0; i < method.methodNodes.Count; i++)
                {
                    Node node = method.methodNodes[i];
                    globalLastDropedShape = buildTree(node, coreX, coreY);
                }
                coreY += 0.3;
                placeEndShape(method.methodNodes);
                moveCordsToNewMethod();
            }

            if(Configuration.testRun == TestRunType.FULL_TEST || Configuration.testRun == TestRunType.SINGLE_TEST)
            {
                Console.WriteLine("Press any key to save and close diagram");
                Console.ReadKey();
            }

            Configuration.finalFilePath = visioManipulator.closeDocument();
        }

        /// <summary>
        /// Update current cords to buils next method diagram
        /// </summary>
        private void moveCordsToNewMethod()
        {
            startX += 4.5;
            coreX = startX;
            coreY = startY;
        }

        /// <summary>
        /// Place first method shape (begin)
        /// </summary>
        private void placeBeginShape()
        {
            globalLastDropedShape = visioManipulator.dropShape(ShapeForm.BEGIN, "Начало", coreX, coreY);
            coreY -= 0.75;
        }

        /// <summary>
        /// Place last method shape (end)
        /// </summary>
        private void placeEndShape(List<Node> mainBranch)
        {
            ShapeWrapper endShape = visioManipulator.dropShape(ShapeForm.BEGIN, "Конец", coreX, coreY);
            ShapeConnectionType shapeConnection = BuilderUtills.defineConnectionType(globalLastDropedShape, endShape, globalIsSameBranch);
            visioManipulator.connectShapes(globalLastDropedShape.shape, endShape.shape, ShapeForm.LINE, shapeConnection, coreX, coreY);
        }

        /// <summary>
        /// Change global valus
        /// </summary>
        /// <param name="sameBranch">new inSameBranch value</param>
        /// <param name="lastBranchShape">new globalLastDropedShapeV2 value</param>
        private void updateGlobalValues(bool sameBranch, ShapeWrapper lastBranchShape)
        {
            globalIsSameBranch = sameBranch;
            globalLastDropedShape = lastBranchShape;
        }

        /// <summary>
        /// Break current node branch - next shape will not be connected to last droped shape
        /// </summary>
        private void endNodeBranch()
        {
            updateGlobalValues(false, null);
        }

        /// <summary>
        /// Sets new last droped shape, next shape will be connected to it as if they are in the same branch
        /// </summary>
        /// <param name="parentShape">new last dropped shape</param>
        private void continueNodeBranch(ShapeWrapper parentShape)
        {
            updateGlobalValues(true, parentShape);
        }

        /// <summary>
        /// Sets new last droped shape, next shape will be connected to it as if they are in the different branches
        /// </summary>
        /// <param name="parentShape">new last dropped shape</param>
        private void shiftNodeBranch(ShapeWrapper parentShape)
        {
            updateGlobalValues(false, parentShape);
        }

        private void refrashCoreY(double y)
        {
            if (y < coreY)
            {
                coreY = y;
            }
        }

        /// <summary>
        /// Recursive method connect place new shapes from node and connect them if node have childe nodes - build them
        /// </summary>
        /// <param name="node">node to create shape</param>
        /// <param name="x">new shape x</param>
        /// <param name="y">new shape y</param>
        /// <returns>last places shape in nodes AST branch</returns>
        private ShapeWrapper buildTree(Node node, double x, double y)
        {
            //because first shape in 'do-while' is little invisible block
            if(node.shapeForm == ShapeForm.DO)
            {
                y += 0.4;
            }
            ShapeWrapper currentNodeShape = visioManipulator.dropShape(node, x, y);
            y--;
            if (globalLastDropedShape != null)
            {
                ShapeConnectionType shapeConnectionType = BuilderUtills.defineConnectionType(globalLastDropedShape, currentNodeShape, globalIsSameBranch);
                visioManipulator.connectShapes(globalLastDropedShape.shape, currentNodeShape.shape, ShapeForm.LINE, shapeConnectionType);
            }
            ShapeWrapper lastBranchShape = currentNodeShape;
            if (node.shapeForm == ShapeForm.IF)
            {
                lastBranchShape = startIfElseBranch(node, currentNodeShape, x, y);
                coreY -= 0.2;
            } 
            else if (node.shapeForm == ShapeForm.WHILE)
            {
                y -= 0.2;
                lastBranchShape = currentNodeShape;
                continueNodeBranch(lastBranchShape);
                startWhileBranch(node, currentNodeShape, x, y);
                coreY -= 0.2;
            }
            else if(node.shapeForm == ShapeForm.FOR)
            {
                lastBranchShape = currentNodeShape;
                continueNodeBranch(lastBranchShape);
                buildTreeBranch(node, currentNodeShape, x, y);
                coreY -= 0.2;
            }
            else if(node.shapeForm == ShapeForm.DO)
            {
                y += 0.2;
                lastBranchShape = buildDoWhileBranch(node, currentNodeShape, x, y);
                coreY -= 0.2;
            }
            refrashCoreY(y);
            endNodeBranch();
            return lastBranchShape;
        }

        private ShapeWrapper buildDoWhileBranch(Node node, ShapeWrapper chainParentShape, double x, double y)
        {
            Node currentNode = null;
            ShapeWrapper lastBranchShape = null;
            for (int i = 0; i < node.childNodes.Count; i++)
            {
                currentNode = node.childNodes[i];
                if(i == node.childNodes.Count - 1)
                {
                    y -= 0.2;

                    visioManipulator.addSmallTextField("Да", x - 0.7, y + 0.2);
                    visioManipulator.addSmallTextField("Нет", x + 0.7, y + 0.2);

                    //drop last do while shape (while)
                    lastBranchShape = visioManipulator.dropShape(currentNode, x, y);

                    // connect last shape with perv shape
                    ShapeConnectionType shapeConnectionType = BuilderUtills.defineConnectionType(globalLastDropedShape, lastBranchShape, globalIsSameBranch);
                    visioManipulator.connectShapes(globalLastDropedShape.shape, lastBranchShape.shape, ShapeForm.LINE, shapeConnectionType, coreX, coreY);

                    // connsect last shape with do-while branch parent
                    visioManipulator.connectShapes(chainParentShape.shape, lastBranchShape.shape, ShapeForm.ARROW_LEFT, ShapeConnectionType.FROM_LEFT_TO_CENTER, coreX, coreY);

                    continueNodeBranch(lastBranchShape);
                } 
                else
                {
                    if (currentNode.isSimpleNode())
                    {
                        lastBranchShape = buildTree(currentNode, x, y);
                        continueNodeBranch(lastBranchShape);
                    }
                    else
                    {
                        lastBranchShape = buildTree(currentNode, x, y);
                        shiftNodeBranch(lastBranchShape);
                        y -= BuilderUtills.calcStatementHeight(currentNode);
                        y -= 0.5;
                    }
                }
                y--;
            }
            refrashCoreY(y);
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
        private ShapeWrapper startWhileBranch(Node node, ShapeWrapper currentNodeShape, double x, double y)
        {
            visioManipulator.addSmallTextField("Да", x + 0.28, y + 0.7);
            visioManipulator.addSmallTextField("Нет", x + 0.7, y + 1.3);
            y -= 0.1;
            ShapeWrapper lastBranchShape = buildTreeBranch(node, currentNodeShape, x, y);
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
        private ShapeWrapper startIfElseBranch(Node node, ShapeWrapper currentNodeShape, double x, double y)
        {
            visioManipulator.addSmallTextField("Да", x - 0.7, y + 1.2);
            visioManipulator.addSmallTextField("Нет", x + 0.7, y + 1.2);

            ShapeWrapper lastBranchShape = buildIfElseTreeBranchV2(node.childIfNodes, currentNodeShape, ShapeConnectionType.FROM_LEFT_TO_TOP, x - 1.2, y);

            double statementHeight = BuilderUtills.calcStatementHeight(node);

            ShapeWrapper invisibleBlock = visioManipulator.dropShape(ShapeForm.INVISIBLE_BLOCK, "", x, y - statementHeight);

            visioManipulator.connectLastShapeToInvisibleBlock(invisibleBlock, lastBranchShape);

            if (node.childElseNodes.Count != 0)
            {
                //actions in if branch changes coreY (y - y of first if and else branches operators)
                coreY = y;
                lastBranchShape = buildIfElseTreeBranchV2(node.childElseNodes, currentNodeShape, ShapeConnectionType.FROM_RIGHT_TO_TOP, x + 1.2, y);
                visioManipulator.connectLastShapeToInvisibleBlock(invisibleBlock, lastBranchShape);
            }
            else
            {
                visioManipulator.connectShapes(invisibleBlock.shape, currentNodeShape.shape, ShapeForm.LINE, ShapeConnectionType.FROM_RIGHT_TO_CENTER);
            }
            coreY = y - statementHeight - 0.5;
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
        private ShapeWrapper buildTreeBranch(Node node, ShapeWrapper chainParentShape, double x, double y)
        {
            ShapeWrapper lastBranchShape = null;
            if(node.childNodes.Count == 0)
            {
                lastBranchShape = buildTree(node, x, y);
                continueNodeBranch(lastBranchShape);
                y--;
            }
            foreach(Node currentNode in node.childNodes)
            {
                if (currentNode.isSimpleNode())
                {
                    lastBranchShape = buildTree(currentNode, x, y);
                    continueNodeBranch(lastBranchShape);
                }
                else
                {
                    lastBranchShape = buildTree(currentNode, x, y);
                    shiftNodeBranch(lastBranchShape);
                    y -= BuilderUtills.calcStatementHeight(currentNode);
                    y -= 0.5;
                }
                y--;
            }
            // if parent shape exists - connect last branch shape to it
            if (chainParentShape != null)
            {
                ShapeConnectionType shapeConnectionType = BuilderUtills.defineConnectionTypeWithBranchParent(chainParentShape, lastBranchShape);
                visioManipulator.connectShapes(chainParentShape.shape, lastBranchShape.shape, ShapeForm.ARROW_LEFT, shapeConnectionType);
            }
            if (y < coreY)
            {
                coreY = y;
            }
            refrashCoreY(y);
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
        private ShapeWrapper buildIfElseTreeBranch(List<Node> nodes, ShapeWrapper chainParentShape, ShapeConnectionType ifElseConnectionType, double x, double y)
        {
            Node currentNode = null;
            ShapeWrapper lastBranchShape = null;
            for (int i = 0; i < nodes.Count; i++)
            {
                currentNode = nodes[i];
                if(i == 0)
                {
                    if(currentNode.shapeForm == ShapeForm.IF)
                    {
                        if(ifElseConnectionType == ShapeConnectionType.FROM_LEFT_TO_TOP)
                        {
                            x -= 1.2;
                        }
                        else
                        {
                            x += 1.2;
                            startX = x;
                        }
                        lastBranchShape = visioManipulator.dropShape(currentNode, x, y);
                        visioManipulator.connectShapes(lastBranchShape.shape, chainParentShape.shape, ShapeForm.LINE, ifElseConnectionType);
                        y--;
                        lastBranchShape = startIfElseBranch(currentNode, lastBranchShape, x, y);
                    }
                    else
                    {
                        //IMPORTANT reset global shape baccause current shape already connected manualy
                        endNodeBranch();
                        lastBranchShape = buildTree(currentNode, x, y);
                        visioManipulator.connectShapes(lastBranchShape.shape, chainParentShape.shape, ShapeForm.LINE, ifElseConnectionType);
                    }
                    if (lastBranchShape.isCommonShape())
                    {
                        //IMPORTANT call for connect next shape with FROM_TOP_TO_BOT
                        continueNodeBranch(lastBranchShape);
                    }
                    else
                    {
                        //IMPORTANT call because lastBranchShape isn't common shape so it can not be connected FROM_TOP_TO_BOT
                        shiftNodeBranch(lastBranchShape);
                    }
                    coreY -= 0.2;
                }
                else if (currentNode.isSimpleNode())
                {
                    if (lastBranchShape.isCommonShape())
                    {
                        //IMPOTRANT connect shape in one branch if last shape is common
                        continueNodeBranch(lastBranchShape);
                    }
                    lastBranchShape = buildTree(currentNode, x, y);
                    continueNodeBranch(lastBranchShape);
                }
                else
                {
                    lastBranchShape = buildTree(currentNode, x, y);
                    shiftNodeBranch(lastBranchShape);
                    y -= BuilderUtills.calcStatementHeight(currentNode);
                }
                y--;
                refrashCoreY(y);
            }
            return lastBranchShape;
        }
    }
}
