﻿// <copyright file="EnumPropertyMapper.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using UmbMapper.Extensions;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace UmbMapper.PropertyMappers
{
    /// <summary>
    /// Maps from the <see cref="IPublishedContent"/> to an enum.
    /// </summary>
    public class EnumPropertyMapper : PropertyMapperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumPropertyMapper"/> class.
        /// </summary>
        /// <param name="config">The property configuration</param>
        public EnumPropertyMapper(PropertyMapperConfig config)
            : base(config)
        {
        }

        /// <inheritdoc />
        public override object Map(IPublishedContent content, object value)
        {
            if (value == null)
            {
                return this.DefaultValue;
            }

            Type propertyType = this.PropertyType;

            string strValue = value as string;
            if (strValue != null)
            {
                if (strValue.IndexOf(',') != -1)
                {
                    long convertedValue = 0;
                    IList<string> values = strValue.ToDelimitedList();

                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (string v in values)
                    {
                        // OR assignment. Stolen from ComponentModel EnumConverter.
                        convertedValue |= Convert.ToInt64((Enum)Enum.Parse(propertyType, v, true), this.Culture);
                    }

                    return Enum.ToObject(propertyType, convertedValue);
                }

                return Enum.Parse(propertyType, strValue, true);
            }

            if (value is int)
            {
                // Should handle most cases.
                if (Enum.IsDefined(propertyType, value))
                {
                    return Enum.ToObject(propertyType, value);
                }
            }

            Type valueType = value.GetType();
            if (valueType.IsEnum)
            {
                // This should work for most cases where enums base type is int.
                return Enum.ToObject(propertyType, Convert.ToInt64(value, this.Culture));
            }

            if (valueType.IsEnumerableOfType(typeof(string)))
            {
                long convertedValue = 0;
                var enumerable = ((IEnumerable<string>)value).ToList();

                if (enumerable.Any())
                {
                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (string v in enumerable)
                    {
                        convertedValue |= Convert.ToInt64((Enum)Enum.Parse(propertyType, v, true), this.Culture);
                    }

                    return Enum.ToObject(propertyType, convertedValue);
                }

                return propertyType.GetInstance();
            }

            return this.DefaultValue;
        }
    }
}