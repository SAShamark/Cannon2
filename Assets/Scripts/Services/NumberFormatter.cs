namespace UI
{
    public static class NumberFormatter
    {
        private static readonly string[] Suffixes =
        {
            "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "De",
            "UnDe", "DuDe", "TrDe", "QaDe", "QiDe", "SxDe", "SpDe", "OcDe", "NoDe"
        };

        private const double THOUSAND = 1000.0;
        private const int CUTOFF_FOR_SUFFIX = 10_000;

        public static string FormatNumber(double value)
        {
            if (value < CUTOFF_FOR_SUFFIX)
            {
                return ((int)value).ToString();
            }

            int suffixIndex = 0;
            while (value >= THOUSAND && suffixIndex < Suffixes.Length - 1)
            {
                value /= THOUSAND;
                suffixIndex++;
            }

            return $"{value:0.#}{Suffixes[suffixIndex]}";
        }
    }
}