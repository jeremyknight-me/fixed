﻿using JK.Fixed.Attributes;

namespace JK.Fixed.Tests.TestUtils;

public class MappedTester
{
    [FixedColumn(5)]
    public bool One { get; set; }

    [FixedColumn(2)]
    public string Two { get; set; }

    [FixedColumn(10, StringFormat = "yyyy-MM-dd")]
    public DateTime Three { get; set; }

    [FixedColumn(4, Alignment = FixedColumnAlignment.Right)]
    public int Four { get; set; }

    [FixedColumn(5, StringFormat = "N2", Alignment = FixedColumnAlignment.Right)]
    public decimal Five { get; set; }
}
