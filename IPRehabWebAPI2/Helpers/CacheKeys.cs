namespace IPRehabWebAPI2.Helpers
{
    public static class CacheKeys
    {
        public static string CacheKeyThisUserAccessLevel { get; } = "ThisUserAccessLevel";

        public static string CacheKeyAllPatients { get; } = "AllFacilityPatients";

        public static string CacheKeyAllPatients_TreatingSpeciality { get; } = "AllFacilityPatients_TreatingSpeciality";

        public static string CacheKeyThisFacilityPatients { get; } = "ThisFacilityPatients";

        public static string CacheKeyThisPatient { get; } = "ThisPatient";

        public static string CacheKeyAllQuestions { get; } = "AllQuestions";
    }
}
