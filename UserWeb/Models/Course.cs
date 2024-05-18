using System.ComponentModel.DataAnnotations.Schema;

namespace UserWeb.Models
{
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CourseID { get; set; }
        public required string Title { get; set; }
        public int Credits { get; set; }
    }

}
