using Raylib_CSharp;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace OriNoco
{
    public class PredictableLane
    {
        public float initialRate = 1f;
        public List<LaneChange> changes = new List<LaneChange>();

        public virtual float GetValueFromTime(float time)
        {
            float result = 0f;
            float previousTime = 0f;
            float rate = initialRate;

            for (int i = 0; i < changes.Count; i++)
            {
                if (time < changes[i].time)
                {
                    result += rate * (time - previousTime);
                    return result;
                }
                else
                {
                    result += rate * (changes[i].time - previousTime);
                    previousTime = changes[i].time;
                    rate = changes[i].rate;
                }
            }

            return result + rate * (time - previousTime);
        }

        public virtual float GetTimeFromValue(float value)
        {
            float result = 0f;
            float previousValue = 0f;
            float rate = initialRate;

            for (int i = 0; i < changes.Count; i++)
            {
                if (value < GetValueFromTime(changes[i].time))
                {
                    result += (value - previousValue) / rate;
                    return result;
                }
                else
                {
                    result += (GetValueFromTime(changes[i].time) - previousValue) / rate;
                    previousValue = GetValueFromTime(changes[i].time);
                    rate = changes[i].rate;
                }
            }

            return result + (value - previousValue) / rate;
        }

        public virtual int GetChangeIndexFromTime(float time)
        {
            for (int i = 0; i < changes.Count; i++)
            {
                if (time < changes[i].time)
                    return i - 1;
            }

            return changes.Count - 1;
        }

        public virtual LaneChange? GetChangeFromTime(float time)
        {
            for (int i = 0; i < changes.Count; i++)
            {
                if (time < changes[i].time)
                    return i == 0 ? null : changes[i - 1];
            }

            return changes.Count > 0 ? changes[changes.Count - 1] : null;
        }

        public virtual float AdjustTimeToRate(float time, float division = 1f)
        {
            int index = GetChangeIndexFromTime(time);
            if (index < 0)
            {
                float timePerRate = 1f / (initialRate * division);
                return (int)(time / timePerRate) * timePerRate;
            }
            else
            {
                float offset = changes[index].time - time;
                float timePerRate = 1f / (changes[index].rate * division);
                return (int)(offset / timePerRate) * timePerRate + changes[index].time;
            }
        }

        public virtual bool IsAPartOfRate(float time, float division = 1f)
        {
            int index = GetChangeIndexFromTime(time);
            if (index < 0)
            {
                float timePerRate = 1f / (initialRate * division);
                return time % timePerRate < float.Epsilon;
            }
            else
            {
                float offset = changes[index].time - time;
                float timePerRate = 1f / (changes[index].rate * division);
                return offset % timePerRate < float.Epsilon;
            }
        }

        public virtual void AdjustTimeToRate(float time, out float newTime, out int index, float division = 1f)
        {
            index = GetChangeIndexFromTime(time);
            if (index < 0)
            {
                float timePerRate = 1f / (initialRate * division);
                newTime = (int)(time / timePerRate) * timePerRate;
            }
            else
            {
                float offset = changes[index].time - time;
                float timePerRate = 1f / (changes[index].rate * division);
                newTime = (int)(offset / timePerRate) * timePerRate + changes[index].time;
            }
        }

        public virtual float GetNextTime(float time, float division = 1f)
        {
            float result = 0;
            AdjustTimeToRate(time, out float nt, out int index, division);
            if (index < 0)
            {
                float timePerRate = 1f / (initialRate * division);
                result = nt + timePerRate;
            }
            else
            {
                float timePerRate = 1f / (changes[index].rate * division);
                result = nt + timePerRate;
            }

            if (index + 1 < changes.Count)
            {
                if (changes[index + 1].time < result)
                    result = changes[index + 1].time;
            }

            return result;
        }

        public virtual float GetPreviousTime(float time, float division = 1f)
        {
            float result = 0;
            AdjustTimeToRate(time, out float nt, out int index, division);
            if (index < 0)
            {
                float timePerRate = 1f / (initialRate * division);
                result = nt - timePerRate;
            }
            else
            {
                float timePerRate = 1f / (changes[index].rate * division);
                result = nt - timePerRate;
            }

            if (index - 1 >= 0)
            {
                if (changes[index - 1].time > result)
                    result = changes[index - 1].time;
            }

            return result <= 0 ? 0 : result;
        }
    }

    public class LaneChange
    {
        public float rate = 1f;
        public float time;

        public LaneChange() {}
        public LaneChange(float rate, float time)
        {
            this.rate = rate;
            this.time = time;
        }
    }
}
