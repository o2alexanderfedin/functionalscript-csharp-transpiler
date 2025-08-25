using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Dynamic;

namespace FunctionalScript.Generated {
    public static class Module {
        public static readonly dynamic arr1 = new dynamic[] { 1, 2 };
        public static readonly dynamic arr2 = new dynamic[] { 3, 4 };
        public static readonly dynamic result = FunctionalScript.Runtime.SpreadArray((dynamic[])arr1, (dynamic[])arr2);
        public static dynamic Default => result;
    }
}
