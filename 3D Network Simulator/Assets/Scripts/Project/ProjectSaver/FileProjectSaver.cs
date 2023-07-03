using System.IO;
using Menu.Json;
using Newtonsoft.Json;

namespace Project.ProjectSaver
{
    public class FileProjectSaver : IProjectSaver
    {
        private const string Path = "./LocalGnsProjects";
        private readonly NsJProject _initial;

        public FileProjectSaver(NsJProject nsProject)
        {
            _initial = nsProject;
        }
        
        public void Save(string data)
        {
            var nsProj = new NsJProject
            {
                Id = _initial.Id,
                Name = _initial.Name,
                GnsID = _initial.GnsID,
                JsonAnnotation = data,
                OwnerId = _initial.OwnerId
            };
            
            
            File.WriteAllText($"{Path}/{_initial.Name}", JsonConvert.SerializeObject(nsProj));
        }
    }
}