using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBikes.Services.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    class AdminAttribute : Attribute
    {
        public AdminAttribute() : base() { }
    }
}
