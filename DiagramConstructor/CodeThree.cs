using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramConstructor
{
    class CodeThree
    {
        public String methodSignature { get; }
        public List<Node> methodNodes { get; }

        public CodeThree(String methodSignature, List<Node> methodNodes)
        {
            this.methodSignature = methodSignature;
            this.methodNodes = methodNodes;
        }
    }
}
