using Microsoft.Office.Interop.Visio;
using System;

namespace DiagramConstructor.actor
{
    class BuilderUtills
    {

        /// <summary>
        /// Define connection type between two shapes (first shape is parent shape)
        /// </summary>
        /// <param name="chainParentShape">parent shape</param>
        /// <param name="lastBranchShape">child shape</param>
        /// <returns>connection type</returns>
        public static ShapeConnectionType defineConnectionTypeWithBranchParent(ShapeWrapper chainParentShape, ShapeWrapper lastBranchShape)
        {
            bool lastBranchShapeIsCommon = lastBranchShape.isCommonShape();
            ShapeConnectionType shapeConnectionType = ShapeConnectionType.FROM_TOP_TO_RIGHT;
            if (chainParentShape.shapeType == ShapeForm.FOR && lastBranchShapeIsCommon)
            {
                shapeConnectionType = ShapeConnectionType.FROM_BOT_TO_LEFT;
            }
            else if (chainParentShape.shapeType == ShapeForm.FOR && !lastBranchShapeIsCommon)
            {
                shapeConnectionType = ShapeConnectionType.FROM_RIGHT_TO_LEFT;
            }
            else if (chainParentShape.shapeType == ShapeForm.WHILE && lastBranchShapeIsCommon)
            {
                shapeConnectionType = ShapeConnectionType.FROM_BOT_TO_CENTER;
            }
            else if (chainParentShape.shapeType == ShapeForm.WHILE && !lastBranchShapeIsCommon)
            {
                shapeConnectionType = ShapeConnectionType.FROM_LEFT_TO_CENTER;
            }
            return shapeConnectionType;
        }

        /// <summary>
        /// Define connection type between two shapes (none of the shapes is parent shape)
        /// </summary>
        /// <param name="firstShape">up shape</param>
        /// <param name="secShape">down shape</param>
        /// <returns>connection type</returns>
        public static ShapeConnectionType defineConnectionType(ShapeWrapper firstShape, ShapeWrapper secShape, bool isSameBranch)
        {
            bool firstShapeIsCommon = firstShape.isCommonShape();
            bool secShapeIsCommon = secShape.isCommonShape();
            ShapeConnectionType shapeConnectionType = ShapeConnectionType.FROM_TOP_TO_RIGHT;
            if (!firstShapeIsCommon && secShapeIsCommon)
            {
                if (isSameBranch)
                {
                    shapeConnectionType = ShapeConnectionType.FROM_TOP_TO_BOT;
                }
                else
                {
                    shapeConnectionType = ShapeConnectionType.FROM_TOP_TO_RIGHT;
                }
            }
            else if (firstShapeIsCommon && !secShapeIsCommon)
            {
                shapeConnectionType = ShapeConnectionType.FROM_TOP_TO_BOT;
            }
            else if (!firstShapeIsCommon && !secShapeIsCommon)
            {
                if(isSameBranch)
                {
                    shapeConnectionType = ShapeConnectionType.FROM_TOP_TO_BOT;
                }
                else
                {
                    shapeConnectionType = ShapeConnectionType.FROM_TOP_TO_RIGHT;
                }
            }
            else if (firstShapeIsCommon && secShapeIsCommon)
            {
                shapeConnectionType = ShapeConnectionType.FROM_TOP_TO_BOT;
            }
            return shapeConnectionType;
        }

        /// <summary>
        /// Out return cells aligment from given connection type
        /// </summary>
        /// <param name="conectionToType">Out cell aligment for connection to shape</param>
        /// <param name="conectionFromType">Out cell aligment for connection from shape</param>
        /// <param name="connectionType">type of shape connection</param>
        public static void getCellsAlignsFromConnectionType(out VisCellIndices conectionToType, out VisCellIndices conectionFromType, ShapeConnectionType connectionType)
        {
            conectionToType = VisCellIndices.visAlignTop;
            conectionFromType = VisCellIndices.visAlignBottom;
            switch (connectionType)
            {
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
                case ShapeConnectionType.FROM_RIGHT_TO_LEFT:
                    conectionToType = VisCellIndices.visAlignLeft;
                    conectionFromType = VisCellIndices.visAlignRight;
                    break;
                case ShapeConnectionType.FROM_TOP_TO_BOT:
                    conectionToType = VisCellIndices.visAlignBottom;
                    conectionFromType = VisCellIndices.visAlignTop;
                    break;
                case ShapeConnectionType.FROM_LEFT_TO_CENTER:
                    conectionToType = VisCellIndices.visAlignCenter;
                    conectionFromType = VisCellIndices.visAlignLeft;
                    break;
            }
        }

        /// <summary>
        /// Returns max child branch length (num of nodes)
        /// </summary>
        /// <param name="node">parent node</param>
        /// <returns>max num of nodes</returns>
        public static double calcStatementHeight(Node node)
        {
            double commonBrannchMaxLength = 0;
            double elseBrannchMaxLength = 0;
            double ifBrannchMaxLength = 0;
            if (node.childNodes.Count != 0)
            {
                foreach (Node n in node.childNodes)
                {
                    commonBrannchMaxLength++;
                    commonBrannchMaxLength += calcStatementHeight(n);
                }
            }
            if (node.childIfNodes.Count != 0)
            {
                foreach (Node n in node.childIfNodes)
                {
                    ifBrannchMaxLength++;
                    ifBrannchMaxLength += calcStatementHeight(n);
                }
            }
            if (node.childElseNodes.Count != 0)
            {
                foreach (Node n in node.childElseNodes)
                {
                    elseBrannchMaxLength++;
                    elseBrannchMaxLength += calcStatementHeight(n);
                }
            }
            double result = Math.Max(commonBrannchMaxLength, Math.Max(elseBrannchMaxLength, ifBrannchMaxLength));
            if(node.shapeForm == ShapeForm.WHILE)
            {
                result += 0.2;
            }
            return result;
        }

    }
}
