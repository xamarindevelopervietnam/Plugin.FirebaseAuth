﻿using System;
using System.Collections.Generic;
using System.Linq;
using Android.Runtime;
using Android.Support.V4.Util;

namespace Plugin.FirebaseAuth
{
    public class AdditionalUserInfoWrapper : IAdditionalUserInfo
    {
        internal Firebase.Auth.IAdditionalUserInfo AdditionalUserInfo { get; }

        private IDictionary<string, object> _profile;
        public IDictionary<string, object> Profile
        {
            get
            {
                if (AdditionalUserInfo.Profile != null && _profile == null)
                {
                    _profile = AdditionalUserInfo.Profile.ToDictionary(pair => pair.Key, pair => ConvertProfileValue(pair.Value));
                }
                return _profile;
            }
        }

        public string ProviderId => AdditionalUserInfo.ProviderId;

        public string Username => AdditionalUserInfo.Username;

        public AdditionalUserInfoWrapper(Firebase.Auth.IAdditionalUserInfo additionalUserInfo)
        {
            AdditionalUserInfo = additionalUserInfo;
        }

        private object ConvertProfileValue(Java.Lang.Object profileValue)
        {
            if (profileValue == null)
                return null;

            switch (profileValue)
            {
                case Java.Lang.Boolean @boolean:
                    return (bool)@boolean;
                case Java.Lang.Short @short:
                    return (short)@short;
                case Java.Lang.Integer @integer:
                    return (int)@integer;
                case Java.Lang.Long @long:
                    return (long)@long;
                case Java.Lang.Float @float:
                    return (float)@float;
                case Java.Lang.Double @double:
                    return (double)@double;
                case Java.Lang.Number @number:
                    return (double)@number;
                case Java.Lang.String @string:
                    return profileValue.ToString();
                case JavaList javaList:
                    {
                        var list = new List<object>();
                        foreach (var data in javaList)
                        {
                            var value = data;
                            if (value is Java.Lang.Object javaObject)
                            {
                                value = ConvertProfileValue(javaObject);
                            }
                            list.Add(value);
                        }
                        return list;
                    }
                case JavaDictionary javaDictionary:
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (var key in javaDictionary.Keys)
                        {
                            var value = javaDictionary[key];
                            if (value is Java.Lang.Object javaObject)
                            {
                                value = ConvertProfileValue(javaObject);
                            }
                            dict[key.ToString()] = value;
                        }
                        return dict;
                    }
                case ArrayMap arrayMap:
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (var key in arrayMap.KeySet())
                        {
                            dict[key.ToString()] = ConvertProfileValue(arrayMap.Get(new Java.Lang.String(key.ToString())));
                        }
                        return dict;
                    }
                default:
                    if (profileValue.ToString() == "null")
                    {
                        return null;
                    }
                    return profileValue;
            }
        }
    }
}
