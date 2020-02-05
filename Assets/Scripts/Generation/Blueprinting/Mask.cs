using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Generation.Blueprinting
{
    //0b_0000_0000_0000_0000_0000_0000
    //   \--/_\--/_\--/_\--/_\--/_\--/
    //  Down   Up   D+3  D+2  D+1  D+0 //+X is one direction clockwise, EG N = D+0, E = D+1, S = D+2, W = D+3
    public class Mask
    {
        public uint mask = 0b_0000_0000_0000_0000_0000_0000;

        public MatchCriteria matchCriteria;

        public int configuration;

        public int offset;

        public int precidence = 0;

        public OffsetBias bias; //Offset direction that is favored in pattern relative to actual offset

        public Mask()
        {

        }

        public Mask(int configuration, uint mask, MatchCriteria criteria = MatchCriteria.Fit, int precidence = 0)
        {
            this.mask = mask;
            this.configuration = configuration;
            this.precidence = precidence;
            matchCriteria = criteria;

            offset = 0;
            bias = OffsetBias.None;
        }

        public Mask(int configuration, uint mask, int offset, MatchCriteria criteria = MatchCriteria.Fit, int precidence = 0)
        {
            this.mask = mask;
            this.configuration = configuration;
            this.offset = offset;
            this.precidence = precidence;
            matchCriteria = criteria;

            bias = OffsetBias.None;
        }

        public Mask(int configuration, uint mask, int offset, OffsetBias bias, MatchCriteria criteria = MatchCriteria.Fit, int precidence = 0)
        {
            this.mask = mask;
            this.configuration = configuration;
            this.offset = offset;
            this.bias = bias;
            this.precidence = precidence;
            matchCriteria = criteria;
        }
    }

    public static class MaskF
    {
        public static bool Fit(this Mask mask, Mask pattern)
        {
            return Fit(mask, pattern.mask);
        }

        public static bool Fit(this Mask mask, uint pattern)
        {
            return ((mask.mask & pattern) > 0
                && (mask.mask & (~pattern)) == 0);
        }

        public static bool Excludes(this Mask mask, Mask pattern)
        {
            return Excludes(mask, pattern.mask);
        }

        public static bool Excludes(this Mask mask, uint pattern)
        {
            return (mask.mask & pattern) == 0;
        }

        public static bool Exact(this Mask mask, Mask pattern)
        {
            return Exact(mask, pattern.mask);
        }

        public static bool Exact(this Mask mask, uint pattern)
        {
            return mask.mask == pattern;
        }

        public static bool Hits(this Mask mask, Mask pattern)
        {
            return Hits(mask, pattern.mask);
        }

        public static bool Hits(this Mask mask, uint pattern)
        {
            return (mask.mask & pattern) > 0;
        }

        public static List<Mask> Fits(this Mask mask, List<Mask> patterns)
        {
            var result = new List<Mask>();
            foreach(var pattern in patterns)
            {
                if (mask.Fit(pattern)) result.Add(pattern);
            }
            return result;
        }

        public static Mask FindEqual(this Mask mask, List<Mask> patterns)
        {
            return patterns.FirstOrDefault(x => mask.Equals(x));
        }

        public static List<Mask> FindMatchs(this Mask mask, List<Mask> patterns)
        {
            var result = new List<Mask>();
            foreach(var pattern in patterns)
            {
                if(pattern.matchCriteria == MatchCriteria.Exclusive)
                {
                    if (mask.Excludes(pattern)) result.Add(pattern);
                }
                else if(pattern.matchCriteria == MatchCriteria.Exact)
                {
                    if (mask.Exact(pattern)) result.Add(pattern);
                }
                else
                {
                    if (mask.Fit(pattern)) result.Add(pattern);
                }
            }
            return result;
        }

        public static List<Mask> FindExclusiions(this Mask mask, List<Mask> patterns)
        {
            var result = new List<Mask>();
            foreach(var pattern in patterns)
            {
                if (mask.Excludes(pattern)) result.Add(pattern);
            }
            return result;
        }

        public static void Add(this Mask mask, Direction orientation, Direction direction, uint value)
        {
            if(direction == orientation.GetRightDirection())
            {
                mask.mask = mask.mask | (value << 4);
                return;
            } else if(direction == orientation.GetOppositeDirection())
            {
                mask.mask = mask.mask | (value << 8);
                return;
            } else if(direction == orientation.GetLeftDirection())
            {
                mask.mask = mask.mask | (value << 12);
                return;
            }
            //Direction == orientation
            mask.mask = mask.mask | value;
        }

        public static uint MaskValue(int index, int offset)
        {
            return (uint)((0b_0000_0000_0000_0000_0000_0000 + offset) << (index * 4));
        }
    }

    public enum MatchCriteria
    {
        Fit,
        Exact,
        Exclusive,
    }

    public enum OffsetBias
    {
        None,
        Left,
        Right
    }
}
