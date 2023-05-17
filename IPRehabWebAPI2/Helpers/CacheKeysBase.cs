namespace IPRehabWebAPI2.Helpers
{
    public static class CacheKeysBase
    {
        private static readonly string _cacheKeyThisUserAccessLevel = "ThisUserAccessLevel";
        public static string CacheKeyThisUserAccessLevel
        {
            get { return _cacheKeyThisUserAccessLevel; }
        }

        private static readonly string _cacheKeyAllPatients = "AllFacilityPatients";
        public static string CacheKeyAllPatients
        {
            get { return _cacheKeyAllPatients; }
        }

        private static readonly string _cacheKeyThisFacilityPatients = "ThisFacilityPatients";
        public static string CacheKeyThisFacilityPatients
        {
            get { return _cacheKeyThisFacilityPatients; }
        }

        private static readonly string _cacheKeyThisPatient = "ThisPatient";
        public static string CacheKeyThisPatient
        {
            get { return _cacheKeyThisPatient; }
        }

        private static readonly string _cacheKeyAllQuestions = "AllQuestions";
        public static string CacheKeyAllQuestions
        {
            get { return _cacheKeyAllQuestions; }
        }
    }
}
