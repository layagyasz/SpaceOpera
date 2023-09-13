namespace SpaceOpera.Core
{
    public struct Modifier
    {
        public static readonly Modifier One = new() { Constant = 1 };
        public static readonly Modifier Zero = new();

        public float Constant { get; set; }
        public float Bonus { get; set; }

        public static Modifier FromConstant(float constant)
        {
            return new Modifier() { Constant = constant };
        }

        public Modifier Combine()
        {
            return new()
            {
                Constant = (1 + Bonus) * Constant
            };
        }

        public float GetTotal()
        {
            return (1 + Bonus) * Constant;
        }

        public override string ToString()
        {
            bool showConstant = Math.Abs(Constant) > float.Epsilon;
            bool showBonus = Math.Abs(Bonus) > float.Epsilon;
            if (showConstant && showBonus)
            {
                return string.Format("{0:N2} ({1})", Constant, FormatBonusString(Bonus));
            }
            if (!showConstant && showBonus)
            {
                return FormatBonusString(Bonus);
            }
            return string.Format("{0:N2}", Constant);
        }

        private static string FormatBonusString(float Bonus)
        {
            if (Bonus < 0)
            {
                return string.Format("{0:P0}", Bonus);
            }
            return string.Format("+{0:P0}", Bonus);
        }

        public static Modifier operator +(Modifier left, Modifier right)
        {
            return new()
            {
                Constant = left.Constant + right.Constant,
                Bonus = (1 + left.Bonus) * (1 + right.Bonus) - 1
            };
        }

        public static Modifier operator *(float left, Modifier right)
        {
            return new()
            {
                Constant = left * right.Constant,
                Bonus = left * right.Bonus
            };
        }
    }
}