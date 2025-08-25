using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Dynamic;

namespace FunctionalScript.Generated {
    public static class Module {
        public static readonly dynamic key = @"dynamic";
        public static readonly dynamic obj = FunctionalScript.Runtime.CreateObject(new Dictionary<string, object> { { key, @"value" } });
        public static dynamic Default => obj;
    }
}
