using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiimoteLib;

namespace WhatWiiDo
{
    public interface iController
    {
         //string HIDDevicePath { get; }
         Guid ID { get; }
         WiimoteState WiimoteState { get; }

         event EventHandler<WiimoteChangedEventArgs> WiimoteChanged;
         //event EventHandler<WiimoteExtensionChangedEventArgs> WiimoteExtensionChanged;

         void Connect();
         //void Disconnect();
         //void Dispose();
        //protected virtual void Dispose(bool disposing);
         //void GetStatus();
         //byte[] ReadData(int address, short size);
         //void SetLEDs(int leds);
         void SetLEDs(bool led1, bool led2, bool led3, bool led4);
         //void SetReportType(InputReport type, bool continuous);
         //void SetReportType(InputReport type, IRSensitivity irSensitivity, bool continuous);
         void SetRumble(bool on);
         //void WriteData(int address, byte data);
         //void WriteData(int address, byte size, byte[] buff);
    }
}
