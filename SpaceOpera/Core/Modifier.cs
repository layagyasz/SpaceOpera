namespace SpaceOpera.Core
{
    public struct Modifier
    {
        public static readonly Modifier One = new Modifier() { Constant = 1 };
        public static readonly Modifier Zero = new Modifier();

        public float Constant { get; set; }
        public float Bonus { get; set; }

        public Modifier Combine()
        {
            return new Modifier()
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
            bool showConstant = Math.Abs(Constant) < float.Epsilon;
            bool showBonus = Math.Abs(Bonus) < float.Epsilon;
            if (showConstant && showBonus)
            {
                return String.Format("{0:N2} ({1})", Constant, FormatBonusString(Bonus));
            }
            if (!showConstant && showBonus)
            {
                return FormatBonusString(Bonus);
            }
            return String.Format("{0:N2}", Constant);
        }

        private string FormatBonusString(float Bonus)
        {
            if (Bonus < 0)
            {
                return String.Format("{0:P0}", Bonus);
            }
            return String.Format("+{0:P0}", Bonus);
        }

        public static Modifier operator +(Modifier left, Modifier right)
        {
            return new Modifier()
            {
                Constant = left.Constant + right.Constant,
                Bonus = (1 + left.Bonus) * (1 + right.Bonus) - 1
            };
        }

        public static Modifier operator *(float left, Modifier right)
        {
            return new Modifier()
            {
                Constant = left * right.Constant,
                Bonus = left * right.Bonus
            };
        }
    }
}