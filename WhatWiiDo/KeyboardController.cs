using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Threading;
using System.Threading.Tasks;
using WiimoteLib;

namespace WhatWiiDo
{
    class KeyboardController : iController
    {
        private Guid id;
        private WiimoteState fakeState;

        public KeyboardController()
        {
            id = Guid.NewGuid();
            fakeState = new WiimoteState();
            fakeState.ButtonState = new ButtonState();
            fakeState.AccelState = new AccelState();
            fakeState.AccelState.Values = new Point3F();
        }

        //public string HIDDevicePath { get; }
        public Guid ID { get { return id; } }
        public WiimoteState WiimoteState { get { updateRemoteState();  return fakeState; } }

        public event EventHandler<WiimoteChangedEventArgs> WiimoteChanged;

        

        public void updateRemoteState()
        {
            fakeState.ButtonState.Up = Keyboard.IsKeyDown(Key.Up);
            fakeState.ButtonState.Down = Keyboard.IsKeyDown(Key.Down);
            fakeState.ButtonState.Left = Keyboard.IsKeyDown(Key.Left);
            fakeState.ButtonState.Right = Keyboard.IsKeyDown(Key.Right);
            Point3F asvs = fakeState.AccelState.Values;
            if(Keyboard.IsKeyDown(Key.W))
                asvs.Z = -2.5f;
            else if(Keyboard.IsKeyDown(Key.S))
                asvs.Z = 2.5f;
            else
                asvs.Z = 0;
            if(Keyboard.IsKeyDown(Key.A))
                asvs.X = 2.5f;
            else if(Keyboard.IsKeyDown(Key.D))
                asvs.X = -2.5f;
            else
                asvs.X = 0f;

            fakeState.AccelState.Values = asvs;

            remoteChanged();
        }

        void remoteChanged()
        {
            if(WiimoteChanged != null)
            {
                WiimoteChanged(this, null);
            }
        }

        //public event EventHandler<WiimoteExtensionChangedEventArgs> WiimoteExtensionChanged;

        public void Connect() {}
        //public void Disconnect();
        //public void Dispose();
        //protected virtual void Dispose(bool disposing);
        //public void GetStatus();
        //public byte[] ReadData(int address, short size);
        //public void SetLEDs(int leds);
        public void SetLEDs(bool led1, bool led2, bool led3, bool led4) {}
        //public void SetReportType(InputReport type, bool continuous);
        public void SetReportType(InputReport type, IRSensitivity irSensitivity, bool continuous) {}
        public void SetRumble(bool on) {}
        //public void WriteData(int address, byte data);
        //public void WriteData(int address, byte size, byte[] buff);
    }
}
