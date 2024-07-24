using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OriNoco.Tests
{
    public class LaneTest1
    {
        public PredictableLane lane = new PredictableLane();
        public LaneTest1()
        {
            lane.initialRate = 3f;

            lane.changes.Add(new LaneChange(3f, 2f));
            lane.changes.Add(new LaneChange(4f, 10f));
            lane.changes.Add(new LaneChange(6f, 4f));
            lane.changes.Add(new LaneChange(9f, 3f));
        }

        public void Run()
        {
            Console.WriteLine("Initial Rate: " + lane.initialRate);
            foreach(var change in lane.changes)
            {
                Console.WriteLine($"Changes: At {change.time} - Rate: {change.rate}");
            }

            int testLength = 10;
            float[] values = new float[testLength];

            float initialValue = 2f;
            float increase = 2f;

            for (int i = 0; i < testLength; i++)
            {
                float time = initialValue + (increase * i);
                values[i] = lane.GetValueFromTime(time);
                Console.WriteLine($"GetValueFromTime({time}): {values[i]}");
            }

            Console.WriteLine("");

            foreach (var valueTest in values)
            {
                Console.WriteLine($"GetTimeFromValue({valueTest}): {lane.GetTimeFromValue(valueTest)}");
            }
        }
    }
}
