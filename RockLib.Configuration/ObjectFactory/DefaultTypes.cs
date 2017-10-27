﻿using System;
using System.Collections.Generic;

namespace RockLib.Configuration.ObjectFactory
{
    internal interface IDefaultTypes
    {
        bool TryGet(Type declaringType, string memberName, out Type defaultType);
    }

    public sealed class DefaultTypes : IDefaultTypes
    {
        private readonly Dictionary<string, Type> _dictionary = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        private DefaultTypes() {}

        internal static IDefaultTypes Empty { get; } = new DefaultTypes();

        public static DefaultTypes New(Type declaringType, string memberName, Type defaultType)
        {
            return new DefaultTypes().Add(declaringType, memberName, defaultType);
        }

        public DefaultTypes Add(Type declaringType, string memberName, Type defaultType)
        {
            if (declaringType == null) throw new ArgumentNullException(nameof(declaringType));
            if (memberName == null) throw new ArgumentNullException(nameof(memberName));
            if (defaultType == null) throw new ArgumentNullException(nameof(defaultType));
            _dictionary.Add(GetKey(declaringType, memberName), defaultType);
            return this;
        }

        bool IDefaultTypes.TryGet(Type declaringType, string memberName, out Type defaultType) =>
            _dictionary.TryGetValue(GetKey(declaringType, memberName), out defaultType);

        private static string GetKey(Type declaringType, string memberName) => 
            (declaringType != null && memberName != null) ? declaringType.FullName + ":" + memberName : "";
    }
}
