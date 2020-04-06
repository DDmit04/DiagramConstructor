using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramConstructor
{
    class Node
    {
        public NodeType nodeType { get; set; } = NodeType.COMMON;
        public String nodeText { get; set; } = "no text";
        public List<Node> childNodes { get; set; } = new List<Node>();
        public List<Node> childIfNodes { get; set; } = new List<Node>();
        public List<Node> childElseNodes { get; set; } = new List<Node>();

    }
}
