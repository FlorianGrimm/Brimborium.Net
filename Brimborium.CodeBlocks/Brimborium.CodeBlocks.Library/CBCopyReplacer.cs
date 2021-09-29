namespace Brimborium.CodeBlocks {
    public class CBCopyReplacer {
        public CBCopyReplacer() {
        }

        public string ContentCopyReplace(string nextCode, string oldCode) {
            if (string.IsNullOrEmpty(oldCode)) {
                return nextCode;
            }

            var nextParser = new CBParser();
            var nextAst = nextParser.Parse(nextParser.Tokenize(nextCode));
            var oldParser = new CBParser();
            var oldAst = oldParser.Parse(oldParser.Tokenize(oldCode));

            return nextCode;
        }
    }
}
