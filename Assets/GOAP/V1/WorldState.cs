using System;
using System.Text;
using UnityEngine;

namespace GOAP.V1
{
    public struct WorldState
    {
        public long Values;
        public long Mask;

        public bool Set(Enum atomIndex, bool value)
        {
            return Set(Convert.ToInt16(atomIndex), value);
        }

        public bool Set(short atomIndex, bool value)
        {
            if (atomIndex < 0)
            {
                return false;
            }

            var atomMask = 1L << atomIndex;
            Values = value ? (Values | atomMask) : (Values & ~atomMask);
            
            // add current bit to the Mask
            Mask |= atomMask;

            return true;
        }

        public void Clear()
        {
            Values = 0L;
            Mask = 0L;
        }

        public bool IsSet(short atomIndex)
        {
            return (Mask & (1L << atomIndex)) != 0;
        }

        public bool this[short atomIndex]
        {
            get { return (Values & (1L << atomIndex)) != 0; }
        }


        public string ToString<TAtoms>(string format = null)
        {
            var sb = new StringBuilder();

            var tAtoms = typeof(TAtoms);
            var atoms = Enum.GetNames(tAtoms);

            var sep = "[";
            for (short i = 0; i < 64; i++)
            {
                if (IsSet(i))
                {
                    //if (format == "{0} -> {1},\t")
                    //{
                    //    Debug.Log(IsSet(i) + " " + atoms.Length + " " + i + "\n" + Convert.ToString(Mask, 2).PadLeft(64, '0') + "\n" + Convert.ToString(Values, 2).PadLeft(64, '0'));
                    //}

                    var value = this[i];

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

        public override string ToString()
        {
            return ToString<GOAPTestV1.Atoms>();
        }

        internal bool Match(WorldState other)
        {
            var mask = other.Mask;
            return (Values & mask) == (other.Values & mask);
        }
    }
}
