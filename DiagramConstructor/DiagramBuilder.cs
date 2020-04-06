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

        private Shape pervShape;

        private Shape globalLastDropedShape;
        private Shape globalLastChaneParentShape;

        private double coreX = 4.25;
        private double coreY = 11;

        private CodeThree codeThree;

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
        }

        public void buildDiagram()
        {
            visioApp.Documents.Add("");
            Page visioPage = visioApp.ActivePage;

            globalLastDropedShape = dropShape(begin, null, ShapeConnectionType.USAL, "Начало", coreX, coreY);
            coreY--;

            foreach (Node node in codeThree.main)
            {
                qwe(node, coreX, coreY);
            }

            Shape endShape = dropShape(begin, null, ShapeConnectionType.USAL, "Конец", coreX, coreY);

            //INVERTED SHAPES!!!
            connectShaps(globalLastChaneParentShape, endShape, ShapeConnectionType.OUT_PARENT);

            String documetsRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            visioApp.ActiveDocument.SaveAs(documetsRoot + @"\result.vsdx");
            visioApp.ActiveDocument.Close();
            visioStencil.Close();
        }

        public void qwe(Node node, double x, double y)
        {
            Master figureMasterToAdd = getFigureMasterByNodeType(node.nodeType);
            if (node.nodeType == NodeType.IF)
            {
            }
            else if(node.nodeType != NodeType.COMMON && node.nodeType != NodeType.INOUTPUT)
            {
                Shape shapeCopy = dropShape(figureMasterToAdd, globalLastDropedShape, ShapeConnectionType.USAL, node.nodeText, x, y);
                globalLastDropedShape = shapeCopy;
                if (node.nodeType == NodeType.FOR)
                { 
                }
                else if(node.nodeType == NodeType.WHILE)
                {
                    addTextBox("Да", x + 0.38, y - 0.5);
                    addTextBox("Нет", x + 0.7, y + 0.2);
                    y -= 0.2;
                }
                buldSimpleDiagram(node.childNodes, globalLastDropedShape, x, y - 1);
                if (globalLastChaneParentShape != null)
                {
                    connectShaps(globalLastChaneParentShape, globalLastDropedShape, ShapeConnectionType.OUT_PARENT);
                }
                globalLastChaneParentShape = shapeCopy;
                globalLastDropedShape = null;
            }
            else
            {
                globalLastDropedShape = dropShape(figureMasterToAdd, globalLastDropedShape, ShapeConnectionType.USAL, node.nodeText, x, y);
            }
        }

        public void buldSimpleDiagram(List<Node> nodes, Shape chainParentShape, double x, double y)
        {
            Page visioPage = visioApp.ActivePage;
            foreach (Node node in nodes)
            {
                if (node.nodeType == NodeType.COMMON)
                {
                    Master figureMasterToAdd = getFigureMasterByNodeType(node.nodeType);
                    globalLastDropedShape = dropShape(figureMasterToAdd, globalLastDropedShape, ShapeConnectionType.USAL, node.nodeText, x, y);
                }
                else
                {
                    qwe(node, x, y);
                    y -= node.childNodes.Count;
                }
                y--;
            }
            if (chainParentShape != null)
            {
                connectShaps(chainParentShape, globalLastDropedShape, ShapeConnectionType.BACK_PARENT);
            }
            if (y < coreY)
            {
                coreY = y;
            }
        }

    public Shape dropShape(Master figure, Shape prevShape, ShapeConnectionType connectionType, String text, double x, double y)
        {
            Page visioPage = visioApp.ActivePage;
            Shape shapeToDrop = visioPage.Drop(figure, x, y);
            shapeToDrop.Text = text;
            if (prevShape != null)
            {
                connectShaps(shapeToDrop, prevShape, connectionType);
            }
            return shapeToDrop;
        }

    public Shape addTextBox(String text, double x, double y)
        {
            Page visioPage = visioApp.ActivePage;
            Shape shape = dropShape(yesNoTextField, null, ShapeConnectionType.USAL, text, x, y);
            return shape;
        }

    public void connectShaps(Shape shapeFrom, Shape shapeTo, ShapeConnectionType connectionType)
        {
            VisCellIndices conectionToType = VisCellIndices.visAlignTop;
            VisCellIndices conectionFromType = VisCellIndices.visAlignBottom;
            switch (connectionType)
            {
                case ShapeConnectionType.USAL:
                    conectionToType = VisCellIndices.visAlignTop;
                    conectionFromType = VisCellIndices.visAlignBottom;
                    break;
                case ShapeConnectionType.IF_BRANCH:
                    conectionToType = VisCellIndices.visAlignTop;
                    conectionFromType = VisCellIndices.visAlignLeft;
                    break;
                case ShapeConnectionType.ELSE_BRANCH:
                    conectionToType = VisCellIndices.visAlignTop;
                    conectionFromType = VisCellIndices.visAlignRight;
                    break;
                case ShapeConnectionType.BACK_PARENT:
                    conectionToType = VisCellIndices.visAlignLeft;
                    conectionFromType = VisCellIndices.visAlignBottom;
                    break;
                //INVERTED ALIGNS!!!
                case ShapeConnectionType.OUT_PARENT:
                    conectionToType = VisCellIndices.visAlignRight;
                    conectionFromType = VisCellIndices.visAlignTop;
                    break;
            }
            ConnectWithDynamicGlueAndConnector(shapeFrom, shapeTo, conectionToType, conectionFromType);
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

        private void ConnectWithDynamicGlueAndConnector(Shape shapeFrom, Shape shapeTo, VisCellIndices fromPoint, VisCellIndices toPoint)
        {
            Cell beginXCell;
            Cell endXCell;
            Shape connector;

            Page visioPage = visioApp.ActivePage;
            connector = visioPage.Drop(line, 4, 4);

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
