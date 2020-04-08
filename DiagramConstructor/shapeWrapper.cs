using Microsoft.Office.Interop.Visio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramConstructor
{
    class ShapeWrapper
    {
        public ShapeWrapper(Shape shape, NodeType shapeType)
        {
            this.shape = shape;
            this.shapeType = shapeType;
        }
        public Shape shape { get; }
        public NodeType shapeType {get;}

    }
}
