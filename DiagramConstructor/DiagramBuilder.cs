using Microsoft.Office.Interop.Visio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visio = Microsoft.Office.Interop.Visio;

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

        private Shape globalLastDropedShape;
        private NodeType globalLastDropedShapeType;
        private Shape globalLastChaineParentShape;

        private ShapeConnectionType connectionTypeForEndShape = ShapeConnectionType.FROM_TOP_TO_RIGHT;

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

            for (int i = 0; i < codeThree.main.Count; i++)
            {
                Node node = codeThree.main[i];
                if (i != 0 && globalLastChaineParentShape != null)
                {
                    globalLastDropedShape = null;
                }
                buildTree(node, globalLastChaineParentShape, coreX, coreY);
            }

            placeEndShape(codeThree.main);

            String documetsRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            visioApp.ActiveDocument.SaveAs(documetsRoot + @"\result.vsdx");
            visioApp.ActiveDocument.Close();
            visioStencil.Close();
        }

        public void placeBeginShape()
        {
            globalLastDropedShape = dropShape(begin, null, ShapeConnectionType.FROM_BOT_TO_TOP, "Начало", coreX, coreY);
            coreY -= 0.95;
        }

        public void placeEndShape(List<Node> mainBranch)
        {
            if (mainBranch[mainBranch.Count - 1].nodeType == NodeType.IF)
            {
                connectionTypeForEndShape = ShapeConnectionType.FROM_TOP_TO_CENTER;
            }

            //connect end shape manualy
            Shape endShape = dropShape(begin, null, ShapeConnectionType.FROM_BOT_TO_TOP, "Конец", coreX, coreY);

            if (globalLastChaineParentShape == null)
            {
                globalLastChaineParentShape = globalLastDropedShape;
                connectionTypeForEndShape = ShapeConnectionType.FROM_TOP_TO_BOT;
            }
            connectShapes(globalLastChaineParentShape, endShape, line, connectionTypeForEndShape);
        }

        public Shape buildTree(Node node, Shape lastParentShape, double x, double y)
        {
            Master figureMasterToAdd = getFigureMasterByNodeType(node.nodeType);
            Shape currentNodeShape = dropShape(figureMasterToAdd, globalLastDropedShape, ShapeConnectionType.FROM_BOT_TO_TOP, node.nodeText, x, y);
            y--;
            if (lastParentShape != null)
            {
                connectShapes(currentNodeShape, lastParentShape, line, ShapeConnectionType.FROM_RIGHT_TO_TOP);
            }
            //set last added shape (to connect with begin and end shapes)
            globalLastDropedShape = currentNodeShape;
            Shape lastBranchShape = null;
            if (node.nodeType == NodeType.IF)
            {
                addTextYesNo("Да", x - 0.7, y + 0.2);
                addTextYesNo("Нет", x + 0.7, y + 0.2);
                lastBranchShape = buildIfElseTreeBranch(node.childIfNodes, currentNodeShape, ShapeConnectionType.FROM_LEFT_TO_TOP, x - 1.2, y);
                Shape invisibleBlock = dropShape(littleInvisibleBlock, null, ShapeConnectionType.FROM_BOT_TO_CENTER, "", x, y - node.childIfNodes.Count);
                connectShapes(invisibleBlock, lastBranchShape, line, ShapeConnectionType.FROM_BOT_TO_CENTER);
                if (node.childElseNodes.Count != 0)
                {
                    lastBranchShape = buildIfElseTreeBranch(node.childElseNodes, currentNodeShape, ShapeConnectionType.FROM_RIGHT_TO_TOP, x + 1.2, y);
                    connectShapes(invisibleBlock, lastBranchShape, line, ShapeConnectionType.FROM_BOT_TO_CENTER);
                }
                else
                {
                    connectShapes(invisibleBlock, currentNodeShape, line, ShapeConnectionType.FROM_RIGHT_TO_CENTER);
                }
                globalLastDropedShape = invisibleBlock;
                lastBranchShape = invisibleBlock;
                currentNodeShape = invisibleBlock;
                globalLastChaineParentShape = invisibleBlock;
                coreY -= 0.5;
            }
            else if(node.nodeType != NodeType.COMMON && node.nodeType != NodeType.INOUTPUT)
            {
                globalLastChaineParentShape = currentNodeShape;
                if (node.nodeType == NodeType.WHILE)
                {
                    addTextYesNo("Да", x + 0.38, y - 0.5);
                    addTextYesNo("Нет", x + 0.7, y + 0.2);
                    y -= 0.2;
                    lastBranchShape = buldTreeBranch(node, currentNodeShape, ShapeConnectionType.FROM_RIGHT_TO_LEFT, x, y);
                }
                else
                {
                    lastBranchShape = buldTreeBranch(node, currentNodeShape, ShapeConnectionType.FROM_BOT_TO_LEFT, x, y);
                }
                //connect last droped shape (in branch if it exists)
                if (lastParentShape != null)
                {
                    connectShapes(lastParentShape, globalLastDropedShape, line, ShapeConnectionType.FROM_TOP_TO_RIGHT);
                }
                coreY -= 0.2;
            }
            //set last added shape as new code branch parent (when last brunch is ended by 'buldTreeBranch')
            coreY = y;
            return lastBranchShape;
        }

        public Shape buildIfElseTreeBranch(List<Node> nodes, Shape chainParentShape, ShapeConnectionType ifElseConnectionType, double x, double y)
        {
            Shape lastBranchShape = null;
            for(int i = 0; i < nodes.Count; i++)
            {
                Node node = nodes[i];
                if (node.nodeType == NodeType.COMMON || node.nodeType == NodeType.INOUTPUT)
                {
                    Master figureMasterToAdd = getFigureMasterByNodeType(node.nodeType);
                    if (i == 0)
                    {
                        lastBranchShape = dropShape(figureMasterToAdd, null, ShapeConnectionType.FROM_BOT_TO_TOP, node.nodeText, x, y);
                        //connect first if/else branch shape manualy 
                        connectShapes(lastBranchShape, chainParentShape, line, ifElseConnectionType);
                        globalLastDropedShape = lastBranchShape;
                    }
                    else
                    {
                        globalLastDropedShape = dropShape(figureMasterToAdd, globalLastDropedShape, ShapeConnectionType.FROM_BOT_TO_TOP, node.nodeText, x, y);
                    }
                    lastBranchShape = globalLastDropedShape;
                    globalLastDropedShapeType = node.nodeType;
                }
                else
                {
                    buildTree(node, null, x, y);
                    y -= Math.Max(node.childNodes.Count, Math.Max(node.childIfNodes.Count, node.childElseNodes.Count));
                }
                y--;
                if (y < coreY)
                {
                    coreY = y;
                }
            }
            return lastBranchShape;
        }

        public Shape buldTreeBranch(Node node, Shape chainParentShape, ShapeConnectionType lastShapeConnectionType, double x, double y)
        {
            Shape lastBranchShape = chainParentShape;
            Page visioPage = visioApp.ActivePage;
            foreach (Node currentNode in node.childNodes)
            {
                if (currentNode.nodeType == NodeType.COMMON || currentNode.nodeType == NodeType.INOUTPUT)
                {
                    Master figureMasterToAdd = getFigureMasterByNodeType(currentNode.nodeType);
                    globalLastDropedShape = dropShape(figureMasterToAdd, globalLastDropedShape, ShapeConnectionType.FROM_BOT_TO_TOP, currentNode.nodeText, x, y);
                    lastBranchShape = globalLastDropedShape;
                    globalLastDropedShapeType = currentNode.nodeType;
                }
                else
                {
                    lastBranchShape = buildTree(currentNode, null, x, y);
                    y -= Math.Max(currentNode.childNodes.Count, Math.Max(currentNode.childIfNodes.Count, currentNode.childElseNodes.Count));
                    y -= 0.7;
                }
                y--;
            }
            if (chainParentShape != null)
            {
                if(node.nodeType != NodeType.COMMON 
                    && node.nodeType != NodeType.INOUTPUT 
                    && globalLastDropedShapeType != NodeType.COMMON)
                {
                    lastShapeConnectionType = ShapeConnectionType.FROM_RIGHT_TO_LEFT;
                }
                connectShapes(chainParentShape, lastBranchShape, arrowLeft, lastShapeConnectionType);
            }
            if (y < coreY)
            {
                coreY = y;
            }
            return lastBranchShape;
        }

        public Shape dropShape(Master figure, Shape prevShape, ShapeConnectionType connectionType, String text, double x, double y)
        {
            Page visioPage = visioApp.ActivePage;
            Shape shapeToDrop = visioPage.Drop(figure, x, y);
            shapeToDrop.Text = text;
            if (prevShape != null)
            {
                connectShapes(shapeToDrop, prevShape, line, connectionType);
            }
            return shapeToDrop;
        }

        public Shape addTextYesNo(String text, double x, double y)
        {
            Page visioPage = visioApp.ActivePage;
            Shape shape = dropShape(yesNoTextField, null, ShapeConnectionType.FROM_BOT_TO_TOP, text, x, y);
            return shape;
        }

        public Shape addTextField(String text, double x, double y)
        {
            Page visioPage = visioApp.ActivePage;
            Shape shape = dropShape(textField, null, ShapeConnectionType.FROM_BOT_TO_TOP, text, x, y);
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
