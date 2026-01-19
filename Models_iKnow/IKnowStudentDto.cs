namespace EuropeanStudentCard.Models_iKnow
{
    public class IKnowStudentDto
    {
        public string index { get; set; } = default!;      
        public string name { get; set; } = default!;
        public string surname { get; set; } = default!;
        public string? email { get; set; }                      
        public int status { get; set; }          

        public string? program { get; set; }
        public int? enrolment { get; set; }
        public float? gpa { get; set; }
        public int? ects { get; set; }
    }
}
