using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brimborium.CodeBlocks {
    public class CBCopyReplacer {
        public CBCopyReplacer() {
        }

        public string ContentCopyReplace(string nextCode, string oldCode) {
            if (string.IsNullOrEmpty(oldCode)) {
                return nextCode;
            }

            var parser = new CBParser();
            var nextTokens = parser.Tokenize(nextCode);
            var nextAst = parser.Parse(nextTokens);

            var oldTokens = parser.Tokenize(oldCode);
            var oldAst = parser.Parse(oldTokens);

            StringBuilder result = new StringBuilder();
            var (newAst, _) = this.BuildAst(nextAst, oldAst, true);
            this.BuildCode(newAst, result);
            return result.ToString();
        }
        

        private (CBAstNode result, bool ok) BuildAst(CBAstNode nextAst, CBAstNode oldAst, bool preferNext) {
            var nextKind = nextAst.GetKind();
            var oldKind = oldAst.GetKind();
            //
            if (nextKind == CBAstNodeKind.Items) {
                if (oldKind == CBAstNodeKind.Items) {
                    var result = new CBAstNode();
                    return BuildAstItems(nextAst, oldAst, result, preferNext);
                }
                return (nextAst, true);
            }
            //
            if (nextKind == CBAstNodeKind.Content) {
                if (oldKind == CBAstNodeKind.Content) {
                    if (preferNext) {
                        //if ((nextAst.ContentToken is not null)
                        //    && (string.IsNullOrEmpty(nextAst.ContentToken.Text))) {
                        //    return (oldAst, true);
                        //} else {
                        //}
                            return (nextAst, true);
                    } else {
                        if ((oldAst.ContentToken is not null)
                            && (string.IsNullOrEmpty(oldAst.ContentToken.Text))) {
                            return (nextAst, true);
                        } else { 
                            return (oldAst, true);
                        }
                    }
                }
                return (nextAst, false);
            }
            //
            if (nextKind == CBAstNodeKind.Replacement) {
                if (oldKind == CBAstNodeKind.Replacement) {
                    if (string.Equals(nextAst.StartToken!.Text, oldAst.StartToken!.Text, StringComparison.OrdinalIgnoreCase)) {
                        var result = new CBAstNode();
                        result.StartToken = nextAst.StartToken;
                        result.FinishToken = nextAst.FinishToken;
                        return BuildAstItems(nextAst, oldAst, result, false);
                    }
                }
                return (nextAst, false);
            }
            return (nextAst, false);
        }

        record IndexNode(int Index, CBAstNode Node);

        private (CBAstNode result, bool ok) BuildAstItems(CBAstNode nextAst, CBAstNode oldAst, CBAstNode result, bool preferNext) {
            var lstOldReplacement = new List<IndexNode>();
            {
                for (int idxOld = 0; idxOld<oldAst.Items.Count;idxOld++) {
                    var oldItem = oldAst.Items[idxOld];
                    if (oldItem.GetKind() == CBAstNodeKind.Replacement) {
                        lstOldReplacement.Add( new IndexNode(idxOld, oldItem) );
                    }
                }
            }
            {
                int idxNext = 0;
                int idxOld = 0;
                bool itemsOk = true;
                while (idxNext < nextAst.Items.Count) {
                    CBAstNode nextItem = nextAst.Items[idxNext];
                    var nextItemKind = nextItem.GetKind();

                    CBAstNode oldItem = (idxOld < oldAst.Items.Count) ? oldAst.Items[idxOld] : CBAstNode.Empty;

                    //var oldItemKind = oldItem.GetKind();

                    if (nextItemKind == CBAstNodeKind.Replacement) {
                        IndexNode? foundIndexAndName=null;
                        IndexNode? foundNameOnly=null;
                        for (int idx = 0; idx < lstOldReplacement.Count; idx++) {
                            var oldIndexReplacement = lstOldReplacement[idx];
                            if (CBAstNodeTextName.GetInstance().Compare(nextItem, oldIndexReplacement.Node) == 0) {
                                if (oldIndexReplacement.Index == idxOld) {
                                    foundIndexAndName = oldIndexReplacement;
                                    break;
                                } else { 
                                    foundNameOnly = oldIndexReplacement;
                                }
                            } 
                        }
                        if (foundIndexAndName is not null) {
                            oldItem = foundIndexAndName.Node;
                            idxOld = foundIndexAndName.Index;
                            lstOldReplacement.Remove(foundIndexAndName);
                        } else if (foundNameOnly is not null) {
                            oldItem = foundNameOnly.Node;
                            idxOld = foundNameOnly.Index;
                            lstOldReplacement.Remove(foundNameOnly);
                        }
                    }

                    var (resultItem, resultOk) = this.BuildAst(nextItem, oldItem, preferNext);
                    result.Add(resultItem);
                        idxNext++;
                        idxOld++;
                    if (resultOk) {
                        continue;
                    } else {
                        itemsOk = false;
                    }
                }
                while (idxNext < nextAst.Items.Count) {
                    result.Add(nextAst.Items[idxNext]);
                    idxNext++;
                }
                return (result, itemsOk);
            } 
        }

        private void BuildCode(CBAstNode newAst, StringBuilder result) {
            switch (newAst.GetKind()) {
                case CBAstNodeKind.Replacement: {
                        result.Append(newAst.StartToken!.ToString());
                        //result.Append(newAst.StartToken.IndentWS);
                        foreach (var item in newAst.Items) {
                            this.BuildCode(item, result);
                        }
                        //result.Append(newAst.FinishToken!.PrefixWS);
                        result.Append(newAst.FinishToken!.ToString());
                    }
                    break;
                case CBAstNodeKind.Content: {
                        result.Append(newAst.ContentToken!.Text);
                    }
                    break;
                case CBAstNodeKind.Items: {
                        foreach (var item in newAst.Items) {
                            this.BuildCode(item, result);
                        }
                    }
                    break;
                case CBAstNodeKind.Faulted:
                    break;
                default:
                    break;
            }
        }
    }
}
