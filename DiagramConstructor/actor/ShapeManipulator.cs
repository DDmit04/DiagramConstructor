using Microsoft.Office.Interop.Visio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramConstructor.actor
{
    class ShapeManipulator
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
        private Master smallTextField;
        private Master arrowLeft;
        private Master arrowRight;
        private Master littleInvisibleBlock;

        public ShapeManipulator()
        {
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
            smallTextField = visioStencil.Masters.get_ItemU(@"yesNo");
            arrowLeft = visioStencil.Masters.get_ItemU(@"arrowLeft");
            arrowRight = visioStencil.Masters.get_ItemU(@"arrowRight");
            littleInvisibleBlock = visioStencil.Masters.get_ItemU(@"LittleInvisibleBlock");
        }

        public void openDocument()
        {
            visioApp.Documents.Add("");
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
        public ShapeWrapper dropShapeV2(ShapeForm viosioShapeForm, String text, double x, double y)
        {
            Master shapeMaster = getShapeMasterVisioShapeType(viosioShapeForm);
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
        public ShapeWrapper dropShapeV2(Node node, double x, double y)
        {
            Master shapeMaster = begin;
            Page visioPage = visioApp.ActivePage;
            ShapeForm newShapeType = ShapeForm.BEGIN;
            if(node != null)
            {
                shapeMaster = getShapeMasterVisioShapeType(node.shapeForm);
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
        public ShapeWrapper adSmallTextField(String text, double x, double y)
        {
            Page visioPage = visioApp.ActivePage;
            ShapeWrapper shape = dropShapeV2(ShapeForm.SMALL_TEXT_FIELD, text, x, y);
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
            ShapeWrapper shape = dropShapeV2(ShapeForm.TEXT_FIELD, text, x, y);
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
            Master connectorMaster = getShapeMasterVisioShapeType(shapeForm);
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
        public Master getShapeMasterVisioShapeType(ShapeForm shapeForm)
        {
            Master resultFigure = begin;
            switch (shapeForm)
            {
                case ShapeForm.BEGIN:
                    resultFigure = begin;
                    break;
                case ShapeForm.IF:
                    resultFigure = ifState;
                    break;
                case ShapeForm.FOR:
                    resultFigure = forState;
                    break;
                case ShapeForm.WHILE:
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
