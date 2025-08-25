using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Dynamic;

namespace FunctionalScript.Generated {
    public static class Module {
        public static readonly dynamic name = @"test";
        public static readonly dynamic obj = FunctionalScript.Runtime.CreateObject(new Dictionary<string, object> { { "name", name } });
        public static dynamic Default => obj;
    }
}
