namespace Idea {
    // [Template]
    public class ClassTemplate1 {
        public ClassTemplate1() {
        }

        // <Replacement>
        public int A { get; set; }
        // </Replacement>
    }

    // [Transformation]
    public class ClassTransform1 {
    }

    // [Generated]
    public class ClassGenerated1 {
        public ClassGenerated1() {
        }
        
        // <Replacement>
        public int AB { get; set; }
        // </Replacement>
    }
}