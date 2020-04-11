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
        public ShapeWrapper(Shape shape, ShapeForm shapeType)
        {
            this.shape = shape;
            this.shapeType = shapeType;
        }
        public Shape shape { get; }
        public ShapeForm shapeType {get;}

        public bool isCommonShape()
        {
            return this.shapeType == ShapeForm.PROCESS 
                || this.shapeType == ShapeForm.IN_OUT_PUT
                || this.shapeType == ShapeForm.PROGRAM 
                || this.shapeType == ShapeForm.BEGIN;
        }

    }
}
