namespace PRMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Admins",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        Password = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.AdminActivities",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        UserId = c.String(),
                        Time = c.String(),
                        Message = c.String(),
                        Content = c.String(),
                        FilePath = c.String(),
                        Approved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.AllSessions",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Session = c.String(),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Course_code = c.String(),
                        Course_title = c.String(),
                        Credit_hour = c.Single(nullable: false),
                        Semester = c.Int(nullable: false),
                        UnderFaculty = c.String(),
                        UnderDepartment = c.String(),
                        CourseTeacherID = c.Int(nullable: false),
                        CourseTeacherName = c.String(),
                        CourseTeacherDepartment = c.String(),
                        CourseTeacherFaculty = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.CourseStatus",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Course_code = c.String(),
                        Course_title = c.String(),
                        Credit_hour = c.Single(nullable: false),
                        Semester = c.Int(nullable: false),
                        UnderFaculty = c.String(),
                        UnderDepartment = c.String(),
                        CourseTeacherID = c.Int(nullable: false),
                        CourseTeacherName = c.String(),
                        CourseTeacherDepartment = c.String(),
                        CourseTeacherFaculty = c.String(),
                        UnderSession = c.String(),
                        Status = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        DepartmentName = c.String(),
                        ShortForm = c.String(),
                        Faculty = c.String(),
                        ChairmanId = c.Int(nullable: false),
                        ChairmanName = c.String(),
                        ChairmanEmail = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Faculties",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        FacultyName = c.String(),
                        ShortForm = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Results",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        StudentId = c.Int(nullable: false),
                        RegNo = c.Int(nullable: false),
                        Faculty = c.String(),
                        Session = c.String(),
                        Semester = c.Int(nullable: false),
                        Degree = c.String(),
                        CourseResults = c.String(),
                        GPA = c.Single(nullable: false),
                        PrevCGPA = c.Single(nullable: false),
                        PrevCCH = c.Single(nullable: false),
                        CGPA = c.Single(nullable: false),
                        CCH = c.Single(nullable: false),
                        Remarks = c.String(),
                        IsMeritListed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.StudentInfoes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        StudentId = c.Int(nullable: false),
                        Reg = c.Int(nullable: false),
                        Faculty = c.String(),
                        Session = c.String(),
                        Regularity = c.String(),
                        Hall = c.String(),
                        Blood = c.String(),
                        Sex = c.String(),
                        Fathers_name = c.String(),
                        Mothers_name = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
                        Nationality = c.String(),
                        Religion = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Teachers",
                c => new
                    {
                        TeacherID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Email = c.String(),
                        Mobile = c.String(),
                        Faculty = c.String(),
                        Department = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.TeacherID);
            
            CreateTable(
                "dbo.TeacherActivities",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Designation = c.String(),
                        CourseCode = c.String(),
                        Session = c.String(),
                        UserId = c.String(),
                        Time = c.String(),
                        Message = c.String(),
                        Content = c.String(),
                        FilePath = c.String(),
                        Approved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TeacherActivities");
            DropTable("dbo.Teachers");
            DropTable("dbo.StudentInfoes");
            DropTable("dbo.Results");
            DropTable("dbo.Faculties");
            DropTable("dbo.Departments");
            DropTable("dbo.CourseStatus");
            DropTable("dbo.Courses");
            DropTable("dbo.AllSessions");
            DropTable("dbo.AdminActivities");
            DropTable("dbo.Admins");
        }
    }
}
