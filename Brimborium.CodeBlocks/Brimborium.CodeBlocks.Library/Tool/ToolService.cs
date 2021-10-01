namespace Brimborium.CodeBlocks.Tool {
    public class ToolService {
        public ToolService(FileSystemService baseFileSystem, FileSystemService tempFileSystem) {
            this.BaseFileSystem = baseFileSystem;
            this.TempFileSystem = tempFileSystem;
        }

        public FileSystemService BaseFileSystem { get; }
        public FileSystemService TempFileSystem { get; }
    }
}
