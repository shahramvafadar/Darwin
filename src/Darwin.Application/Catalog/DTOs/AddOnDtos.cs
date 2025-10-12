﻿using System;
using System.Collections.Generic;
using Darwin.Domain.Entities.Catalog;

namespace Darwin.Application.Catalog.DTOs
{
    /// <summary>
    /// Create payload for an AddOnGroup with nested options/values.
    /// </summary>
    public sealed class AddOnGroupCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Currency { get; set; } = "EUR";
        public bool IsGlobal { get; set; } = false;
        public Domain.Enums.AddOnSelectionMode SelectionMode { get; set; } = Domain.Enums.AddOnSelectionMode.Single;
        public int MinSelections { get; set; } = 0;
        public int? MaxSelections { get; set; }
        public bool IsActive { get; set; } = true;
        public List<AddOnOptionDto> Options { get; set; } = new();
    }

    /// <summary>
    /// Edit payload for an AddOnGroup (whole aggregate upsert; simplest approach for phase 1).
    /// </summary>
    public sealed class AddOnGroupEditDto
    {
        public Guid Id { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public string Name { get; set; } = string.Empty;
        public string Currency { get; set; } = "EUR";
        public bool IsGlobal { get; set; } = false;
        public Domain.Enums.AddOnSelectionMode SelectionMode { get; set; } = Domain.Enums.AddOnSelectionMode.Single;
        public int MinSelections { get; set; } = 0;
        public int? MaxSelections { get; set; }
        public bool IsActive { get; set; } = true;
        public List<AddOnOptionDto> Options { get; set; } = new();
    }

    /// <summary>
    /// Option DTO with nested values.
    /// </summary>
    public sealed class AddOnOptionDto
    {
        public string Label { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 0;
        public List<AddOnOptionValueDto> Values { get; set; } = new();
    }

    /// <summary>
    /// Value DTO with price delta (minor units).
    /// </summary>
    public sealed class AddOnOptionValueDto
    {
        public string Label { get; set; } = string.Empty;
        public long PriceDeltaMinor { get; set; } = 0;
        public string? Hint { get; set; }
        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Lightweight list item for Admin grid.
    /// </summary>
    public sealed class AddOnGroupListItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Currency { get; set; } = "EUR";
        public bool IsGlobal { get; set; }
        public bool IsActive { get; set; }
        public int OptionsCount { get; set; }
        public DateTime? ModifiedAtUtc { get; set; }
    }

    /// <summary>
    /// Lookup used by forms to attach a group to products/categories/brands.
    /// </summary>
    public sealed class AddOnGroupLookupItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
