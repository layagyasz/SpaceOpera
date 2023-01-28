namespace SpaceOpera.Core.Languages
{
    public static class PhonemeUtils
    {
        private readonly static float s_I = 100;

        private static readonly float[,] s_ClassDistance = new float[4, 4]
        {
            { 0, 0, 0 ,0 }, // NONE
            { 0, 0, 0, 0 }, // ANY
            { 0, 0, 0, 4 * s_I }, // CONSONANT
            { 0, 0, 4 * s_I, 0 } // VOWEL
        };

        private static readonly float[,] s_PositionDistance = new float[17, 17]
        {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // NONE
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // ANY

            { 0, 0, 0, 1, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, 1 }, // BILABIAL
            { 0, 0, 1, 0, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I }, // LABIODENTAL
            { 0, 0, s_I, s_I, 0, 1, 2, 3, 4, 5, 6, s_I, s_I, s_I, s_I, s_I, s_I }, // DENTAL
            { 0, 0, s_I, s_I, 1, 0, 1, 2, 3, 4, 5, s_I, s_I, s_I, s_I, s_I, s_I }, // ALVEOLAR
            { 0, 0, s_I, s_I, 2, 1, 0, 1, 2, 3, 4, s_I, s_I, s_I, s_I, s_I, s_I }, // POSTALVEOLAR
            { 0, 0, s_I, s_I, 3, 2, 1, 0, 1, 2, 3, s_I, s_I, s_I, s_I, s_I, s_I }, // RETROFLEX
            { 0, 0, s_I, s_I, 4, 3, 2, 1, 0, 1, 2, s_I, s_I, s_I, s_I, s_I, s_I }, // PALATAL
            { 0, 0, s_I, s_I, 5, 4, 3, 2, 1, 0, 1, 2, 3, 1, s_I, s_I, s_I }, // VELAR
            { 0, 0, s_I, s_I, 6, 5, 4, 3, 2, 1, 0, 1, 2, s_I, s_I, s_I, s_I }, // UVULAR
            { 0, 0, s_I, s_I, s_I, s_I, s_I, s_I, s_I, 2, 1, 0, 1, 2, s_I, s_I, s_I }, // PHARYNGEAL
            { 0, 0, s_I, s_I, s_I, s_I, s_I, s_I, s_I, 3, 2, 1, 0, 1, s_I, s_I, s_I }, // GLOTTAL
            { 0, 0, s_I, s_I, s_I, s_I, s_I, s_I, s_I, 1, s_I, 2, 1, 0, s_I, s_I, s_I }, // LABIOVELAR

            { 0, 0, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, 0, 6, s_I }, // FRONT
            { 0, 0, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, 6, 0, 6 }, // CENTRAL
            { 0, 0, 1, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, 6, 0 }, // BACK
        };

        private static readonly float[,] s_TypeDistance = new float[18, 18]
        {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // NONE
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // ANY

            { 0, 0, 0, s_I, 6, 6, s_I, 3, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I }, // PLOSIVE
            { 0, 0, s_I, 0, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I }, // NASAL
            { 0, 0, 6, s_I, 0, 1, s_I, s_I, s_I, s_I, 3, s_I, s_I, s_I, s_I, s_I, s_I, s_I }, // TRILL
            { 0, 0, 6, s_I, 1, 0, s_I, 6, s_I, s_I, 1, s_I, s_I, s_I, s_I, s_I, s_I, s_I }, // TAP
            { 0, 0, s_I, s_I, s_I, s_I, 0, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I }, // SIBILANT_FRICATIVE
            { 0, 0, 3, s_I, s_I, 6, s_I, 0, 6, 6, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I }, // FRICATIVE
            { 0, 0, s_I, s_I, s_I, s_I, s_I, 6, 0, s_I, 1, s_I, s_I, s_I, s_I, s_I, s_I, s_I }, // LATERAL_FRICATIVE
            { 0, 0, s_I, s_I, s_I, s_I, s_I, 6, s_I, 0, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I }, // APPROXIMANT
            { 0, 0, s_I, s_I, 3, 1, s_I, s_I, 1, s_I, 0, s_I, s_I, s_I, s_I, s_I, s_I, s_I }, // LATERAL_APPROXIMANT

            { 0, 0, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, 0, 1, 2, 3, 4, 5, 6 }, // CLOSED
            { 0, 0, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, 1, 0, 1, 2, 3, 4, 5 }, // CLOSED_CLOSED_MIDDLE
            { 0, 0, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, 2, 1, 0, 1, 2, 3, 4 }, // CLOSED_MIDDLE
            { 0, 0, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, 3, 2, 1, 0, 1, 2, 3 }, // MIDDLE
            { 0, 0, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, 4, 3, 2, 1, 0, 1, 2 }, // OPEN_MIDDLE
            { 0, 0, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, 5, 4, 3, 2, 1, 0, 1 }, // OPEN_OPEN_MIDDLE
            { 0, 0, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, s_I, 6, 5, 4, 3, 2, 1, 0 }, // OPEN
        };

        private static readonly float[,] s_VoiceDistance = new float[6, 6]
        {
            { 0, 0, 0, 0, 0, 0 }, // NONE
            { 0, 0, 0, 0, 0, 0 }, // NONE

            { 0, 0, 0, 6, s_I, s_I }, // VOICED
            { 0, 0, 6, 0, s_I, s_I }, // UNVOICED

            { 0, 0, s_I, s_I, 0, 6 }, // ROUNDED
            { 0, 0, s_I, s_I, 6, 0 }, // UNROUNDED
        };

        public static bool Contains(PhonemeClass Outer, PhonemeClass Inner)
        {
            if (Outer == PhonemeClass.Any || Inner == PhonemeClass.Any || Inner == PhonemeClass.None)
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
            return s_ClassDistance[(int)Left, (int)Right];
        }

        public static float Distance(PhonemePosition Left, PhonemePosition Right)
        {
            return s_PositionDistance[(int)Left, (int)Right];
        }

        public static float Distance(PhonemeType Left, PhonemeType Right)
        {
            return s_TypeDistance[(int)Left, (int)Right];
        }

        public static float Distance(PhonemeVoice Left, PhonemeVoice Right)
        {
            return s_VoiceDistance[(int)Left, (int)Right];
        }
    }
}