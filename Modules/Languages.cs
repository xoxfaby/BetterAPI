using System;
using System.Collections.Generic;
using System.Text;

namespace BetterAPI
{
    public static class Languages
    {
        private static readonly Dictionary<String, Dictionary<String, String>> languages;
        static Languages()
        {
            languages = new Dictionary<string, Dictionary<string, string>>();
            languages.Add("", new Dictionary<string, string>());

            On.RoR2.Language.GetLocalizedStringByToken += Language_GetLocalizedStringByToken;
            On.RoR2.Language.TokenIsRegistered += Language_TokenIsRegistered;
        }

        private static bool Language_TokenIsRegistered(On.RoR2.Language.orig_TokenIsRegistered orig, RoR2.Language self, string token)
        {
            return languages.ContainsKey(self.name) && languages[self.name].ContainsKey(token) || languages[""].ContainsKey(token) || orig(self, token);
        }

        private static string Language_GetLocalizedStringByToken(On.RoR2.Language.orig_GetLocalizedStringByToken orig, RoR2.Language self, string token)
        {
            String token_string;
            if(languages.ContainsKey(self.name) && languages[self.name].TryGetValue(token, out token_string) || languages[""].TryGetValue(token, out token_string)) {
                return token_string;
            }
            return orig(self, token);
        }

        public static void AddTokenString(String token, String token_string, String language = "")
        {
            if (languages.ContainsKey(language))
            {
                languages[language].Add(token, token_string);
            }
            else
            {
                languages.Add(language, new Dictionary<string, string>());
                languages[language].Add(token, token_string);
            }
            if (!String.IsNullOrEmpty(language) && !languages[""].ContainsKey(token)) languages[""].Add(token, token_string);
        }
    }
}
