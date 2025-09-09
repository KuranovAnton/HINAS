using System;
using System.Collections.Generic;

namespace HINAS;

public partial class CheckList
{
    public int Id { get; set; }

    public int? PlannerId { get; set; }

    public string? Task { get; set; }

    public bool? Done { get; set; }

    public virtual Planner? Planner { get; set; }
}
