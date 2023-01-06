using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Maui.TwitchLogin.Core
{
    public record class TwitchLoginState
    {
        private const string GUID_FORMAT_NO_SEPARATORS = "N";

        public static TwitchLoginState Empty { get; } = new TwitchLoginState(string.Empty);

        public string Value { get; }

        public TwitchLoginState(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;

        public static TwitchLoginState GenerateNew()
        {
            Guid guid = Guid.NewGuid();

            string stateValue = guid.ToString(GUID_FORMAT_NO_SEPARATORS);

            return new TwitchLoginState(stateValue);
        }

    }
}
