using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IrrKlang;


namespace WhatWiiDo
{
    static class CommonSounds
    {
        static String commonSoundDirectory = "../../sounds/common/";
        public static String Go = commonSoundDirectory + "voice_go.wav";
        public static String Yes = commonSoundDirectory + "enthusiastic_yes.wav";
        public static Dictionary<wiiButton, String> buttonSounds = new Dictionary<wiiButton, String>
        {
            {wiiButton.A, commonSoundDirectory + "voice_A.wav"},
            {wiiButton.B, commonSoundDirectory + "voice_B.wav"},
            {wiiButton.Right, commonSoundDirectory + "voice_right.wav"},
            {wiiButton.Left, commonSoundDirectory + "voice_left.wav"},
            {wiiButton.Up, commonSoundDirectory + "voice_up.wav"},
            {wiiButton.Down, commonSoundDirectory + "voice_down.wav"},
            {wiiButton.Plus, commonSoundDirectory + "voice_plus.wav"},
            {wiiButton.Minus, commonSoundDirectory + "voice_minus.wav"},
            {wiiButton.Home, commonSoundDirectory + "voice_home.wav"},
            {wiiButton.One, commonSoundDirectory + "voice_1.wav"},
            {wiiButton.Two, commonSoundDirectory + "voice_2.wav"}
        };
        public static Dictionary<int, String> scoreSounds = new Dictionary<int, String>
        {
            {1, commonSoundDirectory + "ones.wav"},
            {5, commonSoundDirectory + "fives.wav"},
            {10, commonSoundDirectory + "tens.wav"}
        };
    }
}
