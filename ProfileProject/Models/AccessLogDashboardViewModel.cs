using ProfileProject.Models;

public class AccessLogDashboardViewModel
{
    public List<UserAccessCount> UserAccesses { get; set; }
    public List<PageAccessCount> PageAccesses { get; set; }
    public List<DailyAccessCount> DailyAccesses { get; set; }
    public List<LastAccessLogs> LastAccessLogs { get; set; }
    public List<LoginControl> LoginControls { get; set; }
    public List<UserVisitCount> UserVisitCounts { get; set; }
    public List<UserVisitList> UserVisitList { get; set; }
    public List<UserVisitGroup> UserVisitGroups { get; set; }
}

public class UserAccessCount
{
    public int? UserID { get; set; }
    public string? UserName { get; set; }
    public int Count { get; set; }
}

public class PageAccessCount
{
    public string Controller { get; set; }
    public string Action { get; set; }
    public int Count { get; set; }
}

public class DailyAccessCount
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
}

public class LastAccessLogs
{
    public int? UserID { get; set; }
    public string ControllerName { get; set; } = "";
    public string ActionName { get; set; } = "";
    public DateTime AccessTime { get; set; } = DateTime.Now;
    public string IPAddress { get; set; } = "";
    public string? Description { get; set; }
    public string? UserName { get; set; } = "";
}

public class UserVisitCount
{
    public int? UserID { get; set; }
    public string? Username { get; set; }
    public int VisitedCount { get; set; } = 0;
}

public class UserVisitList
{
    public int? UserID { get; set; }
    public string? Username { get; set; }
    public int? VisitedID { get; set; }
    public string? VisitedUsername { get; set; }
    public DateTime AccessTime { get; set; } = DateTime.Now;
}

public class UserVisitGroup
{
    public int? UserID { get; set; }
    public string? Username { get; set; }
    public int? VisitedID { get; set; }
    public string? VisitedUsername { get; set; }
    public int count { get; set; } = 0;
}