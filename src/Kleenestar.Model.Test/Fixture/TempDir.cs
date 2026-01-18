namespace Kleenestar.Model.Test.Fixture
{
    /// <summary>
    /// Provides a temporary directory that is automatically created upon instantiation 
    /// and deleted when disposed.
    /// </summary>
    public class TempDir : IDisposable
    {
        /// <summary>
        /// Returns the file system path associated with this instance.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Initializes a new instance of the class and creates a unique temporary directory on disk.
        /// </summary>
        public TempDir()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(Path);
        }

        /// <summary>
        /// Releases all resources used by the object and deletes the associated directory 
        /// and its contents.
        /// </summary>
        public void Dispose()
        {
            try { Directory.Delete(Path, true); } catch { }
        }
    }
}
