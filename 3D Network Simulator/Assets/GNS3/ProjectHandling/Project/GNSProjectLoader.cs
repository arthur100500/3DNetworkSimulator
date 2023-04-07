namespace GNSHandling
{
    /*
    Class for containg global parameters like project, last node name id etc
    */
    internal static class GlobalGNSParameters
    {
        private static int nid = 1;
        private static GNSProject project;

        public static int GetNextFreeID()
        {
            return nid++;
        }

        public static GNSProject GetProject()
        {
            return project ??= new GNSProject(GNSProjectConfig.LocalGNSProjectConfig(), "unity_project");
        }
    }
}