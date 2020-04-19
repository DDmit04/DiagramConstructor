using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramConstructor
{
    class Node
    {
        public ShapeForm shapeForm { get; set; } = ShapeForm.PROCESS;
        public String nodeText { get; set; } = "";
        public List<Node> childNodes { get; set; } = new List<Node>();
        public List<Node> childIfNodes { get; set; } = new List<Node>();
        public List<Node> childElseNodes { get; set; } = new List<Node>();

        public bool isSimpleNode()
        {
            return shapeForm == ShapeForm.PROCESS || shapeForm == ShapeForm.PROGRAM || shapeForm == ShapeForm.IN_OUT_PUT;
        }

        public bool isNoChileNodes()
        {
            return childNodes.Count == 0 && childIfNodes.Count == 0 && childElseNodes.Count == 0;
        }

    }
}
