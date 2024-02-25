#region Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved
// Web: https://technosoftware.com 
//
// The Software is subject to the Technosoftware GmbH Software License 
// Agreement, which can be found here:
// https://technosoftware.com/documents/Source_License_Agreement.pdf
//
// The Software is based on the OPC Foundation MIT License. 
// The complete license agreement for that can be found here:
// http://opcfoundation.org/License/MIT/1.00/
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2011-2023 Technosoftware GmbH. All rights reserved

#region Using Directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.Globalization;
using System.Xml;
using System.Reflection;

using Opc.Ua;

#endregion

namespace Technosoftware.UaServer.NodeManager
{
    /// <summary>
    /// An object that manages access to localized resources.
    /// </summary>
    public class ResourceManager : IDisposable, ITranslationManager
    {
        #region Constructors, Destructor, Initialization
        /// <summary>
        /// Initializes the resource manager with the server instance that owns it.
        /// </summary>
        public ResourceManager(IUaServerData server, ApplicationConfiguration configuration)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));    
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            server_ = server;
            translationTables_ = new List<TranslationTable>();
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// May be called by the application to clean up resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Cleans up all resources held by the object.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // nothing to do at this time.
            }
        }
        #endregion

        #region ITranslationManager Members
        /// <summary cref="ITranslationManager.Translate(IList{string}, string, string, object[])" />
        public virtual LocalizedText Translate(IList<string> preferredLocales, string key, string text, params object[] args)
        {
            return Translate(preferredLocales, null, new TranslationInfo(key, String.Empty, text, args));
        }

        /// <virtual cref="ITranslationManager.Translate(IList{string}, LocalizedText)" />
        public LocalizedText Translate(IList<string> preferredLocales, LocalizedText text)
        {
            return Translate(preferredLocales, text, text.TranslationInfo);
        }

        /// <summary>
        /// Translates a service result.
        /// </summary>
        public ServiceResult Translate(IList<string> preferredLocales, ServiceResult result)
        {
            if (result == null)
            {
                return null;
            }

            // translate localized text.
            var translatedText = result.LocalizedText;

            if (LocalizedText.IsNullOrEmpty(result.LocalizedText))
            {
                // extract any additional arguments from the translation info.
                object[] args = null;

                if (result.LocalizedText != null && result.LocalizedText.TranslationInfo != null)
                {
                    var info = result.LocalizedText.TranslationInfo;

                    if (info != null && info.Args != null && info.Args.Length > 0)
                    {
                        args = info.Args;
                    }
                }

                if (!String.IsNullOrEmpty(result.SymbolicId))
                {
                    translatedText = TranslateSymbolicId(preferredLocales, result.SymbolicId, result.NamespaceUri, args);
                }
                else
                {
                    translatedText = TranslateStatusCode(preferredLocales, result.StatusCode, args);
                }
            }
            else
            {
                if (preferredLocales == null || preferredLocales.Count == 0)
                {
                    return result;
                }

                translatedText = Translate(preferredLocales, result.LocalizedText);
            }

            // construct new service result.
            var translatedResult = new ServiceResult(
                result.StatusCode,
                result.SymbolicId,
                result.NamespaceUri,
                translatedText,
                result.AdditionalInfo,
                Translate(preferredLocales, result.InnerResult));

            return translatedResult;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns the locales supported by the resource manager.
        /// </summary>
        public virtual string[] GetAvailableLocales()
        {
            lock (lock_)
            {
                var availableLocales = new string[translationTables_.Count];

                for (var ii = 0; ii < translationTables_.Count; ii++)
                {
                    availableLocales[ii] = translationTables_[ii].Locale.Name;
                }

                return availableLocales;
            }
        }

        /// <summary>
        /// Returns the locales supported by the resource manager.
        /// </summary>
        [Obsolete("preferredLocales argument is ignored.")]
        public string[] GetAvailableLocales(IEnumerable<string> preferredLocales)
        {
            return GetAvailableLocales();
        }

        /// <summary>
        /// Returns the localized form of the text that best matches the preferred locales.
        /// </summary>
        [Obsolete("Replaced by the overrideable ITranslationManager methods.")]
        public LocalizedText GetText(IList<string> preferredLocales, string textId, string defaultText, params object[] args)
        {
            return Translate(preferredLocales, textId, defaultText, args);
        }

        /// <summary>
        /// Adds a translation to the resource manager.
        /// </summary>
        public void Add(string key, string locale, string text)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (locale == null) throw new ArgumentNullException(nameof(locale));
            if (text == null) throw new ArgumentNullException(nameof(text));

            var culture = new CultureInfo(locale);

            if (culture.IsNeutralCulture)
            {
                throw new ArgumentException("Cannot specify neutral locales for translation tables.", nameof(locale));
            }

            lock (lock_)
            {
                var table = GetTable(culture.Name);
                table.Translations[key] = text;
            }
        }

        /// <summary>
        /// Adds the translations to the resource manager.
        /// </summary>
        public void Add(string locale, IDictionary<string,string> translations)
        {
            if (locale == null) throw new ArgumentNullException(nameof(locale));
            if (translations == null) throw new ArgumentNullException(nameof(translations));

            var culture = new CultureInfo(locale);

            if (culture.IsNeutralCulture)
            {
                throw new ArgumentException("Cannot specify neutral locales for translation tables.", nameof(locale));
            }

            lock (lock_)
            {
                var table = GetTable(culture.Name);

                foreach (var translation in translations)
                {
                    table.Translations[translation.Key] = translation.Value;
                }
            }
        }

        /// <summary>
        /// Adds the translations to the resource manager.
        /// </summary>
        public void Add(uint statusCode, string locale, string text)
        {
            lock (lock_)
            {
                var key = statusCode.ToString();

                Add(key, locale, text);

                if (statusCodeMapping_ == null)
                {
                    statusCodeMapping_ = new Dictionary<uint,TranslationInfo>();
                }

                if (String.IsNullOrEmpty(locale) || locale == "en-US")
                {
                    statusCodeMapping_[statusCode] = new TranslationInfo(key, locale, text);
                }
            }
        }

        /// <summary>
        /// Adds the translations to the resource manager.
        /// </summary>
        public void Add(XmlQualifiedName symbolicId, string locale, string text)
        {
            lock (lock_)
            {
                if (symbolicId != null)
                {
                    var key = symbolicId.ToString();

                    Add(key, locale, text);

                    if (symbolicIdMapping_ == null)
                    {
                        symbolicIdMapping_ = new Dictionary<XmlQualifiedName, TranslationInfo>();
                    }

                    if (String.IsNullOrEmpty(locale) || locale == "en-US")
                    {
                        symbolicIdMapping_[symbolicId] = new TranslationInfo(key, locale, text);
                    }
                }
            }
        }

        /// <summary>
        /// Uses reflection to load default text for standard StatusCodes.
        /// </summary>
        public void LoadDefaultText()
        {
            var fields = typeof(StatusCodes).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            foreach (var field in fields)
            {
                var id = field.GetValue(typeof(StatusCodes)) as uint?;

                if (id != null)
                {
                    this.Add(id.Value, "en-US", field.Name);
                }
            }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Translates the text provided.
        /// </summary>
        protected virtual LocalizedText Translate(IList<string> preferredLocales, LocalizedText defaultText, TranslationInfo info)
        {
            // check for trivial case.
            if (info == null || String.IsNullOrEmpty(info.Text))
            {
                return defaultText;
            }

            // check for exact match.
            if (preferredLocales != null && preferredLocales.Count > 0)
            {
                if (defaultText != null && preferredLocales[0] == defaultText.Locale)
                {
                    return defaultText;
                }

                if (preferredLocales[0] == info.Locale)
                {
                    return new LocalizedText(info);
                }
            }

            // use the text as the key.
            var key = info.Key;

            if (key == null)
            {
                key = info.Text;
            }

            // find the best translation.
            var translatedText = info.Text;
            var culture = CultureInfo.InvariantCulture;

            lock (lock_)
            {
                translatedText = FindBestTranslation(preferredLocales, key, out culture);

                // use the default if no translation available.
                if (translatedText == null)
                {
                    return defaultText;
                }

                // get a culture to use for formatting
                if (culture == null)
                {
                    if (info.Args != null && info.Args.Length > 0 && !String.IsNullOrEmpty(info.Locale))
                    {
                        try
                        {
                            culture = new CultureInfo(info.Locale);
                        }
                        catch
                        {
                            culture = CultureInfo.InvariantCulture;
                        }
                    }
                }
            }

            // format translated text.
            var formattedText = translatedText;

            if (info.Args != null && info.Args.Length > 0)
            {
                try
                {
                    formattedText = String.Format(culture, translatedText, info.Args);
                }
                catch
                {
                    formattedText = translatedText;
                }
            }

            // construct translated localized text.
            var finalText = new LocalizedText(culture.Name, formattedText);
            finalText.TranslationInfo = info;
            return finalText;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Stores the translations for a locale.
        /// </summary>
        private class TranslationTable
        {
            public CultureInfo Locale;
            public SortedDictionary<string, string> Translations = new SortedDictionary<string, string>();
        }

        /// <summary>
        /// Finds the translation table for the locale. Creates a new table if it does not exist.
        /// </summary>
        private TranslationTable GetTable(string locale)
        {
            lock (lock_)
            {
                // search for table.
                for (var ii = 0; ii < translationTables_.Count; ii++)
                {
                    var translationTable = translationTables_[ii];

                    if (translationTable.Locale.Name == locale)
                    {
                        return translationTable;
                    }
                }

                // add table.
                var table = new TranslationTable();
                table.Locale = new CultureInfo(locale);
                translationTables_.Add(table);

                return table;
            }
        }

        /// <summary>
        /// Finds the best translation for the requested locales.
        /// </summary>
        private string FindBestTranslation(IList<string> preferredLocales, string key, out CultureInfo culture)
        {
            culture = null;
            TranslationTable match = null;

            if (preferredLocales == null || preferredLocales.Count == 0) { return null; }

            for (var jj = 0; jj < preferredLocales.Count; jj++)
            {
                // parse the locale.
                var language = preferredLocales[jj];

                if (language == null)
                {
                    continue;
                }

                var index = language.IndexOf('-');

                if (index != -1)
                {
                    language = language.Substring(0, index);
                }

                // search for translation.
                string translatedText = null;

                for (var ii = 0; ii < translationTables_.Count; ii++)
                {
                    var translationTable = translationTables_[ii];

                    // all done if exact match found.
                    if (translationTable.Locale.Name == preferredLocales[jj])
                    {
                        if (translationTable.Translations.TryGetValue(key, out translatedText))
                        {
                            culture = translationTable.Locale;
                            return translatedText;
                        }
                    }

                    // check for matching language but different region.
                    if (match == null && translationTable.Locale.TwoLetterISOLanguageName == language)
                    {
                        if (translationTable.Translations.TryGetValue(key, out translatedText))
                        {
                            culture = translationTable.Locale;
                            match = translationTable;
                        }

                        continue;
                    }
                }

                // take a partial match if one found.
                if (match != null)
                {
                    return translatedText;
                }
           }

           // no translations available.
           return null;
        }

        /// <summary>
        /// Translates a status code.
        /// </summary>
        private LocalizedText TranslateStatusCode(IList<string> preferredLocales, StatusCode statusCode, object[] args)
        {
            lock (lock_)
            {
                if (statusCodeMapping_ != null)
                {
                    TranslationInfo info = null;

                    if (statusCodeMapping_.TryGetValue(statusCode.Code, out info))
                    {
                        // merge the argument list with the trahslateion info cached for the status code.
                        if (args != null)
                        {
                            info = new TranslationInfo(
                                info.Key,
                                info.Locale,
                                info.Text,
                                args);
                        }

                        return Translate(preferredLocales, null, info);
                    }
                }
            }

            return String.Format("{0:X8}", statusCode.Code);
        }

        /// <summary>
        /// Translates a symbolic id.
        /// </summary>
        private LocalizedText TranslateSymbolicId(IList<string> preferredLocales, string symbolicId, string namespaceUri, object[] args)
        {
            lock (lock_)
            {
                if (symbolicIdMapping_ != null)
                {
                    TranslationInfo info = null;

                    if (symbolicIdMapping_.TryGetValue(new XmlQualifiedName(symbolicId, namespaceUri), out info))
                    {
                        // merge the argument list with the trahslateion info cached for the symbolic id.
                        if (args != null)
                        {
                            info = new TranslationInfo(
                                info.Key,
                                info.Locale,
                                info.Text,
                                args);
                        }

                        return Translate(preferredLocales, null, info);
                    }
                }
            }

            return symbolicId;
        }
        #endregion

        #region Private Fields
        private readonly object lock_ = new object();
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private IUaServerData server_;
        private List<TranslationTable> translationTables_;
        private Dictionary<uint, TranslationInfo> statusCodeMapping_;
        private Dictionary<XmlQualifiedName, TranslationInfo> symbolicIdMapping_;
        #endregion
    }
}
