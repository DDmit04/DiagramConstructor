﻿using Microsoft.Office.Interop.Visio;
using System;
using System.Collections.Generic;

namespace DiagramConstructor
{
    class DiagramBuilder
    {
        private Application visioApp = new Application();
        private Document visioStencil;

        private Master begin;
        private Master process;
        private Master inoutPut;
        private Master ifState;
        private Master forState;
        private Master program;
        private Master connector;
        private Master pageConnector;
        private Master line;
        private Master textField;
        private Master yesNoTextField;
        private Master arrowLeft;
        private Master arrowRight;
        private Master littleInvisibleBlock;

        private ShapeWrapper globalLastDropedShapeV2;

        private double coreX = 4.25;
        private double coreY = 11;

        private CodeThree codeThree;

        public NodeType ShapeConn { get; private set; }

        public DiagramBuilder(CodeThree codeThree)
        {
            this.codeThree = codeThree;
            visioStencil = visioApp.Documents.OpenEx(
               AppDomain.CurrentDomain.BaseDirectory + @"\Help\Shapes.vssx",
               (short)VisOpenSaveArgs.visOpenDocked
            );
            begin = visioStencil.Masters.get_ItemU(@"Начало");
            process = visioStencil.Masters.get_ItemU(@"Процесс");
            inoutPut = visioStencil.Masters.get_ItemU(@"Ввод/вывод");
            ifState = visioStencil.Masters.get_ItemU(@"Ветвление");
            forState = visioStencil.Masters.get_ItemU(@"Блок модификаций");
            program = visioStencil.Masters.get_ItemU(@"Предопределенный процесс");
            connector = visioStencil.Masters.get_ItemU(@"Соединитель");
            pageConnector = visioStencil.Masters.get_ItemU(@"Межстраничный соединитель");
            line = visioStencil.Masters.get_ItemU(@"line");
            textField = visioStencil.Masters.get_ItemU(@"textField");
            yesNoTextField = visioStencil.Masters.get_ItemU(@"yesNo");
            arrowLeft = visioStencil.Masters.get_ItemU(@"arrowLeft");
            arrowRight = visioStencil.Masters.get_ItemU(@"arrowRight");
            littleInvisibleBlock = visioStencil.Masters.get_ItemU(@"LittleInvisibleBlock");
        }

        public void buildDiagram()
        {
            visioApp.Documents.Add("");
            Page visioPage = visioApp.ActivePage;

            placeBeginShape();

            for (int i = 0; i < codeThree.methodNodes.Count; i++)
            {
                Node node = codeThree.methodNodes[i];
                globalLastDropedShapeV2 = buildTreeV2(node, null, coreX, coreY);
            }

            placeEndShape(codeThree.methodNodes);

            String documetsRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            visioApp.ActiveDocument.SaveAs(documetsRoot + @"\result.vsdx");
            visioApp.ActiveDocument.Close();
            visioStencil.Close();
        }

        public void placeBeginShape()
        {
            dropShapeV2(begin, "Начало", coreX, coreY);
            coreY -= 0.95;
        }

        public void placeEndShape(List<Node> mainBranch)
        {
            ShapeWrapper endShape = dropShapeV2(begin, "Конец", coreX, coreY);
            ShapeConnectionType shapeConnection = ShapeConnectionType.FROM_TOP_TO_BOT;
            if (globalLastDropedShapeV2.shapeType != NodeType.COMMON && globalLastDropedShapeV2.shapeType != NodeType.INOUTPUT)
            {
                shapeConnection = ShapeConnectionType.FROM_TOP_TO_RIGHT;
            }
            connectShapes(globalLastDropedShapeV2.shape, endShape.shape, line, shapeConnection);
        }

        public ShapeWrapper buildTreeV2(Node node, ShapeWrapper lastParentShape, double x, double y)
        {
            Master figureMasterToAdd = getFigureMasterByNodeType(node.nodeType);
            ShapeWrapper currentNodeShape = dropShapeV2(figureMasterToAdd, node, x, y);
            if (globalLastDropedShapeV2 != null)
            {
                ShapeConnectionType shapeConnectionType = ShapeConnectionType.FROM_TOP_TO_RIGHT;
                NodeType globalShapeType = globalLastDropedShapeV2.shapeType;
                if ((globalShapeType == NodeType.FOR || globalShapeType == NodeType.WHILE) && currentNodeShape.shapeType == NodeType.COMMON)
                {
                    shapeConnectionType = ShapeConnectionType.FROM_TOP_TO_RIGHT;
                }
                else if((globalShapeType == NodeType.FOR || globalShapeType == NodeType.WHILE) && (currentNodeShape.shapeType == NodeType.FOR || currentNodeShape.shapeType == NodeType.WHILE))
                {
                    shapeConnectionType = ShapeConnectionType.FROM_RIGHT_TO_TOP;
                }
                else if ((globalShapeType == NodeType.COMMON || globalShapeType == NodeType.INOUTPUT || globalShapeType == NodeType.PROGRAM) 
                    && (currentNodeShape.shapeType == NodeType.COMMON || currentNodeShape.shapeType == NodeType.INOUTPUT || currentNodeShape.shapeType == NodeType.PROGRAM))
                {
                    shapeConnectionType = ShapeConnectionType.FROM_TOP_TO_BOT;
                }
                connectShapes(globalLastDropedShapeV2.shape, currentNodeShape.shape, line, shapeConnectionType);
            }
            y--;
            ShapeWrapper lastBranchShape = currentNodeShape;
            if (node.nodeType == NodeType.IF)
            {
                addTextYesNo("Да", x - 0.7, y + 1 + 0.2);
                addTextYesNo("Нет", x + 0.7, y + 1 + 0.2);
                lastBranchShape = buildIfElseTreeBranchV2(node.childIfNodes, currentNodeShape, ShapeConnectionType.FROM_LEFT_TO_TOP, x - 1.2, y);
                int statementHeight = Math.Max(node.childIfNodes.Count, node.childElseNodes.Count);
                ShapeWrapper invisibleBlock = dropShapeV2(littleInvisibleBlock, "", x, coreY + 1 - statementHeight);
                if(lastBranchShape.shapeType == NodeType.COMMON || lastBranchShape.shapeType == NodeType.INOUTPUT)
                {
                    connectShapes(invisibleBlock.shape, lastBranchShape.shape, line, ShapeConnectionType.FROM_BOT_TO_CENTER);
                }
                else
                {
                    connectShapes(invisibleBlock.shape, lastBranchShape.shape, line, ShapeConnectionType.FROM_RIGHT_TO_CENTER);
                }
                if (node.childElseNodes.Count != 0)
                {
                    lastBranchShape = buildIfElseTreeBranchV2(node.childElseNodes, currentNodeShape, ShapeConnectionType.FROM_RIGHT_TO_TOP, x + 1.2, y);
                    connectShapes(invisibleBlock.shape, lastBranchShape.shape, line, ShapeConnectionType.FROM_BOT_TO_CENTER);
                }
                else
                {
                    connectShapes(invisibleBlock.shape, currentNodeShape.shape, line, ShapeConnectionType.FROM_RIGHT_TO_CENTER);
                }
                lastBranchShape = invisibleBlock;
                coreY -= 0.5;
            } 
            else if (node.nodeType == NodeType.WHILE)
            {
                addTextYesNo("Да", x + 0.38, y + 1 - 0.5);
                addTextYesNo("Нет", x + 0.7, y + 1 + 0.2);
                y -= 0.2;
                lastBranchShape = currentNodeShape;
                buildTreeBranchV2(node, currentNodeShape, x, y);
                coreY -= 0.2;
            }
            else if(node.nodeType == NodeType.FOR)
            {
                lastBranchShape = currentNodeShape;
                buildTreeBranchV2(node, currentNodeShape, x, y);
                coreY -= 0.2;
            }
            if (y < coreY)
            {
                coreY = y;
            }
            globalLastDropedShapeV2 = lastBranchShape;
            return lastBranchShape;
        }

        public ShapeWrapper buildTreeBranchV2(Node node, ShapeWrapper chainParentShape, double x, double y)
        {
            ShapeWrapper lastBranchShape = null;
            ShapeWrapper prevShape = chainParentShape;
            Page visioPage = visioApp.ActivePage;
            if(node.childNodes.Count == 0)
            {
                lastBranchShape = buildTreeV2(node, chainParentShape, x, y);
                y--;
            }
            for (int i = 0; i < node.childNodes.Count; i++)
            {
                
                Node currentNode = node.childNodes[i];
                if (currentNode.nodeType == NodeType.COMMON || currentNode.nodeType == NodeType.INOUTPUT)
                {
                    //Master figureMasterToAdd = getFigureMasterByNodeType(currentNode.nodeType);
                    //lastBranchShape = dropShapeV2(figureMasterToAdd, currentNode, x, y);
                    //connectShapes(prevShape.shape, lastBranchShape.shape, line, ShapeConnectionType.FROM_TOP_TO_BOT);
                    lastBranchShape = buildTreeV2(currentNode, chainParentShape, x, y);
                }
                else
                {
                    lastBranchShape = buildTreeV2(currentNode, chainParentShape, x, y);
                    y -= Math.Max(currentNode.childNodes.Count, Math.Max(currentNode.childIfNodes.Count, currentNode.childElseNodes.Count));
                    y -= 0.7;
                }
                prevShape = lastBranchShape;
                y--;
            }
            if (chainParentShape != null)
            {
                ShapeConnectionType shapeConnectionType = ShapeConnectionType.FROM_BOT_TO_LEFT;
                if (chainParentShape.shapeType == NodeType.FOR && lastBranchShape.shapeType == NodeType.COMMON)
                {
                    shapeConnectionType = ShapeConnectionType.FROM_BOT_TO_LEFT;
                }
                else if (chainParentShape.shapeType == NodeType.FOR && lastBranchShape.shapeType != NodeType.COMMON)
                {
                    shapeConnectionType = ShapeConnectionType.FROM_RIGHT_TO_LEFT;
                }
                else if (chainParentShape.shapeType == NodeType.WHILE)
                {
                    shapeConnectionType = ShapeConnectionType.FROM_BOT_TO_CENTER;
                }
                connectShapes(chainParentShape.shape, lastBranchShape.shape, arrowLeft, shapeConnectionType);
            }
            if (y < coreY)
            {
                coreY = y;
            }
            return lastBranchShape;
        }

        public ShapeWrapper buildIfElseTreeBranchV2(List<Node> nodes, ShapeWrapper chainParentShape, ShapeConnectionType ifElseConnectionType, double x, double y)
        {
            ShapeWrapper lastBranchShape = null;
            for (int i = 0; i < nodes.Count; i++)
            {
                Node node = nodes[i];
                if (node.nodeType == NodeType.COMMON || node.nodeType == NodeType.INOUTPUT)
                {
                    Master figureMasterToAdd = getFigureMasterByNodeType(node.nodeType);
                    if (i == 0)
                    {
                        lastBranchShape = dropShapeV2(figureMasterToAdd, node, x, y);
                        //connect first if/else branch shape manualy 
                        connectShapes(lastBranchShape.shape, chainParentShape.shape, line, ifElseConnectionType);
                    }
                    else
                    {
                        lastBranchShape = buildTreeV2(node, chainParentShape, x, y);
                    }
                }
                else
                {
                    lastBranchShape = buildTreeV2(node, lastBranchShape, x, y);
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

        public ShapeWrapper dropShapeV2(Master figure, String text, double x, double y)
        {
            Page visioPage = visioApp.ActivePage;
            Shape shapeToDrop = visioPage.Drop(figure, x, y);
            shapeToDrop.Text = text;
            ShapeWrapper dropedShape = new ShapeWrapper(shapeToDrop, NodeType.COMMON);
            return dropedShape;
        }

        public ShapeWrapper dropShapeV2(Master figure, Node node, double x, double y)
        {
            Page visioPage = visioApp.ActivePage;
            Shape shapeToDrop = visioPage.Drop(figure, x, y);
            NodeType newShapeType = NodeType.COMMON;
            if (node != null)
            {
                newShapeType = node.nodeType;
                shapeToDrop.Text = node.nodeText;
            }
            ShapeWrapper dropedShape = new ShapeWrapper(shapeToDrop, newShapeType);
            return dropedShape;
        }

        public ShapeWrapper addTextYesNo(String text, double x, double y)
        {
            Page visioPage = visioApp.ActivePage;
            ShapeWrapper shape = dropShapeV2(yesNoTextField, text, x, y);
            return shape;
        }

        public ShapeWrapper addTextField(String text, double x, double y)
        {
            Page visioPage = visioApp.ActivePage;
            ShapeWrapper shape = dropShapeV2(textField, text, x, y);
            return shape;
        }

        public void connectShapes(Shape shapeFrom, Shape shapeTo, Master connectorMaster, ShapeConnectionType connectionType)
        {
            VisCellIndices conectionToType = VisCellIndices.visAlignTop;
            VisCellIndices conectionFromType = VisCellIndices.visAlignBottom;
            switch (connectionType)
            {
                case ShapeConnectionType.FROM_BOT_TO_TOP:
                    conectionToType = VisCellIndices.visAlignTop;
                    conectionFromType = VisCellIndices.visAlignBottom;
                    break;
                case ShapeConnectionType.FROM_LEFT_TO_TOP:
                    conectionToType = VisCellIndices.visAlignTop;
                    conectionFromType = VisCellIndices.visAlignLeft;
                    break;
                case ShapeConnectionType.FROM_RIGHT_TO_TOP:
                    conectionToType = VisCellIndices.visAlignTop;
                    conectionFromType = VisCellIndices.visAlignRight;
                    break;
                case ShapeConnectionType.FROM_BOT_TO_LEFT:
                    conectionToType = VisCellIndices.visAlignLeft;
                    conectionFromType = VisCellIndices.visAlignBottom;
                    break;
                case ShapeConnectionType.FROM_TOP_TO_RIGHT:
                    conectionToType = VisCellIndices.visAlignRight;
                    conectionFromType = VisCellIndices.visAlignTop;
                    break;
                case ShapeConnectionType.FROM_BOT_TO_CENTER:
                    conectionToType = VisCellIndices.visAlignCenter;
                    conectionFromType = VisCellIndices.visAlignBottom;
                    break;
                case ShapeConnectionType.FROM_RIGHT_TO_CENTER:
                    conectionToType = VisCellIndices.visAlignCenter;
                    conectionFromType = VisCellIndices.visAlignRight;
                    break;
                case ShapeConnectionType.FROM_TOP_TO_CENTER:
                    conectionToType = VisCellIndices.visAlignCenter;
                    conectionFromType = VisCellIndices.visAlignTop;
                    break;
                case ShapeConnectionType.FROM_RIGHT_TO_LEFT:
                    conectionToType = VisCellIndices.visAlignLeft;
                    conectionFromType = VisCellIndices.visAlignRight;
                    break;
                case ShapeConnectionType.FROM_TOP_TO_BOT:
                    conectionToType = VisCellIndices.visAlignBottom;
                    conectionFromType = VisCellIndices.visAlignTop;
                    break;
            }
            ConnectWithDynamicGlueAndConnector(shapeFrom, shapeTo, connectorMaster, conectionToType, conectionFromType);
        }

    public Master getFigureMasterByNodeType(NodeType nodeType)
        {
            Master resultFigure = begin;
            switch (nodeType)
            {
                case NodeType.COMMON:
                    resultFigure = process;
                    break;
                case NodeType.IF:
                    resultFigure = ifState;
                    break;
                case NodeType.INOUTPUT:
                    resultFigure = inoutPut;
                    break;
                case NodeType.WHILE:
                    resultFigure = ifState;
                    break;
                 case NodeType.FOR:
                    resultFigure = forState;
                    break;
                case NodeType.PROGRAM:
                    resultFigure = program;
                    break;
            }
            return resultFigure;
        }

        private void ConnectWithDynamicGlueAndConnector(Shape shapeFrom, Shape shapeTo, Master connectorMaster, VisCellIndices fromPoint, VisCellIndices toPoint)
        {
            Cell beginXCell;
            Cell endXCell;
            Shape connector;

            Page visioPage = visioApp.ActivePage;
            connector = visioPage.Drop(connectorMaster, 4.25, 11);

            beginXCell = connector.get_CellsSRC(
                (short)VisSectionIndices.visSectionObject,
                (short)VisRowIndices.visRowXForm1D,
                (short)VisCellIndices.vis1DBeginX);

            beginXCell.GlueTo(shapeFrom.get_CellsSRC(
                (short) VisSectionIndices.visSectionObject,
                (short) VisRowIndices.visRowAlign,
                (short) fromPoint));

            endXCell = connector.get_CellsSRC(
                (short)VisSectionIndices.visSectionObject,
                (short)VisRowIndices.visRowXForm1D,
                (short)VisCellIndices.vis1DEndX);

            endXCell.GlueTo(shapeTo.get_CellsSRC(
                (short) VisSectionIndices.visSectionObject,
                (short) VisRowIndices.visRowAlign,
                (short) toPoint));
        }

    }
}
