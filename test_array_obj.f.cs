using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Dynamic;

namespace FunctionalScript.Generated {
    public static class Module {
        public static readonly dynamic arr = new dynamic[] { FunctionalScript.Runtime.CreateObject(new Dictionary<string, object> { { "id", 1 }, { "name", @"first" } }), FunctionalScript.Runtime.CreateObject(new Dictionary<string, object> { { "id", 2 }, { "name", @"second" } }) };
        public static dynamic Default => arr;
    }
}
