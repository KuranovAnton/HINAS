using System;
using System.Collections.Generic;

namespace HINAS;

public partial class Planner
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public DateOnly? TripDate { get; set; }

    public string? Route { get; set; }

    public string? ItemsToPack { get; set; }

    public string? Events { get; set; }

    public string? Reminders { get; set; }

    public string? Maintenance { get; set; }

    public string? Supplies { get; set; }

    public virtual ICollection<CheckList> CheckLists { get; set; } = new List<CheckList>();
}
