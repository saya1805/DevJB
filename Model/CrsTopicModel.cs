namespace DevJBackend.Model
{
    public class CrsTopicModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double crsPrice { get; set; }
        public int crsDurationInDays { get; set; }
        public DateTime startdate { get; set; } = DateTime.Now;
        public DateTime EndDate { get { return DateTime.Now.AddDays(crsDurationInDays); } set { } } 
        public string Author { get; set; }
        public string VideoUrl { get; set; }
        public int CrsId { get; set; }

        public List<ModuleDetail> Modules { get; set; } = new List<ModuleDetail>();
    }

    public class ModuleDetail
    {
        public int Id { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public string ModuleDescription { get; set; } = string.Empty;
    }
}
