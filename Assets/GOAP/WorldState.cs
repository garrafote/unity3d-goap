using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GOAP
{
    public struct WorldState
    {
        public long Values;
        public long Mask;

        public void Clear()
        {
            Values = 0L;
            Mask = 0L;
        }

        public bool? this[short atomIndex]
        {
            get
            {
                if ((Mask & (1L << atomIndex)) == 0)
                    return null;

                return (Values & (1L << atomIndex)) != 0; 
            }
            set
            {
                if (atomIndex < 0)
                {
                    return;
                }

                var atomMask = 1L << atomIndex;
                Values = (value.HasValue && value.Value) ? (Values | atomMask) : (Values & ~atomMask);

                // add current bit to the Mask if value was set or remove if value was unset
                Mask = value.HasValue ? (Mask | atomMask) : (Mask & ~atomMask);
            }
        }


        public string ToString(IList<string> atoms, string format = null)
        {
            var sb = new StringBuilder();

            var sep = "[";
            for (short i = 0; i < 64; i++)
            {
                if (this[i] != null)
                {
                    //if (format == "{0} -> {1},\t")
                    //{
                    //    Debug.Log(IsSet(i) + " " + atoms.Length + " " + i + "\n" + Convert.ToString(Mask, 2).PadLeft(64, '0') + "\n" + Convert.ToString(Values, 2).PadLeft(64, '0'));
                    //}

                    var value = this[i].Value;

                    if (string.IsNullOrEmpty(format))
                    {
                        sb.Append(sep + (value ? " " : "!") + atoms[i]);
                        sep = ", ";
                    }
                    else
                    {
                        sb.AppendFormat(format, atoms[i], value);
                    }
                }
            }
            sb.Append("]");

            return sb.ToString();
        }

        private List<string> l; 
        public override string ToString()
        {
            if (l == null)
            {
                l = new List<string>(64);
                for (int i = 0; i < 64; i++)
                {
                    l.Add(string.Format("A{0:00}", i));
                }
            }

            return ToString(l);
        }

        internal bool Match(WorldState other)
        {
            var mask = other.Mask;
            return (Values & mask) == (other.Values & mask);
        }
    }
}
