﻿// <copyright file="PostMap.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using UmbMapper.Sample.Models.Pages;

namespace UmbMapper.Sample.ComponentModel.Mappers
{
    /// <summary>
    /// Configures mapping for post pages
    /// </summary>
    public class PostMap : PublishedPageMap<Post>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostMap"/> class.
        /// </summary>
        public PostMap()
        {
            this.MapAll().ForEach(x => x.AsLazy());
        }
    }
}