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

        private Shape pervShape;

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
        }

        public void buildDiagram()
        {
            visioApp.Documents.Add("");
            Page visioPage = visioApp.ActivePage;

            pervShape = dropFigure(visioPage, begin, null, "Начало", coreX, coreY);
            coreY--;

            Shape lastFigure = buldSimpleDiagram(codeThree.main, null, coreX, coreY);

            dropFigure(visioPage, begin, lastFigure, "Конец", coreX, coreY);

            String documetsRoot = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            visioApp.ActiveDocument.SaveAs(documetsRoot + @"\result.vsdx");
            //visioApp.ActiveDocument.Close();
            //visioStencil.Close();
        }

        public Shape buldSimpleDiagram(List<Node> nodes, Shape chainParentShape, double x, double y)
        {
            Page visioPage = visioApp.ActivePage;
            Shape preEndFigure = chainParentShape;
            foreach (Node node in nodes)
            {
                Master figureMasterToAdd = getFigureMasterByNodeType(node.nodeType);
                preEndFigure = dropFigure(visioPage, figureMasterToAdd, preEndFigure, node.nodeText, x, y);
                pervShape = preEndFigure;
                if (node.nodeType == NodeType.IF)
                {
                    addTextBox("Да", x - 1.2, y + 0.2);
                    buldSimpleDiagram(node.childIfNodes, pervShape, x - 1.3, y - 1);
                    addTextBox("Нет", x + 1.2, y + 0.2);
                    buldSimpleDiagram(node.childElseNodes, pervShape, x + 1.3, y - 1);
                    y -= Math.Max(node.childIfNodes.Count, node.childElseNodes.Count);
                    y -= 0.2;
                }
                else if (node.nodeType == NodeType.FOR)
                {
                    buldSimpleDiagram(node.childNodes, pervShape, x, y - 1);
                    y -= node.childNodes.Count;
                    y -= 0.2;
                }
                else if (node.nodeType == NodeType.WHILE)
                {
                    y -= 0.2;
                    addTextBox("Да", x + 0.3, y - 0.3);
                    addTextBox("Нет", x + 1.2, y + 0.4);
                    buldSimpleDiagram(node.childNodes, pervShape, x, y - 1);
                    y -= node.childNodes.Count;
                    y -= 0.2;
                }
                y--;
            }
            if (chainParentShape != null)
            {
                //TO DO normalize line connections
                //ConnectWithDynamicGlueAndConnector(chainParentShape, pervShape);
                //chainParentShape.AutoConnect(pervShape, VisAutoConnectDir.visAutoConnectDirNone, line);
            }
            if (y < coreY)
            {
                coreY = y;
            }
            return preEndFigure;
        }

    public Shape dropFigure(Page visioPage, Master figure, Shape prevFigure, String text, double x, double y)
        {
            Shape shapeToDrop = visioPage.Drop(figure, x, y);
            shapeToDrop.Text = text;
            if (prevFigure != null)
            {
                //TO DO normalize line connections
                //ConnectWithDynamicGlueAndConnector(shapeToDrop, pervShape);
                //shapeToDrop.AutoConnect(prevFigure, VisAutoConnectDir.visAutoConnectDirNone, line);
            }
            return shapeToDrop;
        }

    public Shape addTextBox(String text, double x, double y)
        {
            Page visioPage = visioApp.ActivePage;
            Shape shape = dropFigure(visioPage, process, null, text, x, y);
            shape.LineStyle = "None";
            shape.Style = "None";
            return shape;
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

        private void ConnectWithDynamicGlueAndConnector(Shape shapeFrom, Shape shapeTo)
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
                (short)VisSectionIndices.visSectionObject,
                (short)VisRowIndices.visRowXFormOut,
                (short)VisCellIndices.visXFormPinX));

            endXCell = connector.get_CellsSRC(
                (short)VisSectionIndices.visSectionObject,
                (short)VisRowIndices.visRowXForm1D,
                (short)VisCellIndices.vis1DEndX);

            endXCell.GlueTo(shapeTo.get_CellsSRC(
                (short)VisSectionIndices.visSectionObject,
                (short)VisRowIndices.visRowXFormOut,
                (short)VisCellIndices.visXFormPinX));
        }

    }
}
