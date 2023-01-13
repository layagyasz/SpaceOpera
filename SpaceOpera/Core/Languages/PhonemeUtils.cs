namespace SpaceOpera.Core.Languages
{
    public static class PhonemeUtils
    {
        private readonly static float I = 100;

        private static readonly float[,] CLASS_DISTANCE = new float[4, 4]
        {
            { 0, 0, 0 ,0 }, // NONE
            { 0, 0, 0, 0 }, // ANY
            { 0, 0, 0, 4 * I }, // CONSONANT
            { 0, 0, 4 * I, 0 } // VOWEL
        };

        private static readonly float[,] POSITION_DISTANCE = new float[17, 17]
        {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // NONE
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // ANY

            { 0, 0, 0, 1, I, I, I, I, I, I, I, I, I, I, I, I, 1 }, // BILABIAL
            { 0, 0, 1, 0, I, I, I, I, I, I, I, I, I, I, I, I, I }, // LABIODENTAL
            { 0, 0, I, I, 0, 1, 2, 3, 4, 5, 6, I, I, I, I, I, I }, // DENTAL
            { 0, 0, I, I, 1, 0, 1, 2, 3, 4, 5, I, I, I, I, I, I }, // ALVEOLAR
            { 0, 0, I, I, 2, 1, 0, 1, 2, 3, 4, I, I, I, I, I, I }, // POSTALVEOLAR
            { 0, 0, I, I, 3, 2, 1, 0, 1, 2, 3, I, I, I, I, I, I }, // RETROFLEX
            { 0, 0, I, I, 4, 3, 2, 1, 0, 1, 2, I, I, I, I, I, I }, // PALATAL
            { 0, 0, I, I, 5, 4, 3, 2, 1, 0, 1, 2, 3, 1, I, I, I }, // VELAR
            { 0, 0, I, I, 6, 5, 4, 3, 2, 1, 0, 1, 2, I, I, I, I }, // UVULAR
            { 0, 0, I, I, I, I, I, I, I, 2, 1, 0, 1, 2, I, I, I }, // PHARYNGEAL
            { 0, 0, I, I, I, I, I, I, I, 3, 2, 1, 0, 1, I, I, I }, // GLOTTAL
            { 0, 0, I, I, I, I, I, I, I, 1, I, 2, 1, 0, I, I, I }, // LABIOVELAR

            { 0, 0, I, I, I, I, I, I, I, I, I, I, I, I, 0, 6, I }, // FRONT
            { 0, 0, I, I, I, I, I, I, I, I, I, I, I, I, 6, 0, 6 }, // CENTRAL
            { 0, 0, 1, I, I, I, I, I, I, I, I, I, I, I, I, 6, 0 }, // BACK
        };

        private static readonly float[,] TYPE_DISTANCE = new float[18, 18]
        {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // NONE
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // ANY

            { 0, 0, 0, I, 6, 6, I, 3, I, I, I, I, I, I, I, I, I, I }, // PLOSIVE
            { 0, 0, I, 0, I, I, I, I, I, I, I, I, I, I, I, I, I, I }, // NASAL
            { 0, 0, 6, I, 0, 1, I, I, I, I, 3, I, I, I, I, I, I, I }, // TRILL
            { 0, 0, 6, I, 1, 0, I, 6, I, I, 1, I, I, I, I, I, I, I }, // TAP
            { 0, 0, I, I, I, I, 0, I, I, I, I, I, I, I, I, I, I, I }, // SIBILANT_FRICATIVE
            { 0, 0, 3, I, I, 6, I, 0, 6, 6, I, I, I, I, I, I, I, I }, // FRICATIVE
            { 0, 0, I, I, I, I, I, 6, 0, I, 1, I, I, I, I, I, I, I }, // LATERAL_FRICATIVE
            { 0, 0, I, I, I, I, I, 6, I, 0, I, I, I, I, I, I, I, I }, // APPROXIMANT
            { 0, 0, I, I, 3, 1, I, I, 1, I, 0, I, I, I, I, I, I, I }, // LATERAL_APPROXIMANT

            { 0, 0, I, I, I, I, I, I, I, I, I, 0, 1, 2, 3, 4, 5, 6 }, // CLOSED
            { 0, 0, I, I, I, I, I, I, I, I, I, 1, 0, 1, 2, 3, 4, 5 }, // CLOSED_CLOSED_MIDDLE
            { 0, 0, I, I, I, I, I, I, I, I, I, 2, 1, 0, 1, 2, 3, 4 }, // CLOSED_MIDDLE
            { 0, 0, I, I, I, I, I, I, I, I, I, 3, 2, 1, 0, 1, 2, 3 }, // MIDDLE
            { 0, 0, I, I, I, I, I, I, I, I, I, 4, 3, 2, 1, 0, 1, 2 }, // OPEN_MIDDLE
            { 0, 0, I, I, I, I, I, I, I, I, I, 5, 4, 3, 2, 1, 0, 1 }, // OPEN_OPEN_MIDDLE
            { 0, 0, I, I, I, I, I, I, I, I, I, 6, 5, 4, 3, 2, 1, 0 }, // OPEN
        };

        private static readonly float[,] VOICE_DISTANCE = new float[6, 6]
        {
            { 0, 0, 0, 0, 0, 0 }, // NONE
            { 0, 0, 0, 0, 0, 0 }, // NONE

            { 0, 0, 0, 6, I, I }, // VOICED
            { 0, 0, 6, 0, I, I }, // UNVOICED

            { 0, 0, I, I, 0, 6 }, // ROUNDED
            { 0, 0, I, I, 6, 0 }, // UNROUNDED
        };

        public static bool Contains(PhonemeClass Outer, PhonemeClass Inner)
        {
            if (Outer == PhonemeClass.ANY || Inner == PhonemeClass.ANY || Inner == PhonemeClass.NONE)
            {
                return true;
            }
            return Outer == Inner;
        }

        public static bool Contains(PhonemePosition Outer, PhonemePosition Inner)
        {
            if (Outer == PhonemePosition.Any || Inner == PhonemePosition.Any || Inner == PhonemePosition.None)
            {
                return true;
            }
            return Outer == Inner;
        }

        public static bool Contains(PhonemeType Outer, PhonemeType Inner)
        {
            if (Outer == PhonemeType.Any || Inner == PhonemeType.Any || Inner == PhonemeType.None)
            {
                return true;
            }
            return Outer == Inner;
        }

        public static bool Contains(PhonemeVoice Outer, PhonemeVoice Inner)
        {
            if (Outer == PhonemeVoice.Any || Inner == PhonemeVoice.Any || Inner == PhonemeVoice.None)
            {
                return true;
            }
            return Outer == Inner;
        }

        public static float Distance(PhonemeClass Left, PhonemeClass Right)
        {
            return CLASS_DISTANCE[(int)Left, (int)Right];
        }

        public static float Distance(PhonemePosition Left, PhonemePosition Right)
        {
            return POSITION_DISTANCE[(int)Left, (int)Right];
        }

        public static float Distance(PhonemeType Left, PhonemeType Right)
        {
            return TYPE_DISTANCE[(int)Left, (int)Right];
        }

        public static float Distance(PhonemeVoice Left, PhonemeVoice Right)
        {
            return VOICE_DISTANCE[(int)Left, (int)Right];
        }
    }
}