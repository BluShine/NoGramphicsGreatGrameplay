using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiimoteLib;

namespace WhatWiiDo
{
    class WiimoteWrapper : Wiimote, iController
    {
        public WiimoteWrapper() { }
    }
}
