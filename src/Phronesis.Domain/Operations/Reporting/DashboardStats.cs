namespace Phronesis.Domain.Operations.Reporting;

public class AdminDashboardStats
{
    public decimal TotalRevenue { get; set; }
    public int ActiveSubscriptions { get; set; }
    public int TotalUsers { get; set; }
    public int TotalClasses { get; set; }
}

public class TeacherDashboardStats
{
    public int UpcomingClasses { get; set; }
    public int TotalStudentsTaught { get; set; }
    public double AverageAttendanceRate { get; set; } // Percentage 0-100
}

public class LearnerDashboardStats
{
    public int ClassesAttended { get; set; }
    public int CoursesEnrolled { get; set; }
    public int UpcomingClasses { get; set; }
}
