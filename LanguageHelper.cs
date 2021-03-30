using System;
using System.Collections.Generic;
using System.Text;

namespace ItemBase
{
    class LanguageHelper
    {
        static readonly Dictionary<String, Dictionary<String, String>> Languages = new Dictionary<string, Dictionary<string, string>>();
        static LanguageHelper()
        {
            Languages.Add("", new Dictionary<string, string>());

            On.RoR2.Language.GetLocalizedStringByToken += Language_GetLocalizedStringByToken;
            On.RoR2.Language.TokenIsRegistered += Language_TokenIsRegistered;
        }

        private static bool Language_TokenIsRegistered(On.RoR2.Language.orig_TokenIsRegistered orig, RoR2.Language self, string token)
        {
            return Languages.ContainsKey(self.name) && Languages[self.name].ContainsKey(token) || Languages[""].ContainsKey(token) || orig(self, token);
        }

        private static string Language_GetLocalizedStringByToken(On.RoR2.Language.orig_GetLocalizedStringByToken orig, RoR2.Language self, string token)
        {
            String token_string;
            if(Languages.ContainsKey(self.name) && Languages[self.name].TryGetValue(token, out token_string) || Languages[""].TryGetValue(token, out token_string)) {
                return token_string;
            }
            return orig(self, token);
        }

        public static void Add(String token, String token_string, String language = "")
        {
            if (Languages.ContainsKey(language))
            {
                Languages[language].Add(token, token_string);
            }
            else
            {
                Languages.Add(language, new Dictionary<string, string>());
                Languages[language].Add(token, token_string);
            }
            if (!String.IsNullOrEmpty(language) && !Languages[""].ContainsKey(token)) Languages[""].Add(token, token_string);
        }
    }
}
