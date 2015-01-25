using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiimoteLib;

namespace WhatWiiDo
{
    interface Minigame
    {
        void update(Dictionary<Guid, Wiimote> players, int deltaTime);

        bool isOver();
    }
}
