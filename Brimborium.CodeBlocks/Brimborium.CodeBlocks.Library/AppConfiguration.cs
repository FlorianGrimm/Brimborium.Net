using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brimborium.CodeBlocks {
    /// <summary>
    /// codegen.json
    /// </summary>
    public class AppConfiguration {
        /// <summary>
        /// BaseFolder root for relative path
        /// </summary>
        public string? BaseFolder { get; set; }

        /// <summary>
        /// Project folder / Project.csproj
        /// </summary>
        public string? Project { get; set; }
        
        /// <summary>
        /// the output assembly
        /// </summary>
        public string? Assembly { get; set; }

        public string? TempFolder { get; set; }

        public string? ConfigurationFile { get; set; }
    }
}
