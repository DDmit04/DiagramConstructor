using Microsoft.Office.Interop.Visio;
using System;

namespace DiagramConstructor.actor
{
    class VisioManipulator
    {

        private Application visioApp;
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
        private Master smallTextField;
        private Master arrowLeft;
        private Master arrowRight;
        private Master littleInvisibleBlock;

        public void openDocument()
        {
            this.visioApp = new Application();
            this.visioStencil = visioApp.Documents.OpenEx(
               AppDomain.CurrentDomain.BaseDirectory + @"\Help\Shapes.vssx",
               (short)VisOpenSaveArgs.visOpenDocked
            );
            this.begin = visioStencil.Masters.get_ItemU(@"Начало");
            this.process = visioStencil.Masters.get_ItemU(@"Процесс");
            this.inoutPut = visioStencil.Masters.get_ItemU(@"Ввод/вывод");
            this.ifState = visioStencil.Masters.get_ItemU(@"Ветвление");
            this.forState = visioStencil.Masters.get_ItemU(@"Блок модификаций");
            this.program = visioStencil.Masters.get_ItemU(@"Предопределенный процесс");
            this.connector = visioStencil.Masters.get_ItemU(@"Соединитель");
            this.pageConnector = visioStencil.Masters.get_ItemU(@"Межстраничный соединитель");
            this.line = visioStencil.Masters.get_ItemU(@"line");
            this.textField = visioStencil.Masters.get_ItemU(@"textField");
            this.smallTextField = visioStencil.Masters.get_ItemU(@"yesNo");
            this.arrowLeft = visioStencil.Masters.get_ItemU(@"arrowLeft");
            this.arrowRight = visioStencil.Masters.get_ItemU(@"arrowRight");
            this.littleInvisibleBlock = visioStencil.Masters.get_ItemU(@"LittleInvisibleBlock");
            this.visioApp.Documents.Add("");
        }

        public String closeDocument()
        {
            String resulrFilePath = Configuration.customFilePath + @"\result" + DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss_fff") + ".vsdx";
            if (Configuration.testRun)
            {
                resulrFilePath = Configuration.customFilePath + @"\result_test" + ".vsdx";
            }
            visioApp.ActiveDocument.SaveAs(resulrFilePath);
            visioStencil.Close();
            visioApp.ActiveDocument.Close();
            visioApp.Quit();
            return resulrFilePath;
        }

        /// <summary>
        /// Place common shape with text
        /// </summary>
        /// <param name="viosioShapeForm">visio shape form to place</param>
        /// <param name="text">shape text</param>
        /// <param name="x">shape x</param>
        /// <param name="y">shape y</param>
        /// <returns>placed shape</returns>
        public ShapeWrapper dropShape(ShapeForm viosioShapeForm, String text, double x, double y)
        {
            Master shapeMaster = getShapeMasterByShapeType(viosioShapeForm);
            Page visioPage = visioApp.ActivePage;
            Shape shapeToDrop = visioPage.Drop(shapeMaster, x, y);
            shapeToDrop.Text = text;
            ShapeWrapper dropedShape = new ShapeWrapper(shapeToDrop, viosioShapeForm);
            return dropedShape;
        }

        /// <summary>
        /// Place shape based on node
        /// </summary>
        /// <param name="shapeMaster">shape master to place</param>
        /// <param name="node">shape node</param>
        /// <param name="x">shape x</param>
        /// <param name="y">shape x</param>
        /// <returns>placed shape</returns>
        public ShapeWrapper dropShape(Node node, double x, double y)
        {
            Master shapeMaster = begin;
            Page visioPage = visioApp.ActivePage;
            ShapeForm newShapeType = ShapeForm.BEGIN;
            if(node != null)
            {
                shapeMaster = getShapeMasterByShapeType(node.shapeForm);
            }
            Shape shapeToDrop = visioPage.Drop(shapeMaster, x, y);
            if (node != null)
            {
                newShapeType = node.shapeForm;
                shapeToDrop.Text = node.nodeText;
            }
            ShapeWrapper dropedShape = new ShapeWrapper(shapeToDrop, newShapeType);
            return dropedShape;
        }

        /// <summary>
        /// Place little text field
        /// </summary>
        /// <param name="text">shape text</param>
        /// <param name="x">shape x</param>
        /// <param name="y">shape y</param>
        /// <returns>placed shape</returns>
        public ShapeWrapper addSmallTextField(String text, double x, double y)
        {
            Page visioPage = visioApp.ActivePage;
            ShapeWrapper shape = dropShape(ShapeForm.SMALL_TEXT_FIELD, text, x, y);
            return shape;
        }

        /// <summary>
        /// Place common text field
        /// </summary>
        /// <param name="text">shape text</param>
        /// <param name="x">shape x</param>
        /// <param name="y">shape y</param>
        /// <returns>placed shape</returns>
        public ShapeWrapper addTextField(String text, double x, double y)
        {
            Page visioPage = visioApp.ActivePage;
            ShapeWrapper shape = dropShape(ShapeForm.TEXT_FIELD, text, x, y);
            return shape;
        }

        /// <summary>
        /// Connect two shapes 
        /// </summary>
        /// <param name="shapeFrom">first shape to connect</param>
        /// <param name="shapeTo">sec shape to connect</param>
        /// <param name="connectorMaster">shape wich connect shapes</param>
        /// <param name="connectionType">shape connection type</param>
        public void connectShapes(Shape shapeFrom, Shape shapeTo, ShapeForm shapeForm, ShapeConnectionType connectionType)
        {
            Master connectorMaster = getShapeMasterByShapeType(shapeForm);
            VisCellIndices conectionToType = VisCellIndices.visAlignTop;
            VisCellIndices conectionFromType = VisCellIndices.visAlignBottom;
            BuilderUtills.getCellsAlignsFromConnectionType(out conectionToType, out conectionFromType, connectionType);
            ConnectWithDynamicGlueAndConnector(shapeFrom, shapeTo, connectorMaster, conectionToType, conectionFromType);
        }

        /// <summary>
        /// Connect last if/else branch shape with invisibe shape (connect both branches in one place)
        /// </summary>
        /// <param name="invisibleBlock">invisible block shape wrapper</param>
        /// <param name="lastBranchShape">last branch shape wrapper</param>
        public void connectLastShapeToInvisibleBlock(ShapeWrapper invisibleBlock, ShapeWrapper lastBranchShape)
        {
            if (lastBranchShape.isCommonShape())
            {
                connectShapes(invisibleBlock.shape, lastBranchShape.shape, ShapeForm.LINE, ShapeConnectionType.FROM_BOT_TO_CENTER);
            }
            else
            {
                connectShapes(invisibleBlock.shape, lastBranchShape.shape, ShapeForm.LINE, ShapeConnectionType.FROM_RIGHT_TO_CENTER);
            }
        }

        /// <summary>
        /// Get figure master from visio shape form
        /// </summary>
        /// <param name="shapeForm">form to get master</param>
        /// <returns>Shape master</returns>
        public Master getShapeMasterByShapeType(ShapeForm shapeForm)
        {
            Master resultFigure = begin;
            switch (shapeForm)
            {
                case ShapeForm.BEGIN:
                    resultFigure = begin;
                    break;
                case ShapeForm.FOR:
                    resultFigure = forState;
                    break;
                case ShapeForm.WHILE:
                case ShapeForm.IF:
                    resultFigure = ifState;
                    break;
                case ShapeForm.PROCESS:
                    resultFigure = process;
                    break;
                case ShapeForm.PROGRAM:
                    resultFigure = program;
                    break;
                case ShapeForm.ARROW_LEFT:
                    resultFigure = arrowLeft;
                    break;
                case ShapeForm.ARROW_RIGHT:
                    resultFigure = arrowRight;
                    break;
                case ShapeForm.LINE:
                    resultFigure = line;
                    break;
                case ShapeForm.IN_OUT_PUT:
                    resultFigure = inoutPut;
                    break;
                case ShapeForm.CONNECTOR:
                    resultFigure = connector;
                    break;
                case ShapeForm.PAGE_CONNECTOR:
                    resultFigure = pageConnector;
                    break;
                case ShapeForm.TEXT_FIELD:
                    resultFigure = textField;
                    break;
                case ShapeForm.SMALL_TEXT_FIELD:
                    resultFigure = smallTextField;
                    break;
                case ShapeForm.INVISIBLE_BLOCK:
                case ShapeForm.DO:
                    resultFigure = littleInvisibleBlock;
                    break;
            }
            return resultFigure;
        }


        /// <summary>
        /// Actually connect two shapes using Visio API
        /// </summary>
        /// <param name="shapeFrom">first shape</param>
        /// <param name="shapeTo">sec shape</param>
        /// <param name="connectorMaster">shape wich connect shapes</param>
        /// <param name="fromPoint">point to connect on first shape</param>
        /// <param name="toPoint">point to connect on sec shape</param>
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
                (short)VisSectionIndices.visSectionObject,
                (short)VisRowIndices.visRowAlign,
                (short)fromPoint));

            endXCell = connector.get_CellsSRC(
                (short)VisSectionIndices.visSectionObject,
                (short)VisRowIndices.visRowXForm1D,
                (short)VisCellIndices.vis1DEndX);

            endXCell.GlueTo(shapeTo.get_CellsSRC(
                (short)VisSectionIndices.visSectionObject,
                (short)VisRowIndices.visRowAlign,
                (short)toPoint));
        }

    }
}
