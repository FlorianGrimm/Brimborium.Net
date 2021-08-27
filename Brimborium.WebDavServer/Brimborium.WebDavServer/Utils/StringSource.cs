using System;

namespace Brimborium.WebDavServer.Utils
{
    internal class StringSource
    {
        private readonly string _s;

        private int _currentIndex;

        public StringSource(string s)
        {
            this._s = s;
        }

        public bool Empty => this._currentIndex >= this._s.Length;

        public string Remaining => this._s.Substring(this._currentIndex);

        public char Get()
        {
            return this._s[this._currentIndex++];
        }

        public void Back()
        {
            this._currentIndex -= 1;
        }

        public bool AdvanceIf(string text)
        {
            return this.AdvanceIf(text, StringComparison.Ordinal);
        }

        public bool AdvanceIf(string text, StringComparison comparer)
        {
            if (!this._s.Substring(this._currentIndex).StartsWith(text, comparer)) {
                return false;
            }

            this._currentIndex += text.Length;
            return true;
        }

        public StringSource Advance(int count)
        {
            this._currentIndex += count;
            return this;
        }

        public bool SkipWhiteSpace()
        {
            while (!this.Empty)
            {
                if (!char.IsWhiteSpace(this._s, this._currentIndex)) {
                    break;
                }

                this._currentIndex += 1;
            }

            return this.Empty;
        }

        public string? GetUntil(char ch)
        {
            var index = this._s.IndexOf(ch, this._currentIndex);
            if (index == -1) {
                return null;
            }

            var result = this._s.Substring(this._currentIndex, index - this._currentIndex);
            this._currentIndex = index;
            return result;
        }
    }
}
