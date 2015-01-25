using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiimoteLib;

namespace WhatWiiDo
{
    class buttonHandler
    {
        bool bPressed = false;
        bool aPressed = false;
        bool onePressed = false;
        bool twoPressed = false;
        bool plusPressed = false;
        bool minusPressed = false;
        bool homePressed = false;
        bool upPressed = false;
        bool rightPressed = false;
        bool leftPressed = false;
        bool downPressed = false;

        public buttonHandler()
        {

        }

        //returns two lists. First list has buttons just pressed, second list has buttons just released
        public List<List<wiiButton>> update(Wiimote mote) {
            List<wiiButton> pressed = new List<wiiButton>();
            List<wiiButton> released = new List<wiiButton>();

            //A
            if (!aPressed && mote.WiimoteState.ButtonState.A)
            {
                aPressed = true;
                pressed.Add(wiiButton.A);
            }
            else if (aPressed && !mote.WiimoteState.ButtonState.A)
            {
                aPressed = false;
                released.Add(wiiButton.A);
            }
            //B
            if (!bPressed && mote.WiimoteState.ButtonState.B)
            {
                bPressed = true;
                pressed.Add(wiiButton.B);
            }
            else if (bPressed && !mote.WiimoteState.ButtonState.B)
            {
                bPressed = false;
                released.Add(wiiButton.B);
            }
            //One
            if (!onePressed && mote.WiimoteState.ButtonState.One)
            {
                onePressed = true;
                pressed.Add(wiiButton.One);
            }
            else if (onePressed && !mote.WiimoteState.ButtonState.One)
            {
                onePressed = false;
                released.Add(wiiButton.One);
            }
            //Two
            if (!twoPressed && mote.WiimoteState.ButtonState.Two)
            {
                twoPressed = true;
                pressed.Add(wiiButton.Two);
            }
            else if (twoPressed && !mote.WiimoteState.ButtonState.Two)
            {
                twoPressed = false;
                released.Add(wiiButton.Two);
            }
            //Plus
            if (!plusPressed && mote.WiimoteState.ButtonState.Plus)
            {
                plusPressed = true;
                pressed.Add(wiiButton.Plus);
            }
            else if (plusPressed && !mote.WiimoteState.ButtonState.Plus)
            {
                plusPressed = false;
                released.Add(wiiButton.Plus);
            }
            //Minus
            if (!minusPressed && mote.WiimoteState.ButtonState.Minus)
            {
                minusPressed = true;
                pressed.Add(wiiButton.Minus);
            }
            else if (minusPressed && !mote.WiimoteState.ButtonState.Minus)
            {
                minusPressed = false;
                released.Add(wiiButton.Minus);
            }
            //Home
            if (!homePressed && mote.WiimoteState.ButtonState.Home)
            {
                homePressed = true;
                pressed.Add(wiiButton.Home);
            }
            else if (homePressed && !mote.WiimoteState.ButtonState.Home)
            {
                homePressed = false;
                released.Add(wiiButton.Home);
            }
            //up
            if (!upPressed && mote.WiimoteState.ButtonState.Up)
            {
                upPressed = true;
                pressed.Add(wiiButton.Up);
            }
            else if (upPressed && !mote.WiimoteState.ButtonState.Up)
            {
                upPressed = false;
                released.Add(wiiButton.Up);
            }
            //down
            if (!downPressed && mote.WiimoteState.ButtonState.Down)
            {
                downPressed = true;
                pressed.Add(wiiButton.Down);
            }
            else if (downPressed && !mote.WiimoteState.ButtonState.Down)
            {
                downPressed = false;
                released.Add(wiiButton.Down);
            }
            //left
            if (!leftPressed && mote.WiimoteState.ButtonState.Left)
            {
                leftPressed = true;
                pressed.Add(wiiButton.Left);
            }
            else if (leftPressed && !mote.WiimoteState.ButtonState.Left)
            {
                leftPressed = false;
                released.Add(wiiButton.Left);
            }
            //right
            if (!rightPressed && mote.WiimoteState.ButtonState.Right)
            {
                rightPressed = true;
                pressed.Add(wiiButton.Right);
            }
            else if (rightPressed && !mote.WiimoteState.ButtonState.Right)
            {
                rightPressed = false;
                released.Add(wiiButton.Right);
            }

            List<List<wiiButton>> buttonData = new List<List<wiiButton>>();
            buttonData.Add(pressed);
            buttonData.Add(released);
            return buttonData;
        }
    }

    enum wiiButton
    {
        A, B, One, Two, Up, Down, Left, Right, Plus, Minus, Home
    }
}
