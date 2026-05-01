using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Domain.Common.Helpers
{
    public static class Helper
    {
        public static int CountWords(string text) =>
            text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
