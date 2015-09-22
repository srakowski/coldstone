using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coldstone
{
    class EdgeResult
    {
        public bool ok;
        public object result;

        public static object Ok(object result)
        {
            return new EdgeResult()
            {
                ok = true,
                result = result
            };
        }

        public static object NotOk(object result)
        {
            return new EdgeResult()
            {
                ok = false,
                result = result
            };
        }
    }
}
