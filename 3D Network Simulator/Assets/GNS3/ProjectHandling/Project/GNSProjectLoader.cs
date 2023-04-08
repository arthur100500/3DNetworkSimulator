namespace GNS3.ProjectHandling.Project
{
    /*
    Class for containg global parameters like project, last node name id etc
    */
    internal static class GlobalGnsParameters
    {
        private static int _nid = 1;
        private static GnsProject _project;

        public static int GetNextFreeID()
        {
            return _nid++;
        }

        public static GnsProject GetProject()
        {
            return _project ??= new GnsProject(GnsProjectConfig.LocalGnsProjectConfig(), "unity_project");
        }
    }
}