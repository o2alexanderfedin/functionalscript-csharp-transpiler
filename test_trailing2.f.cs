using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Dynamic;

namespace FunctionalScript.Generated {
    public static class Module {
        public static readonly dynamic arr = new dynamic[] { 1, 2, 3 };
        public static readonly dynamic obj = FunctionalScript.Runtime.CreateObject(new Dictionary<string, object> { { "a", 1 }, { "b", 2 } });
        public static dynamic Default => FunctionalScript.Runtime.CreateObject(new Dictionary<string, object> { { "arr", arr }, { "obj", obj } });
    }
}
