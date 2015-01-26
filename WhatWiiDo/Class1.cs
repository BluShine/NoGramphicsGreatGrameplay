using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IrrKlang;
using WiimoteLib;


namespace WhatWiiDo
{
    public interface PauseCondition
    {
        bool update(int timeDelta);
        Object nextState();
    }

    public class TimedPause : PauseCondition
    {
        int targetPauseTime, elapsedPauseTime;
        Object returnObject;

        public TimedPause(int pauseTime, Object finishedObject)
        {
            targetPauseTime = pauseTime;
            elapsedPauseTime = 0;
            returnObject = finishedObject; 
        }

        public bool update(int timeDelta)
        {
            elapsedPauseTime += timeDelta;
            return elapsedPauseTime >= targetPauseTime;
        }

        public Object nextState(){
            return returnObject;
        }

    }

    class SoundPause : PauseCondition
    {
        ISound sound;
        Object returnedObject;
       
        public SoundPause(ISound s, Object finishedObject)
        {
            sound = s;
            returnedObject = finishedObject; 
        }

        public bool update(int timeDelta)
        {
            return sound.Finished;
        }

        public Object nextState(){
            return returnedObject;
        }

    }
    
    public class AndPause : PauseCondition
    {
        PauseCondition p1, p2;
        Object returnedObject;
       
        public AndPause(PauseCondition p1, PauseCondition p2, Object finishedObject)
        {
            this.p1 = p1;
            this.p2 = p2;
            returnedObject = finishedObject; 
        }

        public bool update(int timeDelta)
        {
            return p1.update(timeDelta) && p2.update(timeDelta);
        }

        public Object nextState(){
            return returnedObject;
        }
    }

    public class OrPause : PauseCondition
    {
        PauseCondition p1, p2;
        Object returnedObject;

        public OrPause(PauseCondition p1, PauseCondition p2, Object finishedObject)
        {
            this.p1 = p1;
            this.p2 = p2;
            returnedObject = finishedObject;
        }

        public bool update(int timeDelta)
        {
            return p1.update(timeDelta) || p2.update(timeDelta);
        }

        public Object nextState()
        {
            return returnedObject;
        }
    }

    public class ThenPause : PauseCondition
    {
        PauseCondition p1, p2;
        bool p1Done;
        Object returnedObject;

        public ThenPause(PauseCondition p1, PauseCondition p2, Object finishedObject)
        {
            this.p1 = p1;
            this.p2 = p2;
            p1Done = false;
            returnedObject = finishedObject;
        }

        public bool update(int timeDelta)
        {
            if (p1Done)
            {
                return p2.update(timeDelta);
            }
            else
            {
                if (p1.update(timeDelta))
                {
                    p1Done = true;
                }
            }
            return false;
        }

        public Object nextState()
        {
            return returnedObject;
        }
    }

    public class VibrateDuringPause : PauseCondition
    {
        PauseCondition pause;
        IEnumerable<Wiimote> motes;
        bool done;

        public VibrateDuringPause(Wiimote w, PauseCondition pause)
        {
            this.pause = pause;
            List<Wiimote> l = new List<Wiimote>(1);
            l.Add(w);
            w.SetRumble(true);
            motes = l;
            done = false;
        }

        public VibrateDuringPause(IEnumerable<Wiimote> ws, PauseCondition pause)
        {
            this.pause = pause;
            motes = ws;
            foreach (Wiimote w in ws)
            {
                w.SetRumble(true);
            }
            done = false;
        }

        public bool update(int timeDelta)
        {
            done = pause.update(timeDelta);
            if (done)
            {
                foreach (Wiimote w in motes)
                {
                    w.SetRumble(false);
                }
            }
            return done;
        }

        public Object nextState()
        {
            return pause.nextState();
        }
    }

    public class SoundAndTimePause : PauseCondition
    {
        PauseCondition pause;
        public SoundAndTimePause(ISound sound, int time, Object next)
        {
            pause = new AndPause(new SoundPause(sound, null), new TimedPause(time, null), next);
        }
        public bool update(int timeDelta)
        {
            return pause.update(timeDelta);
        }

        public Object nextState()
        {
            return pause.nextState();
        }
    }
}

