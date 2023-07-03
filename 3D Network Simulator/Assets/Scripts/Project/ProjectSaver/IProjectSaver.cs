namespace Project.ProjectSaver
{
    /// <summary>
    /// Interface used for classes saving projects
    /// </summary>
    public interface IProjectSaver
    {
        public void Save(string data);
    }
}