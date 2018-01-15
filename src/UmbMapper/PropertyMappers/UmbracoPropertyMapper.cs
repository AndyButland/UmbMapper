﻿// <copyright file="UmbracoPropertyMapper.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Concurrent;
using UmbMapper.Extensions;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace UmbMapper.PropertyMappers
{
    /// <summary>
    /// Maps directly from the published content
    /// </summary>
    public class UmbracoPropertyMapper : PropertyMapperBase
    {
        private static readonly ConcurrentDictionary<string, FastPropertyAccessor> ContentAccessorCache
            = new ConcurrentDictionary<string, FastPropertyAccessor>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UmbracoPropertyMapper"/> class.
        /// </summary>
        /// <param name="info">The property map information</param>
        public UmbracoPropertyMapper(PropertyMapInfo info)
            : base(info)
        {
        }

        /// <inheritdoc/>
        public override object Map(IPublishedContent content, object value)
        {
            PropertyMapInfo info = this.Info;

            // First try custom properties
            foreach (string name in info.Aliases)
            {
                value = this.CheckConvertType(content.GetPropertyValue(name, info.Recursive));
                if (info.PropertyType.IsInstanceOfType(value))
                {
                    break;
                }
            }

            // Then try class properties
            if (value.IsNullOrEmptyString() || value == info.DefaultValue)
            {
                Type contentType = content.GetType();
                string key = contentType.AssemblyQualifiedName;

                if (key != null)
                {
                    ContentAccessorCache.TryGetValue(key, out FastPropertyAccessor accessor);
                    if (accessor == null)
                    {
                        accessor = new FastPropertyAccessor(contentType);
                        ContentAccessorCache.TryAdd(key, accessor);
                    }

                    foreach (string name in info.Aliases)
                    {
                        value = this.CheckConvertType(accessor.GetValue(name, content));
                        if (!value.IsNullOrEmptyString() && !value.Equals(info.DefaultValue))
                        {
                            break;
                        }
                    }
                }
            }

            return value ?? info.DefaultValue;
        }
    }
}