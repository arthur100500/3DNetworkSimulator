using GNS3.GNSThread;
using GNS3.ProjectHandling.Project;
using Menu.Json;
using Tasks.Requests;

namespace Gameplay.MainMenu.InterScene
{
    public static class MenuToGameExchanger
    {
        public static IRequestMaker RequestMaker;
        public static IQueuedTaskDispatcher Dispatcher;
        public static GnsProjectConfig ProjectConfig;
        public static NsJProject InitialProject;
    }
}